
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderDebug : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Slider received pointer down!");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Slider received drag!");
    }
}