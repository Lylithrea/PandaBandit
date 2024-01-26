using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    public delegate void OnClick();

    public OnClick onLeftClick;
    public OnClick onRightClick;

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
        onLeftClick?.Invoke();
        
    }

    public void OnLeftClickUp(InputValue value)
    {
        Debug.Log("Left click up!");
    }

    public void OnShiftClick(InputValue value)
    {
        Debug.Log("Shift click up!");
    }


    public void OnRightClick(InputValue value)
    {
        Debug.Log("Right click!");
        onRightClick?.Invoke();
    }



}
