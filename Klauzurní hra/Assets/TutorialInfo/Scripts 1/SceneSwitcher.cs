using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    /// <summary>
    /// Loads the next scene in the Build Settings queue.
    /// Call this from a Timeline Signal or Animation Event.
    /// </summary>
    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    /// <summary>
    /// Loads a specific scene by its name (e.g., "Level 1_Popirani").
    /// </summary>
    /// <param name="sceneName">Exact name of the scene</param>
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}