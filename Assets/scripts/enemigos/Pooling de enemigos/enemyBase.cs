using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private EnemyData _data;
    private int _vidaMaxima = 100;
    private int _vidaActual;

    // Llamado automáticamente por el PoolManager al crear la instancia
    public void Inicializar(EnemyData data)
    {
        _data = data;
    }

    // Llamado cada vez que el enemigo es sacado de la pool
    void OnEnable()
    {
        _vidaActual = _vidaMaxima;
    }

    public void RecibirDaño(int cantidad)
    {
        _vidaActual -= cantidad;
        if (_vidaActual <= 0)
            Morir();
    }

    private void Morir()
    {
        // Aquí animaciones, drops, efectos...
        GameManagerBatalla.Instance?.SumarPunto();
        poolManager.Instance.ReturnToPool(_data, gameObject);
    }

}