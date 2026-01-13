using System;

namespace MPD.TeklaWPFViewModelToolkit;

/// <summary>
/// This attribute is used to rewrite the getter logic for plugin data model property
/// and return Initializer value from the template for inputs exiding defined range.
/// For string type getter will check string length from provided length.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class UseInitializerValueOutsideRangeAttribute : Attribute
{
    public double Min { get; }
    public double Max { get; }

    public UseInitializerValueOutsideRangeAttribute(double min, double max)
    {
        Min = min;
        Max = max;
    }
}
