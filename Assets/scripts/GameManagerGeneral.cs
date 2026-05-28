using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManagerGeneral : MonoBehaviour
{
    [Header("Cinematica")]
    public static GameManagerGeneral Instancia { get; private set; }
    public PlayableDirector director; 

    [Header("UI")]
    [SerializeField] private GameObject uiJuego;


    [Header("Paneles de carga")]
    [SerializeField] GameObject panelCargaViaje;
    [SerializeField] GameObject panelCargaVictoria;
    [SerializeField] GameObject panelRecursos;
    [SerializeField] GameObject panelCargaDerrota;
    float tiempoCarga = 2f;
    public int nivelCiudad = 0; 

    
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

    void Start()
    {
        // Inicializa el inventario con los valores de nivel 0
        AplicarBeneficiosDeNivel();
        StartCoroutine(IntroBruja());
    }

    IEnumerator IntroBruja()
    {
         yield return new WaitForSeconds(2f);

        texto.Instancia.MostrarMensaje(
            "Eso estuvo cerca, casi me atrapan los cazadores..."
        );

        yield return new WaitForSeconds(4f);

        texto.Instancia.MostrarMensaje(
            "por suerte tenia mi escoba cerca"
        );

         yield return new WaitForSeconds(2f);

        texto.Instancia.MostrarMensaje(
            "ya no tengo magia para seguir volando, creo que nadie vive en esta granja... la usaré como refugio por ahora"
        );

         yield return new WaitForSeconds(5f);

        texto.Instancia.MostrarMensaje(
            "tal vez pueda aprovchar la arena para crear unos cuantos soldados de cristal... lo que le hicieron a mis hermanas lo van a pagar"
        );
         yield return new WaitForSeconds(3f);
    }

    public void AplicarBeneficiosDeNivel()
    {
        var inv = InventarioJugador.Instancia;
        if (inv == null) return;

        switch (nivelCiudad)
        {
            case 0: inv.magia = 20; break;
            case 1: inv.magia = 40; break;
            case 2: inv.magia = 60; break;
        }

        Debug.Log($"Nivel {nivelCiudad} — Magia: {inv.magia}");
    }

    public bool PuedeViajarACiudad() => nivelCiudad > 0;

    public void SubirNivel()
    {
        nivelCiudad = Mathf.Min(nivelCiudad + 1, 2);
        AplicarBeneficiosDeNivel();
    }

    public void InicioJuego()
    {
        if(nivelCiudad == 0)
            StartCoroutine(IntroBruja());
        else
            pauseMenu.SetActive(false);
            uiJuego.SetActive(true);
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
        uiJuego.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        uiJuego.SetActive(true);
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

    /*viajes*/

    public void ViajarACiudad(int nivel)
    {
        if (!PuedeViajarACiudad())
        {
            texto.Instancia.MostrarMensaje("Necesitas subir de nivel primero");
            Debug.Log("Necesitas subir de nivel primero");
            return;
        }

        // NUEVO
        if (!InventarioJugador.Instancia.TieneAnimales())
        {
            texto.Instancia.MostrarMensaje("Necesitas fabricar animales para viajar");
            return;
        }
        
        StartCoroutine(CargarConPanel(panelCargaViaje, "ciudad1"));
    }

    public void FinCiudadGanaste()
    {
        StartCoroutine(CargarConPanel(panelCargaVictoria, "SampleScene"));
    }

    public void FinCiudadPerdiste()
    {
        StartCoroutine(CargarConPanel(panelCargaDerrota, "SampleScene"));
    }

    IEnumerator CargarConPanel(GameObject panel, string escenaDestino)
    {
        panelRecursos.SetActive(false);
        uiJuego.SetActive(false);
        panel.SetActive(true);

        AsyncOperation carga = SceneManager.LoadSceneAsync(escenaDestino);
        carga.allowSceneActivation = false;

        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < tiempoCarga || carga.progress < 0.9f)
        {
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        carga.allowSceneActivation = true;
        yield return new WaitForEndOfFrame();
        panel.SetActive(false);
        uiJuego.SetActive(true);
    }  
}

