using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Inst;

    void Awake()
    {
        Inst = this;
    }

    void Update()
    {
        float mouseDeltaX = Input.GetAxis("Mouse X");
        float mouseDeltaY = Input.GetAxis("Mouse Y");

        if(mouseDeltaX != 0f || mouseDeltaY != 0f)
            SignalManager.Inst.FireSignal(new MouseMovedSignal(mouseDeltaX, mouseDeltaY));

        if(Input.GetMouseButtonDown(0))
            SignalManager.Inst.FireSignal(new ButtonPressedSignal(InputButton.LEFTCLICK));

        if(Input.GetKeyDown(KeyCode.Space))
        {
            SignalManager.Inst.FireSignal(new ButtonPressedSignal(InputButton.SPACE));
            if(Cursor.lockState == CursorLockMode.None)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            SignalManager.Inst.FireSignal(new ButtonPressedSignal(InputButton.UP));
        }

        if(Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            SignalManager.Inst.FireSignal(new ButtonReleasedSignal(InputButton.UP));
        }
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            SignalManager.Inst.FireSignal(new ButtonPressedSignal(InputButton.DOWN));
        }

        if(Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            SignalManager.Inst.FireSignal(new ButtonReleasedSignal(InputButton.DOWN));
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            SignalManager.Inst.FireSignal(new ButtonPressedSignal(InputButton.LEFT));
        }
        if(Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
        {
            SignalManager.Inst.FireSignal(new ButtonReleasedSignal(InputButton.LEFT));
        }
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            SignalManager.Inst.FireSignal(new ButtonPressedSignal(InputButton.RIGHT));
        }
        if(Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
        {
            SignalManager.Inst.FireSignal(new ButtonReleasedSignal(InputButton.RIGHT));
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            SignalManager.Inst.FireSignal(new ButtonPressedSignal(InputButton.P));
        }
    }

    public Vector2 GetMoveVector()
    {
        Vector2 moveVector = new Vector2(0, 0);

        if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            moveVector.y += 1;
        if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            moveVector.y -= 1;
        if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            moveVector.x += 1;
        if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            moveVector.x -= 1;

        return moveVector;
    }
}

public enum InputButton {SPACE, LEFTCLICK, UP, DOWN, LEFT, RIGHT, P}