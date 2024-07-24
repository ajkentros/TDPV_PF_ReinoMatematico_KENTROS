using UnityEngine;
using System;
using System.Collections;
using TMPro;

public class BoxMatematico : MonoBehaviour
{
 
    //public static event Action OnPlayerCollideBoxMatematico; // Evento est�tico
    public static event Action<BoxMatematico> OnPlayerCollideBoxMatematico; // Evento est�tico con referencia al BoxMatematico

    private PlayerControl playerControl; // Referencia al script del Player

    [Header("Texto")]
    [SerializeField] private TextMeshProUGUI letra; // Referencia al TextMeshPro del hijo

    private Animator animator;
    private Collider2D boxCollider; // Referencia al Collider2D del BoxMatematico



    private void Start()
    {
        // Buscar y asignar el PlayerControl en tiempo de ejecuci�n
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerControl = playerObject.GetComponent<PlayerControl>();
        }
        else
        {
            Debug.LogError("No se encontr� un objeto con la etiqueta 'Player' en la escena.");
        }

        animator = GetComponentInChildren<Animator>(); // Obtener el Animator del hijo
        boxCollider = GetComponent<Collider2D>(); // Obtener el Collider2D del BoxMatematico

        // Inicialmente desactivar el texto
        if (letra != null)
        {
            letra.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Desactivar el movimiento del Player
            if (playerControl != null)
            {
                playerControl.SetMovimientoPlayer(false);
                
            }
            
            // Desactivar el collider del BoxMatematico
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }

            // Activar la transici�n par abrir la caja
            animator.SetBool("Abre", true);

            // Invocar el evento cuando colisiona con el Player
            OnPlayerCollideBoxMatematico?.Invoke(this);
        }
    }

    // M�todo llamado al finalizar la animaci�n Open-Close
    public void IniciaAnimationCajaAbierta()
    {
        //Debug.Log("IniciaAnimationCajaAbierta");
        if (letra != null)
        {
            letra.gameObject.SetActive(true); // Activar el texto cuando la animaci�n abrir caja comienza
        }

    }


    // M�todo llamado al finalizar la animaci�n Open-Close
    public void FinalizaAnimacionCajaAbierta()
    {
        //Debug.Log("FinalizaAnimacionCajaAbierta");
        
        StartCoroutine(DestroyAfterDelay(2f)); // Iniciar Coroutine para destruir despu�s de 2 segundos
    }



    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        playerControl.SetMovimientoPlayer(true);

        Destroy(gameObject); // Destruir el GameObject
        
    }

    // Funci�n p�blica para establecer la letra desde el controlador
    public void SetLetra(char nuevaLetra)
    {
       // Debug.Log(nuevaLetra.ToString());
        if (letra != null)
        {
            letra.text = nuevaLetra.ToString();
        }
    }
}

