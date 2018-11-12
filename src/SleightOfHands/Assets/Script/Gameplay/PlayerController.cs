﻿using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The state of the player character
/// </summary>
public enum PlayerState : int
{
    Uncontrollable,
    Idle,

    MovementPlanning,
    MovementConfirmation,
    Move,

    CardBrowsing,
    CardUsagePlanning,
    CardUsageConfirmation,
    UseCard,
}

/// <summary>
/// A FSM for the player character recieving user inputs to control the player character
/// </summary>
public class PlayerController : MouseInteractable
{
    /// <summary>
    /// An event triggered whenever the state of the player character changes
    /// </summary>
    public EventOnDataChange<PlayerState> onCurrentPlayerStateChange = new EventOnDataChange<PlayerState>();

    /// <summary>
    /// An event triggered whenever the planned path is changed by the player
    /// </summary>
    public EventOnDataUpdate<Path<Tile>> onPathUpdate = new EventOnDataUpdate<Path<Tile>>();

    public EventOnDataUpdate<Card> onCardToUseUpdate = new EventOnDataUpdate<Card>();
    
    /// <summary>
    /// The player controlled by this controller
    /// </summary>
    public player Player { get; private set; }

    private bool isEnabled = false;

    private Path<Tile> path;
    private Path<Tile> Path
    {
        set
        {
            if (value != path)
            {
                path = value;
                onPathUpdate.Invoke(path);
            }
        }
    }

    private Card cardToUse;
    private Card CardToUse
    {
        set
        {
            if (value != cardToUse)
            {
                cardToUse = value;
                onCardToUseUpdate.Invoke(cardToUse);
            }
        }
    }

    private Tile targetTile;

    private PlayerState currentPlayerState;

    /// <summary>
    /// The current state of the player character
    /// </summary>
    public PlayerState CurrentPlayerState
    {
        get
        {
            return currentPlayerState;
        }

        private set
        {
#if UNITY_EDITOR
            LogUtility.PrintLogFormat("PlayerController", "Made a transition to {0}.", value);
#endif

            // Reset current state
            if (value == currentPlayerState)
            {
                switch (currentPlayerState)
                {
                    case PlayerState.MovementPlanning:
                        path.Clear();
                        onPathUpdate.Invoke(path);
                        break;
                }
            }
            else
            {
                // Before leaving the previous state
                switch (currentPlayerState)
                {
                    case PlayerState.Uncontrollable:
                        Enable();
                        break;
                }

                PlayerState previousPlayerState = CurrentPlayerState;
                currentPlayerState = value;

                // After entering the new state
                switch (currentPlayerState)
                {
                    case PlayerState.Uncontrollable:
                        Path = null;
                        Disable();
                        break;

                    case PlayerState.Idle:
                        CameraManager.Instance.UnboundCameraFollow();
                        if (previousPlayerState == PlayerState.MovementPlanning)
                            Path = null;
                        break;

                    case PlayerState.MovementPlanning:
                        CameraManager.Instance.FocusAt(transform.position);
                        CameraManager.Instance.BoundCameraFollow(transform);
                        Path = new Path<Tile>(GridManager.Instance.GetTile(Player.transform.position));
                        break;

                    case PlayerState.MovementConfirmation:
                        Vector3 tileCenter = GridManager.Instance.GetWorldPosition(path.Destination);
                        tileCenter.y += GridManager.Instance.TileSize;
                        UIManager.Singleton.Open("ListMenu", UIManager.UIMode.DEFAULT, UIManager.Singleton.GetCanvasPosition(Camera.main.WorldToScreenPoint(tileCenter)), "MOVE", (UnityAction)InitiateMovement, "CANCEL", (UnityAction)ResetMovement);
                        break;

                    case PlayerState.Move:
                        for (Tile tile = path.Reset(); !path.IsFinished(); tile = path.MoveForward())
                            ActionManager.Singleton.AddBack(new Movement(GetComponent<player>(), tile));
                        Path = null;
                        ActionManager.Singleton.Execute(ResetToIdle);
                        break;

                    case PlayerState.CardUsageConfirmation:
                        Vector3 tileCenterCard = GridManager.Instance.GetWorldPosition(targetTile);
                        tileCenterCard.y += GridManager.Instance.TileSize;
                        UIManager.Singleton.Open("ListMenu", UIManager.UIMode.DEFAULT, UIManager.Singleton.GetCanvasPosition(Camera.main.WorldToScreenPoint(tileCenterCard)), "USE", (UnityAction)UseCard, "CANCEL", (UnityAction)ResetCardUsage);
                        break;
                }

                onCurrentPlayerStateChange.Invoke(previousPlayerState, currentPlayerState);
            }
        }
    }

