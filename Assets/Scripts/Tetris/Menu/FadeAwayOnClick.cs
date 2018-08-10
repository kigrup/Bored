using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeAwayOnClick : MonoBehaviour {

    public Curtain curtainScript;

    private bool startedFadeOut;
    private bool startedFadeIn;

    Image image;

    private bool afterFadeInEnableRenderer;

    private void Start()
    {
        image = gameObject.GetComponent<Image>();
    }

    private void OnEnable()
    {
        afterFadeInEnableRenderer = true;
        curtainScript.FadeIn();
        startedFadeIn = true;
    }

    private void Update()
    {
        if (startedFadeIn)
        {
            if (curtainScript.FadingIn == false)
            {
                startedFadeIn = false;
                image.enabled = afterFadeInEnableRenderer;
                gameObject.SetActive(afterFadeInEnableRenderer);
                curtainScript.FadeOut();
                startedFadeOut = true;
            }
        }

        if (startedFadeOut)
        {
            if (curtainScript.FadingOut == false)
            {
                startedFadeOut = false;
            }
        }
    }

    public void Click()
    {
        afterFadeInEnableRenderer = false;

        curtainScript.FadeIn();
        startedFadeIn = true;
    }
}
