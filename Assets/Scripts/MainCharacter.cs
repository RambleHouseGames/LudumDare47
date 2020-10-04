using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainCharacter : MonoBehaviour
{
    [NonSerialized]
    public Vector3 currentRotateAxis = Vector3.zero;

    [NonSerialized]
    public MoveDirection currentMoveDirection = MoveDirection.NONE;

    public static MainCharacter Inst;

    [SerializeField]
    private Animator myAnimator;

    [SerializeField]
    private float cameraRotateSpeed = 1f;

    [SerializeField]
    private float flySpeed = 1f;
    public float FlySpeed {
        get { return flySpeed; }
    }

    [SerializeField]
    private float runSpeed = 5f;
    public float RunSpeed {
        get {return runSpeed; }
    }

    [SerializeField]
    private GameObject cameraPivot;

    [SerializeField]
    private GameObject cameraHolder;

    [SerializeField]
    private ParticleSystem ExplodeParticles;

    [SerializeField]
    private Renderer Avatar;

    private PlayerState currentState;

    public float lookX = 0f;
    public float lookY = 0f;

    public GameObject Collector;

    private GameObject landingPosition = null;
    public GameObject LandingPostion {
        get { return landingPosition; }
    }

    void Awake()
    {
        Inst = this;
        currentState = new PlayerIntroState(this);
    }

    void Start()
    {
        currentState.Start();
        Vector3 startLook = cameraPivot.transform.rotation.eulerAngles;
        lookX = startLook.x;
        lookY = startLook.y;
    }

    void Update()
    {
        PlayerState nextState = currentState.Update();
        if(nextState != currentState)
        {
            currentState.Finish();
            currentState = nextState;
            nextState.Start();
        }
        correctCameraRotation();
    }

    private void correctCameraRotation()
    {
        Quaternion newRotation = Quaternion.Euler(transform.rotation.x + lookY, transform.rotation.y + lookX, 0f);
        cameraPivot.transform.rotation = newRotation;
    }

    public void RotateCamera(float x, float y)
    {
        lookX += x * cameraRotateSpeed;
        lookY -= y * cameraRotateSpeed;
    }

    public bool SetLandingPosition() // Raycasts the jumpVector, if a collider is found a placeholder object is dropped, if not return false;
    {
        Vector3 direction = (cameraPivot.transform.position - cameraHolder.transform.position).normalized;
        RaycastHit hit;
        Ray JumpRay = new Ray(cameraPivot.transform.position, direction);
        if(Physics.Raycast(JumpRay, out hit))
        {
            if(landingPosition != null)
                Destroy(landingPosition);
            landingPosition = new GameObject("Landing Position");
            landingPosition.transform.SetParent(hit.collider.transform);
            landingPosition.transform.position = hit.point;

            Vector3 up = (hit.point - hit.collider.transform.position).normalized;
            Debug.DrawRay(hit.collider.transform.position, up * 100f, Color.blue, 100f);
            Vector3 vel = transform.up.normalized;
            Vector3 forward = vel - up * Vector3.Dot (vel, up);

            landingPosition.transform.rotation = Quaternion.LookRotation(forward.normalized, up);
            return true;
        }
        return false;
    }

    public void DebugJumpRay()
    {
        Vector3 direction = (cameraPivot.transform.position - cameraHolder.transform.position).normalized;
        Debug.DrawRay(cameraPivot.transform.position, direction * 100f);
    }

    public void StartJump()
    {
        //transform.LookAt(landingPosition.transform.position, transform.up);
        myAnimator.SetTrigger("Fly");
    }

    public bool Fly()
    {
        if(Vector3.Distance(transform.position, landingPosition.transform.position) < flySpeed * Time.deltaTime)
            return true;
        else
        {
            float percentageOfRemainingDistance = flySpeed * Time.deltaTime / Vector3.Distance(transform.position, landingPosition.transform.position);
            float remainingRotation = Quaternion.Angle(transform.rotation, landingPosition.transform.rotation);
            float rotationPerFrame = remainingRotation * percentageOfRemainingDistance;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, landingPosition.transform.rotation, rotationPerFrame);
            transform.position = Vector3.MoveTowards(transform.position, landingPosition.transform.position, flySpeed * Time.deltaTime);
            return false;
        }
    }

    public void Land()
    {
        transform.position = landingPosition.transform.position;
        transform.rotation = landingPosition.transform.rotation;
        transform.SetParent(landingPosition.transform.parent);
        myAnimator.SetTrigger("Land");
    }

    public void StartRunning()
    {
        myAnimator.SetBool("AmRunning", true);
    }

    public void StopRunning()
    {
        myAnimator.SetBool("AmRunning", false);
    }

    public Vector3 calculateRotationAxis(int x, int y)
    {
        Vector3 rockCenter = transform.parent.transform.position;
        Vector3 playerToRockCenter = rockCenter - transform.position;
        Vector3 cameraToRockCenter = rockCenter - CameraController.Inst.transform.position;
        Vector3 cameraUp = CameraController.Inst.transform.up;
        Vector3 cameraLeft = CameraController.Inst.transform.right;
        Vector3 cameraForward = CameraController.Inst.transform.forward;

        if(x == 0)
        {
            if(y == 1)
                return -Vector3.Cross(cameraToRockCenter, cameraUp).normalized;
            else if(y == -1)
                return Vector3.Cross(cameraToRockCenter, cameraUp).normalized;
            else if(y == 0)
                return Vector3.zero;
            else
                Debug.Log("INVALID Y VALUE IN MOVE VECTOR: " + y);

        }
        else if(x == -1)
        {
            if(y == 1)
                return -Vector3.Cross(cameraToRockCenter, cameraUp).normalized;
            else if(y == -1)
                return Vector3.Cross(cameraToRockCenter, cameraUp).normalized;
            else if(y == 0)
                return Vector3.Cross(playerToRockCenter, cameraLeft).normalized;
            else
                Debug.Log("INVALID Y VALUE IN MOVE VECTOR: " + y);
        }
        else if(x == 1)
        {
            if(y == 1)
                return -Vector3.Cross(cameraToRockCenter, cameraUp).normalized;
            else if(y == -1)
                return Vector3.Cross(cameraToRockCenter, cameraUp).normalized;
            else if(y == 0)
                return -Vector3.Cross(playerToRockCenter, cameraLeft).normalized;
            else
                Debug.Log("INVALID Y VALUE IN MOVE VECTOR: " + y);
        }
        else
            Debug.Log("INVALID X VALUE IN MOVE VECTOR: " + x);

        return Vector3.zero;
    }

    public void Explode()
    {
        ExplodeParticles.Emit(100);
        Avatar.enabled = false;
    }
}

