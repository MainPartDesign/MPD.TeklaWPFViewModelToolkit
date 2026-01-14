using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MPD.TeklaWPFViewModelToolkit
{
    public partial class TeklaLabelCheckBoxWithInput : UserControl
    {
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register(
                nameof(Property),
                typeof(ITeklaWPFBinding),
                typeof(TeklaLabelCheckBoxWithInput),
                new PropertyMetadata(null, OnBindingChanged));

        public static readonly DependencyProperty LabelContentProperty =
            DependencyProperty.Register(
                nameof(LabelContent),
                typeof(string),
                typeof(TeklaLabelCheckBoxWithInput),
                new PropertyMetadata(null, OnBindingChanged));

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register(
                nameof(LabelWidth),
                typeof(GridLength),
                typeof(TeklaLabelCheckBoxWithInput),
                new PropertyMetadata(GridLength.Auto, OnBindingChanged));

        public ITeklaWPFBinding Property
        {
            get => (ITeklaWPFBinding)GetValue(BindingProperty);
            set => SetValue(BindingProperty, value);
        }

        public string LabelContent
        {
            get => (string)GetValue(LabelContentProperty);
            set => SetValue(LabelContentProperty, value);
        }

        public GridLength LabelWidth
        {
            get => (GridLength)GetValue(LabelWidthProperty);
            set => SetValue(LabelWidthProperty, value);
        }

        public TeklaLabelCheckBoxWithInput()
        {
            InitializeComponent();
        }

        private static void OnBindingChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (TeklaLabelCheckBoxWithInput)d;
            control.ApplyBinding();
        }
        private void ApplyBinding()
        {
            if (this.Content is Grid mainGrid)
            {
                mainGrid.ColumnDefinitions[0].Width = LabelWidth;
            }

            if (!string.IsNullOrEmpty(LabelContent))
            {
                VariableLabel.Content = LabelContent;
            }
            else if (Property != null)
            {
                VariableLabel.Content = Property.FieldName;
            }

            if (Property == null)
                return;

            FilterCheckBox.AttributeName = Property.FieldName;

            InputTextBox.SetBinding(
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
