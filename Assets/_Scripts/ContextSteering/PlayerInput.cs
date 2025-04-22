using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerInput : MonoBehaviour
{
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
    public UnityEvent OnAttack;

    private void Update()
    {
        OnMovementInput?.Invoke(new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical")));
        OnPointerInput?.Invoke(GetPointerInput());
        if(Input.GetMouseButtonDown(0))
            OnAttack?.Invoke();

    }

    private Vector2 GetPointerInput()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    
}
