using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curtain : MonoBehaviour {

    public float fadingSpeed = 2f;
    SpriteRenderer renderer;
    Color color;

    public bool FadeOutOnStart;
    public bool FadeInOnStart;

    private bool fadingIn = false;
    public bool FadingIn {
        get {
            return fadingIn;
        }
    }

    private bool fadingOut = false;
    public bool FadingOut {
        get {
            return fadingOut;
        }
    }

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        color = renderer.color;

        // Remove curtain at app start
        if (FadeOutOnStart)
            FadeOut();
        else if (FadeInOnStart)
            FadeIn();
    }

    private void Update()
    {
        if (fadingOut)
        {
            color.a -= fadingSpeed * Time.deltaTime;
            renderer.color = color;

            if (color.a < 0)
                fadingOut = false;
        }

        if (fadingIn)
        {
            color.a += fadingSpeed * Time.deltaTime;
            renderer.color = color;

            if (color.a > 1)
                fadingIn = false;
        }
    }

    public void FadeIn()
    {
        fadingOut = false;
        fadingIn = true;
    }

    public void FadeOut()
    {
        fadingIn = false;
        fadingOut = true;
    }
}
