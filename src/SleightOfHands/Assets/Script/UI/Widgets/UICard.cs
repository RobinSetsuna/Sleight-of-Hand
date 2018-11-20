using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICard : UIWidget
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Image template;
    [SerializeField] private Image illustration;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private GameObject selectedEffect;

    public Card Card { get; private set; }

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

    internal void ToggleSelection()
    {
        selectedEffect.SetActive(!selectedEffect.activeSelf);
    }
}
