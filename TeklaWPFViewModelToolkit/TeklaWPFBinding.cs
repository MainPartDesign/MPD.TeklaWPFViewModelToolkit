using System.ComponentModel;

namespace MPD.TeklaWPFViewModelToolkit
{
    public interface ITeklaWPFBinding
    {
        string FieldName { get; }
        object Value { get; set; }
    }

    public class TeklaWPFBinding<T> : INotifyPropertyChanged, ITeklaWPFBinding 
    {
        private readonly string _fieldName;
        private T _value;

        public event PropertyChangedEventHandler PropertyChanged;
        public string FieldName => _fieldName;
        public T Value {
            get { return _value; }
            set {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        object ITeklaWPFBinding.Value { get => Value; set => Value = (T)value; }

        public TeklaWPFBinding(string propertyName)
        {
            _fieldName = propertyName;
        }
    }

}
