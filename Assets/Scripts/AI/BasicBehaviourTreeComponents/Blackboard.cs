using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Blackboard
    {
        private Dictionary<string, object> _dictionary;

        public Blackboard()
        {
            _dictionary = new Dictionary<string, object>();
        }

        public void SetData(string key, object value)
        {
            _dictionary[key] = value;
        }

        public object GetData(string key)
        {
            if (_dictionary.ContainsKey(key))
                return _dictionary[key];
            else
                return null;
        }
    }
}
