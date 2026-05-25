using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Rigidbody _rb;
    private Vector3 _input;
    private Joystick _joystickMovement;
    [SerializeField] private float _rotationSpeed = 360f;
    [SerializeField]private Animator _animator;

    void Start()
    {
        _input = Vector3.zero;
        _animator = GetComponent<Animator>();
    }
    void OnEnable() 
    {
        SceneManager.sceneLoaded += OnScenaCargada;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnScenaCargada;
    }

    void OnScenaCargada(Scene escena, LoadSceneMode mode) // Busca el joystick cada vez que se carga una escena, para evitar errores al cambiar de escena
    {
        _input = Vector3.zero;
        StartCoroutine(BuscarJoystick());
    }

    IEnumerator BuscarJoystick()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame(); // doble frame por seguridad

        Joystick[] joysticks = FindObjectsByType<Joystick>(FindObjectsSortMode.None);
        if (joysticks.Length > 0)
        {
            _joystickMovement = joysticks[0];
            Debug.Log("Joystick encontrado: " + _joystickMovement.name);
        }
        else
        {
            Debug.LogWarning("No se encontró joystick en la escena.");
        }
    }

    void Update()
    {
        GatherInput();
        Look();
        
    }

    void FixedUpdate()
    {
        Move();
        Animation();
    }

    void GatherInput() // Obtiene la entrada del joystick para mover al jugador, si no hay joystick no hace nada
    {
        if (_joystickMovement == null) return;
        _input = new Vector3(_joystickMovement.Horizontal, 0, _joystickMovement.Vertical);
    }

    void Look() // Hace que el jugador mire en la dirección del movimiento, si no hay input no hace nada
    {
        if (_input != Vector3.zero)
        {
            var relative = (transform.position + _input) - transform.position;
            var rotation = Quaternion.LookRotation(relative, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
        }
    }

    void Move() // Mueve al jugador en la dirección del input, si no hay input no hace nada
    {
        if (_rb == null) return;
        _rb.MovePosition(transform.position + (transform.forward * _input.magnitude) * _speed * Time.fixedDeltaTime);

    }

    public void OnTriggerEnter(Collider other) // Cambia de escena al entrar en las puertas, dependiendo de la etiqueta del objeto con el que colisiona (IMPORTANTE: aqui hay que cambiiar el noombre de las escenas si es que se llegan a cambiar en el editor )
    {
        if (other.CompareTag("puertaABatalla"))
        {
            Bruja.Instancia.ViajarACiudad();
        } else if (other.CompareTag("puertaAGranja"))
        {
            SceneManager.LoadScene("sampleScene");
        }
    }

    void Animation() {
        if (_animator == null) return;
        _animator.SetFloat("speed", _input.magnitude);
    }
}