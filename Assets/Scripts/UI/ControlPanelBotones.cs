using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanelBotones : MonoBehaviour
{
    public void BotonPausa()
    {
        GameManager.gameManager.PausarJuego();
    }
}
