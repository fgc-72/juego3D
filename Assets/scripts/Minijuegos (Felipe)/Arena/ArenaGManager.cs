using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class ArenaGManager : MonoBehaviour
{
    [Header("Referencias")]
    public DrawController drawingController;
    public OrbSpawner orbSpawner;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gemasText;
    public TextMeshProUGUI resultadoText;

    [SerializeField] private GameObject panelJuego;

    [Header("Configuración General")]
    public float gameDuration = 30f;
    public int pointsPerShape = 2;
    public int orbPenalty = 20;

    [Header("Objetivos por nivel")]
    private int[] maximoArena = { 20, 40, 60 };

    [Header("Dificultad por nivel")]
    [SerializeField] private float[] spawnPorNivel = { 2.5f, 1.8f, 1.2f };
    [SerializeField] private float[] velocidadPorNivel = { 0.8f, 1.2f, 1.7f };

    public event Action<float> OnGameEnded;

    private float timeLeft;
    private bool running;

    private int puntosEsteIntento = 0;

    // Recursos temporales SOLO del intento actual
    private int rubiCount = 0;
    private int esmeraldaCount = 0;
    private int cuarzoCount = 0;
    private int lapisLazuliCount = 0;
    private int amatistaCount = 0;
    private int zafiroCount = 0;
    private int diamanteCount = 0;

    // Cache UI
    private uiJuego uiJuegoRef;
    private arena_boton botonRef;

    private void Awake()
    {
        uiJuegoRef = FindObjectOfType<uiJuego>();
        botonRef = FindObjectOfType<arena_boton>();
    }

    int ObtenerPuntosMaximos()
    {
        int nivel = GameManagerGeneral.Instancia != null
            ? GameManagerGeneral.Instancia.nivelCiudad
            : 0;

        nivel = Mathf.Clamp(nivel, 0, maximoArena.Length - 1);

        return maximoArena[nivel];
    }

    void ConfigurarDificultad()
    {
        int nivel = GameManagerGeneral.Instancia != null
            ? GameManagerGeneral.Instancia.nivelCiudad
            : 0;

        nivel = Mathf.Clamp(nivel, 0, spawnPorNivel.Length - 1);

        orbSpawner.spawnInterval = spawnPorNivel[nivel];
        orbSpawner.orbSpeed = velocidadPorNivel[nivel];

        Debug.Log($"Nivel {nivel} | Spawn: {orbSpawner.spawnInterval} | Speed: {orbSpawner.orbSpeed}");
    }

    public void StartGame()
    {
        panelJuego.SetActive(true);

        ResetearIntento();

        timeLeft = gameDuration;
        running = true;

        ConfigurarDificultad();

        drawingController.IsEnabled = true;
        drawingController.OnShapeDrawn += HandleShapeDrawn;

        orbSpawner.OnOrbReachedCenter += HandleOrbPenalty;
        orbSpawner.OnOrbCompleted += HandleOrbCompleted;
        orbSpawner.OnMineralCompleted += HandleMineralCompleted;

        orbSpawner.StartSpawning();

        RefreshUI();
        RefreshGemasUI();

        if (resultadoText != null)
            resultadoText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!running) return;

        timeLeft -= Time.deltaTime;

        RefreshUI();

        if (timeLeft <= 0f)
            EndGame();
    }

    void ResetearIntento()
    {
        puntosEsteIntento = 0;

        rubiCount = 0;
        esmeraldaCount = 0;
        cuarzoCount = 0;
        lapisLazuliCount = 0;
        amatistaCount = 0;
        zafiroCount = 0;
        diamanteCount = 0;
    }

    void HandleShapeDrawn(ShapeType drawn)
    {
        foreach (Orb orb in orbSpawner.ActiveOrbs)
        {
            if (orb.CurrentShape == drawn)
                orb.TryMatch(drawn);
        }
    }

    void HandleOrbPenalty(Orb orb)
    {
        puntosEsteIntento = Mathf.Max(0, puntosEsteIntento - orbPenalty);

        RefreshUI();
    }

    void HandleOrbCompleted(Orb orb)
    {
        if (!orb.IsMineral)
        {
            puntosEsteIntento += pointsPerShape;

            RefreshUI();
        }
    }

    void HandleMineralCompleted(Orb.MineralType type)
    {
        switch (type)
        {
            case Orb.MineralType.Rubi:
                rubiCount++;
                break;

            case Orb.MineralType.Esmeralda:
                esmeraldaCount++;
                break;

            case Orb.MineralType.Cuarzo:
                cuarzoCount++;
                break;

            case Orb.MineralType.LapisLazuli:
                lapisLazuliCount++;
                break;

            case Orb.MineralType.Amatista:
                amatistaCount++;
                break;

            case Orb.MineralType.Zafiro:
                zafiroCount++;
                break;

            case Orb.MineralType.Diamante:
                diamanteCount++;
                break;
        }

        RefreshGemasUI();
    }

    void EndGame()
    {
        running = false;

        drawingController.IsEnabled = false;
        drawingController.OnShapeDrawn -= HandleShapeDrawn;

        orbSpawner.StopSpawning();

        orbSpawner.OnOrbReachedCenter -= HandleOrbPenalty;
        orbSpawner.OnOrbCompleted -= HandleOrbCompleted;
        orbSpawner.OnMineralCompleted -= HandleMineralCompleted;

        orbSpawner.DestruirTodosLosOrbes();

        int puntosMaximos = ObtenerPuntosMaximos();

        bool cumplio = puntosEsteIntento >= puntosMaximos;

        float resourceRatio = Mathf.Clamp01((float)puntosEsteIntento / puntosMaximos);

        OnGameEnded?.Invoke(resourceRatio);

        StartCoroutine(MostrarResultadoYCerrar(cumplio, puntosMaximos));
    }

    IEnumerator MostrarResultadoYCerrar(bool cumplio, int puntosMaximos)
    {
        if (resultadoText != null)
        {
            resultadoText.gameObject.SetActive(true);

            if (cumplio)
            {
                GuardarRecursosEnInventario();

                uiJuegoRef?.ActualizarRecursos();

                if (GameManagerGeneral.Instancia.nivelCiudad >= 2)
                {
                    resultadoText.text = "¡Completaste todos los niveles!";

                    yield return new WaitForSeconds(2f);

                    panelJuego.SetActive(false);

                    GameManagerGeneral.Instancia.FinCiudadGanaste();

                    yield break;
                }

                resultadoText.text =
                    $"¡Lograste {puntosEsteIntento}/{puntosMaximos}!\n¡Subiste de nivel!";

                GameManagerGeneral.Instancia.SubirNivel();

                botonRef?.DesactivarBoton();
            }
            else
            {
                resultadoText.text =
                    $"Obtuviste {puntosEsteIntento}/{puntosMaximos}.\n¡Inténtalo de nuevo!";
            }
        }

        yield return new WaitForSeconds(2f);

        panelJuego.SetActive(false);

        botonRef?.ActivarPanel();
    }

    void GuardarRecursosEnInventario()
    {
        var inv = InventarioJugador.Instancia;

        if (inv == null) return;

        // Arena
        inv.arena += puntosEsteIntento;

        // Minerales
        inv.rubies += rubiCount;
        inv.esmeraldas += esmeraldaCount;
        inv.cuarzos += cuarzoCount;
        inv.lapislazulis += lapisLazuliCount;
        inv.amatistas += amatistaCount;
        inv.zafiros += zafiroCount;
        inv.diamantes += diamanteCount;

        Debug.Log(
            $"Recursos guardados | Arena: {puntosEsteIntento} | Rubíes: {rubiCount}");
    }

    void RefreshUI()
    {
        int puntosMaximos = ObtenerPuntosMaximos();

        if (timerText != null)
            timerText.text = $"{Mathf.CeilToInt(timeLeft)}s";

        if (scoreText != null)
            scoreText.text = $"{puntosEsteIntento} / {puntosMaximos} pts";

        uiJuegoRef?.ActualizarRecursos();
    }

    void RefreshGemasUI()
    {
        int totalGemas =
            rubiCount +
            esmeraldaCount +
            cuarzoCount +
            lapisLazuliCount +
            amatistaCount +
            zafiroCount +
            diamanteCount;

        if (gemasText == null) return;

        if (totalGemas > 0)
        {
            gemasText.gameObject.SetActive(true);

            gemasText.text =
                $"Rubí: {rubiCount} | " +
                $"Esmeralda: {esmeraldaCount} | " +
                $"Cuarzo: {cuarzoCount} | " +
                $"Lapis: {lapisLazuliCount} | " +
                $"Amatista: {amatistaCount} | " +
                $"Zafiro: {zafiroCount} | " +
                $"Diamante: {diamanteCount}";
        }
        else
        {
            gemasText.gameObject.SetActive(false);
        }
    }
}