using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimalNPC : MonoBehaviour
{
    [SerializeField] float velocidad = 2f;
    [SerializeField] float tiempoEspera = 3f;

    Rigidbody rb;
    Vector3 direccion;
    float temporizador;

    void Start()
    {
        
        if (SceneManager.GetActiveScene().name == "ciudad1") // Cambia al movimiento correspondiente a cada animal dependiendo de la escena
        {
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        CambiarDireccion();
    }

    void Update()
    {
        temporizador -= Time.deltaTime;
        if (temporizador <= 0)
            CambiarDireccion();
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + direccion * velocidad * Time.fixedDeltaTime);
    }

    void CambiarDireccion()
    {
        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        direccion = new Vector3(x, 0, z).normalized;
        temporizador = tiempoEspera;
        
        if (direccion != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direccion);
    }

    void OnCollisionEnter(Collision col)
    {
        CambiarDireccion();
    }
}