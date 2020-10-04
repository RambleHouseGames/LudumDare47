using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCube : MonoBehaviour
{
    [SerializeField]
    private float collectDistance = 1f;

    [SerializeField]
    private ParticleSystem PopParticles;

    [SerializeField]
    private Renderer renderer;

    private bool didPop = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!didPop)
        {
            transform.Rotate(new Vector3(5f * Time.deltaTime, 3f * Time.deltaTime, 4f * Time.deltaTime));
            if(Vector3.Distance(MainCharacter.Inst.Collector.transform.position, transform.position) <= collectDistance)
            {
                SignalManager.Inst.FireSignal(new IceCollectedSignal());
                renderer.enabled = false;
                PopParticles.Emit(100);
                didPop = true;
            }
        }
        else if(PopParticles.particleCount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
