using UnityEngine;
using UnityEngine.UI;

public class arena_boton : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float distanciaInteraccion = 3f;

    [Header("Referencias")]
    [SerializeField] private Button botonInteraccion;
    [SerializeField] private GameObject uiJuego;

    private Transform jugador;
    private ArenaGManager arenaManager;

    private bool jugadorCerca;

    private void Awake()
    {
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        arenaManager = FindObjectOfType<ArenaGManager>();

        if (botonInteraccion != null)
        {
            botonInteraccion.gameObject.SetActive(false);
            botonInteraccion.interactable = true;
        }
    }

    private void Update()
    {
        if (jugador == null || botonInteraccion == null)
            return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        bool nuevoEstado = distancia <= distanciaInteraccion;

        // Solo actualizar si cambia el estado
        if (nuevoEstado != jugadorCerca)
        {
            jugadorCerca = nuevoEstado;
            botonInteraccion.gameObject.SetActive(jugadorCerca);
        }
    }

    public void AbrirMinijuego()
    {
        if (uiJuego != null)
            uiJuego.SetActive(false);
            botonInteraccion.gameObject.SetActive(false);

        if (arenaManager != null)
        {
            arenaManager.StartGame();
            Debug.Log("Iniciando minijuego Arena.");
        }
        else
        {
            Debug.LogWarning("No se encontró ArenaGManager.");
        }
    }

    public void ActivarPanel()
    {
        if (uiJuego != null)
            uiJuego.SetActive(true);
    }

    public void ViajarBatalla()
    {
        if (Bruja.Instancia != null)
        {
            Bruja.Instancia.ViajarACiudad(1);
        }
    }

    public void DesactivarBoton()
    {
        if (botonInteraccion != null)
        {
            botonInteraccion.interactable = false;
        }
    }
}