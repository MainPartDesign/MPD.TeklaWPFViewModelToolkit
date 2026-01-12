using System;
using System.Reflection;
using Xunit;
using Tekla.Structures.Dialog;
using TeklaWPFViewModelToolkit;
using Xunit.Sdk;
using System.ComponentModel;

namespace TeklaWPFViewModelGenerator.IntegrationTests;

public static class GeneratorAssertions
{
    public static void AssertFieldWithAttribute<T>(
        Type type,
        string fieldName,
        Type attributeType,
        string expectedFieldNameInAttribute,
        string because = "")
    {
        var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

        Assert.NotNull(field);
        Assert.Equal(typeof(T), field.FieldType);

        var attribute = field.GetCustomAttribute(attributeType);
        Assert.NotNull(attribute);

        // Check attribute properties if needed
        var fieldNameProperty = attribute.GetType().GetProperty("AttributeName");
        if (fieldNameProperty != null)
        {
            var actualFieldName = fieldNameProperty.GetValue(attribute) as string;
            Assert.Equal(expectedFieldNameInAttribute, actualFieldName);
        }
    }

    public static void AssertBindingProperty<T>(
        Type type,
        string propertyName,
        string expectedFieldName,
        Type expectedTeklaType,
        string because = "")
    {
        // Get the property
        var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

        // Assert with clear messages
        Assert.True(property != null,
            $"Property '{propertyName}' not found on type '{type.Name}'. {because}");

        // Check property type
        var expectedBindingType = typeof(TeklaWPFBinding<>).MakeGenericType(expectedTeklaType);
        Assert.True(property.PropertyType == expectedBindingType,
            $"Property '{propertyName}' has type '{property.PropertyType}', but expected '{expectedBindingType}'. {because}");

        // Check getter
        var getter = property.GetGetMethod();
        Assert.True(getter != null,
            $"Property '{propertyName}' has no getter. {because}");

        // Check setter
        var setter = property.GetSetMethod();
        Assert.True(setter != null,
            $"Property '{propertyName}' has no setter. {because}");

        // Verify the binding can be instantiated and has correct field name
        try
        {
            // Create an instance of the type to test the property
            var instance = Activator.CreateInstance(type);
            var bindingValue = property.GetValue(instance);

            Assert.NotNull(bindingValue);

            // Verify the field name in the binding if possible
            var fieldNameProperty = bindingValue.GetType().GetProperty("FieldName");
            if (fieldNameProperty != null)
            {
                var actualFieldName = fieldNameProperty.GetValue(bindingValue) as string;
                Assert.Equal(expectedFieldName, actualFieldName);
            }
        }
        catch (Exception ex) when (ex is not XunitException)
        {
            Assert.Fail($"Failed to test property '{propertyName}': {ex.Message}. {because}");
        }
    }

    public static void AssertInternalProperty(
        Type type,
        string propertyName,
        string expectedFieldName,
        Type expectedTeklaType,
        Type originalType,
        string because = "")
    {
        var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

        Assert.True(property != null,
            $"Property '{propertyName}' not found on type '{type.Name}'. {because}");

        Assert.Equal(expectedTeklaType, property.PropertyType);

        // Check for StructuresDialog attribute
        var attribute = property.GetCustomAttribute<StructuresDialogAttribute>();
        Assert.True(attribute != null,
            $"Property '{propertyName}' is missing StructuresDialogAttribute. {because}");

        Assert.Equal(expectedFieldName, attribute.AttributeName);
        Assert.Equal(expectedTeklaType, attribute.AttributeType);

        // Check getter and setter
        var getter = property.GetGetMethod();
        Assert.True(getter != null,
            $"Property '{propertyName}' has no getter. {because}");

        var setter = property.GetSetMethod();
        Assert.True(setter != null,
            $"Property '{propertyName}' has no setter. {because}");
    }

    public static void AssertPropertyChangeEvent(Type type, string because = "")
    {
        Assert.True(typeof(INotifyPropertyChanged).IsAssignableFrom(type),
            $"Type '{type.Name}' does not implement INotifyPropertyChanged. {because}");

        var eventInfo = type.GetEvent("PropertyChanged");
        Assert.True(eventInfo != null,
            $"Type '{type.Name}' has no PropertyChanged event. {because}");

        Assert.Equal(typeof(PropertyChangedEventHandler), eventInfo.EventHandlerType);
    }
}