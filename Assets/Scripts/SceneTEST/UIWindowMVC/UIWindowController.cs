public interface IUIWindowController
{
    public void Init(IWindowView view, UIWindowModel model);
    public void SetActive();

    public void SetDisactive();
}

