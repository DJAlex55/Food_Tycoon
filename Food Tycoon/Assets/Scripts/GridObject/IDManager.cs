using System;
using System.Collections.Generic;
using UnityEngine;

public class IDManager : MonoBehaviour
{
    public static IDManager Instance { get; private set; }

    public GridObjectData[] GridObjects = new GridObjectData[Enum.GetNames(typeof(GridObjectID)).Length];
    
    private void Awake()
    { 

        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Debug.LogError(gameObject + " was a second Instance! and was destroyed for it!");
            Destroy(gameObject);
        }

        if(GridObjects.Length != Enum.GetNames(typeof(GridObjectID)).Length)
        {
            Debug.LogError("Il database GridObjects non contiene il giusto numero di Classi. Procedere a Implementarli!");
        }

        for (int i = 0; i < GridObjects.Length; i++)
        {
            GridObjectData DATA = GridObjects[i];

            DATA.ID = (GridObjectID)i;
            DATA.Name = DATA.ID.ToString();

            DATA.Size.x = DATA.Size.x == 0 ? 1 : DATA.Size.x;
            DATA.Size.y = DATA.Size.y == 0 ? 1 : DATA.Size.y;


            GridObjects[i] = DATA;
        }

    }

    private void Start()
    {
        BuildManager.Instance.SetObjectToBuild(GetData(GridObjectID.wall));
    }

    public GridObjectData GetData(GridObjectID ID)
    {
        //TODO: Maybe make this more secure
        if((int)ID < GridObjects.Length)        
            return GridObjects[(int)ID];
        return null;
    }

}


[Serializable]
public class GridObjectData
{
    public GridObjectID ID = GridObjectID.wall;

    public NodeGridPosition Size;

    public bool CanRotate;

    public bool CanBeBuiltInLines;

    //public bool IsInteractable;

    public string Name;

    public GridObject Prefab;

}

[Serializable]
public class InteractableGridObjectData
{
    public float InteractRange;



}



[Serializable]
public enum GridObjectID
{
    wall,
    Floor,
    cooker1

}