public enum MoveDirection {UP, DOWN, LEFT, RIGHT, NONE}

public abstract class PlayerState
{
    protected MainCharacter mainCharacter;
    public PlayerState(MainCharacter mainCharacter)
    {
        this.mainCharacter = mainCharacter;
    }
    public virtual void Start() {}
    public abstract PlayerState Update();

    public virtual void Finish() {}
}

public class PlayerIntroState : PlayerState
{
    public PlayerIntroState(MainCharacter mainCharacter) : base (mainCharacter) {}

    private PlayerState nextState;
    public override void Start()
    {
        nextState = this;
        SignalManager.Inst.AddListener<GoSignal>(onGo);
    }
    public override PlayerState Update()
    {
        return nextState;
    }

    public override void Finish()
    {
        SignalManager.Inst.RemoveListener<GoSignal>(onGo);
    }

    private void onGo(Signal signal)
    {
        nextState = new PlayerIdleState(mainCharacter);
    }
}

public class PlayerIdleState : PlayerState
{
    private PlayerState nextState;

    public PlayerIdleState (MainCharacter mainCharacter) : base (mainCharacter) {}

    public override void Start()
    {
        nextState = this;
        SignalManager.Inst.AddListener<MouseMovedSignal>(onMouseMoved);
        SignalManager.Inst.AddListener<ButtonPressedSignal>(onButtonPressed);
    }
    public override PlayerState Update()
    {
        Vector2 moveVector = InputManager.Inst.GetMoveVector();
        if(moveVector.x == 0 && moveVector.y == 1)
        {
            nextState = new PlayerRunUpState(mainCharacter);
        }
        else if(moveVector.x == 0 && moveVector.y == -1)
        {
            nextState = new PlayerRunDownState(mainCharacter);
        }
        else if(moveVector.x == -1 && moveVector.y == 0)
        {
            nextState = new PlayerRunLeftState(mainCharacter);
        }
        else if(moveVector.x == 1 && moveVector.y == 0)
        {
            nextState = new PlayerRunRightState(mainCharacter);
        }

        return nextState;
    }

