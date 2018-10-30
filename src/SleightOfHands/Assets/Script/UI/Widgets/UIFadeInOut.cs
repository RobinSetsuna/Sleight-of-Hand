using UnityEngine;

public class UIFadeInOut : UIEffect
{
    [SerializeField] private float fadeInSpeed = 0.5f;
    [SerializeField] private float fadeOutSpeed = 0.5f;

    private bool isFadingIn = true;

    private void OnEnable()
    {
        isFadingIn = true;
        GetComponent<CanvasRenderer>().SetAlpha(0);
    }

    private void Update()
    {
        CanvasRenderer canvasRenderer = GetComponent<CanvasRenderer>();

        if (isFadingIn)
        {
            float newAlpha = canvasRenderer.GetAlpha() + fadeOutSpeed * Time.deltaTime;

            if (newAlpha >= 1)
            {
                canvasRenderer.SetAlpha(1);
                isFadingIn = false;
            }
            else
                canvasRenderer.SetAlpha(newAlpha);
        }
        else
        {
            float newAlpha = canvasRenderer.GetAlpha() - fadeOutSpeed * Time.deltaTime;

            if (newAlpha <= 0)
            {
                canvasRenderer.SetAlpha(0);
                gameObject.SetActive(false);
            }
            else
                canvasRenderer.SetAlpha(newAlpha);
        }
    }
}
