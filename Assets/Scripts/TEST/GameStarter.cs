using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameStarter : MonoBehaviour
{
    private IGameManager _gameManager;

    [Inject]
    public void Construct(IGameManager gameManager)
    {
        _gameManager = gameManager;
    }



    void Start()
    {
        _gameManager.Init();

        var container = FindObjectOfType<ProjectContext>().Container;
        container.Inject(this);

        LoadingScreenManager.SetScenesToLoad(new string[] { "Menu" });
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);

    }

}