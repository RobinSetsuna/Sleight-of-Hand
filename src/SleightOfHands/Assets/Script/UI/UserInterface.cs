using UnityEngine;

public abstract class UserInterface : MonoBehaviour
{
    public virtual void OnOpen() {}
    public virtual void OnClose() {}

    public virtual void Close()
    {
        UIManager.Singleton.Close(GetType().Name);
    }
}
