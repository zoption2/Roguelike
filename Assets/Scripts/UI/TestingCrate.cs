using CharactersStats;
using SaveSystem;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TestingCrate : MonoBehaviour
{
    private IDataService _dataService;
    private IStatsProvider _statsProvider;
    private Button _crateButton;
    private ICharacterSelector _characterSelector;
    [field: SerializeField] public CharacterType CharacterType { get; set; }
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
        OriginStats load = _dataService.PlayerData.GetStats(CharacterType);
        //Debug.Log(GPrefs.dataPath);
        if (load == null)
        {
            OriginStats stats = _statsProvider.GetPlayerStats(CharacterType);
            _dataService.PlayerData.SetStats(CharacterType, stats);
            _characterSelector.AddPanel(CharacterType);
            Destroy(gameObject);
        }
        //GPrefs.DeleteAll();
    }

    [ContextMenu("Reset Saves")]
    private void ResetPlayerSaves()
    {
        List<CharacterType> characterTypes = _dataService.PlayerData.GetAvailablePlayers();
        foreach (CharacterType characterType in characterTypes)
        {
            OriginStats defaultStats = _statsProvider.GetDefaultPlayerStats(characterType);
            _dataService.PlayerData.SetStats(characterType, defaultStats);
        }
        Debug.LogWarning("Saves were reset to default values!");
    }
}
