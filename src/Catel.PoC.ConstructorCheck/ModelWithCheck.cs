namespace Catel.PoC.ConstructorCheck
{
    public class ModelWithCheck : IModel
    {
        private bool _finishedConstructor = false;
        private string _value;

        public ModelWithCheck()
        {
            Value = "initial value";
        }

        private bool IsRunningInConstructor
        {
            get
            {
                if (!_finishedConstructor)
                {
                    var stackTrace = new System.Diagnostics.StackTrace(true);

                    var finished = true;
                    var thisType = GetType();
                    var lastBaseClass = thisType;

                    // We can skip the first few frames, because they are not relevant
                    for (var i = 1; i < stackTrace.FrameCount; i++)
                    {
                        var frame = stackTrace.GetFrame(i);
                        if (frame is null)
                        {
                            continue;
                        }

                        var methodInfo = frame.GetMethod();
                        if (methodInfo is not null)
                        {
                            var declaringType = methodInfo.DeclaringType;
                            if (declaringType is not null)
                            {
                                if (declaringType == thisType)
                                {
                                    if (methodInfo.IsConstructor)
                                    {
                                        finished = false;
                                        break;
                                    }

                                    continue;
                                }

                                if (!declaringType.IsAssignableFrom(typeof(ModelWithCheck)))
                                {
                                    // Left the type, finished constructor
                                    break;
                                }

                                if (methodInfo.IsConstructor)
                                {
                                    finished = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (finished)
                    {
                        _finishedConstructor = true;
                    }
                }

                return !_finishedConstructor;
            }
        }

        public int ChangeCount { get; set; }

        public string Value
        {
            get => _value;
            set
            {
                _value = value;

                if (!IsRunningInConstructor)
                {
                    OnValueChanged();
                }
            }
        }

        private void OnValueChanged()
        {
            ChangeCount++;
        }
    }
}
