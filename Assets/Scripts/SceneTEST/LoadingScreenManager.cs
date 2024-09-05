using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    private static string[] scenesToLoad;
    private float delayBeforeUnload = 1f;
    private List<AsyncOperation> loadOperations = new List<AsyncOperation>();

    private void Start()
    {
        StartCoroutine(LoadScenesAsync());
    }

    public static void SetScenesToLoad(string[] scenes)
    {
        scenesToLoad = scenes;
    }

    private IEnumerator LoadScenesAsync()
    {
        foreach (string sceneName in scenesToLoad)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            loadOperation.allowSceneActivation = false;
            loadOperations.Add(loadOperation);

            while (loadOperation.isDone)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(delayBeforeUnload);

        SceneManager.UnloadSceneAsync(gameObject.scene);

        foreach (var loadOperation in loadOperations)
        {
            loadOperation.allowSceneActivation = true;

            while (!loadOperation.isDone)
            {
                yield return null;
            }
        }
    }
}