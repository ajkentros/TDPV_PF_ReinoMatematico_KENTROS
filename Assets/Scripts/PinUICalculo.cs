
using UnityEngine;

public class PinUICalculo : MonoBehaviour
{
    public Transform worldPosition;
    new public Camera camera;
    RectTransform rect;
    private void Start()
    {
        rect = GetComponent<RectTransform>();
        if (camera == null)
        {
            camera = Camera.main;
        }
    }
    private void LateUpdate()
    {
        if (worldPosition)
        {
            Vector2 pos = camera.WorldToViewportPoint(worldPosition.position);
            rect.anchorMax = pos;
            rect.anchorMin = pos;
        }
    }
}
