using UnityEngine;
using TMPro;
using System.Collections;

public class texto : MonoBehaviour
{
    public static texto Instancia;

    [Header("UI")]
    [SerializeField] private GameObject panelMensaje;
    [SerializeField] private TextMeshProUGUI textoMensaje;
    [SerializeField] private GameObject panelRecursos;
    [SerializeField] private GameObject panelRecursosBatalla;


    [Header("Configuración")]
    [SerializeField] private float velocidadEscritura = 0.03f;
    [SerializeField] private float tiempoVisible = 2f;

    private Coroutine rutinaActual;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        panelMensaje.SetActive(false);
    }

    public void MostrarMensaje(string mensaje)
    {
        if (rutinaActual != null)
            StopCoroutine(rutinaActual);

        rutinaActual = StartCoroutine(EscribirMensaje(mensaje));
    }

    IEnumerator EscribirMensaje(string mensaje)
    {
        if (panelRecursosBatalla != null)
        {
            panelRecursosBatalla.SetActive(false);
        }

        if (panelRecursos != null)
        {
            panelRecursos.SetActive(false);
        }

        panelMensaje.SetActive(true);

        textoMensaje.text = "";

        foreach (char letra in mensaje)
        {
            textoMensaje.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        yield return new WaitForSeconds(tiempoVisible);

        panelMensaje.SetActive(false);

        if (panelRecursos != null)
        {
            panelRecursos.SetActive(true);
        }

        if (panelRecursosBatalla != null)
        {
            panelRecursosBatalla.SetActive(true);
        }
    }


}

