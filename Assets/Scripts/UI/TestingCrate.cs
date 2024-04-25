using CharactersStats;
using Prefab;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using SaveSystem;
using System.Collections.Generic;
using Interactions;

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
        if (load==null)
        {
            OriginStats stats = _statsProvider.GetPlayerStats(CharacterType);
            List<InteractionType> abilities = _statsProvider.GetPlayerAbilitiesTypes(CharacterType);
            //CharacterModel savedModel = new CharacterModel(stats, CharacterType, abilities);
            _dataService.PlayerData.SetStats(CharacterType, stats);
            //_dataService.PlayerData.SetAbilities(abilities,CharacterType);
            _characterSelector.AddPanel(CharacterType);
            Destroy(gameObject);
        }
        //GPrefs.DeleteAll();
    }
}
