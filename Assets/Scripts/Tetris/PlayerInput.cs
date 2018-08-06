using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    // Tetris master reference
    public TetrisMaster GameMaster;

    static float moveDelay = 0.2f;
    static float moveSpeed = 0.07f;

    static bool quickFalling = false;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PassMoveLeft();

            CancelInvoke();
            InvokeRepeating("PassMoveLeft", moveDelay, moveSpeed);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            PassMoveRight();

            CancelInvoke();
            InvokeRepeating("PassMoveRight", moveDelay, moveSpeed);
        }
        else if (Input.GetKeyDown(KeyCode.S)){
            quickFalling = true;
            GameMaster.StartQuickFall();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            GameMaster.Rotate();
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyDown(KeyCode.S))
        {
            CancelInvoke();
            if (quickFalling)
            {
                quickFalling = false;
                GameMaster.StopQuickFall();
            }
        }


		if (Input.GetMouseButtonDown(0))
        {
            // Bottom left part of the screen moves the player left
            if (Input.mousePosition.x < (Screen.width / 3) && Input.mousePosition.y < Screen.height / 2)
            {
                PassMoveLeft();

                CancelInvoke();
                InvokeRepeating("PassMoveLeft", moveDelay, moveSpeed);
            }

            // Bottom right part of the screen moves the player right
            else if (Input.mousePosition.x > 2 * (Screen.width / 3) && Input.mousePosition.y < Screen.height / 2)
            {
                PassMoveRight();

                CancelInvoke();
                InvokeRepeating("PassMoveRight", moveDelay, moveSpeed);
            }

            // Bottom mid part of the screen makes the block fall fast
            else if (Input.mousePosition.y < Screen.width / 2) 
            {
                if (Input.mousePosition.x > (Screen.width / 3) && Input.mousePosition.x < 2 * (Screen.width / 3))
                {
                    quickFalling = true;
                    GameMaster.StartQuickFall();
                }
            }

            // Upper half of the screen rotates the block clockwise
            else if(Input.mousePosition.y > (Screen.width / 2))
            {
                GameMaster.Rotate();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            CancelInvoke();
            if (quickFalling)
            {
                quickFalling = false;
                GameMaster.StopQuickFall();
            }
        }
    }

    private void PassMoveLeft()
    {
        GameMaster.MoveLeft();
    }

    private void PassMoveRight()
    {
        GameMaster.MoveRight();
    }
}
