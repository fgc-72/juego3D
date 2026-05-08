using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject orbPrefab;

    public float spawnInterval  = 3f;
    public float orbSpeed       = 1.5f;
    public int   maxShapesPerOrb = 3;

    public event Action<Orb> OnOrbReachedCenter;
    public event Action<Orb.MineralType> OnMineralCompleted;
    public event Action<Orb> OnOrbCompleted;

    public List<Orb> ActiveOrbs { get; private set; } = new List<Orb>();

    private Camera cam;

    void Awake() => cam = Camera.main;

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

    void SpawnOrb()
    {
        if (orbPrefab == null) { Debug.LogWarning("OrbSpawner: falta asignar orbPrefab"); return; }

        Vector3 pos    = RandomEdgePosition();
        GameObject go  = Instantiate(orbPrefab, pos, Quaternion.identity);
        Orb orb        = go.GetComponent<Orb>();

        if (orb == null) { Debug.LogError("El prefab no tiene componente Orb."); Destroy(go); return; }

        float rand = UnityEngine.Random.value;
        Orb.MineralType mineralType = Orb.MineralType.None;
        int shapeCount;
        float speed = orbSpeed;
        List<ShapeType> shapes;

        if (rand < 0.01f)
        {
            mineralType = Orb.MineralType.Diamante;
            shapeCount = maxShapesPerOrb + 10;
            speed = orbSpeed * 0.35f;
        }
        else if (rand < 0.105f)
        {
            mineralType = (Orb.MineralType)UnityEngine.Random.Range(1, 7);
            shapeCount = maxShapesPerOrb + 1;
        }
        else
        {
            shapeCount = UnityEngine.Random.Range(1, maxShapesPerOrb + 1);
        }

        shapes = GenerateShapes(shapeCount);

        orb.Initialize(shapes, speed, mineralType);
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
        if (orb.IsMineral)
        {
            OnMineralCompleted?.Invoke(orb.mineralType);
        }
        Destroy(orb.gameObject, 0.05f);
    }

    List<ShapeType> GenerateShapes(int count)
    {
        ShapeType[] all    = (ShapeType[])Enum.GetValues(typeof(ShapeType));
        var result         = new List<ShapeType>(count);
        for (int i = 0; i < count; i++)
            result.Add(all[UnityEngine.Random.Range(0, all.Length)]);
        return result;
    }

    Vector3 RandomEdgePosition()
    {
        float orthoH = cam.orthographicSize;
        float orthoW = orthoH * cam.aspect;
        float margin = 1f;

        int edge = UnityEngine.Random.Range(0, 4);
        return edge switch
        {
            0 => new Vector3(UnityEngine.Random.Range(-orthoW, orthoW),  orthoH + margin, 0),
            1 => new Vector3(UnityEngine.Random.Range(-orthoW, orthoW), -orthoH - margin, 0),
            2 => new Vector3(-orthoW - margin, UnityEngine.Random.Range(-orthoH, orthoH), 0),
            _ => new Vector3( orthoW + margin, UnityEngine.Random.Range(-orthoH, orthoH), 0),
        };
    }
}