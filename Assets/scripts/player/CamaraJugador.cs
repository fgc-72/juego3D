using UnityEngine;

public class CamaraJugador : MonoBehaviour
{
    [SerializeField] float sensibilidad = 2f;
    [SerializeField] Transform cuerpo;
    float rotacionX = 0f;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch toque in Input.touches)
            {
                // solo detecta toques en la mitad derecha de la pantalla
                if (toque.position.x > Screen.width / 2)
                {
                    float mirarX = toque.deltaPosition.x * sensibilidad * Time.deltaTime;
                    float mirarY = toque.deltaPosition.y * sensibilidad * Time.deltaTime;

                    cuerpo.Rotate(Vector3.up * mirarX);

                    rotacionX -= mirarY;
                    rotacionX = Mathf.Clamp(rotacionX, -80f, 80f);
                    transform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
                }
            }
        }
    }
}