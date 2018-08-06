using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperiencePopup : MonoBehaviour {

    Text text;
    float moved = 0f;
    float alpha = 1f;

    private void Awake()
    {
        text = gameObject.GetComponent<Text>();
    }

    void Update () {
        Destroy(gameObject, 2f);

        gameObject.transform.Translate(new Vector2(0, 0.01f));
        moved += 0.01f;
        
        if (moved > 0.5f)
        {
            text.color = new Color(255, 255, 255, alpha);
            alpha -= 0.1f;
        }
	}
}
