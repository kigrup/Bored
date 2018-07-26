using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLoadData : MonoBehaviour {

    public GameObject level_text_go;
    public GameObject level_slider_go;

    private void Start()
    {
        // Check if graphics missing
        if (level_text_go == null || level_slider_go == null)
        {
            Debug.Log("ERROR: Menu level text and slider gameobjects missing or not assigned.");
            return;
        }

        // Get components
        Text level_text = level_text_go.GetComponent<Text>();
        Slider level_slider = level_slider_go.GetComponent<Slider>();

        // Get data from file
        float data_exp = DataControl.control.exp;

        // Change menu graphics
        int level = (int)data_exp / 1000;
        level_text.text = "Level " + level;

        float level_percent = (data_exp % 1000f) / 1000f;
        Debug.Log(level_percent);
        level_slider.value = level_percent;
    }
}
