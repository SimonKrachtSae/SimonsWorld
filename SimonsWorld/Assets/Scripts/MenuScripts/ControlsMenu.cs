using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsMenu : MonoBehaviour
{
    public void GoToStartScene()
    {
        SceneManager.LoadScene(0);
    }
}
