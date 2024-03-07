using CharactersStats;
using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TestingCrate : MonoBehaviour
{
    private const string LAST_ID = "LastID";
    private IStatsProvider _statsProvider;
    private ModelSaveSystem _saveSystem;
    private Button _crateButton;
    [field: SerializeField] public PlayerType PlayerType { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        _saveSystem = ModelSaveSystem.GetInstance();
        _crateButton = GetComponent<Button>();
        _crateButton.onClick.AddListener(CreateSavedModel);
    }


    [Inject]
    public void Construct(IStatsProvider statsProvider)
    {
        _statsProvider = statsProvider;
    }

    private void CreateSavedModel()
    {
        Stats stats = _statsProvider.GetCharacterStats(PlayerType);
        int id = GenerateID();
        SavedModel savedModel = new SavedModel(stats, id, PlayerType);
        _saveSystem.Save(savedModel);
        Destroy(gameObject);
    }

    private int GenerateID()
    {
        int lastID = PlayerPrefs.GetInt(LAST_ID, 1);
        PlayerPrefs.SetInt(LAST_ID, ++lastID);
        return lastID;
    }
}
