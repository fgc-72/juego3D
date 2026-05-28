using UnityEngine;

public abstract class AnimalesGeneral : MonoBehaviour
{
    public DatosAnimal datos;

    public abstract void Atacar(/*int daño*/); // sirve para que cada animal tenga un ataque diferente
    public abstract void Morir(); // sirve para que cada animal tenga una animacion de muerte diferente
    public abstract void Spawn(); //sirve para que cada animal trenga una animacion de spawn diferente
    public abstract void Movimiento();// sirve para que cada animal tenga un movimiento diferente

    public virtual void AplicarBuff() //Solo para el gallo y el toro
    {

    }

    public void Sobrevivir() // Para q los sobrevivientes vuelvan al inventario
    {
        InventarioJugador.Instancia.DevolverAnimal(datos);
        Destroy(gameObject);
    }

    public void RecibirDaño(int daño)
    {
        datos.vida -= daño;
        if (datos.vida <= 0)
            Morir();
    }

    public virtual void Invocar(Vector3 posicion, Quaternion rotacion) // Para invocar animales desde el inventario
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
