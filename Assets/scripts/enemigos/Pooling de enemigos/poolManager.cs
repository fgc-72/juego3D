using System.Collections.Generic;
using UnityEngine;

public class poolManager : MonoBehaviour
{
    public static poolManager Instance { get; private set; }

    [SerializeField] private Transform contenedor;
    // Cada EnemyData tiene su propia Queue
    private Dictionary<EnemyData, Queue<GameObject>> _pools = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Llámalo en Start desde WaveManager con los datos que vayas a usar
    public void PreloadPool(EnemyData data)
    {
        if (!_pools.ContainsKey(data))
            _pools[data] = new Queue<GameObject>();

        for (int i = 0; i < data.cuantosPregardos; i++)
            EnqueueNew(data);
    }

    // Obtiene un enemigo activo (desde la pool o instanciando uno nuevo)
    public GameObject GetFromPool(EnemyData data, Vector3 posicion, Quaternion rotacion)
    {
        if (!_pools.ContainsKey(data))
            _pools[data] = new Queue<GameObject>();

        GameObject enemigo = _pools[data].Count > 0
            ? _pools[data].Dequeue()
            : CrearNuevo(data);

        enemigo.transform.SetPositionAndRotation(posicion, rotacion);
        enemigo.SetActive(true);
        return enemigo;
    }

    // Función pública que EnemyBase llama al morir
    public void ReturnToPool(EnemyData data, GameObject enemigo)
    {
        enemigo.SetActive(false);
        _pools[data].Enqueue(enemigo);
    }

    private GameObject CrearNuevo(EnemyData data)
    {
        var go = Instantiate(data.prefab, contenedor);
        go.GetComponent<EnemyBase>().Inicializar(data);
        go.SetActive(false);
        return go;
    }

    private void EnqueueNew(EnemyData data)
    {
        _pools[data].Enqueue(CrearNuevo(data));
    }
}