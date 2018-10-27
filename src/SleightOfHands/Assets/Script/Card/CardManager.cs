using System.Collections;
using System.Collections.Generic;

using System.IO;
using UnityEngine;



public class CardManager : MonoBehaviour {

    private static CardManager instance;
    public static CardManager Instance
    {
        get
        {
            if (instance == null) instance = GameObject.Find("CardManager").GetComponent<CardManager>();
            return instance;
        }
    }

    public CardDeck deck;
    public List<Card> usedCards;
    public List<Card> hand;
    public string deckFolderPath;
    public string deckFilename;



    int rdNumLast = 0;
    // Use this for initialization
    void Start () {
        InitCardDeck();
        //Instantiate(card1);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // randomly take one card from the deck to the player.
    List<Card> RandomGetCard()
    {
        //generate a random num from the number of the cards

        //ensure random number is different
        int rdNum;
        do
        {
            rdNum = Random.Range(0, deck.CountDeck());
        } while (rdNum != rdNumLast);
        rdNumLast = rdNum;

        Card card = deck.GetCardAt(rdNum);
        //add deleted card into hand
        hand.Add(card);
        
        //delete the selected card from deck
        deck.Remove(card);

        return hand;
    }

    // take one specific card from the deck to the player.
    void GetCard(string _cardName)
    {
        Card card = deck.FindCard(_cardName);
        if (card != null)
        {
            deck.Remove(card);
            hand.Add(card);
        }
    }
    
    void UseCard(int index)
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 PlayerPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Tile playerTile = GridManager.Instance.TileFromWorldPoint(PlayerPos);

        switch (hand[index].cardName)
        {
            case "Smoke Grenade":
                //1. hightlight all the possible tile              
                GridManager.Instance.Highlight(playerTile, 3, Tile.HighlightColor.Green);
                //2. get player input tile position

                //3. create a smoke on the tile(make sure when the smoke will disappear)

                //4. change the visibility of enemy

                //5. minus the action point

                break;

            case "Haste":
                //1. add action point
                string effect = deck.FindCard(hand[index].cardName).effect;
                string result = System.Text.RegularExpressions.Regex.Replace(effect, @"[^0-9]+", "");
                int point = int.Parse(result);
                GameObject.FindGameObjectWithTag("Player").GetComponent<player>().AddActionPoint(point);
                //2. move this card to usedCards
                usedCards.Add(hand[index]);
                hand.Remove(hand[index]);
                break;

            case "Chest Key":
                GameObject[] chests = GameObject.FindGameObjectsWithTag("Player");
                //1. find out which chest the player is close to
                for(int i=0;i<chests.Length;i++)
                {
                    Vector3 chestPos = new Vector3(chests[i].transform.position.x, chests[i].transform.position.y, chests[i].transform.position.z);
                    Tile chestTile = GridManager.Instance.TileFromWorldPoint(chestPos);
                    if (GridManager.Instance.IsAdjacent(chestTile, playerTile))
                    {
                        //2. open the chest
                        chests[i].GetComponent<Chest>().isOpen = true;
                    }
                }
                break;

            case "Torch":
                break;

            case "Glue Trap":
                break;

        }

    }

    void InitCardDeck()
    {
        deck = new CardDeck();
        string jsonPath = Path.Combine(Application.streamingAssetsPath, deckFolderPath);
        jsonPath = Path.Combine(jsonPath, deckFilename + ".json");

        string json = File.ReadAllText(jsonPath);

        deck = deck.CreateFromJSON(json);
    }  
}

[System.Serializable]
public class Card
{
    public string cardName;
    public string intro;
    public string type;
    public string effect;
    public int cost;
    public int ID;
}

[System.Serializable]
public class CardDeck
{
    private List<Card> cards;

    public CardDeck CreateFromJSON(string jsonFile)
    {
        CardDeck cardDeckInfo = JsonUtility.FromJson<CardDeck>(jsonFile);

        if (cardDeckInfo.cards == null)
            Debug.Log("null");

        for (int i = 0; i < cardDeckInfo.cards.Count; i++)
        {
            Debug.Log("This Card is " + cardDeckInfo.cards[i].cardName);
        }

        return cardDeckInfo;
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

    public Card FindCard(string _cardName)
    {
        foreach (Card card in cards)
        {
            if(card.cardName == _cardName)
            {
                return card;
            }
        }

        return null;
    }
}