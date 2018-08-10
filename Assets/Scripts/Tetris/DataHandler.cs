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

    public void SetTetrisScore(int newScore)
    {
        data.Load();
        data.tetris_last_score = newScore;

        if (data.tetris_best_score < newScore)
            data.tetris_best_score = newScore;

        data.Save();
    }

    public int GetTetrisLastScore()
    {
        data.Load();
        int last_score = data.tetris_last_score;
        return last_score;
    }

    public int GetTetrisBestScore()
    {
        data.Load();
        int best_score = data.tetris_best_score;
        return best_score;
    }
}
