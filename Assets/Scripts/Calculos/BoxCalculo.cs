using UnityEngine;
using System;


public class BoxCalculo : MonoBehaviour
{
    public static event Action OnPlayerCollide; // Evento est�tico

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("OnCollisionEnter2D triggered"); // A�adido para verificar si la funci�n se llama

        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Collision with Player detected"); // A�adido para verificar si la condici�n se cumple

            OnPlayerCollide?.Invoke();
            Destroy(gameObject);

            //Debug.Log("Player"); // Para verificar que se llega a este punto
        }
        else
        {
            //Debug.Log($"Collision with {collision.gameObject.tag} detected"); // A�adido para verificar otras colisiones
        }
    }

    
}
