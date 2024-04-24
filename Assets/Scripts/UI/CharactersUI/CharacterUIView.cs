using Pool;
using UnityEngine;
using UnityEngine.UI;

public interface ICharacterUIView
{
    public void Init(CharacterView view, CharacterUIViewmodel viewmodel);
}

public class CharacterUIView : MonoBehaviour, IMyPoolable, ICharacterUIView
{
    [SerializeField] Scrollbar _scrollbar;

    private ICharacterView _characterView;
    private CharacterUIViewmodel _viewmodel;

    public void Init(CharacterView view, CharacterUIViewmodel viewmodel)
    {
        _characterView = view;
        _viewmodel = viewmodel;
        Debug.LogError("Hello! My HP:" + _viewmodel.ReactiveHealth.Value);
        //gameObject.SetActive(false);
        _scrollbar.size = _viewmodel.ReactiveHealth.Value;
    }

    private void FixedUpdate()
    {
        transform.position = _characterView.transform.position;
        //_scrollbar.size 
    }

    public void OnCreate()
    {
    }

    public void OnPull()
    {
    }

    public void OnRelease()
    {
    }

    
}
