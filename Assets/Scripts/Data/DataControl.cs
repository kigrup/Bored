using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;

public class DataControl : MonoBehaviour {

    public static DataControl control;

    public float exp;

    private void Awake()
    {
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
        
        Load();
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/playerdata.dat", FileMode.OpenOrCreate);

        PlayerData newData = new PlayerData();
        newData.exp = exp;

        bf.Serialize(file, newData);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerdata.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerdata.dat", FileMode.Open);
            PlayerData loadedData = (PlayerData)bf.Deserialize(file);
            file.Close();

            exp = loadedData.exp;
        }
    }
}

[Serializable]
class PlayerData
{
    public float exp;
}