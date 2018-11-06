using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public virtual void OnOpen(params object[] args) {}
    public virtual void OnClose() {}

    public virtual void Close()
    {
        UIManager.Singleton.Close(GetType().Name);
    }
}