    public override void Finish()
    {
        SignalManager.Inst.RemoveListener<MouseMovedSignal>(onMouseMoved);
        SignalManager.Inst.RemoveListener<ButtonPressedSignal>(onButtonPressed);
    }

    private void onMouseMoved(Signal signal)
    {
        MouseMovedSignal mouseMovedSignal = (MouseMovedSignal)signal;
        mainCharacter.RotateCamera(mouseMovedSignal.DeltaX, mouseMovedSignal.DeltaY);
    }

    private void onButtonPressed(Signal signal)
    {
        ButtonPressedSignal buttonPressedSignal = (ButtonPressedSignal)signal;
        if(buttonPressedSignal.InputButton == InputButton.LEFTCLICK)
        {
            if(mainCharacter.SetLandingPosition())
            {
                if(mainCharacter.LandingPostion.transform.parent.name == "Planet")
                    nextState = new PlayerHitPlanetState(mainCharacter);
                else
                   nextState = new PlayerJumpState(mainCharacter);
            }
            else
            {
                nextState = new PlayerFlailState(mainCharacter);
            }
        }
    }
}

public class PlayerRunUpState : PlayerState
{
    private PlayerState nextState;

    public PlayerRunUpState(MainCharacter mainCharacter) : base (mainCharacter) {}

    public override void Start()
    {
        if(mainCharacter.currentMoveDirection != MoveDirection.UP)
        {
            mainCharacter.currentRotateAxis = mainCharacter.calculateRotationAxis(0, 1);
            mainCharacter.currentMoveDirection = MoveDirection.UP;
        }

        nextState = this;
        mainCharacter.StartRunning();
        SignalManager.Inst.AddListener<MouseMovedSignal>(onMouseMoved);
        SignalManager.Inst.AddListener<ButtonPressedSignal>(onButtonPressed);
        SignalManager.Inst.AddListener<ButtonReleasedSignal>(onButtonReleased);
    }

    public override PlayerState Update()
    {
        Vector3 rockCenter = mainCharacter.transform.parent.position;
        mainCharacter.transform.RotateAround (rockCenter, mainCharacter.currentRotateAxis, mainCharacter.RunSpeed);

        Vector3 playerToRockCenter = rockCenter - mainCharacter.transform.position;
        Vector3 forwardTangent = Vector3.Cross(playerToRockCenter, mainCharacter.currentRotateAxis).normalized;
        mainCharacter.transform.rotation = Quaternion.LookRotation(forwardTangent, -playerToRockCenter);

        Vector2 moveVector = InputManager.Inst.GetMoveVector();

        return nextState;
    }

    public override void Finish()
    {
        mainCharacter.StopRunning();
        SignalManager.Inst.RemoveListener<MouseMovedSignal>(onMouseMoved);
        SignalManager.Inst.RemoveListener<ButtonPressedSignal>(onButtonPressed);
        SignalManager.Inst.RemoveListener<ButtonReleasedSignal>(onButtonReleased);
    }

