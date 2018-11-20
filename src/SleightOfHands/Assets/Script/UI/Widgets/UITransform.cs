using UnityEngine;

public class UITransform : MonoBehaviour
{
    public Vector3 targetLocalScale = Vector3.one;
    public Vector3 targetLocalPosition = Vector3.zero;
    public long duration = 1000;

    private long startTime;
    private Vector3 initialLocalScale = Vector3.one;
    private Vector3 initialLocalPosition = Vector3.zero;

    private void OnEnable()
    {
        startTime = TimeUtility.localTimeInMilisecond;

        initialLocalScale = transform.localScale;
        initialLocalPosition = transform.localPosition;
    }

    private void FixedUpdate()
    {
        float t = (float)(TimeUtility.localTimeInMilisecond - startTime) / duration;

        transform.localScale = Vector3.Lerp(initialLocalScale, targetLocalScale, t);
        transform.localPosition = Vector3.Lerp(initialLocalPosition, targetLocalPosition, t);

        if (t >= 1)
            enabled = false;
    }
}
