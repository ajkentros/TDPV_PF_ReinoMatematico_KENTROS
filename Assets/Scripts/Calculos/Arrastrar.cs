using UnityEngine;
using UnityEngine.EventSystems;

public class Arrastrar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private bool isDroppedInValidZone;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = rectTransform.anchoredPosition;
        isDroppedInValidZone = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f; // Aumentar transparencia al arrastrar
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvasGroup.transform.lossyScale.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        isDroppedInValidZone = false;

        Transform dropTarget = eventData.pointerEnter != null ? eventData.pointerEnter.transform : null;

        // Buscar el objeto con el tag DropZone en la jerarquía de padres
        while (dropTarget != null && !dropTarget.CompareTag("DropZone"))
        {
            dropTarget = dropTarget.parent;
        }

        if (dropTarget != null && dropTarget.CompareTag("DropZone"))
        {
            Soltar dropZone = dropTarget.GetComponent<Soltar>();
            if (dropZone != null && dropZone.GetZonaActiva())
            {
                isDroppedInValidZone = true;
                //Debug.Log("isDroppedInValidZone = " + isDroppedInValidZone);
                dropZone.OnDrop(eventData); // Mover a esta línea
            }
        }

        // Volver a la posición original si no se hizo drop en una zona válida
        if (!isDroppedInValidZone)
        {
            rectTransform.anchoredPosition = originalPosition;
        }



        //Debug.Log($"isDroppedInValidZone = {isDroppedInValidZone}");
    }

    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
        //Debug.Log($"Posición reseteada para {gameObject.name}");
    }
}

