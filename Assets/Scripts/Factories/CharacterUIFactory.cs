using CharactersStats;

public interface ICharacterUIFactory
{
    public CharacterUIViewmodel CreateViewModel(ReactiveStats stats, IUIFactory uIFactory, CharacterUIView uIView);
}
public class CharacterUIFactory : ICharacterUIFactory
{

    public CharacterUIViewmodel CreateViewModel(ReactiveStats stats, IUIFactory uIFactory, CharacterUIView uIView)
    {
        CharacterUIModel model = new CharacterUIModel(stats);
        CharacterUIViewmodel modelView = new CharacterUIViewmodel();
        modelView.Init(model, uIFactory, uIView);
        return modelView;
    }

    
}
