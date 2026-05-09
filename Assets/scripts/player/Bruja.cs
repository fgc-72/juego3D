using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;

public class Bruja : MonoBehaviour
{
    [Header("Invocación")]
    public DatosAnimal animalSeleccionado;
    public Transform puntoDeInvocacion;
    private UiBatalla ui;

    public static Bruja Instancia { get; private set; }

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() 
    {
        SceneManager.sceneLoaded += OnScenaCargada;
    }

    void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnScenaCargada;
    }

    void OnScenaCargada(Scene escena, LoadSceneMode mode) // Busca la UI, el punto de spwan y la camara cada vez que se carga una escena, para actualizar los botones
    {
        StartCoroutine(BuscarUI());
        StartCoroutine(ReposicionarEnSpawn());
        StartCoroutine(BuscarCamara());
    }

    IEnumerator BuscarUI()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        UiBatalla[] uis = FindObjectsByType<UiBatalla>(FindObjectsSortMode.None);
        if (uis.Length > 0)
        {
            ui = uis[0];
            Debug.Log("UI encontrada.");
        }
        else
        {
            ui = null;
            Debug.Log("No hay UiBatalla en esta escena.");
        }
    }

    public void Invocar() // Para invocar el animal seleccionado al hacer click en el botón de la UI
    {
        if (animalSeleccionado == null)
        {
            Debug.Log("No hay animal seleccionado.");
            return;
        }

        if (!InventarioJugador.Instancia.TieneAnimal(animalSeleccionado))
        {
            Debug.Log("No tienes " + animalSeleccionado.nombre);
            return;
        }

        InventarioJugador.Instancia.UsarAnimal(animalSeleccionado);

        // Actualiza el botón solo si hay UI en esta escena
        if (ui != null)
            ui.ActualizarCantidad(animalSeleccionado);

        Vector3 posicion = puntoDeInvocacion != null
            ? puntoDeInvocacion.position
            : transform.position + transform.forward * 2f;

        GameObject nuevoAnimal = Instantiate(animalSeleccionado.prefab3D, posicion, transform.rotation);

        AnimalesGeneral animal = nuevoAnimal.GetComponent<AnimalesGeneral>();
        if (animal != null)
            animal.Spawn();
    }

    public void SeleccionarAnimal(DatosAnimal datos)
    {
        animalSeleccionado = datos;
        Debug.Log("Seleccionado: " + datos.nombre);
    }

    IEnumerator ReposicionarEnSpawn()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        GameObject puntoSpawn = GameObject.Find("spawnBruja");
        if (puntoSpawn != null)
        {
            transform.position = puntoSpawn.transform.position;
            transform.rotation = puntoSpawn.transform.rotation;
            Debug.Log("Bruja reposicionada en: " + puntoSpawn.transform.position);
        }
        else
        {
            Debug.LogWarning("No hay PuntoSpawn en esta escena.");
        }
    }

    IEnumerator BuscarCamara()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        CinemachineCamera cam = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None)[0];
        if (cam != null)
        {
            cam.Follow = transform;
            cam.LookAt = transform;
            Debug.Log("Cámara asignada a la bruja.");
        }
        else
        {
            Debug.LogWarning("No hay CinemachineCamera en esta escena.");
        }
    }
}