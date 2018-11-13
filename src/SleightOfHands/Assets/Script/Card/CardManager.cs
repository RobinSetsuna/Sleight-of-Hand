using System.Collections;
using System.Collections.Generic;
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
    public List<Card> hand = new List<Card>();
    private CardData InEffect;
    public string deckFolderPath;
    public string deckFilename;

    public GameObject Player { get; private set; }

    //gameobject
    public GameObject smokeObject;
    public GameObject GlueObject;

    int rdNumLast = -1;

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown("1"))
    //    {
    //        Player = GameObject.FindGameObjectWithTag("Player");
    //        UseEnhancenmentCard(hand[0], Player);
    //    }
    //    else if(Input.GetKeyDown("2"))
    //    {
    //        UseStrategyCard(0);
    //    }
    //    else if(Input.GetKeyDown("3"))
    //    {
    //        UseStrategyCard(2);
    //    }
    //}

    // randomly take one card from the deck to the player.
    public void RandomGetCard(int n = 1)
    {
        //generate a random num from the number of the cards

        //ensure random number is different
        //int rdNum;
        //do
        //{
        //    rdNum = Random.Range(0, deck.CountDeck());
        //} while (rdNum == rdNumLast);
        //rdNumLast = rdNum;

        //Card card = deck.GetCardAt(Random.Range(0, deck.CountDeck()));

        ////add deleted card into hand
        //AddCard(card);

        ////delete the selected card from deck
        //deck.Remove(card);

        foreach (Card card in deck.GetRandom(n))
            AddCard(card);
    }

    // take one specific card from the deck to the player.
    //public void GetCard(string _cardName)
    //{
    //    Card card = deck.FindCard(_cardName);
    //    if (card != null)
    //    {
    //        deck.Remove(card);
    //        AddCard(card);
    //    }
    //}

    private void AddCard(Card card)
    {
        hand.Add(card);
        onHandChange.Invoke(ChangeType.Incremental, card);
    }

    public void RemoveCard(Card card)
    {
        hand.Remove(card);
        onHandChange.Invoke(ChangeType.Decremental, card);
    }

    //public void AddEffect(Card card, GameObject obj)
    //{
    //    Effects effects = obj.GetComponent<Effects>();
    //    LogUtility.PrintLogFormat("CardManager", "Add Card Effect {0}.", card.intro);
    //    effects.AddEffect(card);
    //    HandToUsed(card);
    //    OnAttributesChangeOnEffects.Invoke(effects);
    //}

    //public void UseEnhancenmentCard(Card card, GameObject obj)
    //{
    //    Effects effects = obj.GetComponent<Effects>();
    //    LogUtility.PrintLogFormat("CardManager", "Add Card Effect {0}.", card.intro);
    //    effects.AddEffect(card);
    //    HandToUsed(card);
    //    OnAttributesChangeOnEffects.Invoke(effects);
    //}

    public void InitCardDeck()
    {
        //string jsonPath = Path.Combine(Application.streamingAssetsPath, deckFolderPath);
        //jsonPath = Path.Combine(jsonPath, deckFilename + ".json");

        //string json = File.ReadAllText(jsonPath);

        //deck = JsonUtility.FromJson<CardDeck>(json);

        //if (deck.cards == null)
        //    Debug.Log("null");

        deck = new CardDeck(new int[3] { 0, 1, 2 });

        //foreach (Card card in deck)
        //    LogUtility.PrintLogFormat("CardManager", card.Data.ToString());
    }

    private Tile GetPlayerTile()
    {
        return GridManager.Instance.GetTile(GameObject.FindGameObjectWithTag("Player").transform.position);
    }

    private void HandToUsed(Card card)
    {
        usedCards.Add(card);
        hand.Remove(card);

        onHandChange.Invoke(ChangeType.Decremental, card);
    }


    void UseStrategyCard(int cardID)
    {
        var card = TableDataManager.Singleton.GetCardData(cardID);
        InEffect = card;
        //1. hightlight all the possible tile              
        GridManager.Instance.Highlight(GetPlayerTile(), card.Range, Tile.HighlightColor.Green);
        
        //3. create a smoke on the tile when mouse click
        MouseInputManager.Singleton.onMouseClick.AddListener(HandleClick);
    }

    void HandleClick(MouseInteractable obj)
    {
        
        if (obj.GetComponent<Tile>().IsHighlighted(Tile.HighlightColor.Green))
        {
            var pos = obj.GetComponent<Tile>().transform.position;
            switch (InEffect.Name)
            {
                case "Smoke":
                    var smoke = Instantiate(smokeObject, pos, Quaternion.identity);
                    smoke.AddComponent<Smoke>();
                    break;
                case "Glue":
                    var Glue = Instantiate(GlueObject, pos, Quaternion.identity);
                    Glue.AddComponent<Glue>();
                    break;
            }

            MouseInputManager.Singleton.onMouseClick.RemoveListener(HandleClick);
            GridManager.Instance.DehighlightAll();
        }
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
}

//[System.Serializable]
//public class Card
//{
//    public string cardName;
//    public string intro;
//    public string type;
//    public int ID;
//    public int HP_c;
//    public int AP_c;
//    public int ATK_c;
//    public int duration;
//    public int HP_f;
//    public int AP_f;
//    public int ATK_f;
//}

public class Card
{
    public CardData Data { get; private set; }

    public Card(int id)
    {
        Data = TableDataManager.Singleton.GetCardData(id);
    }
}

[System.Serializable]
public class CardDeck : IEnumerable<Card>
{
    private List<Card> cards;

    public CardDeck()
    {
        cards = new List<Card>();
    }

    public CardDeck(int[] ids) : this()
    {
        for (int i = 0; i < ids.Length; i++)
            cards.Add(new Card(ids[i]));
    }

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

    public List<Card> GetRandom(int n = 1)
    {
        HashSet<int> indices = new HashSet<int>();

        int N = cards.Count;
        while (indices.Count < n)
        {
            int i;

            do
            {
                i = Random.Range(0, N);
            } while (indices.Contains(i));

            indices.Add(i);
        }

        List<Card> list = new List<Card>();

        foreach (int i in indices)
            list.Add(cards[i]);

        return list;
    }

    public IEnumerator<Card> GetEnumerator()
    {
        return cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return cards.GetEnumerator();
    }

    //public Card FindCard(string _cardName)
    //{
    //    foreach (Card card in cards)
    //    {
    //        if (card.cardName == _cardName)
    //        {
    //            return card;
    //        }
    //    }

    //    return null;
    //}
}