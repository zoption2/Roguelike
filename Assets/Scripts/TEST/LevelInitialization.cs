using Gameplay;
using UnityEngine;

public class LevelInitilization : MonoBehaviour
{
    public IGameplayService _gameplayService;

    public void Init(IGameplayService gameplayService)
    {
        _gameplayService = gameplayService;
        _gameplayService.LevelContext = new LevelContext();

        Debug.LogWarning("Level initialization");
    }
}
