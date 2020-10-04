using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceCounter : MonoBehaviour
{
    [SerializeField]
    private Text textField;

    public int IceCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        SignalManager.Inst.AddListener<IceCollectedSignal>(onIceCollected);
    }

    private void onIceCollected(Signal signal)
    {
        IceCount++;
        textField.text = "Ice collected: " + IceCount;
    }
}
