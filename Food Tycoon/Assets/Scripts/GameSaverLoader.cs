using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;



public static class GameSaverLoader
{
    public static readonly string Extension = @".sav";


    public static void Save(string SaveName)
    {
        Save save = new Save();
        List<GridObjectSaveData> GridObjects = new List<GridObjectSaveData>();
        
        
        foreach (GridObject GridObj in BuildManager.Instance.GetAllGridObject())
        {
            GridObjectSaveData gridObjSave = new GridObjectSaveData(GridObj);

            GridObjects.Add(gridObjSave);
        }


        save.GridObjects = GridObjects.ToArray();
        save.Name = SaveName;

        /* This Still Needs To Be Implemented Yet
         * save.TimeOfSave = System.DateTime.Now.ToLongDateString();
         */

        save.FullName = SaveName /*+ "." + save.TimeOfSave*/ + Extension;

        save.FullPath = Application.dataPath + "/" + save.FullName;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream( save.FullPath, FileMode.Create);

        bf.Serialize(stream, save);

        stream.Close();
    }

    public static Save Load(string FullPath)
    {
        
        if (!File.Exists(FullPath))
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(FullPath, FileMode.Open);

        Save save = (Save)bf.Deserialize(stream);
        stream.Close();

        return save;
    }

}

[Serializable]
public class Save
{
    public string FullPath; //FullPath = SavesPath + FullName 
    public string Name;
    //public string TimeOfSave; STILL TO BE IMPLEMENTED
    public string FullName; //FullName = Name + "." + TimeOfSave + Extension

    public GridObjectSaveData[] GridObjects;

    /*  STILL TO BE IMPLEMENTED:
    *   Repuatation
    *   Money
    *   Entities
    *   Entities Position, type, Skill Etc....
    *   Clients 
    *   Unlocked Recipes
    *   Current Menu
    */
}

[Serializable]
public class GridObjectSaveData
{
    public int ID;    
    public int x;
    public int y;
    public int Rot;


    public GridObjectSaveData(int _ID, int _x, int _y, int _Rot)
    {
        ID = _ID;
        x = _x;
        y = _y;
        Rot = _Rot;
    }
    
    public GridObjectSaveData(GridObject GridObj)
    {
        ID = (int)GridObj.ID;
        x = GridObj.GridPos.x;
        y = GridObj.GridPos.y;
        Rot = GridObj.Rot;
    }
    
    public GridObject ToGridObject()
    {
        GridObject GridObj = new GridObject()
        {
            ID = (GridObjectID)ID,
            GridPos = new NodeGridPosition(x, y),
            Rot = Rot
        };

        return GridObj;
    }

}

