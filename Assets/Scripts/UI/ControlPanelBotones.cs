using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ControlPanelBotones : MonoBehaviour
{
    [SerializeField] private GameObject panelAyuda; // Panel de Ayuda en la escena de juego
    //[SerializeField] private GameObject panelReino; // Panel Reino en la escena de Menú Inicio

    public void BotonPausa()
    {
        GameManager.gameManager.PausarJuego();
    }

    public void BotonMenu()
    {
        // Cargar la escena 0 (MenuInicio)
        SceneManager.LoadScene(0, LoadSceneMode.Single);

        // Activar el PanelReino en la escena MenuInicio
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0) // Verifica que estamos en la escena 0
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Transform panelReinoTransform = canvas.transform.Find("PanelReino");
                if (panelReinoTransform != null)
                {
                    Debug.Log("PanelReino");
                    panelReinoTransform.gameObject.SetActive(true);
                }
            }
            // Desuscribirse del evento
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void BotonAyuda()
    {
        // Activar el PanelAyuda en la escena de juego
        if (panelAyuda != null)
        {
            GameManager.gameManager.PausarJuego();
            panelAyuda.SetActive(true);
        }
    }

    public void BotonVolver()
    {
        // Desactivar el PanelAyuda y reanudar el juego
        if (panelAyuda != null)
        {
            
            if (GameManager.gameManager.GetJuegoPausado())
            {
                GameManager.gameManager.PausarJuego(); // Esta línea alternará el estado de pausa
            }
            panelAyuda.SetActive(false);
        }
        
    }
}
