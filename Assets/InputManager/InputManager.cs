using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    public delegate void OnClick();

    public OnClick onLeftClickUp;
    public OnClick onLeftClickDown;
    public OnClick onRightClickUp;
    public OnClick onRightClickDown;
    public OnClick onShiftChange;


    private bool isShiftPressed = false;
    public bool ShiftPressed { get { return isShiftPressed; } }

    public void Awake()
    {
        singletonCreation();
    }

    public static InputManager Instance
    {
        get
        {
            return instance;
        }
    }

    void singletonCreation()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void OnLeftClickDown(InputValue value)
    {
        Debug.Log("Left click down!");
        onLeftClickDown?.Invoke();
    }

    public void OnLeftClickUp(InputValue value)
    {
        Debug.Log("Left click up!");
        onLeftClickUp?.Invoke();
    }


    public void OnRightClickDown(InputValue value)
    {
        Debug.Log("Right click down!");
        onRightClickDown?.Invoke();
    }

    public void OnRightClickUp(InputValue value)
    {
        Debug.Log("Right click up!");
        onRightClickUp?.Invoke();
    }

    public void OnShift(InputValue value)
    {
        isShiftPressed = !isShiftPressed;
        Debug.Log("Is shift pressed?: " + isShiftPressed);
        onShiftChange?.Invoke();
    }



}
