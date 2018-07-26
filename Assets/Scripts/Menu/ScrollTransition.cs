using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollTransition : MonoBehaviour {

    // Complete scrollview
    public GameObject scrollview;

    private Scrollbar scroll;
    private float value;

    public GameObject title;
    private SpriteRenderer renderer;

    private void Start()
    {
        scroll = GetComponent<Scrollbar>();

        // Save all renderers on title to change alpha when scrolling
        renderer = title.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        value = scroll.value;

        if (!Input.GetMouseButton(0))
        {
            // If value close to 0 or 1 clamp it there
            if (value < 0.01)
            {
                value = 0;
            }
            else if (value > 0.99)
            {
                value = 1;
            }
            // If value at 25% slowly close it to 0
            else if (value < 0.25)
            {
                value *= 0.8f;
            }
            // If value over 25% quickly move it away
            else if (value >= 0.25)
            {
                value *= 1.1f;
            }
        }

        // Update position
        scroll.value = value;

        //// Update alpha according to position
        //Color c = renderer.color;
        //float newValue = (value*1.4f < 1) ? Mathf.Abs(value * 1.4f - 1) : 0;
        //c.a = newValue;
        //renderer.color = c;

        // Destroy scrollbar if it slides completely
        if (value == 1)
        {
            Destroy(scrollview);
        }
    }
}
