namespace Catel.PoC.ConstructorCheck
{
    public class ModelWithoutCheck : IModel
    {
        private string _value;

        public ModelWithoutCheck()
        {
            Value = "initial value";
        }

        public int ChangeCount { get; set; }

        public string Value
        {
            get => _value;
            set
            {
                _value = value;

                OnValueChanged();
            }
        }

        private void OnValueChanged()
        {
            lock (this)
            {
                ChangeCount++;
            }
        }
    }
}
