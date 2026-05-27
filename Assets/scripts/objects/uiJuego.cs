using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class uiJuego : MonoBehaviour
{
    [Header("Referencias")]
    public Bruja bruja;

    [Header("Recursos")]
    public GameObject botonRecursoPrefab;
    public Transform contenedorRecursos;
    public GameObject panelRecursos;

    private Dictionary<string, TextMeshProUGUI> textosRecursos = new();

    void Start()
    {
        if (panelRecursos != null) panelRecursos.SetActive(false);
    }

    // Llama esto cuando el jugador consigue recursos
    public void ActualizarRecursos()
    {
        var inv = InventarioJugador.Instancia;

        // Par de nombre y valor
        var recursos = new (string nombre, int valor)[]
        {
            ("Arena",      inv.arena),
            ("Magia",      inv.magia),
            ("Rubíes",     inv.rubies),
            ("Cuarzos",    inv.cuarzos),
            ("Lapis",      inv.lapislazulis),
            ("Amatistas",  inv.amatistas),
            ("Zafiros",    inv.zafiros),
            ("Esmeraldas", inv.esmeraldas),
            ("Diamantes",  inv.diamantes),
        };

        // Limpia botones anteriores
        foreach (Transform hijo in contenedorRecursos)
            Destroy(hijo.gameObject);
        textosRecursos.Clear();

        bool hayRecursos = false;

        foreach (var recurso in recursos)
        {
            if (recurso.valor <= 0) continue; // solo crea botón si tiene algo

            hayRecursos = true;

            GameObject boton = Instantiate(botonRecursoPrefab, contenedorRecursos);
            TextMeshProUGUI texto = boton.GetComponentInChildren<TextMeshProUGUI>();
            texto.text = $"{recurso.nombre}\n{recurso.valor}";
            textosRecursos[recurso.nombre] = texto;
        }

        if (panelRecursos != null)
            panelRecursos.SetActive(hayRecursos);
    }
}