    private void onButtonPressed(Signal signal)
    {
        ButtonPressedSignal buttonPressedSignal = (ButtonPressedSignal)signal;
        
        if(mainCharacter.SetLandingPosition())
            nextState = new PlayerJumpState(mainCharacter);
        else if(buttonPressedSignal.InputButton == InputButton.DOWN)
            nextState = new PlayerIdleState(mainCharacter);

            
    }

    private void onButtonReleased(Signal signal)
    {
        ButtonReleasedSignal buttonReleasedSignal = (ButtonReleasedSignal)signal;
        if(buttonReleasedSignal.InputButton == InputButton.UP)
            nextState = new PlayerIdleState(mainCharacter);
    }

    private void onMouseMoved(Signal signal)
    {
        MouseMovedSignal mouseMovedSignal = (MouseMovedSignal)signal;
        mainCharacter.RotateCamera(mouseMovedSignal.DeltaX, mouseMovedSignal.DeltaY);
    }
}

public class PlayerRunDownState : PlayerState
{
    private PlayerState nextState;

    public PlayerRunDownState(MainCharacter mainCharacter) : base (mainCharacter) {}

    public override void Start()
    {
        if(mainCharacter.currentMoveDirection != MoveDirection.DOWN)
        {
            mainCharacter.currentRotateAxis = mainCharacter.calculateRotationAxis(0, -1);
            mainCharacter.currentMoveDirection = MoveDirection.DOWN;
        }
        
        nextState = this;
        mainCharacter.StartRunning();
        SignalManager.Inst.AddListener<MouseMovedSignal>(onMouseMoved);
        SignalManager.Inst.AddListener<ButtonPressedSignal>(onButtonPressed);
        SignalManager.Inst.AddListener<ButtonReleasedSignal>(onButtonReleased);
    }

    public override PlayerState Update()
    {
        Vector3 rockCenter = mainCharacter.transform.parent.position;
        mainCharacter.transform.RotateAround (rockCenter, mainCharacter.currentRotateAxis, mainCharacter.RunSpeed);

        Vector3 playerToRockCenter = rockCenter - mainCharacter.transform.position;
        Vector3 forwardTangent = Vector3.Cross(playerToRockCenter, mainCharacter.currentRotateAxis).normalized;
        mainCharacter.transform.rotation = Quaternion.LookRotation(forwardTangent, -playerToRockCenter);

        Vector2 moveVector = InputManager.Inst.GetMoveVector();

        return nextState;
    }

    public override void Finish()
    {
        mainCharacter.StopRunning();
        SignalManager.Inst.RemoveListener<MouseMovedSignal>(onMouseMoved);
        SignalManager.Inst.RemoveListener<ButtonPressedSignal>(onButtonPressed);
        SignalManager.Inst.RemoveListener<ButtonReleasedSignal>(onButtonReleased);
    }

    private void onButtonPressed(Signal signal)
    {
        ButtonPressedSignal buttonPressedSignal = (ButtonPressedSignal)signal;
        if(mainCharacter.SetLandingPosition())
            nextState = new PlayerJumpState(mainCharacter);
        else if(buttonPressedSignal.InputButton == InputButton.UP)
            nextState = new PlayerIdleState(mainCharacter);
    }

    private void onButtonReleased(Signal signal)
    {
        ButtonReleasedSignal buttonReleasedSignal = (ButtonReleasedSignal)signal;
        if(buttonReleasedSignal.InputButton == InputButton.DOWN)
            nextState = new PlayerIdleState(mainCharacter);
    }

    private void onMouseMoved(Signal signal)
    {
        MouseMovedSignal mouseMovedSignal = (MouseMovedSignal)signal;
        mainCharacter.RotateCamera(mouseMovedSignal.DeltaX, mouseMovedSignal.DeltaY);
    }
}

public class PlayerRunLeftState : PlayerState
{
    private PlayerState nextState;

