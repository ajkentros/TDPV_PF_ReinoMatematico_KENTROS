
using UnityEngine;

public class Salida : MonoBehaviour
{
    // Gestiona el trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        // Comprueba si el objeto que entra en el trigger es el jugador
        if (other.CompareTag("Player"))
        {
            //Debug.Log("el player lleg� al final del laberinto");
            // Llama al m�todo para ganar el juego
            DetieneAnimacionPlayer(other.gameObject);
        }

        // Notifica al GameManager que el juego ha terminado
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.TerminaNivel();
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

        AudioManager.audioManager.StopSonidos();
        AudioManager.audioManager.StopMusicaFondo(1);
        AudioManager.audioManager.PlayMusicaFondo(0);
    }
}
