using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using TeklaWPFViewModelToolkit;

namespace TeklaWPFViewModelGenerator.IntegrationTests;

[TemplateToGenerate("PluginModelDummy", "ViewModelDummy")]
internal class ModelTemplateDummy
{
    string test;
    int check;
    double doubleCheck;
}

public class TemplateToGenerateAttributeTest
{
    [Fact]
    public void PluginModel_Creation_DoesntThrow()
    {
        object boxedDummy = null;
        var exception = Record.Exception(() => boxedDummy = new PluginModelDummy());

        Assert.Null(exception);
        Assert.NotNull(boxedDummy);
    }

    [Fact]
    public void ViewModel_Creation_DoesntThrow()
    {
        object boxedDummy = null;
        var exception = Record.Exception(() => boxedDummy = new ViewModelDummy());

        Assert.Null(exception);
        Assert.NotNull(boxedDummy);
    }
}
