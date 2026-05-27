using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManagerBatalla : MonoBehaviour
{
    public static GameManagerBatalla Instance { get; private set; }

    [Header("Configuración por nivel")]
    [SerializeField] private WaveManager waveManager;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoCronometro;
    [SerializeField] private TextMeshProUGUI textoPuntos;
    [SerializeField] private TextMeshProUGUI textoNivel;

    // Configuración por nivel
    private int[] enemigosPorNivel  = { 10, 20, 40 };
    private int[] puntosMinimosPorNivel = { 5, 10, 20 };
    private float tiempoPorNivel = 12f; // mismo tiempo para todos, ajusta si quieres

    private int nivelActual;
    private int puntosMinimos;
    private int puntosActuales = 0;
    private float tiempoRestante;
    private bool juegoActivo = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Lee el nivel desde la Bruja
        nivelActual = GameManagerGeneral.Instancia != null ? GameManagerGeneral.Instancia.nivelCiudad : 1;
        nivelActual = Mathf.Clamp(nivelActual, 1, 3);

        puntosMinimos = puntosMinimosPorNivel[nivelActual - 1];
        tiempoRestante = tiempoPorNivel;

        // Configura el WaveManager con la cantidad de enemigos del nivel
        ConfigurarOleada();

        ActualizarUI();
        juegoActivo = true;
        StartCoroutine(Cronometro());
    }

    void ConfigurarOleada()
    {
        // Le dice al WaveManager cuántos enemigos spawnear
        int totalEnemigos = enemigosPorNivel[nivelActual - 1];
        waveManager.SetCantidadEnemigos(totalEnemigos);
        waveManager.IniciarOleada();

        if (textoNivel != null)
            textoNivel.text = "Nivel " + nivelActual;
    }

    IEnumerator Cronometro()
    {
        while (tiempoRestante > 0 && juegoActivo)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarUI();
            yield return null;
        }

        if (juegoActivo)
            TerminarJuego();
    }

    public void SumarPunto()
    {
        puntosActuales++;
        ActualizarUI();
    }

    void TerminarJuego()
    {
        juegoActivo = false;

        if (puntosActuales >= puntosMinimos)
        {
            Debug.Log("Ganaste nivel " + nivelActual);

            // Si era el nivel 3 termina el juego, si no sube de nivel
            if (nivelActual >= 3)
                GameManagerGeneral.Instancia.FinCiudadGanaste();
            else
            {
                GameManagerGeneral.Instancia.nivelCiudad = nivelActual + 1;
                GameManagerGeneral.Instancia.FinCiudadGanaste();
            }
        }
        else
        {
            Debug.Log("Perdiste nivel " + nivelActual);
            GameManagerGeneral.Instancia.FinCiudadPerdiste();
        }
    }

    void ActualizarUI()
    {
        if (textoCronometro != null)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            textoCronometro.text = $"{minutos:00}:{segundos:00}";
        }

        if (textoPuntos != null)
            textoPuntos.text = $"Puntos: {puntosActuales} / {puntosMinimos}";
    }

    public void DerrotaPorInvasion()
    {
        if (!juegoActivo) return;

        juegoActivo = false;

        Debug.Log("Los enemigos atravesaron el límite");

        GameManagerGeneral.Instancia.FinCiudadPerdiste();
    }
}
