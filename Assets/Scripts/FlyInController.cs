using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyInController : MonoBehaviour
{
    [SerializeField]
    private Animator myAnimator;

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
            myAnimator.SetTrigger("FlyIn");
        }
    }

    private void onFlyInFinished()
    {
        SignalManager.Inst.FireSignal(new FlyInFinishedSignal());
    }
}
