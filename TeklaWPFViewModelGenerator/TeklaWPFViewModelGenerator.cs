using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace MPD.TeklaWPFViewModelGenerator
{
    [Generator]
    class TeklaWPFViewModelGenerator : IIncrementalGenerator
    {
        private const string _targetAttributeName = "MPD.TeklaWPFViewModelToolkit.TemplateToGenerateAttribute";
        private const string _overrideViewModelAttrName = "ViewModelTypeOverrideAttribute";
        private const string _useInitializerOutsideRangeAttName = "UseInitializerValueOutsideRange";
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var provider = context.SyntaxProvider.CreateSyntaxProvider(
               predicate: static (node, _) => node is ClassDeclarationSyntax,
               transform: static (ctx, _) => GetClassForGeneration(ctx)
            ).Where(m => m is not null);

            var compilation = context.CompilationProvider.Combine(provider.Collect());

            context.RegisterSourceOutput(compilation,
                static (spc, source) => Execute(source.Left, source.Right, spc));
        }

        private static ClassDeclarationSyntax GetClassForGeneration(GeneratorSyntaxContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            foreach (var attributeList in classDeclaration.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                        continue;

                    var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    var fullName = attributeContainingTypeSymbol.ToDisplayString();

                    if (fullName == _targetAttributeName)
                    {
                        return classDeclaration;
                    }
                }
            }

            return null;
        }

        private static void Execute(
            Compilation compilation,
            ImmutableArray<ClassDeclarationSyntax> classes,
            SourceProductionContext context)
        {
            if (classes == null)
                return;
            foreach (var classDecl in classes)
            {
                var semanticModel = compilation.GetSemanticModel(classDecl.SyntaxTree);
                var classSymbol = semanticModel.GetDeclaredSymbol(classDecl);

                if (classSymbol == null)
                    continue;

                var attributeData = GetTemplateAttributeData(classSymbol);
                if (attributeData == null
                    || attributeData.Item1 == null
                    || attributeData.Item2 == null)
                    continue;
                var (modelClassName, viewModelClassName) = attributeData;

                // Generate source code
                var modelSource = GenerateModelClass(classDecl, modelClassName!);
                var viewModelSource = GenerateViewModelClass(classDecl, modelClassName!, viewModelClassName!, semanticModel);

                context.AddSource($"{modelClassName}.g.cs", SourceText.From(modelSource, Encoding.UTF8));
                context.AddSource($"{viewModelClassName}.g.cs", SourceText.From(viewModelSource, Encoding.UTF8));
            }
        }


        private static Tuple<string?, string?>? GetTemplateAttributeData(ISymbol classSymbol)
        {
            foreach (var attribute in classSymbol.GetAttributes())
            {
                if (attribute.AttributeClass?.ToDisplayString() == _targetAttributeName)
                {
                    if (attribute.ConstructorArguments.Length == 2)
                    {
                        return new Tuple<string?, string?>
                            (
                                attribute.ConstructorArguments[0].Value?.ToString(),
                                attribute.ConstructorArguments[1].Value?.ToString()
                            );
                    }
                }
            }
            return null;
        }

        private static string GenerateModelClass(ClassDeclarationSyntax templateClass, string modelClassName)
        {
            var sb = new StringBuilder();


            sb.AppendLine("using System;");
            sb.AppendLine("using Tekla.Structures.Plugins;");
            sb.AppendLine("using MPD.TeklaWPFViewModelToolkit;");
            sb.AppendLine(GetNamespaceLine(templateClass));
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine($"    public partial class {modelClassName}");
            sb.AppendLine("    {");

            var fields = GetFieldDeclarations(templateClass);

            foreach (var field in fields)
            {
                var rangeAttr = field.AttributeLists
                            .SelectMany(al => al.Attributes)
                            .FirstOrDefault(a => a.Name.ToString().Contains(_useInitializerOutsideRangeAttName));

                foreach (var variable in field.Declaration.Variables)
                {
                    string typeName = field.Declaration.Type.ToString();
                    string fieldName = variable.Identifier.Text;
                    string internalFieldName = GetInternalFieldName(fieldName);
                    string propertyName = GetPublicPropertyName(fieldName);
                    string defaultValue =
                        variable.Initializer is null ? "default" : variable.Initializer.Value.ToString();

                    sb.AppendLine($"        [StructuresField(nameof({propertyName}))]");
                    sb.AppendLine($"        public {typeName} {internalFieldName};");
                    sb.AppendLine($"        private const {typeName} {internalFieldName}Default = {defaultValue};");
                    sb.AppendLine($"        public {typeName} {propertyName}");
                    sb.AppendLine("        {");
                    sb.AppendLine($"            get");
                    sb.AppendLine("            {");
                    AppendPropertyGetterBody(sb, typeName, internalFieldName, rangeAttr);
                    sb.AppendLine("            }");
                    sb.AppendLine($"            set {{ {internalFieldName} = value; }}");
                    sb.AppendLine("        }");
                    sb.AppendLine();
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
        private static void AppendPropertyGetterBody(
            StringBuilder sb,
            string typeName,
            string internalFieldName,
            AttributeSyntax rangeAttr)
        {
            if (rangeAttr?.ArgumentList is null || rangeAttr.ArgumentList.Arguments.Count < 2)
            {
                // Default check with helper if attrubute UseInitializerValueOutsideRange is missing.
                sb.AppendLine($"                if (DefaultValueHelper.IsDefaultValue({internalFieldName})) return {internalFieldName}Default;");
            }
            else
            {
                var min = rangeAttr.ArgumentList.Arguments[0].Expression.ToString();
                var max = rangeAttr.ArgumentList.Arguments[1].Expression.ToString();

                if (typeName == "string")
                {
                    sb.AppendLine($"                if ({internalFieldName}.Length < {min} || {internalFieldName}.Length > {max})");
                    sb.AppendLine($"                    return {internalFieldName}Default;");
                }
                else if (typeName == "int" || typeName == "double")
                {
                    sb.AppendLine($"                if ({internalFieldName} < {min} || {internalFieldName} > {max})");
                    sb.AppendLine($"                    return {internalFieldName}Default;");
                }
            }
            sb.AppendLine($"                return {internalFieldName};");
        }

        private static string GenerateViewModelClass(
            ClassDeclarationSyntax templateClass, 
            string modelClassName,
            string viewModelClassName,
            SemanticModel semanticModel
            )
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using System;");
            sb.AppendLine("using System.ComponentModel;");
            sb.AppendLine("using Tekla.Structures.Dialog;");
            sb.AppendLine("using MPD.TeklaWPFViewModelToolkit;");
            sb.AppendLine(GetNamespaceLine(templateClass));
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine($"    public partial class {viewModelClassName} : INotifyPropertyChanged");
            sb.AppendLine("    {");
            sb.AppendLine("         public event PropertyChangedEventHandler PropertyChanged;");
            sb.AppendLine();

            var fields = GetFieldDeclarations(templateClass);

            foreach (var field in fields)
            {
                foreach (var variable in field.Declaration.Variables)
                {
                    var typeName = GetViewModelPropertyType(semanticModel, field);
                    var fieldName = variable.Identifier.Text;
                    var pascalFieldName = GetPublicPropertyName(fieldName);

                    // Generate internal property
                    sb.AppendLine($"        [StructuresDialog(nameof({modelClassName}.{pascalFieldName}), typeof({typeName}))]");
                    sb.AppendLine($"        public {typeName} Tekla{pascalFieldName}Property");
                    sb.AppendLine("        {");
                    sb.AppendLine($"            get {{ return {pascalFieldName}.Value; }}");
                    sb.AppendLine($"            set {{ {pascalFieldName}.Value = value; }}");
                    sb.AppendLine("        }");

                    // Generate TeklaWPFBinding property
                    sb.AppendLine($"        public TeklaWPFBinding<{typeName}> {pascalFieldName} {{ get; set; }} = ");
                    sb.AppendLine($"            new TeklaWPFBinding<{typeName}>(nameof({modelClassName}.{pascalFieldName}));");
                    sb.AppendLine();
                    sb.AppendLine();
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static FieldDeclarationSyntax[] GetFieldDeclarations(ClassDeclarationSyntax templateClass)
        {
            return templateClass.DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .ToArray();
        }

        private static string GetViewModelPropertyType(SemanticModel semanticModel, FieldDeclarationSyntax field)
        {
            // Check for the [ViewModelTypeOverride<T>] attribute
            foreach (var attributeList in field.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var attributeSymbol = semanticModel.GetSymbolInfo(attribute).Symbol as IMethodSymbol;
                    var attributeType = attributeSymbol?.ContainingType;

                    if (attributeType != null && 
                        attributeType.IsGenericType && 
                        attributeType.Name == _overrideViewModelAttrName)
                    {
                        var genericArgument = attributeType.TypeArguments.FirstOrDefault();
                        if (genericArgument != null)
                        {
                            return genericArgument.ToDisplayString();
                        }
                    }
                }
            }

            // Fallback to default mapping if no attribute is found
            string typeName = field.Declaration.Type.ToString();
            return typeName switch
            {
                "string" => "Tekla.Structures.Datatype.String",
                "int"    => "Tekla.Structures.Datatype.Integer",
                "double" => "Tekla.Structures.Datatype.Distance",
                _        => "Tekla.Structures.Datatype.String"
            };
        }

        private static string GetNamespaceLine(BaseTypeDeclarationSyntax syntax)
        {
            var nameSpace = syntax.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
            string nameSpaceLine = String.Empty;
            if (nameSpace is not null)
            {
                nameSpaceLine = $"namespace {nameSpace.Name.ToString()}";
            }
            return nameSpaceLine;
        }
        private static string GetPublicPropertyName(string name)
        {
            if (name.Length > 1)
            {
                return char.ToUpper(name[0]) + name.Substring(1);
            }
            else
            {
                return "Property_" + name;
            }
        }

        private static string GetInternalFieldName(string name)
        {
            if (name.Length > 1)
            {
                return $"_{char.ToLower(name[0])}{name.Substring(1)}";
            }
            else
            {
                return "_internal_" + name;
            }
        }

    }
}
