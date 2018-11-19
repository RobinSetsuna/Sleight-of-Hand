using System.Collections;
using UnityEngine;

///	<summary/>
/// Unit
/// abstract class for all walkable unit in map
/// when heading, move to heading, facing heading direction.
/// </summary>
public abstract class Unit : InLevelObject, IDamageReceiver, IStatusEffectReceiver
{
    [SerializeField] private Round actionRound;
    [SerializeField] private float maxSpeed;
    [SerializeField] protected int initialActionPoint;
    [SerializeField] protected int initialHealth = 100;

    public EventOnDataChange<Vector2Int> onGridPositionChange = new EventOnDataChange<Vector2Int>();

    public StatisticSystem.EventOnStatisticChange onStatisticChange;
    public EventOnDataChange3<StatusEffect> onStatusEffectChange = new EventOnDataChange3<StatusEffect>();

    private Vector2Int gridPosition = new Vector2Int(int.MinValue, int.MinValue);
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

    public int InitialActionPoint
    {
        get
        {
            return initialActionPoint;
        }
        set
        {
            initialActionPoint = value;
        }
    }

    public int InitialHealth
    {
        get
        {
            return initialHealth;
        }
    }

    //public int ActionPoint { get; protected set; }

    public StatisticSystem Statistics { get; protected set; }

    public int Ap
    {
        get
        {
            return Mathf.RoundToInt(Statistics[StatisticType.Ap]);
        }
    }

    public int Hp
    {
        get
        {
            return Mathf.RoundToInt(Statistics[StatisticType.Hp]);
        }
    }

    public int AttackRange
    {
        get
        {
            return Mathf.RoundToInt(Statistics[StatisticType.AttackRange]);
        }
    }

    public int DetectionRange
    {
        get
        {
            return Mathf.RoundToInt(Statistics[StatisticType.DetectionRange]);
        }
    }

    public int VisibleRange
    {
        get
        {
            return Mathf.RoundToInt(Statistics[StatisticType.VisibleRange]);
        }
    }

    public float Health { get; protected set; }
    public bool movable = true;

    [Header("Animations")]
    public GameObject modelHolder;
    public float jumpHeight;
    public int jumpsPerMove = 1;

    private CharacterController characterController;
    public float speed;
    //private Vector3 start;
    private Vector3 destination;

    public bool IsAccessibleTo(int x, int y)
    {
        return GridManager.Instance.IsAccessible(this, x, y);
    }

    public bool IsAccessibleTo(Tile tile)
    {
        return GridManager.Instance.IsAccessible(this, tile);
    }

    public void MoveTo(Vector3 destination, System.Action callback)
    {
        //start = transform.position;
        this.destination = destination;

        speed = maxSpeed;

        StartCoroutine(Move(callback));
    }

    public void MoveTo(Tile tile, System.Action callback)
    {
        MoveTo(GridManager.Instance.GetWorldPosition(tile.x, tile.y), callback);
    }

    public void UseCard(Card card, Tile targetTile, System.Action callback)
    {
        CardData cardData = card.Data;

        switch (cardData.Type)
        {
            case "Enhancement":
                string[] values = cardData.Effect.Split(':');
                ActionManager.Singleton.AddFront(new StatusEffectApplication(new StatusEffect(int.Parse(values[0]), int.Parse(values[1])), GridManager.Instance.GetUnit(targetTile)));
                callback.Invoke();
                break;

            case "Strategy":
                ActionManager.Singleton.AddFront(new Casting(ResourceUtility.GetCardEffect(cardData.Effect), targetTile));
                callback.Invoke();
                break;
        }
    }

    protected virtual void Awake()
    {
        //switch (actionRound)
        //{
        //    case Round.Player:
        //        LevelManager.Instance.OnCurrentPhaseChangeForPlayer.AddListener(HandleCurrentPhaseChange);
        //        break;
        //    case Round.Environment:
        //        LevelManager.Instance.OnCurrentPhaseChangeForEnvironment.AddListener(HandleCurrentPhaseChange);
        //        break;
        //}

        //Statistics = new StatisticSystem(new AttributeSet(AttributeType.Ap_i, (float)initialActionPoint,
        //                                                  AttributeType.Hp_i, (float)initialHealth));
        //onAttributeChange = Statistics.onStatisticChange;

        // LevelManager.Instance.onNewTurnUpdateAttribute.AddListener(HandleAttributesChangeOnTurn);
        // CardManager.Instance.OnAttributesChangeOnEffects.AddListener(HandleAttributesChangeOnEffects);
    }

    protected virtual void Start()
    {
        GridPosition = GridManager.Instance.GetTile(transform.position).gridPosition;

        characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        // Push player to ground
        if (characterController != null && !characterController.isGrounded)
            characterController.Move(Vector3.up * (-9.81f * Time.deltaTime));
    }

    public virtual int ApplyDamage(int rawDamage)
    {
        return Statistics.ApplyDamage(rawDamage);
    }

    public virtual bool ApplyStatusEffect(StatusEffect statusEffect)
    {
        Statistics.AddStatusEffect(statusEffect);
        return true;
    }

    public virtual StatusEffect RemoveStatusEffect(int id)
    {
        return Statistics.RemoveStatusEffect(id);
    }

    protected virtual void FinishMovement(System.Action callback)
    {
        //AudioSource audioSource = GetComponent<AudioSource>();
        //audioSource.clip = Resources.Load<AudioClip>("Audio/SFX/jump");
        //audioSource.Play();

        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/SFX/jump"));

        Statistics.ApplyFatigue(1);

        GridPosition = GridManager.Instance.GetTile(transform.position).gridPosition;

        if (callback != null)
            callback.Invoke();
    }

    private IEnumerator Move(System.Action callback)
    {
        float initialDistance = MathUtility.ManhattanDistance(destination.x, destination.z, transform.position.x, transform.position.z);
        Vector3 initialPosition = transform.position;

        float travelDistance = 0;
        while (speed > 0)
        {
            Vector3 position = transform.position;

            //float currentDistance = MathUtility.ManhattanDistance(destination.x, destination.z, position.x, position.z);

            Vector3 orientation = destination - position;
            orientation.y = 0;

            travelDistance = Mathf.Min(initialDistance, travelDistance + speed * Time.deltaTime);
            float travelRatio = travelDistance / initialDistance;
            Vector3 newPosition = Vector3.Lerp(initialPosition, destination, travelRatio);

            transform.forward = orientation.normalized;

            if (characterController != null) {
                Vector3 deltaPos = newPosition - transform.position;
                deltaPos.y = 0;
                characterController.Move(deltaPos);
            } else {
                transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);
            }

            // Hopping
            if (modelHolder != null) {
                float localHeight = jumpHeight * Mathf.Abs(Mathf.Sin(travelRatio * Mathf.PI * jumpsPerMove));
                modelHolder.transform.localPosition = Vector3.up * localHeight;
            }

            // End move
            if (travelRatio >= 1) {
                speed = 0;
                break;
            }

            yield return null;
        }

        FinishMovement(callback);

        yield break;
    }
}
