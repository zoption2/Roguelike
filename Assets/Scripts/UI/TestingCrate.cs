using CharactersStats;
using Prefab;
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
    [field: SerializeField] public CharacterType PlayerType { get; set; }
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
        CharacterModel load = _saveSystem.Load(PlayerType);
        if (load==null)
        {
            OriginStats stats = _statsProvider.GetPlayerStats(PlayerType);
            CharacterModel savedModel = new CharacterModel(stats, PlayerType);
            _saveSystem.Save(savedModel);
            _characterSelector.AddPanel(PlayerType);
            Destroy(gameObject);
        }
    }
}
