using UnityEngine;

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
}