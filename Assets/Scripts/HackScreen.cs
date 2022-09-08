using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackScreen : MonoBehaviour
{
    [SerializeField]
    private Material blankMaterial;

    [SerializeField]
    private Material openMouthMaterial;

    [SerializeField]
    private Material closedMouthMaterial;

    [SerializeField]
    private float frameTime = .5f;

    private bool isPlaying = false;

    private float frameTimer = 0;

    private MeshRenderer myRenderer;

    private bool mouthIsOpen = false;

    public void Awake()
    {
        frameTimer = frameTime;
        myRenderer = GetComponent<MeshRenderer>();
    }

    public void Update()
    {
        if(isPlaying)
        {
            frameTimer -= Time.deltaTime;
            if(frameTimer <= 0f)
            {
                if(mouthIsOpen)
                {
                    myRenderer.material = closedMouthMaterial;
                    mouthIsOpen = false;
                }
                else
                {
                    myRenderer.material = openMouthMaterial;
                    mouthIsOpen = true;
                }
                frameTimer = frameTime;
            }
        }
        else
            myRenderer.material = blankMaterial;
    }

    public void StartAnimating()
    {
        isPlaying = true;
    }

    public void StopAnimating()
    {
        isPlaying = false;
    }
}
