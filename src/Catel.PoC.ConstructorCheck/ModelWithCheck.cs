namespace Catel.PoC.ConstructorCheck
{
    public class ModelWithCheck : IModel
    {
        private bool _finishedConstructor = false;

        // Note: when the next line is commented, it fails. Not sure why, is it because
        // the _finishedConstructor is not set fast enough?
        //private bool _firstCall = true;

        private int _timesIsConstructorFinishedCalled = 0;

        private string _value;

        public ModelWithCheck()
        {
            Value = "initial value";
        }

        private bool IsConstructorFinished
        {
            get
            {
                _timesIsConstructorFinishedCalled++;

                if (!_finishedConstructor)
                {
                    // We can skip the first few frames, because they are not relevant
                    var stackTrace = new System.Diagnostics.StackTrace(1, false);

                    var finished = true;
                    var thisType = GetType();

                    for (var i = 0; i < stackTrace.FrameCount; i++)
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
                        FirstValidStackTrace = $"{stackTrace} | {_timesIsConstructorFinishedCalled}";

                        _finishedConstructor = true;
                    }
                }

                return _finishedConstructor;
            }
        }

        public string FirstValidStackTrace { get; private set; }

        public int ChangeCount { get; set; }

        public string Value
        {
            get => _value;
            set
            {
                _value = value;

                if (IsConstructorFinished)
                {
                    OnValueChanged();
                }
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
