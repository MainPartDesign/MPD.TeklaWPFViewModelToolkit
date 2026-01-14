using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MPD.TeklaWPFViewModelToolkit;
using Tekla.Structures.Plugins;

namespace MPD.TeklaWPFViewModelGenerator.IntegrationTests;

[TemplateToGenerate("RangePluginModelDummy", "RangeViewModelDummy")]
internal static class RangeModelTemplateDummy
{
    // This attribute allows to rewrite Getter for PluginModel property
    // so Tekla values outside this range replaced with initializer (default) value.
    [UseInitializerValueOutsideRange(0.1, 11)]
    public static int num1 = 1, num2 = 2;

    [UseInitializerValueOutsideRange(-10, 10)]
    public static double num3 = 3.14;

    // For string value range is checked against string length.
    [UseInitializerValueOutsideRange(2, 15)]
    public static string text = "hello world";
}
public class UseInitializerValueOutsideRangeAttributeTests
{
    private const string _intAttrName1 = "Num1";
    private const string _intAttrName2 = "Num2";
    private const string _doubleAttrName = "Num3";
    private const string _stringAttrName = "Text";

    [Fact]
    public void PluginModel_Creation_DoesntThrow()
    {
        object boxedPluginModelDummy = null;
        var exception = Record.Exception(() => boxedPluginModelDummy = new RangePluginModelDummy());

        Assert.Null(exception);
        Assert.NotNull(boxedPluginModelDummy);
    }

    [Fact]
    public void ViewModel_Creation_DoesntThrow()
    {
        object boxedViewModelDummy = null;
        var exception = Record.Exception(() => boxedViewModelDummy = new RangeViewModelDummy());

        Assert.Null(exception);
        Assert.NotNull(boxedViewModelDummy);
    }

    [Fact]
    public void PluginModel_HasCorrectFieldsWithAttributes()
    {
        var pluginModelType = typeof(RangePluginModelDummy);
        
        GeneratorAssertions.AssertFieldWithAttribute<string>(
            pluginModelType, "_text", typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _stringAttrName,
            "The 'test' field should exist with correct attribute");
            
        GeneratorAssertions.AssertFieldWithAttribute<int>(
            pluginModelType, "_num1", typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _intAttrName1,
            "The 'check' field should exist with correct attribute");
            
        GeneratorAssertions.AssertFieldWithAttribute<int>(
            pluginModelType, "_num2", typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _intAttrName2,
            "The 'check' field should exist with correct attribute");
            
        GeneratorAssertions.AssertFieldWithAttribute<double>(
            pluginModelType, "_num3", typeof(StructuresFieldAttribute),
            nameof(StructuresFieldAttribute.AttributeName), _doubleAttrName,
            "The 'doubleCheck' field should exist with correct attribute");
    }

    [Fact]
    public void PluginModel_PropertiesReturnTemplateDefaultValues()
    {
        var model = new RangePluginModelDummy();
        // Simulation of Tekla's behavior populating fields with values outside range.
        model._text = "?";
        model._num1 = 100;
        model._num2 = 0;
        model._num3 = -11;

        Assert.Equal(RangeModelTemplateDummy.text, model.Text);
        Assert.Equal(RangeModelTemplateDummy.num1, model.Num1);
        Assert.Equal(RangeModelTemplateDummy.num2, model.Num2);
        Assert.Equal(RangeModelTemplateDummy.num3, model.Num3);
    }

    [Fact]
    public void PluginModel_PropertiesReturnCorrectValues()
    {
        var model = new RangePluginModelDummy();
        string expectedString = "test";
        int expectedInt = 7;
        double expectedDouble = 0.69;
        // Simulation of Tekla's behavior populating fields with values inside range.
        model._text = expectedString;
        model._num1 = expectedInt;
        model._num2 = expectedInt;
        model._num3 = expectedDouble;


        Assert.Equal(expectedString, model.Text);
        Assert.Equal(expectedInt, model.Num1);
        Assert.Equal(expectedInt, model.Num2);
        Assert.Equal(expectedDouble, model.Num3);
    }
    [Fact]
    public void ViewModel_HasCorrectPropertiesWithAttributes()
    {
        var viewModelType = typeof(RangeViewModelDummy);
        
        GeneratorAssertions.AssertPropertyChangeEvent(viewModelType,
            "ViewModel should implement INotifyPropertyChanged");

        GeneratorAssertions.AssertBindingProperty(
            viewModelType, "Text", _stringAttrName, typeof(Tekla.Structures.Datatype.String),
            "Text property should be a TeklaWPFBinding for string type");
            
        GeneratorAssertions.AssertBindingProperty(
            viewModelType, "Num1", _intAttrName1, typeof(Tekla.Structures.Datatype.Integer),
            "Num1 property should be a TeklaWPFBinding for int type");

        GeneratorAssertions.AssertBindingProperty(
            viewModelType, "Num2", _intAttrName2, typeof(Tekla.Structures.Datatype.Integer),
            "Num2 property should be a TeklaWPFBinding for int type");
            
        GeneratorAssertions.AssertBindingProperty(
            viewModelType, "Num3", _doubleAttrName, typeof(Tekla.Structures.Datatype.Distance),
            "Num3 property should be a TeklaWPFBinding for Distance type");

        // Check internal properties with StructuresDialog attribute
        GeneratorAssertions.AssertTeklaProperty<Tekla.Structures.Datatype.String>(
            viewModelType, "TeklaTextProperty", _stringAttrName, 
            "TeklaTextProperty should exist for string field");
            
        GeneratorAssertions.AssertTeklaProperty<Tekla.Structures.Datatype.Integer>(
            viewModelType, "TeklaNum1Property", _intAttrName1, 
            "TeklaNum1Property should exist for int field");
            
        GeneratorAssertions.AssertTeklaProperty<Tekla.Structures.Datatype.Integer>(
            viewModelType, "TeklaNum2Property", _intAttrName2, 
            "TeklaNum2Property should exist for int field");
            
        GeneratorAssertions.AssertTeklaProperty<Tekla.Structures.Datatype.Distance>(
            viewModelType, "TeklaNum3Property", _doubleAttrName, 
            "TeklaNum3Property should exist for double field");
    }
}
