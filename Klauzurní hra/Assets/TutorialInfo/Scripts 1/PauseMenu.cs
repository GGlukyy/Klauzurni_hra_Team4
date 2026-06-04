using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false; // Statická promìnná, aby ostatní scripty vìdìly, jestli je pauza

    [Header("UI Panels")]
    public GameObject pauseMenuUI;

    void Start()
    {
        // Ujistíme se, že menu je na zaèátku vypnuté a hra bìží
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Detekce stisknutí klávesy ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Vrátí èas do normálu
        GameIsPaused = false;

        // Zamkne a schová myš pro hraní (dùležité pro 3D hru)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Zastaví èas (fyziku, animace atd.)
        GameIsPaused = true;

        // Odemkne a ukáže myš, abys mohl klikat na UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMainMenu()
    {
        // DÙLEŽITÉ: Pøed naètením menu musíme vrátit èas na 1, jinak by menu bylo "zamrzlé"
        Time.timeScale = 1f;
        GameIsPaused = false;

        // Naète scénu s indexem 0 (což by mìlo být tvé Main Menu)
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}