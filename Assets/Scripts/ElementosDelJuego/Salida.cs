
using UnityEngine;

public class Salida : MonoBehaviour
{
    // Gestiona cuando otro collider entra en el trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Comprobamos si el objeto que entra en el trigger es el jugador
        if (other.CompareTag("Player"))
        {
            // Llamamos al m�todo para ganar el juego
            DetieneAnimacionPlayer(other.gameObject);
        }

        // Notificar al GameManager que el juego ha terminado
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.NivelTerminado();
        }
    }

    // M�todo para ganar el juego
    private void DetieneAnimacionPlayer(GameObject player)
    {
        
        // Llama al m�todo DetenerAnimacion en el script PlayerControl del jugador
        if (player.TryGetComponent<PlayerControl>(out var playerControl))
        {
            playerControl.DetenerAnimacion();
        }

        
    }
}
