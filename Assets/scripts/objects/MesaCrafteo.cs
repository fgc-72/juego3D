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
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, jugador.position);
        botonInteraccion.SetActive(distancia <= distanciaInteraccion);
    }

    public void AbrirMenu()
    {
        MenuCrafteo.Instancia.Abrir();
    }
}