using UnityEngine;
using UnityEngine.SceneManagement;
public class gallo : AnimalesGeneral
{
    private bool atacando = false;

    public override void Spawn()
    {
        atacando = false;
        Debug.Log("El gallo empieza a moverse.");
    }

    public override void Movimiento()
    {
        if (!atacando)
        {
            transform.Translate(Vector3.forward * datos.velocidad * Time.deltaTime);
            Debug.Log("El gallo se mueve a velocidad: " + datos.velocidad);
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
        Debug.Log("El gallo picotea a: " + objetivo.name);
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
