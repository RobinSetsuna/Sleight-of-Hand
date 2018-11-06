using UnityEngine;

public struct ResourceUtility
{
    public static T GetPrefab<T>(string directory) where T : Object
    {
        return Resources.Load<T>("Prefabs/" + directory);
    }

    public static GameObject GetPrefab(string directory)
    {
        return GetPrefab<GameObject>(directory);
    }

    public static T GetUIPrefab<T>(string name) where T : Object
    {
        return Resources.Load<T>("Prefabs/UI/" + name);
    }

    public static UIWindow GetUIPrefab(string name)
    {
        return GetUIPrefab<UIWindow>(name);
    }

    public static Sprite GetCardBackground(int id)
    {
        return Resources.Load<Sprite>("UI/Card/Background/" + id);
    }

    public static Sprite GetCardIllustration(int id)
    {
        return Resources.Load<Sprite>("UI/Card/Illustration/" + id);
    }
}
