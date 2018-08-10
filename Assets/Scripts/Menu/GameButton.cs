using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameButton : MonoBehaviour {

	public enum Game { MENU = 0, TETRIS = 1, SNAKE = 2, PACMAN = 3 }

    public Game game;

    public Curtain curtainScript;
    
    private bool startedFadeIn = false;

    private void Update()
    {
        // Check if finished fading in
        if (startedFadeIn)
        {
            if (curtainScript.FadingIn == false)
            {
                startedFadeIn = false;
                SceneManager.LoadScene((int)game);
            }
        }
    }

    public void ChangeToGame(Game game)
    {
        this.game = game;

        curtainScript.FadeIn();

        startedFadeIn = true;
    }
    public void ChangeToGame()
    {
        curtainScript.FadeIn();
        
        // Start fade in
        startedFadeIn = true;
    }
}
