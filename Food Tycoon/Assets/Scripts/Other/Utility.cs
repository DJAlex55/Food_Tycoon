using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    /// <summary>
    /// Fires a raycast from the mouse position in the camera viewport
    /// </summary>
    /// <param name="mask"></param>
    /// <returns></returns>
    public static bool DoRayCast(out RaycastHit hit, LayerMask mask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        return Physics.Raycast(ray, out hit, Mathf.Infinity, mask);
    }

    public static void DoRayCastOnGrid(out RaycastHit hit)
    {
        Utility.DoRayCast(out hit, Grid.Instance.GridCollider.GridColliderMask);
    }

}



public static class Utility<T>
{
    public static void CleanUpList(List<T> ListToCleanUp)
    {
        if (ListToCleanUp == null)
            return;

        List<T> ItemsToRemove = new List<T>();
        foreach (T CurrentItem in ListToCleanUp)
            if (CurrentItem == null)
                ItemsToRemove.Add(CurrentItem);

        if (ItemsToRemove.Count > 0)
        {
            foreach (T ItemToRemove in ItemsToRemove)
                ListToCleanUp.Remove(ItemToRemove);
            ItemsToRemove = null;
        }

    }
}
