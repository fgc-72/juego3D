using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Rigidbody _rb      ;
    private Vector3 _input;
    [SerializeField] private Joystick _joystickMovement;
    [SerializeField] private float _rotationSpeed = 360f;

    void Start()
    {
        if (_rb == null) Debug.LogWarning("PlayerMovement: Rigidbody (_rb) not assigned in inspector.");
        if (_joystickMovement == null) Debug.LogWarning("PlayerMovement: Joystick (_joystickMovement) not assigned in inspector.");
    }

    void Update()
    {
        GatherInput();
        Look();
    }

    void FixedUpdate()
    {
        Move();
    }

    void GatherInput()
    {
        _input = new Vector3(_joystickMovement.Horizontal, 0, _joystickMovement.Vertical);

    }

    void Look(){
        if (_input != Vector3.zero) 
        {
            var relative = (transform.position + _input) - transform.position;
            var rotation = Quaternion.LookRotation(relative, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
            
        }
    }
    void Move()
    {
        _rb.MovePosition(transform.position + (transform.forward * _input.magnitude) * _speed * Time.fixedDeltaTime);
        
    }
}
