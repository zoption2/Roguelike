using CharactersStats;
public class CharacterUIModel
{
    public ReactiveInt ReactiveHealth;

    public CharacterUIModel(ReactiveStats stats)
    {
        ReactiveHealth = stats.Health;
    }

    public void SetHealth(int value)
    {
        ReactiveHealth.Value = value;
    }

}
