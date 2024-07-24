
using UnityEngine;

public class Vida : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.gameManager.IncrementaVidas();
            Destroy(gameObject); // Destruir el objeto de vida una vez recogido
        }
    }
}
