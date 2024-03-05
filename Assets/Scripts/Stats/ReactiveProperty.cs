using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharactersStats
{
    public class ReactiveProperty<T> : IDisposable
    {
        private event Action<T> ON_VALUE_CHANGED;
        protected T value;
        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                if (!Equals(value, this.value))
                {
                    this.value = value;
                    ON_VALUE_CHANGED?.Invoke(value);
                }
            }
        }

        public ReactiveProperty(T value)
        {
            this.value = value;
        }

        public ReactiveProperty()
        {

        }

        public void Subscribe(Action<T> action)
        {
            ON_VALUE_CHANGED += action;
        }

        public void Unsubscribe(Action<T> action)
        {
            ON_VALUE_CHANGED -= action;
        }

        public void Dispose()
        {
            ON_VALUE_CHANGED = null;
            GC.SuppressFinalize(this);
        }
    }

    public class ReactiveInt : ReactiveProperty<int> 
    {
        public ReactiveInt()
        {
            
        }

        public ReactiveInt(int value)
        {
            this.value= value;
        }
    }

    public class ReactiveFloat : ReactiveProperty<float>
    {
        public ReactiveFloat()
        {

        }

        public ReactiveFloat(float value)
        {
            this.value = value;
        }
    }

    public static class ReactivePropertyExtensions
    {
        public static ReactiveProperty<T> ToDisposableList<T>(this ReactiveProperty<T> property, List<IDisposable> collection)
        {
            if (collection is null)
            {
                collection = new List<IDisposable>(1);
            }

            collection.Add(property);

            return property;
        }

    }

}
