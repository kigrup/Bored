using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;

public class DataControl : MonoBehaviour {

    public static DataControl control;

    public string name;

    public float exp;

    public int tetris_last_score;
    public int tetris_best_score;

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
        newData.name = name;
        newData.exp = exp;
        newData.tetris_last_score = tetris_last_score;
        newData.tetris_best_score = tetris_best_score;

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

            name = "John";
            exp = loadedData.exp;
            tetris_last_score = loadedData.tetris_last_score;
            tetris_best_score = loadedData.tetris_best_score;
        }
    }
}

[Serializable]
class PlayerData
{
    public string name;

    public float exp;

    public int tetris_last_score;
    public int tetris_best_score;
}