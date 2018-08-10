using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TetrisMenu : MonoBehaviour {

    // Tetris main menu's parent (play, buttons and high score)
    public GameObject TetrisMainMenu;

    public GameObject TetrisControlsMenu;

    public Curtain curtain;

    public TetrisMaster master;
    private DataHandler dataHandler;

    public Text highscore;

    private bool startedFadeIn = false;
    private int buttonPressed = -1;

	void Start () {
        dataHandler = master.data_handler;

        UpdateScoreInfo();
	}

    private void Update()
    {
        if (startedFadeIn)
        {
            if (curtain.FadingIn == false)
            {
                // Play button
                if (buttonPressed == 0)
                {
                    startedFadeIn = false;
                    TetrisMainMenu.SetActive(false);
                    master.StartGame();
                    curtain.FadeOut();
                }
            }
        }
    }

    public void UpdateScoreInfo()
    {
        int last_score = dataHandler.GetTetrisLastScore();
        int best_score = dataHandler.GetTetrisBestScore();

        highscore.text = "Last : " + last_score + "\n" + "Best : " + best_score;
    }

    public void PlayButton()
    {
        buttonPressed = 0;
        curtain.FadeIn();
        startedFadeIn = true;
    }

    public void ControlsButton()
    {
        buttonPressed = 3;
        master.inTetrisMainMenu = false;
        TetrisControlsMenu.SetActive(true);

    }
}
