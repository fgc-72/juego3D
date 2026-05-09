using UnityEngine;
using System.Collections.Generic;

public class InventarioJugador : MonoBehaviour
{
    public static InventarioJugador Instancia { get; private set; }

    [Header("Recursos")]
    public int arena;
    public int magia;
    public int monedas;

    [Header("Minerales")]
    public int rubies;
    public int cuarzos;
    public int lapislazulis;
    public int amatistas;
    public int diamantes;
    public int zafiros;
    public int esmeraldas;

    [Header("Animales fabricados")]
    public Dictionary<DatosAnimal, int> animalesFabricados = new Dictionary<DatosAnimal, int>();

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
    }

    public bool TieneRecursos(int arenaRequerida, int magiaRequerida)
    {
        return arena >= arenaRequerida && magia >= magiaRequerida;
    }

    public void GastarRecursos(int arenaGastada, int magiaGastada)
    {
        arena -= arenaGastada;
        magia -= magiaGastada;
    }

    public void AgregarAnimal(DatosAnimal datos)
    {
        if (animalesFabricados.ContainsKey(datos))
            animalesFabricados[datos]++;
        else
            animalesFabricados[datos] = 1;

        Debug.Log($"Animal agregado: {datos.nombre} — Total: {animalesFabricados[datos]}");
    }

    public bool TieneAnimal(DatosAnimal datos)
    {
        return animalesFabricados.ContainsKey(datos) && animalesFabricados[datos] > 0;
    }

    public void UsarAnimal(DatosAnimal datos)
    {
        if (TieneAnimal(datos))
            animalesFabricados[datos]--;
    }

    public void DevolverAnimal(DatosAnimal datos)
    {
        AgregarAnimal(datos);
        Debug.Log($"{datos.nombre} sobrevivió y volvió al inventario.");
    }

    
}