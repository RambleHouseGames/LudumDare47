using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FlailScreen : MonoBehaviour
{
    [SerializeField]
    private Text messageText;

    [SerializeField]
    private Text continueText;

    void Start()
    {
        SignalManager.Inst.AddListener<FlailedToDeathSignal>(onFlailedToDeath);
    }

    private void onFlailedToDeath(Signal signal)
    {
        messageText.enabled = true;
        continueText.enabled = true;
        SignalManager.Inst.AddListener<ButtonPressedSignal>(onButtonPressed);
    }

    private void onButtonPressed(Signal signal)
    {
        ButtonPressedSignal buttonPressedSignal = (ButtonPressedSignal)signal;
        if(buttonPressedSignal.InputButton == InputButton.SPACE)
            SceneManager.LoadScene("MainScene");
    }

}
