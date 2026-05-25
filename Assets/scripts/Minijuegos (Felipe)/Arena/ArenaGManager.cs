
using System;
using UnityEngine;
using TMPro;

public class ArenaGManager : MonoBehaviour
{
    [Header("Referencias")]
    public DrawController drawingController;
    public OrbSpawner        orbSpawner;
    public TextMeshProUGUI   timerText;
    public TextMeshProUGUI   scoreText;
    public TextMeshProUGUI   gemasText;
    [SerializeField] private GameObject panelJuego;

    public float gameDuration   = 30f;
    public int   pointsPerShape = 10;
    public int   orbPenalty     = 20;
    public int   scoreForMax    = 100;

    public event Action<float> OnGameEnded;

    private float timeLeft;
    private int   score;
    private bool  running;

    // Estos son los contadores de gemas
    private int rubiCount = 0;
    private int esmeraldaCount = 0;
    private int cuarzoCount = 0;
    private int lapisLazuliCount = 0;
    private int amatistaCount = 0;
    private int zafiroCount = 0;
    private int diamanteCount = 0;

    // Métodos públicos para acceder a los recursos recolectados desde otras escenas, hay q llamarlos DESPUÉS de que acabe el minijuego
    public int GetScore() { return score; } // Puntos totales
    public int GetRubiCount() { return rubiCount; }
    public int GetEsmeraldaCount() { return esmeraldaCount; }
    public int GetCuarzoCount() { return cuarzoCount; }
    public int GetLapisLazuliCount() { return lapisLazuliCount; }
    public int GetAmatistaCount() { return amatistaCount; }
    public int GetZafiroCount() { return zafiroCount; }
    public int GetDiamanteCount() { return diamanteCount; }



    public void StartGame()
    {
        panelJuego.SetActive(true);
        score   = 0;
        timeLeft = gameDuration;
        running  = true;

        rubiCount = 0;
        esmeraldaCount = 0;
        cuarzoCount = 0;
        lapisLazuliCount = 0;
        amatistaCount = 0;
        zafiroCount = 0;
        diamanteCount = 0;

        drawingController.IsEnabled       = true;
        drawingController.OnShapeDrawn   += HandleShapeDrawn;

        orbSpawner.OnOrbReachedCenter    += HandleOrbPenalty;
        orbSpawner.OnOrbCompleted        += HandleOrbCompleted;
        orbSpawner.OnMineralCompleted    += HandleMineralCompleted;
        orbSpawner.StartSpawning();

        RefreshUI();
        RefreshGemasUI();
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
            {
                orb.TryMatch(drawn);
            }
        }
    }

    void HandleOrbPenalty(Orb _)
    {
        score = Mathf.Max(0, score - orbPenalty);
        RefreshUI();
    }

    void HandleOrbCompleted(Orb orb)
    {
        if (!orb.IsMineral)
        {
            score += pointsPerShape;
            RefreshUI();
        }
    }

    void HandleMineralCompleted(Orb.MineralType type)
    {
        switch (type)
        {
            case Orb.MineralType.Rubi: rubiCount++; break;
            case Orb.MineralType.Esmeralda: esmeraldaCount++; break;
            case Orb.MineralType.Cuarzo: cuarzoCount++; break;
            case Orb.MineralType.LapisLazuli: lapisLazuliCount++; break;
            case Orb.MineralType.Amatista: amatistaCount++; break;
            case Orb.MineralType.Zafiro: zafiroCount++; break;
            case Orb.MineralType.Diamante: diamanteCount++; break;
        }
        RefreshGemasUI();
    }

    void EndGame()
    {
        running = false;

        drawingController.IsEnabled     = false;
        drawingController.OnShapeDrawn -= HandleShapeDrawn;

        orbSpawner.StopSpawning();
        orbSpawner.OnOrbReachedCenter -= HandleOrbPenalty;
        orbSpawner.OnOrbCompleted     -= HandleOrbCompleted;
        orbSpawner.OnMineralCompleted -= HandleMineralCompleted;

        float resourceRatio = Mathf.Clamp01((float)score / scoreForMax);
        Debug.Log($"[MiniGame] Score: {score} | Recurso obtenido: {resourceRatio:P0}");

        OnGameEnded?.Invoke(resourceRatio);
    }

    void RefreshUI()
    {
        if (timerText) timerText.text = $"{Mathf.CeilToInt(timeLeft)}s";
        if (scoreText) scoreText.text = $"{score} pts";
    }

    void RefreshGemasUI()
    {
        int totalGemas = rubiCount + esmeraldaCount + cuarzoCount + lapisLazuliCount + amatistaCount + zafiroCount + diamanteCount;
        if (totalGemas > 0)
        {
            gemasText.gameObject.SetActive(true);
            gemasText.text = $"Rubí: {rubiCount} | Esmeralda: {esmeraldaCount} | Cuarzo: {cuarzoCount} | Lapis: {lapisLazuliCount} | Amatista: {amatistaCount} | Zafiro: {zafiroCount} | Diamante: {diamanteCount}";
        }
        else
        {
            gemasText.gameObject.SetActive(false);
        }
    }

    // Esto es para el manejo de escenas
    /*          no sé cómo le quieran poner
    public void CambiarEscena(string nombreEscena)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscena);
    }

    public void KeepScore(int puntos)
    {
        PlayerPrefs.SetInt("SandScore", puntos);
        PlayerPrefs.Save();
    }

    public int LoadScore()
    {
        return PlayerPrefs.GetInt("SandScore", 0);
    }
    */
}