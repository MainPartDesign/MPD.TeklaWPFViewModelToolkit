using System;

namespace TeklaWPFViewModelToolkit
{
    /// <summary>
    /// Attribute to mark class as a template for source generator, which outputs partial classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TemplateToGenerateAttribute : Attribute
    {
        public string ModelClassName { get; }
        public string ViewModelClassName { get; }

        public TemplateToGenerateAttribute(string modelClassName, string viewModelClassName)
        {
            ModelClassName = modelClassName;
            ViewModelClassName = viewModelClassName;
        }
    }
}
