using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public static Ring Inst;

    [SerializeField]
    private GameObject startingRock;

    [SerializeField]
    private float rotateSpeed = 1f;

    [SerializeField]
    private GameObject smallRockPrefab;

    [SerializeField]
    private GameObject mediumRockPrefab;

    [SerializeField]
    private GameObject largeRockPrefab;

    [SerializeField]
    private GameObject smallRockWithIcePrefab;

    [SerializeField]
    private GameObject mediumRockWithIcePrefab;

    [SerializeField]
    private GameObject largeRockWithIcePrefab;

    [SerializeField]
    private int numberOfPlanets = 100;

    [SerializeField]
    private float minDistance = 10f;

    [SerializeField]
    private float innerRadius = 100f;

    [SerializeField]
    private float ceiling;

    [SerializeField]
    private float floor;

    [SerializeField]
    private float outterRadius = 200f;

    private List<GameObject> rocks = new List<GameObject>();

    private bool shouldRotate = false;

    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        SignalManager.Inst.AddListener<GoSignal>(onGo);
        SignalManager.Inst.AddListener<JumpStartedSignal>(onJumpStarted);
        buildRing();
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

    private void buildRing()
    {
        rocks.Add(startingRock);
        float rotationProgress = 0f;
        for(int i = 0; i < numberOfPlanets; i++)
        {
            GameObject prefab;
            int rand = UnityEngine.Random.Range(0, 6);
            switch(rand)
            {
                case 0:
                    prefab = smallRockPrefab;
                    break;
                case 1:
                    prefab = mediumRockPrefab;
                    break;
                case 2:
                    prefab = largeRockPrefab;
                    break;
                case 3:
                    prefab = smallRockWithIcePrefab;
                    break;
                case 4:
                    prefab = mediumRockWithIcePrefab;
                    break;
                case 5:
                    prefab = largeRockWithIcePrefab;
                    break;
                default:
                    prefab = null;
                    Debug.Log("INVALID RANDOM NUMBER: " + rand);
                    break;
            }

            float distanceFromCenter = UnityEngine.Random.Range(innerRadius, outterRadius);
            float elevation = UnityEngine.Random.Range(floor, ceiling);
            GameObject newPlanet = Instantiate(prefab, new Vector3(0f, elevation, distanceFromCenter), Quaternion.identity, transform);
            float rotationChange = UnityEngine.Random.Range(360f / (float)numberOfPlanets, 360f / ((float)numberOfPlanets) * 2f);
            if(rotationProgress > 360)
                rotationChange = UnityEngine.Random.Range(0f, 360f);
            newPlanet.transform.RotateAround(transform.position, Vector3.up, rotationProgress + rotationChange);
            while(isTooClose(newPlanet))
            {
                rotationChange += UnityEngine.Random.Range(0f, 10f);
                newPlanet.transform.RotateAround(transform.position, Vector3.up, rotationChange);
            }
            rotationProgress += rotationChange;
        }
    }

    private bool isTooClose(GameObject newPlanet)
    {
        foreach(GameObject existingRock in rocks)
        {
            if(Vector3.Distance(existingRock.transform.position, newPlanet.transform.position) < minDistance)
                return true;
        }
        return false;
    }
}