    public PlayerRunLeftState(MainCharacter mainCharacter) : base (mainCharacter) {}

    public override void Start()
    {
        if(mainCharacter.currentMoveDirection != MoveDirection.LEFT)
        {
            mainCharacter.currentRotateAxis = mainCharacter.calculateRotationAxis(-1, 0);
            mainCharacter.currentMoveDirection = MoveDirection.LEFT;
        }

        nextState = this;
        mainCharacter.StartRunning();
        SignalManager.Inst.AddListener<MouseMovedSignal>(onMouseMoved);
        SignalManager.Inst.AddListener<ButtonPressedSignal>(onButtonPressed);
        SignalManager.Inst.AddListener<ButtonReleasedSignal>(onButtonReleased);
    }

    public override PlayerState Update()
    {
        Vector3 rockCenter = mainCharacter.transform.parent.position;
        mainCharacter.transform.RotateAround (rockCenter, mainCharacter.currentRotateAxis, mainCharacter.RunSpeed);

        Vector3 playerToRockCenter = rockCenter - mainCharacter.transform.position;
        Vector3 forwardTangent = Vector3.Cross(playerToRockCenter, mainCharacter.currentRotateAxis).normalized;
        mainCharacter.transform.rotation = Quaternion.LookRotation(forwardTangent, -playerToRockCenter);

        Vector2 moveVector = InputManager.Inst.GetMoveVector();

        return nextState;
    }

    public override void Finish()
    {
        mainCharacter.StopRunning();
        SignalManager.Inst.RemoveListener<MouseMovedSignal>(onMouseMoved);
        SignalManager.Inst.RemoveListener<ButtonPressedSignal>(onButtonPressed);
        SignalManager.Inst.RemoveListener<ButtonReleasedSignal>(onButtonReleased);
    }

    private void onButtonPressed(Signal signal)
    {
        ButtonPressedSignal buttonPressedSignal = (ButtonPressedSignal)signal;
        if(mainCharacter.SetLandingPosition())
            nextState = new PlayerJumpState(mainCharacter);
        else if(buttonPressedSignal.InputButton == InputButton.RIGHT)
            nextState = new PlayerIdleState(mainCharacter);
    }

    private void onButtonReleased(Signal signal)
    {
        ButtonReleasedSignal buttonReleasedSignal = (ButtonReleasedSignal)signal;
        if(buttonReleasedSignal.InputButton == InputButton.LEFT)
            nextState = new PlayerIdleState(mainCharacter);
    }

    private void onMouseMoved(Signal signal)
    {
        MouseMovedSignal mouseMovedSignal = (MouseMovedSignal)signal;
        mainCharacter.RotateCamera(mouseMovedSignal.DeltaX, mouseMovedSignal.DeltaY);
    }
}

public class PlayerRunRightState : PlayerState
{
    private PlayerState nextState;

    public PlayerRunRightState(MainCharacter mainCharacter) : base (mainCharacter) {}

    public override void Start()
    {
        if(mainCharacter.currentMoveDirection != MoveDirection.RIGHT)
        {
            mainCharacter.currentRotateAxis = mainCharacter.calculateRotationAxis(1, 0);
            mainCharacter.currentMoveDirection = MoveDirection.RIGHT;
        }

        nextState = this;
        mainCharacter.StartRunning();
        SignalManager.Inst.AddListener<MouseMovedSignal>(onMouseMoved);
        SignalManager.Inst.AddListener<ButtonPressedSignal>(onButtonPressed);
        SignalManager.Inst.AddListener<ButtonReleasedSignal>(onButtonReleased);
    }

