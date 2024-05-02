using CharactersStats;

public interface ICharacterUIFactory
{
    public CharacterUIViewmodel CreateViewModel(CharacterModel model, IUIFactory uIFactory, CharacterUIView uIView);
}
public class CharacterUIFactory : ICharacterUIFactory
{

    public CharacterUIViewmodel CreateViewModel(CharacterModel model, IUIFactory uIFactory, CharacterUIView uIView)
    {
        CharacterUIViewmodel modelView = new CharacterUIViewmodel();
        modelView.Init(model, uIFactory, uIView);
        return modelView;
    }

    
}
