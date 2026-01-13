using System;

namespace MPD.TeklaWPFViewModelToolkit
{
    /// <summary>
    /// Attribute to change Tekla's dialog property type in generated viewModel, use Tekla datatype as generic type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class ViewModelTypeOverrideAttribute<T> : Attribute where T : Tekla.Structures.Datatype.IDataType
    {
        public Type ViewModelPropertyType { get; }

        public ViewModelTypeOverrideAttribute()
        {
            ViewModelPropertyType = typeof(T);
        }
    }
}
