using UnityEngine;

public class arena_boton : MonoBehaviour
{
    [SerializeField] float distanciaInteraccion = 3f;
    [SerializeField] GameObject botonInteraccion; // botón UI "Craftear"
    Transform jugador;

    void Start()
    {
        jugador = GameObject.FindWithTag("Player").transform;
        botonInteraccion.SetActive(false);

    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);
        bool jugadorCerca = distancia <= distanciaInteraccion;
        bool objetoActivo = gameObject.activeSelf;
        bool debeActivarse = jugadorCerca && objetoActivo;



        botonInteraccion.SetActive(debeActivarse);
    }

    public void AbrirMinijuego()
    {
        ArenaGManager arena = FindObjectOfType<ArenaGManager>();
        if (arena != null){
            arena.StartGame();
            Debug.Log("Iniciando minijuego Arena desde el botón.");
        }
            
        else{
             Debug.LogWarning("No se encontró ArenaGManager en la escena.");
        }
           
    }
}