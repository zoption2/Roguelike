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
        [SerializeField]
        protected List<DefaultModel<T>> _models;

        public Stats GetDefaultStats(T modelType)
        {
            for (int i = 0; i < _models.Count; i++)
            {
                if (_models[i].Type.Equals(modelType))
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
        [SerializeField]
        protected List<SavedModel> _models;

        public Stats GetSavedStats(int receivedID)
        {
            for (int i = 0; i < _models.Count; i++)
            {
                var model = _models[i];
                if (model.ID.Equals(receivedID))
                {
                    Stats stats = new Stats(model.Speed, model.Health, model.Damage);
                    return stats;
                }
            }
            Debug.LogWarning("Saved model with such id was not found");
            return null;
        }
    }

    [CreateAssetMenu]
    public class SavedModel : ScriptableObject
    {
        public int ID;
        public int Health, Damage, Speed;
    }

    public class DefaultModel<T> : ScriptableObject where T : Enum
    {
        public T Type;
        public int Health, Damage, Speed;
    }

    [CreateAssetMenu]
    public class DefaultPlayerModel : DefaultModel<PlayerType> { }
}
