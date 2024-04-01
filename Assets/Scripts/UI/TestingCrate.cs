using CharactersStats;
using Prefab;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using SaveSystem;

public class TestingCrate : MonoBehaviour
{
    private IDataService _dataService;
    private const string LAST_ID = "LastID";
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
        OriginStats load = _dataService.PlayerStats.GetStats(CharacterType);
        if (load==null)
        {
            OriginStats stats = _statsProvider.GetPlayerStats(CharacterType);
            CharacterModel savedModel = new CharacterModel(stats, CharacterType);
            _dataService.PlayerStats.SetStats(CharacterType, stats);
            _characterSelector.AddPanel(CharacterType);
            Destroy(gameObject);
        }
    }
}
