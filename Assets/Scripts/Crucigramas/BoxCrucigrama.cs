using UnityEngine;
using System;

public class BoxCrucigrama : MonoBehaviour
{
    //public static event Action OnPlayerCollideBoxMatematico; // Evento estático
    public static event Action OnPlayerCollideBoxCrucigrama; // Evento estático con referencia al BoxMatematico

   
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
