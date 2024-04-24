using Pool;
using UnityEngine;

public interface ICharacterUIView
{
    public void Init(ICharacterView view);
}

public class CharacterUIView : MonoBehaviour, IMyPoolable
{
    private ICharacterView _characterView;

    public void Init(ICharacterView view)
    {
        _characterView = view;
    }

    private void FixedUpdate()
    {
        Vector3 position = _characterView.GetRigidbody().position;
        transform.position = new Vector3(position.x, position.y + 1.1f, position.z);
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
