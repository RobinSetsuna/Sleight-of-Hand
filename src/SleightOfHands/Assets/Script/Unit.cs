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

//    public AudioClip Jump;
//    public AudioClip UseEnhancementCard;
//    public AudioClip UseGnomePotion;


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
            return Mathf.RoundToInt(Statistics[Statistic.Ap]);
        }
    }

    public int Hp
    {
        get
        {
            return Mathf.RoundToInt(Statistics[Statistic.Hp]);
        }
    }

    public int AttackRange
    {
        get
        {
            return Mathf.RoundToInt(Statistics[Statistic.AttackRange]);
        }
    }

    public int DetectionRange
    {
        get
        {
            return Mathf.RoundToInt(Statistics[Statistic.DetectionRange]);
        }
    }

    public int VisibleRange
    {
        get
        {
            return Mathf.RoundToInt(Statistics[Statistic.VisibleRange]);
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

        string[] strings;
        switch (cardData.EffectType)
        {
            case 0: // Casting
                ActionManager.Singleton.AddFront(new Casting(ResourceUtility.GetCardEffect(cardData.Effect), targetTile));
                SoundManager.Instance.Enhancement();
                //gameObject.GetComponent<AudioSource>().PlayOneShot(UseEnhancementCard);
                break;

            case 1: // Statistic modification
                strings = cardData.Effect.Split(':');
                ActionManager.Singleton.AddFront(new StatusEffectApplication(new StatusEffect(int.Parse(strings[0]), int.Parse(strings[1])), GridManager.Instance.GetUnit(targetTile)));
                break;

            case 2: // Card acquirement
                strings = cardData.Effect.Split(';');
                SoundManager.Instance.AttackMiss();
                foreach(string s in strings)
                {
                    string[] values = s.Split(':');
                    int n = int.Parse(values[1]);

                    for (int i = 0; i < n; i++)
                        CardManager.Instance.AddCard(new Card(int.Parse(values[0])));
                }
                break;

            case 3: // Single-target
                var target = GridManager.Instance.GetUnit(targetTile);
                target.ApplyDamage(Statistics.CalculateDamageOutput(int.Parse(cardData.Effect)));
                //attacking
                if (target)
                {
                    StartCoroutine(target.GetComponent<Enemy>().Hurt());
                }
                break;
        }

        callback.Invoke();
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

        onStatusEffectChange.AddListener(HandleStatusEffectChange);
    }

    protected virtual void OnDisable()
    {
        GridPosition = new Vector2Int(int.MinValue, int.MinValue);
    }

    protected virtual void OnDestroy()
    {
        onStatusEffectChange.RemoveListener(HandleStatusEffectChange);
    }

    private void FixedUpdate()
    {
        // Push player to ground
        if (characterController != null && !characterController.isGrounded)
            characterController.Move(Vector3.up * (-9.81f * Time.fixedDeltaTime));
    }

    public virtual int ApplyDamage(int rawDamage)
    {
        
        return Statistics.ApplyDamage(rawDamage);
    }

    public virtual bool ApplyStatusEffect(StatusEffect statusEffect)
    {
        return Statistics.AddStatusEffect(statusEffect);
    }

    public virtual StatusEffect RemoveStatusEffect(int id)
    {
        return Statistics.RemoveStatusEffect(id);
    }
    public void Shaking(float Duration,float Magnitude)
    {
        // may add a camera action queue for camera action
        // shaking the screen for effect, like earthquake, explosion
        StartCoroutine(Shake(Duration,Magnitude));
    }

    protected virtual void FinishMovement(System.Action callback)
    {
        Statistics.ApplyFatigue(1, FatigueType.Movement);

        GridPosition = GridManager.Instance.GetTile(transform.position).gridPosition;

        if (callback != null)
            callback.Invoke();
    }
    private IEnumerator Shake(float Duration,float Magnitude)
    {
        var start_time = Time.fixedUnscaledTime;
        var temp = modelHolder.transform.localPosition;
        while (Time.fixedUnscaledTime - start_time < Duration)
        {
            float x = Random.Range(-1f, 1f) * Magnitude;
            float y = Random.Range(-1f, 1f) * Magnitude;
            // shaking
            modelHolder.transform.localPosition = new Vector3(modelHolder.transform.localPosition.x+x,modelHolder.transform.localPosition.y,modelHolder.transform.localPosition.z+y);
            yield return null;
        }

        modelHolder.transform.localPosition = temp;
        yield return null;
    }

    private void HandleStatusEffectChange(ChangeType change, StatusEffect statusEffect)
    {
        switch (statusEffect.Data.Id)
        {
            case 6: // Diminished
                switch (change)
                {
                    case ChangeType.Incremental:
                        //modelHolder.transform.localScale = 0.2f * Vector3.one;
                        StartCoroutine(ScaleDown());
                        break;

                    case ChangeType.Decremental:
                        //modelHolder.transform.localScale = Vector3.one;
                        StartCoroutine(ScaleUp());
                        break;
                }
                break;
        }
    }

    private IEnumerator ScaleDown()
    {
        //gameObject.GetComponent<AudioSource>().PlayOneShot(UseGnomePotion);
        SoundManager.Instance.GnomePotion();
        while (modelHolder.transform.localScale.x > 0.3f )
        {
            modelHolder.transform.localScale =  new Vector3(modelHolder.transform.localScale.x - 0.2f,modelHolder.transform.localScale.y-0.2f,modelHolder.transform.localScale.z-0.2f);
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    private IEnumerator ScaleUp()
    {
        SoundManager.Instance.GnomePotion();
        //gameObject.GetComponent<AudioSource>().PlayOneShot(UseGnomePotion);
        while (modelHolder.transform.localScale.x < 0.9f)
        {
            modelHolder.transform.localScale =  new Vector3(modelHolder.transform.localScale.x + 0.2f,modelHolder.transform.localScale.y + 0.2f,modelHolder.transform.localScale.z + 0.2f);
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    private IEnumerator Move(System.Action callback)
    {
        SoundManager.Instance.Jump();
        //gameObject.GetComponent<AudioSource>().PlayOneShot(Jump);
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
