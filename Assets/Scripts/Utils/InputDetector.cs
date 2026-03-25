using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class InputDetector : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Vector2 fingerDownPos, fingerUpPos;
    private Vector2 lastSwipedPos;
    //private readonly float swipeThreshold = 0.5f;

    public void OnBeginDrag(PointerEventData eventData)
    {
        fingerDownPos = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        fingerUpPos = Input.mousePosition;
    }
}