using UnityEngine;
using UnityEngine.SceneManagement;
public class toro : AnimalesGeneral
{
    private bool atacando = false;

    public override void Spawn()
    {
        atacando = false;
        Debug.Log("El toro empieza a moverse.");
    }

    public override void Movimiento()
    {
        if (!atacando)
        {
            transform.Translate(Vector3.forward * datos.velocidad * Time.deltaTime);
            Debug.Log("El toro se mueve a velocidad: " + datos.velocidad);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemigo") || other.CompareTag("Edificio"))
        {
            atacando = true;
            Atacar(other.gameObject);
        }
    }

    public override void Atacar()
    {
    }

    private void Atacar(GameObject objetivo)
    {
        Debug.Log("El toro embiste a: " + objetivo.name);
    }

    public override void Morir()
    {
        Destroy(gameObject, 2f);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "ciudad1")
        {
            Movimiento();
        }
    }
}
