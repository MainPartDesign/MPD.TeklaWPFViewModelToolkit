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
    int check = 5;
    double doubleCheck = 3.14;
}

public class TemplateToGenerateAttributeTest
{
    private const string _stringAttrName = "Test";
    private const string _intAttrName = "Check";
    private const string _doubleAttrName = "DoubleCheck";

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
            pluginModelType, "_test", typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _stringAttrName,
            "The 'test' field should exist with correct attribute");
            
        GeneratorAssertions.AssertFieldWithAttribute<int>(
            pluginModelType, "_check", typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _intAttrName,
            "The 'check' field should exist with correct attribute");
            
        GeneratorAssertions.AssertFieldWithAttribute<double>(
            pluginModelType, "_doubleCheck", typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _doubleAttrName,
            "The 'doubleCheck' field should exist with correct attribute");
    }

    [Fact]
    public void PluginModel_HasDefaultValueConstants()
    {
        var pluginModelType = typeof(PluginModelDummy);
        
        // Check that default value constants exist
        GeneratorAssertions.AssertDefaultValueConstant(pluginModelType, "_checkDefault", 5);
        GeneratorAssertions.AssertDefaultValueConstant(pluginModelType, "_doubleCheckDefault", 3.14);
    }

    [Fact]
    public void PluginModel_HasPublicProperties()
    {
        var pluginModelType = typeof(PluginModelDummy);
        
        GeneratorAssertions.AssertProperty<string>(pluginModelType, "Test",
            "Plugin model should have public string property 'Test'");
        GeneratorAssertions.AssertProperty<int>(pluginModelType, "Check",
            "Plugin model should have public int property 'Check'");
        GeneratorAssertions.AssertProperty<double>(pluginModelType, "DoubleCheck",
            "Plugin model should have public double property 'DoubleCheck'");
    }

    [Fact]
    public void PluginModel_PropertiesReturnTemplateDefaultValues()
    {
        var model = new PluginModelDummy();
        // Simulation of Tekla's behavior populating fields with its default values.
        model._test = "";
        model._check = int.MinValue;
        model._doubleCheck = (double)int.MinValue;

        Assert.Equal(5, model.Check);
        Assert.Equal(3.14, model.DoubleCheck);
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
            "DoubleCheck property should be a TeklaWPFBinding for Distance type");

        // Check internal properties with StructuresDialog attribute
        GeneratorAssertions.AssertTeklaProperty<Tekla.Structures.Datatype.String>(
            viewModelType, "TeklaTestProperty", _stringAttrName, 
            "InternalTestProperty should exist for string field");
            
        GeneratorAssertions.AssertTeklaProperty<Tekla.Structures.Datatype.Integer>(
            viewModelType, "TeklaCheckProperty", _intAttrName, 
            "InternalCheckProperty should exist for int field");
            
        GeneratorAssertions.AssertTeklaProperty<Tekla.Structures.Datatype.Distance>(
            viewModelType, "TeklaDoubleCheckProperty", _doubleAttrName, 
            "InternalDoubleCheckProperty should exist for double field");
    }

}
