using System;
using UnityEngine;
using TMPro;
using System.Collections;

public class ArenaGManager : MonoBehaviour
{
    [Header("Referencias")]
    public DrawController drawingController;
    public OrbSpawner        orbSpawner;
    public TextMeshProUGUI   timerText;
    public TextMeshProUGUI   scoreText;
    public TextMeshProUGUI   gemasText;
    public TextMeshProUGUI   resultadoText; // texto de resultado al terminar
    [SerializeField] private GameObject panelJuego;

    public float gameDuration   = 30f;
    public int   pointsPerShape = 2;
    public int   orbPenalty     = 20;

    // Puntos maximos por nivel
    private int[] maximoArena = { 20, 40, 60 };

    public event Action<float> OnGameEnded;

    private float timeLeft;
    private bool  running;

    // Puntos acumulativos entre intentos, no se resetean hasta subir de nivel
    private static int puntosAcumulados = 0;
    private int puntosEsteIntento = 0;

    // Gemas acumuladas igual
    private static int rubiCount = 0;
    private static int esmeraldaCount = 0;
    private static int cuarzoCount = 0;
    private static int lapisLazuliCount = 0;
    private static int amatistaCount = 0;
    private static int zafiroCount = 0;
    private static int diamanteCount = 0;

    public int GetScore() => puntosAcumulados;
    public int GetRubiCount() => rubiCount;
    public int GetEsmeraldaCount() => esmeraldaCount;
    public int GetCuarzoCount() => cuarzoCount;
    public int GetLapisLazuliCount() => lapisLazuliCount;
    public int GetAmatistaCount() => amatistaCount;
    public int GetZafiroCount() => zafiroCount;
    public int GetDiamanteCount() => diamanteCount;

    // Resetea todo al subir de nivel
    public static void ResetearProgreso()
    {
        puntosAcumulados = 0;
        rubiCount = 0;
        esmeraldaCount = 0;
        cuarzoCount = 0;
        lapisLazuliCount = 0;
        amatistaCount = 0;
        zafiroCount = 0;
        diamanteCount = 0;
    }

    int ObtenerPuntosMaximos()
    {
        int nivel = Bruja.Instancia != null ? Bruja.Instancia.nivelCiudad : 1;
        nivel = Mathf.Clamp(nivel, 1, maximoArena.Length);
        return maximoArena[nivel - 1];
    }

    public void StartGame()
    {
        
        panelJuego.SetActive(true);
        puntosEsteIntento = 0;
        timeLeft = gameDuration;
        running  = true;

        drawingController.IsEnabled     = true;
        drawingController.OnShapeDrawn += HandleShapeDrawn;

        orbSpawner.OnOrbReachedCenter  += HandleOrbPenalty;
        orbSpawner.OnOrbCompleted      += HandleOrbCompleted;
        orbSpawner.OnMineralCompleted  += HandleMineralCompleted;
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

        if (timeLeft <= 0f) EndGame();
    }

    void HandleShapeDrawn(ShapeType drawn)
    {
        foreach (Orb orb in orbSpawner.ActiveOrbs)
        {
            if (orb.CurrentShape == drawn)
                orb.TryMatch(drawn);
        }
    }

    void HandleOrbPenalty(Orb _)
    {
        // Penalización solo en el intento actual, no baja los acumulados
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
            case Orb.MineralType.Rubi:        rubiCount++;        break;
            case Orb.MineralType.Esmeralda:   esmeraldaCount++;   break;
            case Orb.MineralType.Cuarzo:      cuarzoCount++;      break;
            case Orb.MineralType.LapisLazuli: lapisLazuliCount++; break;
            case Orb.MineralType.Amatista:    amatistaCount++;    break;
            case Orb.MineralType.Zafiro:      zafiroCount++;      break;
            case Orb.MineralType.Diamante:    diamanteCount++;    break;
        }
        RefreshGemasUI();
    }

    void EndGame()
    {
        running = false;

        drawingController.IsEnabled     = false;
        drawingController.OnShapeDrawn -= HandleShapeDrawn;

        orbSpawner.StopSpawning();
        orbSpawner.OnOrbReachedCenter  -= HandleOrbPenalty;
        orbSpawner.OnOrbCompleted      -= HandleOrbCompleted;
        orbSpawner.OnMineralCompleted  -= HandleMineralCompleted;

        // Destruye todos los orbes inmediatamente
        orbSpawner.DestruirTodosLosOrbes();

        int puntosMaximos = ObtenerPuntosMaximos();
        bool cumplio = puntosEsteIntento >= puntosMaximos;

        float resourceRatio = Mathf.Clamp01((float)puntosEsteIntento / puntosMaximos);
        OnGameEnded?.Invoke(resourceRatio);

        StartCoroutine(MostrarResultadoYCerrar(cumplio, puntosMaximos));
    }

    IEnumerator MostrarResultadoYCerrar(bool cumplio, int puntosMaximos)
    {
        arena_boton boton = FindObjectOfType<arena_boton>();
        if (resultadoText != null)
        {
            resultadoText.gameObject.SetActive(true);

            if (cumplio){
                resultadoText.text = $"¡Lograste {puntosEsteIntento}/{puntosMaximos} puntos!\n¡Avanzas al siguiente nivel!";
                GuardarRecursosEnInventario();
                FindObjectOfType<uiJuego>()?.ActualizarRecursos();
                boton.DesactivarBoton();
            }else{
                resultadoText.text = $"Obtuviste {puntosEsteIntento}/{puntosMaximos} puntos.\n¡Inténtalo de nuevo!";
                ResetearProgreso();
            }
                
        }

        yield return new WaitForSeconds(2f);
        panelJuego.SetActive(false);
        
        boton.ActivarPanel();

        
        // Si no cumplió simplemente cierra el panel, puede volver a jugar
        // sin perder los puntos acumulados
    }

    void GuardarRecursosEnInventario()
    {
        var inv = InventarioJugador.Instancia;
        if (inv == null) return;

        // Arena según los puntos obtenidos
        inv.arena += puntosEsteIntento;

        // Minerales recolectados
        inv.rubies       += rubiCount;
        inv.esmeraldas   += esmeraldaCount;
        inv.cuarzos      += cuarzoCount;
        inv.lapislazulis += lapisLazuliCount;
        inv.amatistas    += amatistaCount;
        inv.zafiros      += zafiroCount;
        inv.diamantes    += diamanteCount;

        Debug.Log($"Recursos guardados — Arena: {puntosEsteIntento} | Rubíes: {rubiCount} | Esmeraldas: {esmeraldaCount}");
    }

    void RefreshUI()
    {
        int puntosMaximos = ObtenerPuntosMaximos();
        if (timerText) timerText.text = $"{Mathf.CeilToInt(timeLeft)}s";
        if (scoreText) scoreText.text = $"{puntosEsteIntento} / {puntosMaximos} pts";
        FindObjectOfType<uiJuego>()?.ActualizarRecursos();
    }

    void RefreshGemasUI()
    {
        int totalGemas = rubiCount + esmeraldaCount + cuarzoCount +
                         lapisLazuliCount + amatistaCount + zafiroCount + diamanteCount;
        if (totalGemas > 0)
        {
            gemasText.gameObject.SetActive(true);
            gemasText.text = $"Rubí: {rubiCount} | Esmeralda: {esmeraldaCount} | Cuarzo: {cuarzoCount} | Lapis: {lapisLazuliCount} | Amatista: {amatistaCount} | Zafiro: {zafiroCount} | Diamante: {diamanteCount}";
        }
        else
            gemasText.gameObject.SetActive(false);
    }
}