using System.Windows;
using System.Windows.Controls;

namespace TeklaWPFViewModelToolkit 
{
    public partial class TeklaFilterCheckBoxWithInput : UserControl
    {
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register(
                nameof(Property),
                typeof(ITeklaWPFBinding),
                typeof(TeklaFilterCheckBoxWithInput),
                new PropertyMetadata(null, OnBindingChanged));

        public ITeklaWPFBinding Property
        {
            get => (ITeklaWPFBinding)GetValue(BindingProperty);
            set => SetValue(BindingProperty, value);
        }

        public TeklaFilterCheckBoxWithInput()
        {
            InitializeComponent();
        }

        private static void OnBindingChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (TeklaFilterCheckBoxWithInput)d;
            control.ApplyBinding();
        }
        private void ApplyBinding()
        {
            if (Property == null)
                return;

            FilterCheckBox.AttributeName = Property.FieldName;

            TextBox.SetBinding(
                TextBox.TextProperty,
                new System.Windows.Data.Binding(nameof(ITeklaWPFBinding.Value))
                {
                    Source = Property,
                    Mode = System.Windows.Data.BindingMode.TwoWay,
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
                });
        }
    }
}
