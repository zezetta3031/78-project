using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();

        Debug.Log("Game is exiting...");
        
#if UNITY_EDITOR
        // Exits Play Mode when testing in the Editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
