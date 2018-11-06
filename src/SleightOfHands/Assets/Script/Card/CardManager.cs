using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class CardManager : MonoBehaviour
{

    private static CardManager instance;
    public static CardManager Instance
    {
        get
        {
            if (instance == null) instance = GameObject.Find("CardManager").GetComponent<CardManager>();
            return instance;
        }
    }

    public EventOnDataChange3<Card> onHandChange = new EventOnDataChange3<Card>();
    public EventOnDataUpdate<Attribute> onAttributeTimeOut = new EventOnDataUpdate<Attribute>();
    public EventOnDataUpdate<Effects> OnAttributesChangeOnEffects = new EventOnDataUpdate<Effects>();

    public CardDeck deck;
    public List<Card> usedCards;
    public List<Card> hand;
    public string deckFolderPath;
    public string deckFilename;
    public GameObject Player { get; private set; }

    //gameobject
    public GameObject smokeObject;

    int rdNumLast = -1;

    // Use this for initialization
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("1"))
        {            
            Player = GameObject.FindGameObjectWithTag("Player");
            AddEffect(hand[0], Player);
        }
    }

    // randomly take one card from the deck to the player.
    public void RandomGetCard()
    {
        //generate a random num from the number of the cards

        //ensure random number is different
        int rdNum;
        do
        {
            rdNum = Random.Range(0, deck.CountDeck());
        } while (rdNum == rdNumLast);
        rdNumLast = rdNum;

        Card card = deck.GetCardAt(rdNum);

        //add deleted card into hand
        AddCard(card);

        //delete the selected card from deck
        deck.Remove(card);

    }

    // take one specific card from the deck to the player.
    public void GetCard(string _cardName)
    {
        Card card = deck.FindCard(_cardName);
        if (card != null)
        {
            deck.Remove(card);
            AddCard(card);
        }
    }

    private void AddCard(Card card)
    {
        hand.Add(card);
        onHandChange.Invoke(ChangeType.Incremental, card);
    }
    

    public void AddEffect(Card card, GameObject obj)
    {
        Effects effects = obj.GetComponent<Effects>();
        LogUtility.PrintLogFormat("CardManager", "Add Card Effect {0}.", card.intro);
        effects.AddEffect(card);
        HandToUsed(card);
        OnAttributesChangeOnEffects.Invoke(effects);
    }

    public void InitCardDeck()
    {
        string jsonPath = Path.Combine(Application.streamingAssetsPath, deckFolderPath);
        jsonPath = Path.Combine(jsonPath, deckFilename + ".json");

        string json = File.ReadAllText(jsonPath);

        deck = JsonUtility.FromJson<CardDeck>(json);

        if (deck.cards == null)
            Debug.Log("null");

        for (int i = 0; i < deck.cards.Count; i++)
        {
            LogUtility.PrintLogFormat("CardManager", "This Card is {0} with Effects: {1}.", deck.cards[i].cardName, deck.cards[i].intro);
        }

       
    }

    private Tile GetPlayerTile()
    {
        Tile playerTile = GridManager.Instance.GetTile(GameObject.FindGameObjectWithTag("Player").transform.position);
        Debug.Log(playerTile.x);
        Debug.Log(playerTile.y);
        return playerTile;
    }

    private void HandToUsed(Card card)
    {
        usedCards.Add(card);
        hand.Remove(card);

        onHandChange.Invoke(ChangeType.Decremental, card);
    }


    void Smoke(Card card)
    {
        //1. hightlight all the possible tile              
        //GridManager.Instance.Highlight(GetPlayerTile(), 3, Tile.HighlightColor.Green);
        //2. get player input tile position

        //3. create a smoke on the tile(make sure when the smoke will disappear)

        //4. change the visibility of enemy

        //5. minus the action point
        //GameObject.FindGameObjectWithTag("Player").GetComponent<player>().DeleteActionPoint(card.cost);
        //print("out Smoke");
    }

    public void CreateSmokeOnTile(Tile obj)
    {
        print("enter csot");
        Instantiate(smokeObject, obj.transform);
        //smokeObject.AddComponent<EffectTimer>();
    }

    void ChestKey()
    {
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Player");
        //1. find out which chest the player is close to
        for (int i = 0; i < chests.Length; i++)
        {
            Vector3 chestPos = new Vector3(chests[i].transform.position.x, chests[i].transform.position.y, chests[i].transform.position.z);
            Tile chestTile = GridManager.Instance.GetTile(chestPos);
            if (GridManager.Instance.IsAdjacent(chestTile, GetPlayerTile()))
            {
                //2. open the chest
                chests[i].GetComponent<Chest>().isOpen = true;
            }
        }
    }

    void Haste(Card card)
    {
        //1. add action point
        //string effect = card.effect;
        //string result = System.Text.RegularExpressions.Regex.Replace(effect, @"[^0-9]+", "");
        //int point = int.Parse(result);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<player>().AddActionPoint(point);
        //2. move this card to usedCards
        //usedCards.Add(card);
        //hand.Remove(card);
    }
}

[System.Serializable]
public class Card
{
    public string cardName;
    public string intro;
    public string type;
    public int ID;
    public int HP_c;
    public int AP_c;
    public int ATK_c;
    public int duration;
    public int HP_f;
    public int AP_f;
    public int ATK_f;
}

[System.Serializable]
public class CardDeck
{
    public List<Card> cards;

    public Card GetCardAt(int index)
    {
        return cards[index];
    }

    public int CountDeck()
    {
        return cards.Count;
    }

    public void RemoveAt(int index)
    {
        cards.RemoveAt(index);
    }

    public void Remove(Card card)
    {
        cards.Remove(card);
    }

    public Card FindCard(string _cardName)
    {
        foreach (Card card in cards)
        {
            if (card.cardName == _cardName)
            {
                return card;
            }
        }

        return null;
    }
}