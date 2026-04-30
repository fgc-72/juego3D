using UnityEngine;

public class AnimalNPC : MonoBehaviour
{
    [SerializeField] float velocidad = 2f;
    [SerializeField] float tiempoEspera = 3f;

    Rigidbody rb;
    Vector3 direccion;
    float temporizador;

    void Start()
    {
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
        // dirección aleatoria en el plano horizontal
        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        direccion = new Vector3(x, 0, z).normalized;
        temporizador = tiempoEspera;

        // rota el animal hacia donde va
        if (direccion != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direccion);
    }

    void OnCollisionEnter(Collision col)
    {
        // si choca con algo cambia de dirección
        CambiarDireccion();
    }
}