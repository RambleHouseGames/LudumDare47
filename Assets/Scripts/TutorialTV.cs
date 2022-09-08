using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialTV : MonoBehaviour
{
    [SerializeField]
    private Animator myAnimator;

    [SerializeField]
    private GameObject LeftPropeller;

    [SerializeField]
    private GameObject RightPropeller;

    [SerializeField]
    private AudioSource AudioSource;

    [SerializeField]
    private HackScreen HackScreen;

    private bool tutorialIsPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        SignalManager.Inst.AddListener<FlyInFinishedSignal>(onFlyInFinished);
        SignalManager.Inst.AddListener<ButtonPressedSignal>(onButtonPressed);
    }

    // Update is called once per frame
    void Update()
    {
        LeftPropeller.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
        RightPropeller.transform.Rotate(0f, 720f * Time.deltaTime, 0f);

        if(tutorialIsPlaying && !AudioSource.isPlaying)
        {
            onVideoEnd();
            tutorialIsPlaying = false;
        }
    }

    private void onFlyInFinished(Signal signal)
    {
        SignalManager.Inst.RemoveListener<FlyInFinishedSignal>(onFlyInFinished);
        myAnimator.SetTrigger("FlyIn");
    }

    private void onReachedDisplayPosition()
    {
        AudioSource.Play();
        HackScreen.StartAnimating();
        tutorialIsPlaying = true;
    }

    private void onVideoEnd()
    {
        HackScreen.StopAnimating();
        myAnimator.SetTrigger("FlyOut");
    }

    private void onFinishedFlyOut()
    {
        SignalManager.Inst.FireSignal(new TutorialCompleteSignal ());
    }

    private void onButtonPressed(Signal signal)
    {
        ButtonPressedSignal buttonPressedSignal = (ButtonPressedSignal)signal;

        if(buttonPressedSignal.InputButton == InputButton.P)
        {
            myAnimator.SetTrigger("Abort");
            SignalManager.Inst.FireSignal(new TutorialCompleteSignal());
        }
    }
}
