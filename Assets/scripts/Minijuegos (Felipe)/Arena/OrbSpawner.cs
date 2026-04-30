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

        int shapeCount         = UnityEngine.Random.Range(1, maxShapesPerOrb + 1);
        List<ShapeType> shapes = GenerateShapes(shapeCount);

        orb.Initialize(shapes, orbSpeed);
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