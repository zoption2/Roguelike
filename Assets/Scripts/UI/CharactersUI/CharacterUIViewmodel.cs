using CharactersStats;
using Pool;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIViewmodel
{
    public ReactiveInt ReactiveHealth;

    private CharacterUIModel _model;

    public void Init(CharacterUIModel model)
    {
        _model = model;
        ReactiveHealth = model.ReactiveHealth;
        //ReactiveHealth = new ReactiveInt(model.ReactiveHealth.Value);
    }

    public void UpdateStats(ReactiveStats stats)
    {
        ReactiveHealth = stats.Health;
        _model.SetHealth(ReactiveHealth.Value);
    }

    public void AddItemToGrid(GameObject itemPrefab, Transform parentTransform)
    {
        GameObject newItem = UnityEngine.Object.Instantiate(itemPrefab, parentTransform);
    }


    public void Submit()
    {
        _model.SetHealth(ReactiveHealth.Value);
    }
}
