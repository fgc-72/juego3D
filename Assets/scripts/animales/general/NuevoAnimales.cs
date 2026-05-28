using UnityEngine;

public class NuevoAnimales : MonoBehaviour
{
 [Header("Datos")]
    public DatosAnimal datos;

    private float vidaActual;

    private Rigidbody rb;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        vidaActual = datos.vida;
    }

    void Start()
    {
        Spawn();
    }

    void Update()
    {
        Movimiento();
    }

    // =========================
    // MOVIMIENTO
    // =========================

    void Movimiento()
    {
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * datos.velocidad;
        }
    }

    // =========================
    // ATAQUE
    // =========================

    public void Atacar()
    {
        if (animator != null)
            animator.SetTrigger("Atacar");

        Debug.Log(datos.nombre + " hizo " + datos.daño + " de daño");
    }

    // =========================
    // RECIBIR DAÑO
    // =========================

    public void RecibirDaño(int daño)
    {
        vidaActual -= daño;

        if (animator != null)
            animator.SetTrigger("Golpe");

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    // =========================
    // MUERTE
    // =========================

    public void Morir()
    {
        if (animator != null)
            animator.SetTrigger("Morir");

        Destroy(gameObject, 2f);
    }

    // =========================
    // SPAWN
    // =========================

    public void Spawn()
    {
        if (animator != null)
            animator.SetTrigger("Spawn");
    }

    // =========================
    // SOBREVIVIR
    // =========================

    public void Sobrevivir()
    {
        InventarioJugador.Instancia.DevolverAnimal(datos);
        Destroy(gameObject);
    }

    // =========================
    // INVOCAR
    // =========================

    public void InvocarAnimal(Vector3 posicion, Quaternion rotacion) // Para invocar animales desde el inventario
    {
        if (!InventarioJugador.Instancia.TieneAnimal(datos))
        {
            Debug.Log($"No quedan {datos.nombre} en el inventario.");
            return;
        }

        InventarioJugador.Instancia.UsarAnimal(datos);

        GameObject nuevo = Instantiate(datos.prefab3D, posicion, rotacion);

        
        Rigidbody rb = nuevo.GetComponent<Rigidbody>(); // para impulsar a los animales en la dirección que mira la bruja al invocarlos
        if (rb != null)
            rb.linearVelocity = rotacion * Vector3.forward * datos.velocidad;
    }
}

