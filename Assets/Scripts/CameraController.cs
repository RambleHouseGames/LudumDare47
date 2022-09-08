using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Inst;

    public float positionMoveSpeed = 1f;

    [SerializeField]
    public float RotationSpeed { get { return rotationSpeed; } }

    [SerializeField]
    private float rotationSpeed = 1f;

    [SerializeField]
    private GameObject focalPoint;

    public GameObject FollowPlayerCameraHolder;

    public GameObject FollowPlayerFocalPointHolder;

    private CameraState currentState;

    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        currentState = new CameraIntroState();
        currentState.Start();
    }

    void Update()
    {
        CameraState nextState = currentState.Update();
        if(nextState != currentState)
        {
            currentState.Finish();
            currentState = nextState;
            nextState.Start();
        }
    }

    public void AttachToPlayerFollowHolder()
    {
        Camera.main.transform.SetParent(FollowPlayerCameraHolder.transform);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
    }

    public void LookAtFocalPoint(Vector3 targetUp)
    {
        transform.LookAt(focalPoint.transform.position, targetUp);
    }
}

public abstract class CameraState
{
    public virtual void Start() {}
    public abstract CameraState Update();
    public virtual void Finish() {}
}

public class CameraIntroState : CameraState
{
    private CameraState nextState;

    public override void Start()
    {
        nextState = this;
        SignalManager.Inst.AddListener<TutorialCompleteSignal>(onTutorialCompleted);
    }

    public override CameraState Update()
    {
        return nextState;
    }

    public override void Finish()
    {
        SignalManager.Inst.RemoveListener<TutorialCompleteSignal>(onTutorialCompleted);
    }

    private void onTutorialCompleted(Signal signal)
    {
        nextState = new CameraFollowPlayerState();
    }
}

public class CameraFollowPlayerState : CameraState
{
    private CameraState nextState;
    public override void Start()
    {
        CameraController.Inst.AttachToPlayerFollowHolder();
        nextState = this;
    }

    public override CameraState Update()
    {
        GameObject holder = CameraController.Inst.FollowPlayerCameraHolder;
        if(Vector3.Distance(Camera.main.transform.localPosition, Vector3.zero) > CameraController.Inst.positionMoveSpeed * Time.deltaTime)
            Camera.main.transform.localPosition = Vector3.MoveTowards(Camera.main.transform.localPosition, Vector3.zero, CameraController.Inst.positionMoveSpeed * Time.deltaTime);
        else
            Camera.main.transform.localPosition = Vector3.zero;
        if(Quaternion.Angle(Camera.main.transform.rotation, Quaternion.Euler(0f, 180f, 0f)) >= CameraController.Inst.RotationSpeed * Time.deltaTime)
            Camera.main.transform.localRotation = Quaternion.RotateTowards(Camera.main.transform.localRotation, Quaternion.Euler(0f, 180f, 0f), CameraController.Inst.RotationSpeed * Time.deltaTime);
        else
            Camera.main.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        return nextState;
    }
}

public class CameraFlyState : CameraState
{
    private CameraState nextState;

    public override void Start()
    {
        nextState = this;
        SignalManager.Inst.AddListener<PlayerLandedSignal>(onPlayerLanded);
    }

    public override CameraState Update()
    {
        CameraController.Inst.transform.position = Vector3.MoveTowards(CameraController.Inst.transform.position, MainCharacter.Inst.LandingPostion.transform.position, MainCharacter.Inst.FlySpeed * Time.deltaTime);
        return nextState;
    }

    public override void Finish()
    {
        SignalManager.Inst.RemoveListener<PlayerLandedSignal>(onPlayerLanded);
    }

    private void onPlayerLanded(Signal signal)
    {
        nextState = new CameraFollowPlayerState();
    }
}