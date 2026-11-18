using System;
using System.Collections.Generic;

namespace UnityCore
{
    public sealed class BindableProperty<T>
    {
        private T _value;

        public BindableProperty(T value = default)
        {
            _value = value;
        }

        public Action OnChangeBefore;
        public Action OnChange;

        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value))
                    return;

                OnChangeBefore?.Invoke();
                _value = value;
                OnChange?.Invoke();
            }
        }

        public void Clear() => _value = default;

        public override string ToString() => _value == null ? "null" : Convert.ToString(_value);

        public static implicit operator T(BindableProperty<T> property) => property.Value;
    }
}
