using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MPD.TeklaWPFViewModelToolkit;
using Tekla.Structures.Plugins;

namespace MPD.TeklaWPFViewModelGenerator.IntegrationTests;

// This attribute is filtered by source generator
// and classes should be created based on its arguments.
[TemplateToGenerate("PluginModelDummy", "ViewModelDummy")]
internal class ModelTemplateDummy
{
    string test;
    int check;
    double doubleCheck;
}

public class TemplateToGenerateAttributeTest
{
    private const string _stringAttrName = "test";
    private const string _intAttrName = "check";
    private const string _doubleAttrName = "doubleCheck";

    [Fact]
    public void PluginModel_Creation_DoesntThrow()
    {
        object boxedPluginModelDummy = null;
        var exception = Record.Exception(() => boxedPluginModelDummy = new PluginModelDummy());

        Assert.Null(exception);
        Assert.NotNull(boxedPluginModelDummy);
    }

    [Fact]
    public void ViewModel_Creation_DoesntThrow()
    {
        object boxedViewModelDummy = null;
        var exception = Record.Exception(() => boxedViewModelDummy = new ViewModelDummy());

        Assert.Null(exception);
        Assert.NotNull(boxedViewModelDummy);
    }


    [Fact]
    public void PluginModel_HasCorrectFieldsWithAttributes()
    {
        var pluginModelType = typeof(PluginModelDummy);
        
        GeneratorAssertions.AssertFieldWithAttribute<string>(
            pluginModelType, _stringAttrName, typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _stringAttrName,
            "The 'test' field should exist with correct attribute");
            
        GeneratorAssertions.AssertFieldWithAttribute<int>(
            pluginModelType, _intAttrName, typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _intAttrName,
            "The 'check' field should exist with correct attribute");
            
        GeneratorAssertions.AssertFieldWithAttribute<double>(
            pluginModelType, _doubleAttrName, typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _doubleAttrName,
            "The 'doubleCheck' field should exist with correct attribute");
    }

    [Fact]
    public void ViewModel_HasCorrectPropertiesWithAttributes()
    {
        var viewModelType = typeof(ViewModelDummy);
        
        GeneratorAssertions.AssertPropertyChangeEvent(viewModelType,
            "ViewModel should implement INotifyPropertyChanged");

        GeneratorAssertions.AssertBindingProperty(
            viewModelType, "Test", _stringAttrName, typeof(Tekla.Structures.Datatype.String),
            "Test property should be a TeklaWPFBinding for string type");
            
        GeneratorAssertions.AssertBindingProperty(
            viewModelType, "Check", _intAttrName, typeof(Tekla.Structures.Datatype.Integer),
            "Check property should be a TeklaWPFBinding for int type");
            
        GeneratorAssertions.AssertBindingProperty(
            viewModelType, "DoubleCheck", _doubleAttrName, typeof(Tekla.Structures.Datatype.Distance),
            "DoubleCheck property should be a TeklaWPFBinding for double type");

        // Check internal properties with StructuresDialog attribute
        GeneratorAssertions.AssertInternalProperty(
            viewModelType, "InternalTestProperty", _stringAttrName, 
            typeof(Tekla.Structures.Datatype.String),
            "InternalTestProperty should exist for string field");
            
        GeneratorAssertions.AssertInternalProperty(
            viewModelType, "InternalCheckProperty", _intAttrName, 
            typeof(Tekla.Structures.Datatype.Integer),
            "InternalCheckProperty should exist for int field");
            
        GeneratorAssertions.AssertInternalProperty(
            viewModelType, "InternalDoubleCheckProperty", _doubleAttrName, 
            typeof(Tekla.Structures.Datatype.Distance),
            "InternalDoubleCheckProperty should exist for double field");
    }


}