    public override PlayerState Update()
    {
        Vector3 rockCenter = mainCharacter.transform.parent.position;
        mainCharacter.transform.RotateAround (rockCenter, mainCharacter.currentRotateAxis, mainCharacter.RunSpeed);

        Vector3 playerToRockCenter = rockCenter - mainCharacter.transform.position;
        Vector3 forwardTangent = Vector3.Cross(playerToRockCenter, mainCharacter.currentRotateAxis).normalized;
        mainCharacter.transform.rotation = Quaternion.LookRotation(forwardTangent, -playerToRockCenter);

        Vector2 moveVector = InputManager.Inst.GetMoveVector();

        return nextState;
    }

    public override void Finish()
    {
        mainCharacter.StopRunning();
        SignalManager.Inst.RemoveListener<MouseMovedSignal>(onMouseMoved);
        SignalManager.Inst.RemoveListener<ButtonPressedSignal>(onButtonPressed);
        SignalManager.Inst.RemoveListener<ButtonReleasedSignal>(onButtonReleased);
    }

    private void onButtonPressed(Signal signal)
    {
        ButtonPressedSignal buttonPressedSignal = (ButtonPressedSignal)signal;
        if(mainCharacter.SetLandingPosition())
            nextState = new PlayerJumpState(mainCharacter);
        else if(buttonPressedSignal.InputButton == InputButton.LEFT)
            nextState = new PlayerIdleState(mainCharacter);
    }

    private void onButtonReleased(Signal signal)
    {
        ButtonReleasedSignal buttonReleasedSignal = (ButtonReleasedSignal)signal;
        if(buttonReleasedSignal.InputButton == InputButton.RIGHT)
            nextState = new PlayerIdleState(mainCharacter);
    }

    private void onMouseMoved(Signal signal)
    {
        MouseMovedSignal mouseMovedSignal = (MouseMovedSignal)signal;
        mainCharacter.RotateCamera(mouseMovedSignal.DeltaX, mouseMovedSignal.DeltaY);
    }
}

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState (MainCharacter mainCharacter) : base (mainCharacter) {}

    public override void Start()
    {
        SignalManager.Inst.AddListener<MouseMovedSignal>(onMouseMoved);
        SignalManager.Inst.FireSignal(new JumpStartedSignal());
        mainCharacter.StartJump();
        //mainCharacter.ResetCameraPivot();
    }

    public override PlayerState Update()
    {
        if(mainCharacter.Fly())
            return new PlayerIdleState(mainCharacter);
        else
            return this;
    }

    public override void Finish()
    {
        SignalManager.Inst.RemoveListener<MouseMovedSignal>(onMouseMoved);
        SignalManager.Inst.FireSignal(new PlayerLandedSignal());
        mainCharacter.Land();
    }

    private void onMouseMoved(Signal signal)
    {
        MouseMovedSignal mouseMovedSignal = (MouseMovedSignal)signal;
        mainCharacter.RotateCamera(mouseMovedSignal.DeltaX, mouseMovedSignal.DeltaY);
    }
    
}

public class PlayerFlailState : PlayerState
{
    public PlayerFlailState (MainCharacter mainCharacter) : base (mainCharacter) {}

    private float flailTimer = 5f;

    private Vector3 direction;

    public override void Start()
    {
        direction = Camera.main.transform.forward.normalized;
        mainCharacter.StartJump();
    }

    public override PlayerState Update()
    {
        mainCharacter.transform.position = mainCharacter.transform.position + (direction * mainCharacter.FlySpeed);
        mainCharacter.transform.Rotate(3f, 0f, 2f);
        flailTimer -= Time.deltaTime;
        if(flailTimer <= 0f)
            SignalManager.Inst.FireSignal(new FlailedToDeathSignal());
        return this;
    }
}

public class PlayerHitPlanetState : PlayerState
{
    public PlayerHitPlanetState (MainCharacter mainCharacter) : base (mainCharacter) {} 

    public override void Start()
    {
        mainCharacter.StartJump();
    }

    public override PlayerState Update()
    {
        if(mainCharacter.Fly())
        {
            SignalManager.Inst.FireSignal(new PlayerCollidedWithPlanetSignal());
            mainCharacter.Explode();
        }
        return this;
    }
}