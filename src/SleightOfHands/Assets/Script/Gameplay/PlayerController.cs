using UnityEngine;
using UnityEngine.Events;

public enum PlayerState : int
{
    Idle = 0,
    MovementPlanning,
    MovementConfirmation,
    Move,
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController Singleton { get; private set; }

    public int mouseDragThreshold = 200;

    private Object path;

    private bool isDragging;
    private long mouseDownTime;

    public class EventOnStateChange : UnityEvent<PlayerState, PlayerState> { }
    public EventOnStateChange onStateChange = new EventOnStateChange();

    public PlayerState CurrentPlayerState { get; private set; }

    private void Awake()
    {
        CurrentPlayerState = 0;

        if (!Singleton)
            Singleton = this;
        else
            Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        mouseDownTime = TimeUtility.localTimeInMilisecond;
    }

    private void OnMouseDrag()
    {
        if (!isDragging)
            isDragging = TimeUtility.localTimeInMilisecond - mouseDownTime > mouseDragThreshold;
        //else if (CurrentPlayerState == PlayerState.MovementPlanning)
            // TODO: Add bypassed tiles
    }

    private void OnMouseUp()
    {
        if (!isDragging) // On click
            switch (CurrentPlayerState)
            {
                case PlayerState.Idle:
                    MakeTransitionTo(PlayerState.MovementPlanning);
                    break;
                case PlayerState.MovementPlanning:
                    MakeTransitionTo(PlayerState.Idle);
                    break;
            }
        else // On releasing a drag
        {
            switch (CurrentPlayerState)
            {
                case PlayerState.MovementPlanning:
                    if (path)
                        MakeTransitionTo(PlayerState.MovementConfirmation);
                    break;
            }

            isDragging = false;
        }
    }

    //private void Update()
    //{
    //    switch (CurrentState)
    //    {
    //        case PlayerState.Idle:
    //            break;
    //    }
    //}

    private void OnDestroy()
    {
        if (this == Singleton)
            Singleton = null;
    }

    private void MakeTransitionTo(PlayerState newState)
    {
#if UNITY_EDITOR
        Debug.LogFormat("[GameController] MakeTransitionTo({0})", newState);
#endif

        if (CurrentPlayerState == newState)
            return;

        // Before leaving the previous state
        //switch (CurrentPlayerState)
        //{
        //}

        PlayerState previousState = CurrentPlayerState;
        CurrentPlayerState = newState;

        // After entering the new state
        switch (CurrentPlayerState)
        {
            case PlayerState.MovementConfirmation:
                // TODO: Show ListMenu
                break;
            case PlayerState.Move:
                // TODO: Make player character move
                break;
        }

        onStateChange.Invoke(previousState, newState);
    }
}
