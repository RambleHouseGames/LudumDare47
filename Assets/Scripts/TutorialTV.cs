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
    private VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        SignalManager.Inst.AddListener<FlyInFinishedSignal>(onFlyInFinished);
    }

    // Update is called once per frame
    void Update()
    {
        LeftPropeller.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
        RightPropeller.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
    }

    private void onFlyInFinished(Signal signal)
    {
        SignalManager.Inst.RemoveListener<FlyInFinishedSignal>(onFlyInFinished);
        myAnimator.SetTrigger("FlyIn");
    }

    private void onReachedDisplayPosition()
    {
        videoPlayer.Play();
        videoPlayer.loopPointReached += onVideoEnd;
    }

    private void onVideoEnd(UnityEngine.Video.VideoPlayer vp)
    {
        myAnimator.SetTrigger("FlyOut");
    }

    private void onFinishedFlyOut()
    {
        SignalManager.Inst.FireSignal(new TutorialCompleteSignal ());
    }
}
