using System;
using System.Collections.Generic;
using UnityEngine;
using Prefab;
using CharactersStats;


namespace Player
{
    public abstract class DefaultCharacterModelHolder<T> : ScriptableObject where T : Enum
    {
        [SerializeField]
        protected List<DefaultModel<T>> _models;

        public Stats GetDefaultStats(T modelType)
        {
            for (int i = 0; i < _models.Count; i++)
            {
                if (_models[i].Type.Equals(modelType))
                {
                    Stats stats = new Stats(_models[i].Speed, _models[i].Health, _models[i].Damage, _models[i].LaunchPower);
                    return stats;
                }
            }
            throw new System.ArgumentException(string.Format("Model of type {0} not exists at holder", modelType));
        }
    }
}
