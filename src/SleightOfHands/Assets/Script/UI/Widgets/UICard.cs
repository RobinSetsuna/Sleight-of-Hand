using UnityEngine;
using UnityEngine.UI;

public class UICard : UIWidget
{
    [SerializeField] private Text title;
    [SerializeField] private Image background;
    [SerializeField] private Image illustration;
    [SerializeField] private Text description;

    public Card Card { get; private set; }

    internal void Clear()
    {
        Card = null;

        title.text = "";
        background.sprite = null;
        illustration.sprite = null;
        description.text = "";
    }

    public override void Refresh(params object[] args)
    {
        Card card = (Card)args[0];

        if (card != null)
            Card = card;

        // TODO: Load card information
        if (Card != null)
        {
            title.text = Card.Data.Name;
            background.sprite = ResourceUtility.GetCardBackground(Card.Data.Template);
            //illustration.sprite = ResourceUtility.GetCardIllustration(0);
            description.text = Card.Data.Description;
        }
    }
}
