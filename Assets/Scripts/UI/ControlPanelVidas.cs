using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanelVidas : MonoBehaviour
{
    public Image[] corazones; // Array de imágenes de los corazones

    private void OnEnable()
    {
        //Debug.Log("ControlPanelVidas OnEnable");

        // Suscribirse al evento del GameManager
        if (GameManager.gameManager != null)
        {
            //Debug.Log("GameManager encontrado, registrando ControlPanelVidas");
            GameManager.gameManager.RegistrarControlPanelVidas(this);
            //Debug.Log("GameManager.gameManager.RegistrarControlPanelVidas(this)");
        }
    }

    private void OnDisable()
    {
        //Debug.Log("ControlPanelVidas OnDisable");

        // Desuscribirse del evento del GameManager
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.DesregistrarControlPanelVidas(this);
            //Debug.Log("DesregistrarControlPanelVidas(this)");
        }


    }


    public void ActualizaVidas(int vidas)
    {
        //Debug.Log("ActualizaVidas llamada con vidas: " + vidas);
        for (int i = 0; i < corazones.Length; i++)
        {
            corazones[i].gameObject.SetActive(i < vidas);
            //Debug.Log("Vida " + i + corazones[i].gameObject.activeSelf);S

        }
    }
}
