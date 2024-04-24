using CharactersStats;
public class CharacterUIModel
{
    public ReactiveInt ReactiveHealth;

    public CharacterUIModel(ReactiveStats stats)
    {
        ReactiveHealth = new ReactiveInt(stats.Health.Value);
    }

    public void SetHealth(int value)
    {
        ReactiveHealth.Value = value;
    }

}
