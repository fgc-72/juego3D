using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UiBatalla : MonoBehaviour
{
    [Header("Referencias")]
    public Bruja bruja;
    public GameObject botonPrefab;
    public Transform contenedorBotones;

    // Guarda los botones para actualizar cantidad en tiempo real
    private Dictionary<DatosAnimal, TextMeshProUGUI> textosBotones 
        = new Dictionary<DatosAnimal, TextMeshProUGUI>();

    void Start() // Busca la bruja al iniciar la escena de batalla
    {
        bruja = FindObjectsByType<Bruja>(FindObjectsSortMode.None)[0];
        GenerarBotones();
    }

    void GenerarBotones()
    {
        foreach (var entrada in InventarioJugador.Instancia.animalesFabricados)
        {
            DatosAnimal datos = entrada.Key;
            int cantidad = entrada.Value;

            if (cantidad <= 0) continue;

            GameObject boton = Instantiate(botonPrefab, contenedorBotones);
            boton.GetComponent<Image>().sprite = datos.icono;

            TextMeshProUGUI texto = boton.GetComponentInChildren<TextMeshProUGUI>();
            texto.text = $"{datos.nombre}\n{cantidad}";
            textosBotones[datos] = texto;

            Button btn = boton.GetComponent<Button>();
            DatosAnimal datosCopia = datos;

            btn.onClick.AddListener(() =>
            {
                bruja.SeleccionarAnimal(datosCopia);
                bruja.Invocar();
                ActualizarCantidad(datosCopia);      // actualiza el texto
                ActualizarInteractuable(datosCopia, btn); // desactiva si llega a 0
            });
        }
    }

    public void ActualizarCantidad(DatosAnimal datos)
    {
        if (!textosBotones.ContainsKey(datos)) return;

        int cantidad = InventarioJugador.Instancia.animalesFabricados[datos];
        textosBotones[datos].text = $"{datos.nombre}\n{cantidad}";
    }

    // Desactiva el botón si ya no quedan animales
    void ActualizarInteractuable(DatosAnimal datos, Button boton)
    {
        int cantidad = InventarioJugador.Instancia.animalesFabricados[datos];
        boton.interactable = cantidad > 0;
    }
}
