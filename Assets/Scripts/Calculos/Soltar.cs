using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class Soltar : MonoBehaviour, IDropHandler
{
    [SerializeField] private bool zonaActiva; // Para activar o desactivar la zona de drop

    public event Action<Sprite> OnDropEvent; // Evento que se dispara al soltar una imagen

    public void OnDrop(PointerEventData eventData)
    {
        if (!zonaActiva)
        {
            //Debug.Log("Zona de drop no está activa.");
            return; // No permitir el drop si la zona no está activa
        }

        if (eventData.pointerDrag != null)
        {
            //Debug.Log($"Objeto arrastrado: {eventData.pointerDrag.name}");

            RectTransform draggedRectTransform = eventData.pointerDrag.GetComponent<RectTransform>();
            RectTransform dropZoneRectTransform = GetComponent<RectTransform>();

            // Calcular la nueva posición para asegurar que el objeto arrastrado esté completamente dentro de la zona de drop
            Vector2 dropZoneSize = dropZoneRectTransform.sizeDelta;
            Vector2 draggedSize = draggedRectTransform.sizeDelta;

            // Calcular la posición para centrar el objeto arrastrado en la zona de drop
            Vector2 newPosition = dropZoneRectTransform.anchoredPosition;
            newPosition.x += (dropZoneSize.x - draggedSize.x) / 2;
            newPosition.y += (dropZoneSize.y - draggedSize.y) / 2;

            // Ajustar la posición del objeto arrastrado
            draggedRectTransform.anchoredPosition = newPosition;

            // Disparar el evento OnDropEvent pasando la imagen que se soltó
            Image draggedImage = eventData.pointerDrag.GetComponent<Image>();

            if (draggedImage != null && OnDropEvent != null)
            {
                //Debug.Log($"OnDropEvent disparado para {draggedImage.sprite.name}");
                OnDropEvent(draggedImage.sprite);
            }
        }
    }

    public bool GetZonaActiva()
    {
        return zonaActiva;
    }

    public void SetZonaActiva(bool _zonaActiva)
    {
        zonaActiva = _zonaActiva;
    }
}