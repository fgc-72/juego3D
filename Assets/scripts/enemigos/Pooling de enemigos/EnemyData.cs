using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public GameObject prefab;
    public int cuantosPregardos = 5; // cuántos pre-instanciar al inicio
}