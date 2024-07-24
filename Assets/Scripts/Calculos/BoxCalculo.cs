using UnityEngine;
using System;


public class BoxCalculo : MonoBehaviour
{
    public static event Action OnPlayerCollide; // Evento estático

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("OnCollisionEnter2D triggered"); // Añadido para verificar si la función se llama

        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Collision with Player detected"); // Añadido para verificar si la condición se cumple

            OnPlayerCollide?.Invoke();
            Destroy(gameObject);

            //Debug.Log("Player"); // Para verificar que se llega a este punto
        }
        else
        {
            //Debug.Log($"Collision with {collision.gameObject.tag} detected"); // Añadido para verificar otras colisiones
        }
    }

    
}
