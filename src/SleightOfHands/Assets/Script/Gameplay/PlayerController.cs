using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The state of the player character
/// </summary>
public enum PlayerState : int
{
    Uncontrollable = 0,
    Idle = 1,

    MovementPlanning = 10,
    MovementConfirmation = 11,
    Move = 12,

    CardBrowsing = 20,
    CardUsagePlanning = 21,
    CardUsageConfirmation = 22,
    UseCard = 23,
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
                        cardToUse = null;
                        Disable();
                        break;

                    case PlayerState.Idle:

                        CameraManager.Instance.UnboundCameraFollow();
                        switch (previousPlayerState)
                        {
                            case PlayerState.MovementPlanning:
                                Path = null;
                                break;

                            case PlayerState.CardUsagePlanning:
                                CardToUse = null;
                                break;
                        }
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
                            ActionManager.Singleton.AddBack(new Movement(GetComponent<player>(), tile), tile == path.Destination ? (System.Action)ResetToIdle : null);
                        Path = null;
                        // ActionManager.Singleton.Execute(ResetToIdle);
                        break;

                    case PlayerState.CardBrowsing:
                        CardToUse = null;
                        break;

                    case PlayerState.CardUsageConfirmation:
                        Vector3 tileCenterCard = GridManager.Instance.GetWorldPosition(targetTile);
                        tileCenterCard.y += GridManager.Instance.TileSize;
                        if (GridManager.Instance.HasEnemyOn(targetTile))
                            UIManager.Singleton.Open("ListMenu", UIManager.UIMode.DEFAULT, UIManager.Singleton.GetCanvasPosition(Camera.main.WorldToScreenPoint(tileCenterCard)), "USE", (UnityAction)UseCard, "DETECTION", (UnityAction)ToggleDetectionAreaInCardUsageConfirmation, "CANCEL", (UnityAction)ResetCardUsage);
                        else
                            UIManager.Singleton.Open("ListMenu", UIManager.UIMode.DEFAULT, UIManager.Singleton.GetCanvasPosition(Camera.main.WorldToScreenPoint(tileCenterCard)), "USE", (UnityAction)UseCard, "CANCEL", (UnityAction)ResetCardUsage);
                        break;

                    case PlayerState.UseCard:
                        ActionManager.Singleton.AddBack(new CardUsage(Player, cardToUse, targetTile), ResetToIdle);
                        CardManager.Instance.RemoveCard(cardToUse);
                        CardToUse = null;
                        // ActionManager.Singleton.Execute(ResetToIdle);
                        break;
                }

                onCurrentPlayerStateChange.Invoke(previousPlayerState, currentPlayerState);
            }
        }
    }

    public void Back()
    {
        Debug.LogWarning(currentPlayerState);
        Debug.LogWarning((int)currentPlayerState % 10 == 0);
        if ((int)currentPlayerState % 10 == 0)
            ResetToIdle();
        else
            CurrentPlayerState = currentPlayerState - 1;
    }

    private PlayerController() {}

    private void Awake()
    {
        Player = GetComponent<player>();

        CurrentPlayerState = 0;

        LevelManager.Instance.onCurrentPhaseChangeForPlayer.AddListener(HandleCurrentPhaseChange);
    }

    private void OnDestroy()
    {
        Disable();

        LevelManager.Instance.onCurrentPhaseChangeForPlayer.RemoveListener(HandleCurrentPhaseChange);
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
        CurrentPlayerState = PlayerState.UseCard;
    }

    private void ToggleDetectionAreaInCardUsageConfirmation()
    {
        GridManager.Instance.ToggleDetectionArea(GridManager.Instance.GetUnit(targetTile).GetComponent<EnemyController>().UID);
        ResetCardUsage();
    }

    /// <summary>
    /// An event listener for MouseInputManager.Singleton.onMouseClick
    /// </summary>
    /// <param name="obj"> The clicked object </param>
    private void HandleMouseClick(MouseInteractable obj)
    {
        // handle click sound
        //AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        switch (currentPlayerState)
        {
            case PlayerState.Idle:
                if (obj == this || (obj.GetComponent<Tile>() == GridManager.Instance.GetTile(Player.transform.position)))
                {
                    CurrentPlayerState = PlayerState.MovementPlanning;
                    SoundManager.Instance.TapTile();
                    //audioSource.PlayOneShot(TapTile);
                }
                else if (obj.GetComponent<Enemy>())
                {
                    GridManager.Instance.ToggleDetectionArea(obj.GetComponent<EnemyController>().UID);
                    //audioSource.PlayOneShot(TapTile);
                    SoundManager.Instance.TapTile();
                }
                else if (obj.GetComponent<UICard>())
                {
                    //audioSource.PlayOneShot(TapCard);
                    SoundManager.Instance.TapTile();
                    CurrentPlayerState = PlayerState.CardBrowsing;
                }
                break;

            case PlayerState.MovementPlanning:
                SoundManager.Instance.TapTile();
                //audioSource.PlayOneShot(TapTile);
                if (obj == this)
                    CurrentPlayerState = PlayerState.Idle;
                else if (obj.GetComponent<Enemy>())
                    GridManager.Instance.ToggleDetectionArea(obj.GetComponent<EnemyController>().UID);
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
                            Path = Navigation.FindPath(GridManager.Instance, playerTile, tile, Player.IsAccessibleTo);
                            CurrentPlayerState = PlayerState.MovementConfirmation;
                        }
                    }
                }
                break;

            case PlayerState.CardBrowsing:
                if (obj.GetComponent<UICard>())
                {
                    CardToUse = obj.GetComponent<UICard>().Card;
                    audioSource.PlayOneShot(TapCard);
                    CurrentPlayerState = PlayerState.CardUsagePlanning;
                }
                break;

            case PlayerState.CardUsagePlanning:
                if (obj == this)
                {
                    targetTile = GridManager.Instance.GetTile(transform.position);

                    if (targetTile.IsHighlighted(Tile.HighlightColor.Green))
                        CurrentPlayerState = PlayerState.CardUsageConfirmation;
                    else
                        targetTile = null;
                }
                else if (obj.GetComponent<Enemy>())
                {
                    targetTile = GridManager.Instance.GetTile(obj.transform.position);

                    if (targetTile.IsHighlighted(Tile.HighlightColor.Green))
                        CurrentPlayerState = PlayerState.CardUsageConfirmation;
                    else
                    {
                        GridManager.Instance.ToggleDetectionArea(obj.GetComponent<EnemyController>().UID);
                        targetTile = null;
                    }
                }
                else if (obj.GetComponent<Tile>())
                {
                    targetTile = obj.GetComponent<Tile>();

                    if (targetTile.IsHighlighted(Tile.HighlightColor.Green))
                        CurrentPlayerState = PlayerState.CardUsageConfirmation;
                    else
                        targetTile = null;
                }
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
