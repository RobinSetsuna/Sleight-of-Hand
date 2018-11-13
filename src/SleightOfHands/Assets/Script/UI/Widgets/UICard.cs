using UnityEngine;
using UnityEngine.UI;

public class UICard : UIWidget
{
    [SerializeField] private Text title;
    [SerializeField] private Image template;
    [SerializeField] private Image illustration;
    [SerializeField] private Text description;

    public Card Card { get; private set; }

    internal void Clear()
    {
        Card = null;

        title.text = "";
        template.sprite = null;
        illustration.sprite = null;
        description.text = "";
    }

    public override void Refresh(params object[] args)
    {
        Card card = (Card)args[0];

        if (card != null)
            Card = card;

        if (Card != null)
        {
            CardData cardData = Card.Data;

            title.text = cardData.Name;
            template.sprite = ResourceUtility.GetCardTemplate(cardData.Template);
            illustration.sprite = ResourceUtility.GetCardIllustration(cardData.Illustration);
            description.text = cardData.Description;
        }
    }
}
