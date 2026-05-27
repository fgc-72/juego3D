using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Rigidbody _rb;
    private Vector3 _input;
    [SerializeField] private Joystick _joystickMovement;
    [SerializeField] private float _rotationSpeed = 360f;
    [SerializeField] private Animator _animator;

    void Start()
    {
        _input = Vector3.zero;
        _animator = GetComponent<Animator>();
        
    }

    /*void OnEnable()
    {
        SceneManager.sceneLoaded += OnScenaCargada;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnScenaCargada;
    }

    void OnScenaCargada(Scene escena, LoadSceneMode mode)
    {
        _input = Vector3.zero;
        _joystickMovement = null;

        // Solo busca joystick en las escenas que lo tienen
        if (escena.name == "ciudad1" || escena.name == "SampleScene")
        {
            StartCoroutine(BuscarJoystick());
        }
    }

    IEnumerator BuscarJoystick()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

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
    }*/

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

    void GatherInput()
    {
        if (_joystickMovement == null) return;
        _input = new Vector3(_joystickMovement.Horizontal, 0, _joystickMovement.Vertical);
    }

    void Look()
    {
        if (_input != Vector3.zero)
        {
            var relative = (transform.position + _input) - transform.position;
            var rotation = Quaternion.LookRotation(relative, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
        }
    }

    void Move()
    {
        if (_rb == null) return;
        _rb.MovePosition(transform.position + (transform.forward * _input.magnitude) * _speed * Time.fixedDeltaTime);
    }

    void Animation()
    {
        if (_animator == null) return;
        _animator.SetFloat("speed", _input.magnitude);
    }
}