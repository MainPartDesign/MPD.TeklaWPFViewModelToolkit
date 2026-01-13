using Xunit;
using Tekla.Structures.Plugins;
using MPD.TeklaWPFViewModelToolkit;
using TD = Tekla.Structures.Datatype;

namespace MPD.TeklaWPFViewModelGenerator.IntegrationTests;

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
    int strictBoolean = 1;
}

public class ViewModelTypeOverrideAttributeTest
{
    public const string _doubleAttrName = "StrictDouble";
    public const string _booleanAttrName = "StrictBoolean";

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
            pluginModelType, "_strictDouble", typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _doubleAttrName,
            "The 'strictDouble' field should exist with correct attribute");

        GeneratorAssertions.AssertFieldWithAttribute<int>(
            pluginModelType, "_strictBoolean", typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _booleanAttrName,
            "The 'strictBoolean' field should exist with correct attribute");
    }

    [Fact]
    public void PluginModel_HasDefaultValueConstants()
    {
        var pluginModelType = typeof(PluginModelTestDummy);
        
        // Check that default value constants exist
        GeneratorAssertions.AssertDefaultValueConstant(pluginModelType, "_strictDoubleDefault", default(double));
        GeneratorAssertions.AssertDefaultValueConstant(pluginModelType, "_strictBooleanDefault", 1);
    }

    [Fact]
    public void PluginModel_HasPublicProperties()
    {
        var pluginModelType = typeof(PluginModelTestDummy);
        
        GeneratorAssertions.AssertProperty<double>(pluginModelType, "StrictDouble",
            "Plugin model should have public string property 'StrictDouble'");
        GeneratorAssertions.AssertProperty<int>(pluginModelType, "StrictBoolean",
            "Plugin model should have public int property 'StrictBoolean'");
    }

    [Fact]
    public void PluginModel_PropertiesReturnTemplateDefaultValues()
    {
        var model = new PluginModelTestDummy();
        // Simulation of Tekla's behavior populating fields with its default values.
        model._strictBoolean = int.MinValue;
        model._strictDouble = (double)int.MinValue;

        Assert.Equal(1, model.StrictBoolean);
        Assert.Equal(default(double), model.StrictDouble);
    }

    [Fact]
    public void ViewModel_HasOverridenPropertyTypesWithAttributes()
    {
        var viewModelType = typeof(OverridenPropTypeVMDummy);
        
        GeneratorAssertions.AssertPropertyChangeEvent(viewModelType,
            "ViewModel should implement INotifyPropertyChanged");
            
        GeneratorAssertions.AssertBindingProperty(
            viewModelType, "StrictBoolean", _booleanAttrName, typeof(TD.Boolean),
            "StrictBoolean property should be a TeklaWPFBinding for boolean type");
            
        GeneratorAssertions.AssertBindingProperty(
            viewModelType, "StrictDouble", _doubleAttrName, typeof(TD.Double),
            "StrictDouble property should be a TeklaWPFBinding for double type");

        // Check internal properties with StructuresDialog attribute
        GeneratorAssertions.AssertTeklaProperty<TD.Boolean>(
            viewModelType, "TeklaStrictBooleanProperty", _booleanAttrName, 
            "InternalStrictBooleanProperty should exist for boolean field");
            
        GeneratorAssertions.AssertTeklaProperty<TD.Double>(
            viewModelType, "TeklaStrictDoubleProperty", _doubleAttrName, 
            "InternalStrictDoubleProperty should exist for double field");
    }

}
