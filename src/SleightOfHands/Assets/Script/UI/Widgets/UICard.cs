using UnityEngine;
using UnityEngine.UI;

public class UICard : UIWidget
{
    [SerializeField] private Text title;
    [SerializeField] private Image background;
    [SerializeField] private Image illustration;
    [SerializeField] private Text description;

    public Card card { get; private set; }

    internal void Clear()
    {
        card = null;

        title.text = "";
        background.sprite = null;
        illustration.sprite = null;
        description.text = "";
    }

    public override void Refresh(params object[] args)
    {
        Card card = (Card)args[0];

        if (card != null)
            this.card = card;

        // TODO: Load card information
        if (this.card != null)
        {
            title.text = this.card.cardName;
            background.sprite = ResourceUtility.GetCardBackground(0);
            //illustration.sprite = ResourceUtility.GetCardIllustration(0);
            //description.text = ;
        }
    }
}
