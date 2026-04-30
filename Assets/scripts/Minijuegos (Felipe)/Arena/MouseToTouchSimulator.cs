using UnityEngine;

/// <summary>
/// Simula entrada táctil usando el mouse del PC.
/// Eliminar este archivo después de probar en dispositivo móvil.
/// </summary>
public class MouseToTouchSimulator : MonoBehaviour
{
    public static Touch[] simulatedTouches = new Touch[0];

    void Update()
    {
        simulatedTouches = GetSimulatedTouches();
    }

    Touch[] GetSimulatedTouches()
    {
        if (Input.GetMouseButton(0))
        {
            Touch touch = new Touch();
            
            touch.fingerId = 0;
            touch.position = Input.mousePosition;
            touch.deltaPosition = Input.mousePosition - lastMousePosition;
            touch.deltaTime = Time.deltaTime;
            touch.tapCount = 1;
            
            if (Input.GetMouseButtonDown(0))
                touch.phase = TouchPhase.Began;
            else if (Input.GetMouseButtonUp(0))
                touch.phase = TouchPhase.Ended;
            else
                touch.phase = TouchPhase.Moved;

            lastMousePosition = Input.mousePosition;
            return new Touch[] { touch };
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            Touch touch = new Touch();
            touch.fingerId = 0;
            touch.position = lastMousePosition;
            touch.phase = TouchPhase.Ended;
            lastMousePosition = Vector3.zero;
            return new Touch[] { touch };
        }

        return new Touch[0];
    }

    private Vector3 lastMousePosition;
}