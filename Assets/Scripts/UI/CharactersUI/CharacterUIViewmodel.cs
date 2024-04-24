using CharactersStats;

public class CharacterUIViewmodel
{
    public ReactiveInt ReactiveHealth;

    private CharacterUIModel _model;

    public void Init(CharacterUIModel model)
    {
        _model = model;
        ReactiveHealth = new ReactiveInt(model.ReactiveHealth.Value);
    }

    public void DoHealthDown()
    {
        ReactiveHealth.Value--;
    }

    public void Submit()
    {
        _model.SetHealth(ReactiveHealth.Value);
    }
}
