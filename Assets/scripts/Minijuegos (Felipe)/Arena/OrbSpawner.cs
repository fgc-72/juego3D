using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    [SerializeField] RectTransform canvasRect; // arrastra el Canvas aquí

    [Header("Prefab")]
    public GameObject orbPrefab;

    public float spawnInterval   = 3f;
    public float orbSpeed        = 1.5f;
    public int   maxShapesPerOrb = 3;

    public event Action<Orb> OnOrbReachedCenter;
    public event Action<Orb.MineralType> OnMineralCompleted;
    public event Action<Orb> OnOrbCompleted;

    public List<Orb> ActiveOrbs { get; private set; } = new List<Orb>();

    public void StartSpawning()
    {
        StopAllCoroutines();
        StartCoroutine(SpawnLoop());
    }

    public void StopSpawning() => StopAllCoroutines();

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            SpawnOrb();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void DestruirTodosLosOrbes()
    {
        foreach (Orb orb in ActiveOrbs.ToArray())
        {
            if (orb != null)
                Destroy(orb.gameObject);
        }
        ActiveOrbs.Clear();
    }
    void SpawnOrb()
    {
        if (orbPrefab == null) { Debug.LogWarning("OrbSpawner: falta asignar orbPrefab"); return; }

        // Instancia como hijo del Canvas, sin posición world
        GameObject go = Instantiate(orbPrefab, canvasRect);
        RectTransform orbRT = go.GetComponent<RectTransform>();
        orbRT.anchoredPosition = RandomCanvasEdge();

        Orb orb = go.GetComponent<Orb>();
        if (orb == null) { Debug.LogError("El prefab no tiene componente Orb."); Destroy(go); return; }

        float rand = UnityEngine.Random.value;
        Orb.MineralType mineralType = Orb.MineralType.None;
        int   shapeCount;
        float speed = orbSpeed;

        if (rand < 0.01f)
        {
            mineralType = Orb.MineralType.Diamante;
            shapeCount  = maxShapesPerOrb + 10;
            speed       = orbSpeed * 0.35f;
        }
        else if (rand < 0.105f)
        {
            mineralType = (Orb.MineralType)UnityEngine.Random.Range(1, 7);
            shapeCount  = maxShapesPerOrb + 1;
        }
        else
        {
            shapeCount = UnityEngine.Random.Range(1, maxShapesPerOrb + 1);
        }

        orb.Initialize(GenerateShapes(shapeCount), speed, mineralType);
        orb.OnReachedCenter += HandleReachedCenter;
        orb.OnCompleted     += HandleCompleted;

        ActiveOrbs.Add(orb);
    }

    void HandleReachedCenter(Orb orb)
    {
        ActiveOrbs.Remove(orb);
        OnOrbReachedCenter?.Invoke(orb);
        Destroy(orb.gameObject, 0.05f);
    }

    void HandleCompleted(Orb orb)
    {
        ActiveOrbs.Remove(orb);
        OnOrbCompleted?.Invoke(orb);
        if (orb.IsMineral) OnMineralCompleted?.Invoke(orb.mineralType);
        Destroy(orb.gameObject, 0.05f);
    }

    List<ShapeType> GenerateShapes(int count)
    {
        ShapeType[] all = (ShapeType[])Enum.GetValues(typeof(ShapeType));
        var result = new List<ShapeType>(count);
        for (int i = 0; i < count; i++)
            result.Add(all[UnityEngine.Random.Range(0, all.Length)]);
        return result;
    }

    Vector2 RandomCanvasEdge()
    {
        float w = canvasRect.rect.width  / 2f;
        float h = canvasRect.rect.height / 2f;
        float margin = 100f;

        int edge = UnityEngine.Random.Range(0, 4);
        return edge switch
        {
            0 => new Vector2(UnityEngine.Random.Range(-w, w),  h + margin),
            1 => new Vector2(UnityEngine.Random.Range(-w, w), -h - margin),
            2 => new Vector2(-w - margin, UnityEngine.Random.Range(-h, h)),
            _ => new Vector2( w + margin, UnityEngine.Random.Range(-h, h)),
        };
    }
}