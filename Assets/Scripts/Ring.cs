using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public static Ring Inst;

    [SerializeField]
    private float rotateSpeed = 1f;

    private bool shouldRotate = false;

    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        SignalManager.Inst.AddListener<GoSignal>(onGo);
        SignalManager.Inst.AddListener<JumpStartedSignal>(onJumpStarted);
    }

    void Update()
    {
        if(shouldRotate)
            transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
    }

    private void onGo(Signal signal)
    {
        shouldRotate = true;
    }

    private void onJumpStarted( Signal signal)
    {
        //shouldRotate = false;
    }
}
