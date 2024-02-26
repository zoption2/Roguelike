using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharactersStats
{
    public class ReactiveProperty<T>
    {
        public event Action<T> On_Value_Changed;
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
                    value = this.value;
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
            On_Value_Changed += action;
        }

        public void Unsubscribe(Action<T> action)
        {
            On_Value_Changed -= action;
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

}