    private PlayerController() {}

    private void Awake()
    {
        Player = GetComponent<player>();

        CurrentPlayerState = 0;

        LevelManager.Instance.OnCurrentPhaseChangeForPlayer.AddListener(HandleCurrentPhaseChange);
    }

    private void OnDestroy()
    {
        Disable();

        LevelManager.Instance.OnCurrentPhaseChangeForPlayer.RemoveListener(HandleCurrentPhaseChange);
    }

    /// <summary>
    /// Start to recieve user inputs by adding listeners to events in MouseInputManager
    /// </summary>
    private void Enable()
    {
        if (!isEnabled)
        {
            MouseInputManager.Singleton.onMouseClick.AddListener(HandleMouseClick);
            MouseInputManager.Singleton.onMouseDragEnd.AddListener(HandleMouseDragEnd);
            MouseInputManager.Singleton.onMouseEnter.AddListener(HandleMouseTargetChange);

            isEnabled = true;
        }
    }

    /// <summary>
    /// End recieving user inputs by removing added listeners to events in MouseInputManager
    /// </summary>
    private void Disable()
    {
        if (isEnabled)
        {
            MouseInputManager.Singleton.onMouseClick.RemoveListener(HandleMouseClick);
            MouseInputManager.Singleton.onMouseDragEnd.RemoveListener(HandleMouseDragEnd);
            MouseInputManager.Singleton.onMouseEnter.RemoveListener(HandleMouseTargetChange);

            isEnabled = false;
        }
    }

    /// <summary>
    /// Add a way point to Path
    /// </summary>
    /// <param name="tile"></param>
    private void AddWayPoint(Tile tile)
    {
        path.AddLast(tile);

        onPathUpdate.Invoke(path);
    }

    /// <summary>
    /// Remove the last way point added to Path
    /// </summary>
    private void RemoveWayPoint()
    {
        path.RemoveLast();

        onPathUpdate.Invoke(path);
    }

    /// <summary>
    /// Make a transition to PlayerState.Move
    /// </summary>
    private void ResetToIdle()
    {
        CurrentPlayerState = PlayerState.Idle;
    }

    /// <summary>
    /// Make a transition to PlayerState.MovementPlanning
    /// </summary>
    private void ResetMovement()
    {
        CurrentPlayerState = PlayerState.MovementPlanning;
    }

    /// <summary>
    /// Make a transition to PlayerState.Move
    /// </summary>
    private void InitiateMovement()
    {
        CurrentPlayerState = PlayerState.Move;
    }

    private void ResetCardUsage()
    {
        CurrentPlayerState = PlayerState.CardUsagePlanning;
    }

    private void UseCard()
    {
        ActionManager.Singleton.AddBack(new CardUsage(Player, cardToUse, targetTile));
        ActionManager.Singleton.Execute(ResetToIdle);

        CardToUse = null;
    }

