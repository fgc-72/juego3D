using UnityEngine;

public class MesaCrafteo : MonoBehaviour
{
    [SerializeField] float distanciaInteraccion = 3f;
    [SerializeField] GameObject botonInteraccion; // botón UI "Craftear"
    Transform jugador;

    void Start()
    {
        jugador = GameObject.FindWithTag("Player").transform;
        botonInteraccion.SetActive(false);
        FindObjectOfType<uiJuego>()?.ActualizarRecursos();

    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, jugador.position);
        bool objetoActivo = this.gameObject.activeSelf;
        bool jugadorCerca = distancia <= distanciaInteraccion;

        botonInteraccion.SetActive(objetoActivo && jugadorCerca);
    }

    void OnDisable()
    {
        // Esto se ejecuta justo cuando el objeto se desactiva
        botonInteraccion.SetActive(false);
    }
    public void AbrirMenu()
    {
        MenuCrafteo.Instancia.Abrir();
    }

  

}