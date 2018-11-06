﻿using System.Collections;
using UnityEngine;

///	<summary/>
/// Unit
/// abstract class for all walkable unit in map
/// when heading, move to heading, facing heading direction.
/// </summary>
public abstract class Unit : InLevelObject
{
    [SerializeField] private Round actionRound;
    [SerializeField] private float maxSpeed;

    public EventOnDataChange<Vector2Int> onGridPositionChange = new EventOnDataChange<Vector2Int>();

    private Vector2Int gridPosition;
    public Vector2Int GridPosition
    {
        get
        {
            return gridPosition;
        }

        private set
        {
            Vector2Int previousGridPosition = gridPosition;

            gridPosition = value;
            onGridPositionChange.Invoke(previousGridPosition, value);

            GridManager.Instance.NotifyUnitPositionChange(this, previousGridPosition, gridPosition);
        }
    }

    public float MaxSpeed
    {
        get
        {
            return maxSpeed;
        }
    }

    [SerializeField] private int initialActionPoint;
    [SerializeField] private  int initialHealth = 100;
    public int InitialActionPoint
    {
        get
        {
            return initialActionPoint;
        }
    }

    public int InitialHealth
    {
        get
        {
            return initialHealth;
        }
    }

    public int ActionPoint { get; protected set; }
    public float Health { get; protected set; }

    [Header("Animations")]
    public GameObject modelHolder;
    public float jumpHeight;
    public int jumpsPerMove = 1;

    private CharacterController characterController;
    private float speed;
    private Vector3 start;
    private Vector3 destination;
    

    public void MoveTo(Vector3 destination, System.Action callback)
    {
        start = transform.position;
        this.destination = destination;

        speed = maxSpeed;

        StartCoroutine(Move(callback));
    }

    public void MoveTo(Tile tile, System.Action callback)
    {
        MoveTo(GridManager.Instance.GetWorldPosition(tile.x, tile.y), callback);
    }

    protected virtual void Awake()
    {
        /*switch (actionRound)
        {
            case Round.Player:
                LevelManager.Instance.OnCurrentPhaseChangeForPlayer.AddListener(HandleCurrentPhaseChange);
                break;
            case Round.Environment:
                LevelManager.Instance.OnCurrentPhaseChangeForEnvironment.AddListener(HandleCurrentPhaseChange);
                break;
        }*/

        LevelManager.Instance.onNewTurnUpdateAttribute.AddListener(HandleAttributesChangeOnTurn);
        CardManager.Instance.OnAttributesChangeOnEffects.AddListener(HandleAttributesChangeOnEffects);
        gridPosition = GridManager.Instance.GetTile(transform.position).gridPosition;
        GridManager.Instance.NotifyUnitPositionChange(this, new Vector2Int(-1, -1), gridPosition);
    }

    private void Start() {
        characterController = GetComponent<CharacterController>();
    }

    private void Update() {

        // Push player to ground
        if (characterController != null && !characterController.isGrounded) {
            characterController.Move(Vector3.up * (-9.81f * Time.deltaTime));
        }

    }

    /*private void HandleCurrentPhaseChange(Phase currentPhase)
    {
        switch (currentPhase)
        {
            case Phase.Start:
                ResetActionPoint();
                break;
        }
    }*/

    //when add a new effect to a unit, this will be called to immediately calculate the newest attributes.
    private void HandleAttributesChangeOnEffects(Effects effects)
    {
        //Debug.Log(effects.CurrentAP_c());
        //Debug.Log(effects.CurrentAP_f());
        ActionPoint = (ActionPoint + effects.CurrentAP_c()) * ( 1 + effects.CurrentAP_f());
        Health = (Health + effects.CurrentHP_c()) * (1 + effects.CurrentHP_f());
    }

    //When enter a new turn, this will be called to calculate the newest attributes
    private void HandleAttributesChangeOnTurn(Effects effects)
    {
        ActionPoint = (initialActionPoint + (int)effects.GetAP_c()) * (1 + (int)effects.GetAP_f());
        Health = (initialHealth + effects.GetHP_c()) * (1 + effects.GetHP_f());
    }

    //private void ResetActionPoint()
    //{
    //    ActionPoint = initialActionPoint;
    //}

    private IEnumerator Move(System.Action callback)
    {

        float initialDistance = MathUtility.ManhattanDistance(destination.x, destination.z, transform.position.x, transform.position.z);
        Vector3 initialPosition = this.transform.position;

        float travelDistance = 0;
        while (speed > 0)
        {
            Vector3 position = transform.position;

            // End move
            float currentDistance = MathUtility.ManhattanDistance(destination.x, destination.z, position.x, position.z);

            Vector3 orientation = destination - position;
            orientation.y = 0;

            travelDistance = Mathf.Min(initialDistance, travelDistance + speed * Time.deltaTime);
            float travelRatio = travelDistance / initialDistance;
            Vector3 newPosition = Vector3.Lerp(initialPosition, destination, travelRatio);

            transform.forward = orientation.normalized;

            if (characterController != null) {
                Vector3 deltaPos = newPosition - this.transform.position;
                deltaPos.y = 0;
                characterController.Move(deltaPos);
            } else {
                this.transform.position = new Vector3(newPosition.x, this.transform.position.y, newPosition.z);
            }

            // Hopping
            if (modelHolder != null) {
                float localHeight = jumpHeight * Mathf.Abs(Mathf.Sin(travelRatio * Mathf.PI * jumpsPerMove));
                modelHolder.transform.localPosition = Vector3.up * localHeight;
            }

            if (travelRatio >= 1) {
                speed = 0;
                break;
            }

            yield return null;
        }

        transform.position = new Vector3(destination.x, transform.position.y, destination.z);

        GridPosition = GridManager.Instance.GetTile(transform.position).gridPosition;

        ActionPoint--;

        if (callback != null)
            callback.Invoke();

        yield return null;
    }

    //protected Transform heading;

    //// Update is called once per frame
    //protected void FixedUpdate(){
    //	if ( heading!= null && heading.position != Vector3.zero)
    //	{
    //		// heading detected
    //		Vector3 desiredPosition = new Vector3(heading.position.x, transform.position.y, heading.position.z);
    //		Facing(desiredPosition);
    //		transform.position = Vector3.MoveTowards(transform.position, desiredPosition, movementSpeed * Time.deltaTime);
    //		if ((transform.position - heading.position).sqrMagnitude < 1.05)
    //		{
    //			// arrived heading tile
    //			heading = null;
    //		}
    //	}
    //}

    //   // change direction of unit facing, sync with heading
    //void Facing(Vector3 pos)
    //{
    //	transform.LookAt(pos);
    //}

    //public void setHeading(Tile tile)
    //{
    //	//heading is the next tile unit will move to.
    //	heading = tile.transform;
    //}
}
