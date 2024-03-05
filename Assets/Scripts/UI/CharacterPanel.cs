using CharactersStats;
using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public interface ICharacterPanel
    {
        public SavedModel IDModel { get; set; }
    }
    public class CharacterPanel : MonoBehaviour, ICharacterPanel
    {
        public SavedModel IDModel { get; set; }
        [field: SerializeField] public PlayerType PlayerType { get; set; }
        private Toggle _toggle;
        [Inject]
        private IStatsProvider _statsProvider;
        private bool _isEnabled;

        void Start()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(ChangeBool);
        }
        public void ChangeBool(bool toggleValue)
        {
            _isEnabled = toggleValue;
        }

        private SavedModel GetSavedModel()
        {
            Stats stats = _statsProvider.GetCharacterStats(PlayerType);
            SavedModel savedModel= new SavedModel();
            savedModel.Speed = stats.Speed;
            savedModel.Health = stats.Health;
            savedModel.Damage = stats.Damage;
            savedModel.ID = GenerateID();
            DataTransfer.IdCollection.Add(savedModel.ID);
            return savedModel;
        }

        private int GenerateID()
        {
            int id = Random.Range(0, 1000);
            return id;
        }
    }
}
