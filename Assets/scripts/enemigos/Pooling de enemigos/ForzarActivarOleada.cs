using UnityEngine;

public class ForzarActivarOleada : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            waveManager.IniciarOleada();
    }
}