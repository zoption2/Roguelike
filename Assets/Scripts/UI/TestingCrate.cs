using CharactersStats;
using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TestingCrate : MonoBehaviour
{
    private const string LAST_ID = "LastID";
    private IStatsProvider _statsProvider;
    private ModelSaveSystem _saveSystem;
    private Button _crateButton;
    private ICharacterSelector _characterSelector;
    [field: SerializeField] public PlayerType PlayerType { get; set; }
    void Start()
    {
        _saveSystem = ModelSaveSystem.GetInstance();
        _crateButton = GetComponent<Button>();
        _crateButton.onClick.AddListener(CreateSavedModel);
    }

    [Inject]
    public void Construct(IStatsProvider statsProvider, ICharacterSelector characterSelector)
    {
        _statsProvider = statsProvider;
        _characterSelector = characterSelector;
    }

    private void CreateSavedModel()
    {
        PlayerModel load = _saveSystem.Load(PlayerType);
        if (load==null)
        {
            Stats stats = _statsProvider.GetPlayerStats(PlayerType);
            int id = GenerateID();
            PlayerModel savedModel = new PlayerModel(stats, PlayerType, id);
            _saveSystem.Save(savedModel);
            _characterSelector.AddPanel(PlayerType);
            Destroy(gameObject);
        }
    }

    private int GenerateID()
    {
        int lastID = PlayerPrefs.GetInt(LAST_ID, 1);
        PlayerPrefs.SetInt(LAST_ID, ++lastID);
        return lastID;
    }
}
