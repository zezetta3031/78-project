using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    public void OpenLevelSelect()
    {
        SceneManager.LoadScene("Level Select");
    }
    
    public void OpenMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