    /// <summary>
    /// An event listener for MouseInputManager.Singleton.onMouseClick
    /// </summary>
    /// <param name="obj"> The clicked object </param>
    private void HandleMouseClick(MouseInteractable obj)
    {
        //handle click sound
        AudioSource _audioSource = this.gameObject.GetComponent<AudioSource>();
        AudioClip audioClip = Resources.Load<AudioClip>("Audio/SFX/tapTile");

        _audioSource.clip = audioClip;
        _audioSource.Play();

        switch (currentPlayerState)
        {
            case PlayerState.Idle:
                if (obj == this || (obj.GetComponent<Tile>() == GridManager.Instance.GetTile(Player.transform.position)))
                    CurrentPlayerState = PlayerState.MovementPlanning;
                else if (obj.GetComponent<Enemy>())
                    obj.GetComponent<Enemy>().HighlightDetection();
                else if (obj.GetComponent<UICard>())
                {
                    CurrentPlayerState = PlayerState.CardUsagePlanning;
                    CardToUse = obj.GetComponent<UICard>().Card;
                }
                break;

            case PlayerState.MovementPlanning:
                if (obj == this)
                    CurrentPlayerState = PlayerState.Idle;
                else if (obj.GetComponent<Tile>())
                {
                    Tile tile = obj.GetComponent<Tile>();

                    if (tile == path.Start)
                        CurrentPlayerState = PlayerState.Idle;
                    else
                    {
                        Tile playerTile = GridManager.Instance.GetTile(Player.transform.position);

                        if (tile.IsHighlighted(Tile.HighlightColor.Blue))
                        {
                            Path = Navigation.FindPath(GridManager.Instance, playerTile, tile, GridManager.Instance.IsAccessible);
                            CurrentPlayerState = PlayerState.MovementConfirmation;
                        }
                    }
                }
                break;

            case PlayerState.CardUsagePlanning:
                if (obj == this)
                {
                    Tile playerTile = GridManager.Instance.GetTile(Player.transform.position);

                    if (playerTile.IsHighlighted(Tile.HighlightColor.Green))
                    {
                        targetTile = playerTile;
                        CurrentPlayerState = PlayerState.CardUsageConfirmation;
                    }
                }
                else if (obj.GetComponent<Tile>())
                {
                    Tile tile = obj.GetComponent<Tile>();

                    if (tile.IsHighlighted(Tile.HighlightColor.Green))
                    {
                        targetTile = tile;
                        CurrentPlayerState = PlayerState.CardUsageConfirmation;
                    }
                }
                else if (obj.GetComponent<UICard>() && obj.GetComponent<UICard>().Card == cardToUse)
                    CurrentPlayerState = PlayerState.Idle;
                break;
        }
    }

    /// <summary>
    /// An event listener for MouseInputManager.Singleton.onMouseDragEnd
    /// </summary>
    /// <param name="obj"> The object from which the player starts to drag </param>
    private void HandleMouseDragEnd(MouseInteractable obj)
    {
        switch (currentPlayerState)
        {
            case PlayerState.MovementPlanning:
                if (path.Count > 0 && (obj == this || obj.GetComponent<Tile>() == path.Start))
                    CurrentPlayerState = PlayerState.MovementConfirmation;
                break;
        }
    }

    /// <summary>
    /// An event listener for MouseInputManager.Singleton.onMouseEnter
    /// </summary>
    /// <param name="obj"> The object at which the mouse is pointing at </param>
    private void HandleMouseTargetChange(MouseInteractable obj)
    {
        if (MouseInputManager.Singleton.IsMouseDragging)
            switch (currentPlayerState)
            {
                case PlayerState.MovementPlanning:
                    if (obj == this)
                    {
                        if (path.Count == 1)
                            RemoveWayPoint();
                    }
                    else if (obj.GetComponent<Tile>())
                    {
                        Tile tile = obj.GetComponent<Tile>();

                        if (path.Count > 0)
                        {
                            if (tile == path.Last.Previous.Value)
                                RemoveWayPoint();
                            else if (path.Count < Player.Ap && tile.IsHighlighted(Tile.HighlightColor.Blue) && GridManager.Instance.IsAdjacent(tile, path.Last.Value))
                                AddWayPoint(tile);
                        }
                        else if (path.Count < Player.Ap && tile.IsHighlighted(Tile.HighlightColor.Blue) && GridManager.Instance.IsAdjacent(tile, path.Start))
                            AddWayPoint(tile);
                    }
                    break;
            }
    }

    /// <summary>
    /// An event listener for LevelManager.Instance.onCurrentPhaseChangeForPlayer
    /// </summary>
    /// <param name="currentPhase"> The phase which the LevelManager just entered </param>
    private void HandleCurrentPhaseChange(Phase currentPhase)
    {
        switch (currentPhase)
        {
            case Phase.Action:
                CurrentPlayerState = PlayerState.Idle;
                CameraManager.Instance.FocusAt(transform.position);
                break;

            case Phase.End:
                CurrentPlayerState = PlayerState.Uncontrollable;
                break;
        }
    }
}
