using UnityEngine;
using UnityEngine.Playables;

public class GameManagerGeneral : MonoBehaviour
{
    [Header("Cinematica")]
    public static GameManagerGeneral Instancia { get; private set; }
    public PlayableDirector director; 

    [Header("UI")]
    [SerializeField] private GameObject uiJuego;
    
    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public GameObject PauseButton;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //cinematicas

    public void InicioJuegoCinematica()
    {
        director.Play();
    }

    //pause menu

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        PauseButton.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        PauseButton.SetActive(true);
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
