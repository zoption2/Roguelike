using CharactersStats;
using UnityEngine;

public interface ICharacterUIFactory
{
    public CharacterUIViewmodel CreateViewModel(ReactiveStats stats);
}
public class CharacterUIFactory : ICharacterUIFactory
{
    public CharacterUIViewmodel CreateViewModel(ReactiveStats stats)
    {
        CharacterUIModel model = new CharacterUIModel(stats);
        CharacterUIViewmodel modelView = new CharacterUIViewmodel();
        modelView.Init(model);
        return modelView;
    }
}
