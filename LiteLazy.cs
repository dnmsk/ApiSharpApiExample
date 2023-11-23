using System;

namespace ApiSharpApiExample
{

    public class LiteLazy<T> where T : class {
        private Func<T> _getter;
        private object _lockObj = new object();
        private volatile T _value;

//        public LiteLazy(Func<Task<T>> taskGetter) : this(() => taskGetter().GetAwaiter().GetResult()) {}

        public LiteLazy(Func<T> getter) {
            _getter = getter;
        }

        protected LiteLazy(T result) {
            _value = result;
        }

        public void Destroy() {
            _value = null;
        }

        public T Value {
            get {
                if (_value != null) return _value;

                if (_getter == null) {
                    return null;
                }

                var lockObj = _lockObj;
                if (lockObj == null) return _value;

                lock (lockObj) {
                    if (_value != null) {
                        return _value;
                    }
                    try {
                        var value = _getter();
                        _value = value;
                        _getter = null;
                    }
                    catch (Exception ex) {
                    }
                }
                _lockObj = null;

                return _value;
            }
        }
    }
}
