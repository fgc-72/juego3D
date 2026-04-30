//En la línea 101 está el código para cambiar de escena

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

    public float gameDuration   = 30f;
    public int   pointsPerShape = 10;
    public int   orbPenalty     = 20;
    public int   scoreForMax    = 100;

    public event Action<float> OnGameEnded;

    private float timeLeft;
    private int   score;
    private bool  running;

    void Start() => StartGame();

    public void StartGame()
    {
        score   = 0;
        timeLeft = gameDuration;
        running  = true;

        drawingController.IsEnabled       = true;
        drawingController.OnShapeDrawn   += HandleShapeDrawn;

        orbSpawner.OnOrbReachedCenter    += HandleOrbPenalty;
        orbSpawner.StartSpawning();

        RefreshUI();
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
        Orb bestMatch  = null;
        float minDist  = float.MaxValue;

        foreach (Orb orb in orbSpawner.ActiveOrbs)
        {
            if (orb.CurrentShape != drawn) continue;

            float dist = orb.transform.position.magnitude;
            if (dist < minDist) { minDist = dist; bestMatch = orb; }
        }

        if (bestMatch != null && bestMatch.TryMatch(drawn))
        {
            score += pointsPerShape;
            RefreshUI();
        }
    }

    void HandleOrbPenalty(Orb _)
    {
        score = Mathf.Max(0, score - orbPenalty);
        RefreshUI();
    }

    void EndGame()
    {
        running = false;

        drawingController.IsEnabled     = false;
        drawingController.OnShapeDrawn -= HandleShapeDrawn;

        orbSpawner.StopSpawning();
        orbSpawner.OnOrbReachedCenter -= HandleOrbPenalty;

        float resourceRatio = Mathf.Clamp01((float)score / scoreForMax);
        Debug.Log($"[MiniGame] Score: {score} | Recurso obtenido: {resourceRatio:P0}");

        OnGameEnded?.Invoke(resourceRatio);
    }

    void RefreshUI()
    {
        if (timerText) timerText.text = $"{Mathf.CeilToInt(timeLeft)}s";
        if (scoreText) scoreText.text = $"{score} pts";
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