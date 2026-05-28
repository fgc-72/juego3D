using UnityEngine;

public class PauseMenuManager: MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject PauseButton;
    [SerializeField] public GameObject UIJuego;

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        UIJuego.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        UIJuego.SetActive(true);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
