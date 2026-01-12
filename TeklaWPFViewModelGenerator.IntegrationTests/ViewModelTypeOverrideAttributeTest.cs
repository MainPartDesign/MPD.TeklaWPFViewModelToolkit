using Xunit;
using Tekla.Structures.Plugins;
using TeklaWPFViewModelToolkit;
using TD = Tekla.Structures.Datatype;

namespace TeklaWPFViewModelGenerator.IntegrationTests;

[TemplateToGenerate("PluginModelTestDummy", "OverridenPropTypeVMDummy")]
internal class OverridenViewModelPropDummy
{
    // This attribute allows to redefine view model property type.
    // Here double is used over default Distance type.
    [ViewModelTypeOverride<Tekla.Structures.Datatype.Double>]
    double strictDouble;

    // It should support using alias directives.
    // Here Boolean is used over default Integer type.
    [ViewModelTypeOverride<TD.Boolean>]
    int strictBoolean;
}

public class ViewModelTypeOverrideAttributeTest
{
    public const string _doubleAttrName = "strictDouble";
    public const string _booleanAttrName = "strictBoolean";

    [Fact]
    public void PluginModel_Creation_DoesntThrow()
    {
        object boxedPluginModelDummy = null;
        var exception = Record.Exception(() => boxedPluginModelDummy = new PluginModelTestDummy());

        Assert.Null(exception);
        Assert.NotNull(boxedPluginModelDummy);
    }

    [Fact]
    public void ViewModel_Creation_DoesntThrow()
    {
        object boxedViewModelDummy = null;
        var exception = Record.Exception(() => boxedViewModelDummy = new OverridenPropTypeVMDummy());

        Assert.Null(exception);
        Assert.NotNull(boxedViewModelDummy);
    }

    [Fact]
    public void PluginModel_HasCorrectFieldsWithAttributes()
    {
        var pluginModelType = typeof(PluginModelTestDummy);
        
        GeneratorAssertions.AssertFieldWithAttribute<double>(
            pluginModelType, _doubleAttrName, typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _doubleAttrName,
            "The 'doubleCheck' field should exist with correct attribute");

        GeneratorAssertions.AssertFieldWithAttribute<int>(
            pluginModelType, _booleanAttrName, typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _booleanAttrName,
            "The 'check' field should exist with correct attribute");
    }

    [Fact]
    public void ViewModel_HasOverridenPropertyTypesWithAttributes()
    {
        var viewModelType = typeof(OverridenPropTypeVMDummy);
        
        GeneratorAssertions.AssertPropertyChangeEvent(viewModelType,
            "ViewModel should implement INotifyPropertyChanged");
            
        GeneratorAssertions.AssertBindingProperty(
            viewModelType, "StrictBoolean", _booleanAttrName, typeof(TD.Boolean),
            "Check property should be a TeklaWPFBinding for boolean type");
            
        GeneratorAssertions.AssertBindingProperty(
            viewModelType, "StrictDouble", _doubleAttrName, typeof(TD.Double),
            "DoubleCheck property should be a TeklaWPFBinding for double type");

        // Check internal properties with StructuresDialog attribute
        GeneratorAssertions.AssertInternalProperty(
            viewModelType, "InternalStrictBooleanProperty", _booleanAttrName, 
            typeof(TD.Boolean),
            "InternalCheckProperty should exist for boolean field");
            
        GeneratorAssertions.AssertInternalProperty(
            viewModelType, "InternalStrictDoubleProperty", _doubleAttrName, 
            typeof(TD.Double),
            "InternalDoubleCheckProperty should exist for double field");
    }
}
