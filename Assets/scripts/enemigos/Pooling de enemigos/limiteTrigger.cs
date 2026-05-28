using UnityEngine;

public class limiteTrigger : MonoBehaviour
{

    [SerializeField] private GameManagerBatalla gameManagerBatalla;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemigo"))
        {
            gameManagerBatalla.DerrotaPorInvasion();
        }
    }
    
}
