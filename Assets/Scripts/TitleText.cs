using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SignalManager.Inst.AddListener<ButtonPressedSignal>(onButtonPressed);
    }

    private void onButtonPressed(Signal signal)
    {
        ButtonPressedSignal buttonPressedSignal = (ButtonPressedSignal)signal;
        if(buttonPressedSignal.InputButton == InputButton.SPACE)
        {
            SignalManager.Inst.RemoveListener<ButtonPressedSignal>(onButtonPressed);
            Destroy(gameObject);
        }
    }
}
