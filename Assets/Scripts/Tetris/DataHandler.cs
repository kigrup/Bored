using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour {

    public DataControl data;

    private void OnEnable()
    {
        data = GameObject.Find("Data Control").GetComponent<DataControl>();
    }

    public void AddExp(int exp)
    {
        data.Load();
        data.exp = data.exp + exp;
        data.Save();

        Debug.Log("+" + exp + " EXP");
    }
}
