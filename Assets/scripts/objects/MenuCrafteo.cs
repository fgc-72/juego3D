using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuCrafteo : MonoBehaviour
{
    public static MenuCrafteo Instancia { get; private set; }

    [SerializeField] GameObject panelMenu;
    [SerializeField] Transform contenedorAnimales; // donde se generan los botones
    [SerializeField] GameObject prefabBotonAnimal; // botón con icono y costo
    [SerializeField] DatosAnimal[] animalesDisponibles; // arrastra los ScriptableObjects aquí
    [SerializeField] GameObject uiJuego; // para ocultar la UI del juego al abrir el menú

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        panelMenu.SetActive(false);
    }

    public void Abrir()
    {
        panelMenu.SetActive(true);
        uiJuego.SetActive(false);
        GenerarBotones();
    }

    public void Cerrar()
    {
        panelMenu.SetActive(false);
        uiJuego.SetActive(true);
    }

    void GenerarBotones()
    {
        // limpia botones anteriores
        foreach (Transform hijo in contenedorAnimales)
            Destroy(hijo.gameObject);

        foreach (DatosAnimal animal in animalesDisponibles)
        {
            GameObject boton = Instantiate(prefabBotonAnimal, contenedorAnimales);
            boton.GetComponent<BotonAnimal>().Configurar(animal);
        }
    }
}