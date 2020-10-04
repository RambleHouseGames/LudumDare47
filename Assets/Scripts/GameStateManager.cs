using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Inst;

    private GameState currentState = new IntroGameState();

    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        currentState.Start();
    }

    void Update()
    {
        GameState nextState = currentState.Update();
        if(nextState != currentState)
        {
            currentState.Finish();
            currentState = nextState;
            nextState.Start();
        }
    }
}

public abstract class GameState
{
    public virtual void Start() {}
    public abstract GameState Update();
    public virtual void Finish() {}
}

public class IntroGameState : GameState
{
    private GameState nextState;

    public override void Start()
    {
        nextState = this;
        SignalManager.Inst.AddListener<TutorialCompleteSignal>(onTutorialComplete);
    }

    public override GameState Update()
    {
        return nextState;
    }

    public override void Finish()
    {
        SignalManager.Inst.RemoveListener<TutorialCompleteSignal>(onTutorialComplete);
    }

    private void onTutorialComplete(Signal signal)
    {
        nextState = new PlayGameState();
    }
}

public class PlayGameState : GameState
{
    public override void Start()
    {
        SignalManager.Inst.FireSignal(new GoSignal());
    }

    public override GameState Update()
    {
        return this;
    }
}