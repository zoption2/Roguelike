using System;
using System.Collections.Generic;
using UnityEngine;
using Prefab;


namespace Player
{
    public interface ISavedCharacterModelHolder
    {
        Stats GetSavedStats(int receivedID);
    }
    public abstract class DefaultCharacterModelHolder<T> : ScriptableObject where T : Enum
    {
        [Serializable]
        public class CleanModelMapper
        {
            public T Key;
            public int Health, Damage, Speed;
        }

        [SerializeField]
        protected List<CleanModelMapper> _models;

        public Stats GetDefaultStats(T modelType)
        {
            for (int i = 0; i < _models.Count; i++)
            {
                if (_models[i].Key.Equals(modelType))
                {
                    Stats stats = new Stats(_models[i].Speed, _models[i].Health, _models[i].Damage);
                    return stats;
                }
            }
            throw new System.ArgumentException(string.Format("Model of type {0} not exists at holder", modelType));
        }
    }

    [CreateAssetMenu]
    public class DefaultPlayerModelHolder : DefaultCharacterModelHolder<PlayerType>
    {

    }

    [CreateAssetMenu]
    public class SavedCharacterModelHolder : ScriptableObject, ISavedCharacterModelHolder 
    {
        [Serializable]
        public class SavedModelMapper
        {
            public int ID;
            public int Health,Damage,Speed;
        }

        [SerializeField]
        protected List<SavedModelMapper> _models;

        public Stats GetSavedStats(int receivedID)
        {
            for (int i = 0; i < _models.Count; i++)
            {
                if (_models[i].ID.Equals(receivedID))
                {
                    Stats stats = new Stats(_models[i].Speed, _models[i].Health, _models[i].Damage);
                    return stats;
                }
            }
            Debug.LogWarning("Saved model with such id was not found");
            return null;
        }
    }
}
