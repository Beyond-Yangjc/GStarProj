using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public enum E_EventType
    {
        eNull = 0,
        Down,
        Hold,
        Up,
    }

    public delegate void OnMouse0Handler(E_EventType _event, Vector3 _posi);

    public static OnMouse0Handler OnMouse0 = null;

    public delegate void OnMouse1Handler(E_EventType _event, Vector3 _pos);

    public static OnMouse1Handler OnMouse1 = null;

    public delegate void OnKeySHandler(E_EventType _event);

    public static OnKeySHandler onKeyS = null;
    
    public static bool bLock = false;

    void Update()
    {
        if(bLock)return;
        if (Input.GetMouseButtonDown(0))
            OnMouse0?.Invoke(E_EventType.Down, Input.mousePosition);
        if (Input.GetMouseButton(0))
            OnMouse0?.Invoke(E_EventType.Hold, Input.mousePosition);
        if (Input.GetMouseButtonUp(0))
            OnMouse0?.Invoke(E_EventType.Up, Input.mousePosition);

        if (Input.GetMouseButtonDown(1))
            OnMouse1?.Invoke(E_EventType.Down, Input.mousePosition);
        if (Input.GetMouseButton(1))
            OnMouse1?.Invoke(E_EventType.Hold, Input.mousePosition);
        if (Input.GetMouseButtonUp(1))
            OnMouse1?.Invoke(E_EventType.Up, Input.mousePosition);
        
        if(Input.GetKeyDown(KeyCode.S))
            onKeyS?.Invoke(E_EventType.Down);
        if(Input.GetKey(KeyCode.S))
            onKeyS?.Invoke(E_EventType.Hold);
        if(Input.GetKeyUp(KeyCode.S))
            onKeyS?.Invoke(E_EventType.Up);
    }
}