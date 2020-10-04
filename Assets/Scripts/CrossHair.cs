using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
    [SerializeField]
    private Image myImage;

    // Start is called before the first frame update
    void Start()
    {
        SignalManager.Inst.AddListener<GoSignal>(onGo);
    }

    // Update is called once per frame
    private void onGo(Signal signal)
    {
        myImage.enabled = true;
    }
}
