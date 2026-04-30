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

    public void Configurar(DatosAnimal animal)
    {
        datos = animal;
        icono.sprite = animal.icono;
        nombreTexto.text = animal.nombre;
        costoTexto.text = $"Arena: {animal.costoArena}  Magia: {animal.costoMagia}";

        // desactiva el botón si no hay recursos suficientes
        boton.interactable = InventarioJugador.Instancia.TieneRecursos(animal.costoArena, animal.costoMagia);
    }

    public void Craftear()
    {
    if (!InventarioJugador.Instancia.TieneRecursos(datos.costoArena, datos.costoMagia)) return;

    InventarioJugador.Instancia.GastarRecursos(datos.costoArena, datos.costoMagia);
    
    // aparece físicamente en la granja
    Instantiate(datos.prefab3D, datos.zonaSpawn.position, Quaternion.identity);
    
    
    }
}