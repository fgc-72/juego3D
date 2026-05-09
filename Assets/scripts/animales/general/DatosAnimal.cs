using UnityEngine;

[CreateAssetMenu(fileName = "NuevoAnimal", menuName = "Granja/Animal")]
public class DatosAnimal : ScriptableObject
{
    [Header("Información")]
    public string nombre;
    public Sprite icono;
    public TipoAnimal tipo;

    [Header("Prefab")]
    public GameObject prefab3D; 
    public Transform zonaSpawn; 

    [Header("Stats")] // IMPORTANTE: Aca es lo que tiene que referenciar para las mecanicas de batalla
    public float vida;
    public float resistencia;
    public float daño;
    public float velocidad;

    [Header("Costo")]
    public int costoArena;
    public int costoMagia;
}

public enum TipoAnimal
{
    Cerdo,
    Gallina,
    Cabra,
    Oveja,
    Vaca,
    Toro,
    Gallo
}