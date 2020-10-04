using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Inst;

    [SerializeField]
    private float positionMoveSpeed = 1f;

    [SerializeField]
    private float focalPointMoveSpeed = 1f;

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
        currentState = new CameraFollowPlayerState();
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

    public void MoveFocalPointTowardsHolder(GameObject holder)
    {
        if(Vector3.Distance(focalPoint.transform.position, holder.transform.position) < focalPointMoveSpeed * Time.deltaTime)
        {
            focalPoint.transform.position = holder.transform.position;
            focalPoint.transform.SetParent(holder.transform);
        }
        else
            focalPoint.transform.position = Vector3.MoveTowards(focalPoint.transform.position, holder.transform.position, focalPointMoveSpeed * Time.deltaTime);
    }

    public void MoveCameraTowardsHolder(GameObject holder)
    {
        if(Vector3.Distance(transform.position, holder.transform.position) <= positionMoveSpeed * Time.deltaTime)
        {
            transform.position = holder.transform.position;
            transform.SetParent(holder.transform);
        }
        else
            transform.position = Vector3.MoveTowards(transform.position, holder.transform.position, positionMoveSpeed * Time.deltaTime);
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

public class CameraFollowPlayerState : CameraState
{
    private CameraState nextState;
    public override void Start()
    {
        nextState = this;
    }

    public override CameraState Update()
    {
        //CameraController.Inst.MoveCameraTowardsHolder(CameraController.Inst.FollowPlayerCameraHolder);
        //CameraController.Inst.MoveFocalPointTowardsHolder(CameraController.Inst.FollowPlayerFocalPointHolder);
        //CameraController.Inst.LookAtFocalPoint(CameraController.Inst.FollowPlayerCameraHolder.transform.up);
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