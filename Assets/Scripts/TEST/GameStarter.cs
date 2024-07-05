using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{

    void Start()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
    }

}
