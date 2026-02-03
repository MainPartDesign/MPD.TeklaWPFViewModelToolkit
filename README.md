# TeklaWPFViewModelToolkit
### by [Main Part Design LTD](https://woodencon.com/)
[![NuGet](https://img.shields.io/nuget/v/TeklaPluginWPFViewModelToolkit.svg)](https://www.nuget.org/packages/TeklaPluginWPFViewModelToolkit/)

Use single template class to generate code for most of your Tekla Structures WPF plugin data model and view model and have a typesafe way of binding it in xaml.

# Usage

1. Install nuget package.
```bash
dotnet add package TeklaPluginWPFViewModelToolkit
```

2. Add using statement. 
```csharp
using MPD.TeklaWPFViewModelToolkit;
```

3.Add the `TemplateToGenerate` attribute to the class which fields you want to be used in your plugin data model and view model.
Since classes are partial - you can implement custom logic alongside the template.
```csharp
// This attribute is filtered by source generator
// and classes should be created based on its arguments.
[TemplateToGenerate("PluginModel", nameof(ViewModel))]
class ModelTemplate
{
    string text;
    int num1 = 1;
    double num2, num3;
}

public partial class ViewModel : IYourCustomInterface
{
    // Implement custom logic here.
}
```

4. You can wire up plugin data model and view model now. Gererated classes are easy to view and debug.
```csharp
[Plugin("WPFPlugin")]
[PluginUserInterface("WPFPlugin.MainWindow")]
public class WPFPlugin : PluginBase
{
    // Generated class here.
    private PluginModel _data;

    public WPFPlugin(PluginModel data)
    {
        _data = data;
    }

    public override bool Run(List<InputDefinition> Input)
    {
        // Properties will return value from initializer if Tekla will pass default value into plugin.
        var count = _data.Num1
        // Plugin Logic here.
    }
```

5. In the plugin's xaml window add this line for a type safety:
```xml
<tsd:PluginWindowBase
    x:Class="WPFPlugin.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ... important attributes are below ...
    xmlns:toolkit="clr-namespace:MPD.TeklaWPFViewModelToolkit;assembly=MPD.TeklaWPFViewModelToolkit"
    d:DataContext="{d:DesignInstance Type=local:ViewModel, IsDesignTimeCreatable=True}"
    >
```

6. Now in the xaml you can use typesafe binding with components `TeklaFilterCheckBoxWithInput` and  `TeklaLabelCheckBoxWithInput`.
```xml
<StackPanel Orientation="Horizontal">
    <Label
        x:Name="RegularLabel"
        Width="100"
        Height="30"
        Content="{tsd:Loc albl_Text}" />
    <toolkit:TeklaFilterCheckBoxWithInput
        x:Name="ItsWithoutBuiltinLabel"
        Property="{Binding Text}"
        Width="150"
        Height="30"
        />
</StackPanel>
<toolkit:TeklaLabelCheckBoxWithInput
    x:Name="WillHavePropertyNameAsLabel"
    Property="{Binding Num1}"
    LabelWidth="100"
    Width="250"
    Height="30"
    />
<!-- LabelWidth and LabelContent attributes are optional -->
<toolkit:TeklaLabelCheckBoxWithInput
    x:Name="HasOverwrittenLabelValue"
    LabelContent="{tsd:Loc albl_Num2}"
    Property="{Binding Num2}"
    Width="250"
    Height="30"
    />
```

7. Dont forget to pack `MPD.TeklaWPFViewModelToolkit.dll` along with your plugin .dll's!

# Additional Features

Attribute `ViewModelTypeOverride<T>` allows to change type of your `ViewModel` properties.
Attribute `UseInitializerValueOutsideRange` allows to set range for the variable, otside which `PluginModel` property will return the initializer value.

```csharp
[TemplateToGenerate("MyPluginModel", "MyViewModel")]
class ModelTemplate
{
    // Here double is used in MyViewModel.Num1 over default Distance type.
    [ViewModelTypeOverride<Tekla.Structures.Datatype.Double>]
    double num1 = 1.0

    // This attribute allows to rewrite Getter for MyPluginModel.Num2 property.
    // Tekla values outside this range in the property are replaced with initializer value (1).
    [UseInitializerValueOutsideRange(0.1, 10)]
    int num2 = 1;

    // For string value range is checked against string length.
    // Emty string or '0' from tekla will return "Steel_Undefined" from MyPluginModel.Material.
    [UseInitializerValueOutsideRange(2, 15)]
    string material = "Steel_Undefined";
}
```

# Legal Notice

This package is an independent development and is not affiliated with, sponsored by, or endorsed by Trimble Inc. "Tekla" and "Tekla Structures" are registered trademarks of Trimble Inc.

**Prerequisites**: This package references the Tekla.Structures.* NuGet packages provided by Trimble. This package does not include Trimble's proprietary binaries. To use this package, you must have a valid license for Tekla Structures and the necessary Tekla NuGet packages installed in your environment. Use of those dependencies is governed by the [Trimble End User License Agreement (EULA)](https://www.tekla.com/terms-and-conditions/eula).

## License

This project code is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Note: This license applies ONLY to the code in this repository, NOT to the Tekla API itself, 
which is subject to Trimble's licensing terms.
