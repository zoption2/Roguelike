using CharactersStats;
using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using SaveSystem;

public class TestingCrate : MonoBehaviour
{
    private const string LAST_ID = "LastID";
    private IStatsProvider _statsProvider;
    private Button _crateButton;
    private ICharacterSelector _characterSelector;
    private IDataService _dataService;
    [field: SerializeField] public PlayerType PlayerType { get; set; }
    void Start()
    {
        _crateButton = GetComponent<Button>();
        _crateButton.onClick.AddListener(CreateSavedModel);
    }

    [Inject]
    public void Construct(IStatsProvider statsProvider, ICharacterSelector characterSelector,
        IDataService dataService)
    {
        _statsProvider = statsProvider;
        _characterSelector = characterSelector;
        _dataService = dataService;
    }

    private void CreateSavedModel()
    {
        Stats load = _dataService.PlayerStats.GetStats(PlayerType);
        if (load==null)
        {
            Stats stats = _statsProvider.GetPlayerStats(PlayerType);
            PlayerModel savedModel = new PlayerModel(stats, PlayerType);
            _dataService.PlayerStats.SetStats(PlayerType, stats);
            _characterSelector.AddPanel(PlayerType);
            Destroy(gameObject);
        }
    }
}
