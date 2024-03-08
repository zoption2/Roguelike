using Gameplay;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Zenject;

public class MenuStarter : MonoBehaviour
{

    private ICharacterSelector _characterSelector;
    [SerializeField]
    private RectTransform _rectTransform;
    [SerializeField]
    private int _requiredPlayers = 1;

    [Inject]
    public void Construct(ICharacterSelector characterSelector)
    {
        _characterSelector = characterSelector;
    }
    
    void Start()
    {
        _characterSelector.Init(_requiredPlayers,_rectTransform);
    }

}
