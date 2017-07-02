using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;

enum Slot_ID
{
	AutoSave = 1,
	Slot_1 = 2,
	Slot_2 = 3
}

public class ThingsToSave
{
	public int ID;
	public int Rotation;
	public int x;
	public int y;
}


public class SaveState : MonoBehaviour
{
	private const string GAME_NAME = "GameDev Tycoon Food";
//	private Slot_ID DEFAULT_SLOT = Slot_ID.Slot_1;

	private bool checkDir(string path) { return Directory.Exists(path); }
	private int num = Enum.GetNames(typeof(GridObjectID)).Length;

	private bool createDir()
	{
		string _path = Application.dataPath + "/" + Slot_ID.Slot_1 + "/";
		Directory.CreateDirectory(_path);
		FileStream f = new FileStream(_path + "Slot_1.sav", FileMode.Create);
		f.Close();
		return checkDir(_path);
	}

	public void Save_Object()
	{

		string _path = Application.dataPath + "/" + Slot_ID.Slot_1 + "/";
		  if (!checkDir(_path))
			if (!createDir())
			{
				Debug.LogError("Error Create Directory...");
				return;
			}


			FileStream f = new FileStream(_path + "Slot_1.sav", FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter();
		    ThingsToSave[] ts = new ThingsToSave[num];

		for (int i = 0; i < num; i++)
		{
			GridObjectData e = IDManager.Instance.GetData(i);
			ts[i].ID = num;
			ts[i].Rotation = 0;
			ts[i].x = e.Size.x;
			ts[i].y = e.Size.y;
		}

			bf.Serialize(f, ts);

			f.Close();
	
	}
	
	public void Load_Object()
	{
		string _path = Application.dataPath + "/" + Slot_ID.Slot_1 + "/";

		FileStream f = new FileStream(_path + "Slot_1.sav", FileMode.Open);
		BinaryFormatter bf = new BinaryFormatter();

		GridObjectData[] e = new GridObjectData[num];
		ThingsToSave[] ts = (ThingsToSave[])bf.Deserialize(f);

		for (int i = 0; i < num; i++)
		{
			e[i].ID = (GridObjectID)ts[i].ID;
			e[i].Size.x = ts[i].x;
			e[i].Size.y = ts[i].y;
			//e[i].Size.x = ts[i].Rotation;
			BuildManager.Instance.SetObjectToBuild(e[i]);
		}


		f.Close();

	}


}


