using CharactersStats;
using Prefab;
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
    [field: SerializeField] public CharacterType PlayerType { get; set; }
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
        OriginalStats load = _dataService.PlayerStats.GetStats(PlayerType);
        if (load==null)
        {
            OriginalStats stats = _statsProvider.GetPlayerStats(PlayerType);
            CharacterModel savedModel = new CharacterModel(stats, PlayerType);
            _dataService.PlayerStats.SetStats(PlayerType, stats);
            _characterSelector.AddPanel(PlayerType);
            Destroy(gameObject);
        }
    }
}
