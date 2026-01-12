namespace TeklaWPFViewModelToolkit;

public static class DefaultValueHelper
{
    public static bool IsDefaultValue(string value)
    {
        return value == "";
    }
    public static bool IsDefaultValue(int value)
    {
        return value == int.MinValue;
    }
    public static bool IsDefaultValue(double value)
    {
        return value == (double)int.MinValue;
    }
}
