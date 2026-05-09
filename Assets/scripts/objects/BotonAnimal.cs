using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotonAnimal : MonoBehaviour
{
    [SerializeField] Image icono;
    [SerializeField] TextMeshProUGUI nombreTexto;
    [SerializeField] TextMeshProUGUI costoTexto;
    [SerializeField] Button boton;

    DatosAnimal datos;

    public void Configurar(DatosAnimal animal) // Para configurar el botón con los datos del animal
    {
        datos = animal;
        icono.sprite = animal.icono;
        nombreTexto.text = animal.nombre;
        costoTexto.text = $"Arena: {animal.costoArena}  Magia: {animal.costoMagia}";

        // desactiva el botón si no hay recursos suficientes
        boton.interactable = InventarioJugador.Instancia.TieneRecursos(animal.costoArena, animal.costoMagia);
    }

    public void Craftear() //  Para craftear el animal al hacer click en el botón
    {
    if (!InventarioJugador.Instancia.TieneRecursos(datos.costoArena, datos.costoMagia)) return;

    InventarioJugador.Instancia.GastarRecursos(datos.costoArena, datos.costoMagia);

    // Aparece en la granja visualmente
    Instantiate(datos.prefab3D, datos.zonaSpawn.position, Quaternion.identity);

    // Se guarda en el inventario
    InventarioJugador.Instancia.AgregarAnimal(datos);
    }
}