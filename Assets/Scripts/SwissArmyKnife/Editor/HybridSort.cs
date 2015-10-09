using UnityEngine;
using UnityEditor;

public class HybridSort : BaseHierarchySort
{
    public override int Compare(GameObject lhs, GameObject rhs)
    {
        if (lhs == rhs) return 0;
        if (lhs == null) return -1;
        if (rhs == null) return 1;

        if (lhs.GetComponent<CanvasRenderer>() != null || rhs.GetComponent<CanvasRenderer>() != null)
        {
            // UI Elements are sorted by simbing index
            return (lhs.transform.GetSiblingIndex() < rhs.transform.GetSiblingIndex()) ? -1 : 1;
        }
        else
        {
            // Everything else is rendered alphabetically
            return EditorUtility.NaturalCompare(lhs.name, rhs.name);
        }
    }

}