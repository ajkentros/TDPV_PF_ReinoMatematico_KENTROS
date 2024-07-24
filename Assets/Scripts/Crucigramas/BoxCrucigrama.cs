using UnityEngine;
using System;

public class BoxCrucigrama : MonoBehaviour
{
    //public static event Action OnPlayerCollideBoxMatematico; // Evento est�tico
    public static event Action OnPlayerCollideBoxCrucigrama; // Evento est�tico con referencia al BoxMatematico

   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Invocar el evento cuando colisiona con el Player
            OnPlayerCollideBoxCrucigrama?.Invoke();

            Destroy(gameObject);

            //Debug.Log("Player");
        }
    }
}
