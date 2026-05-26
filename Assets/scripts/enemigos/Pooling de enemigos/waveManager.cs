using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public struct EntradaOleada
    {
        public EnemyData tipoEnemigo;
        public int cantidad;
        public float delayEntreSpawns;
    }

    [SerializeField] private EntradaOleada[] oleada;
    [SerializeField] private Transform[] puntosDeSpawn;

    void Start()
    {
        // Pre-carga todas las pools antes de que empiece la acción
        foreach (var entrada in oleada)
            poolManager.Instance.PreloadPool(entrada.tipoEnemigo);
    }
    public void SetCantidadEnemigos(int total)
    {
        // Distribuye el total entre los tipos de enemigos que tengas
        for (int i = 0; i < oleada.Length; i++)
        {
            oleada[i].cantidad = total / oleada.Length;
        }
    }
    public void IniciarOleada()
    {
        foreach (var entrada in oleada)
            StartCoroutine(SpawnGrupo(entrada));
    }

    private IEnumerator SpawnGrupo(EntradaOleada entrada)
    {
        for (int i = 0; i < entrada.cantidad; i++)
        {
            var punto = puntosDeSpawn[Random.Range(0, puntosDeSpawn.Length)];
            poolManager.Instance.GetFromPool(
                entrada.tipoEnemigo,
                punto.position,
                punto.rotation
            );
            yield return new WaitForSeconds(entrada.delayEntreSpawns);
        }
    }
}