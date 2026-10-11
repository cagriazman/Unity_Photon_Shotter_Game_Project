using UnityEngine;

public static class TransformExtensions
{
    public static Transform SearchAndFindInChildren(this Transform parent, string searchName)
    {
        if (!parent.gameObject.activeInHierarchy) return null;
        if (parent.name == searchName) return parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform result = parent.GetChild(i).SearchAndFindInChildren(searchName);
            if (result != null) return result;
        }
        return null;
    }
}