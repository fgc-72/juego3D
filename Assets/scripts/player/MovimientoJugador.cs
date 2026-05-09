using UnityEngine;
using UnityEngine.SceneManagement;
public class MovimientoJugador : MonoBehaviour
{
    [SerializeField] float velocidad = 5f;
    [SerializeField] Joystick joystickMovimiento;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        Vector3 direccion = transform.right * joystickMovimiento.Horizontal + transform.forward * joystickMovimiento.Vertical;
        rb.linearVelocity = new Vector3(direccion.x * velocidad, rb.linearVelocity.y, direccion.z * velocidad);
    }

    
}