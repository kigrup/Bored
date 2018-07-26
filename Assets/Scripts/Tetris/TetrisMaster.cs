using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisMaster : MonoBehaviour {

    /* Square width and height = +0.958
     * Coord [0,0] at        X = -4.344
     *                       Y = -8.526
     *                       Z = +8.720
     *                       
     * Screen size is 10 by 19. Arrays have some extra space up for spawning pieces.
     * Default piece spawn coord = [5, 18]
    */

    public static int WIDTH = 10;
    public static int HEIGHT = 22;

    public GameObject BlockPrefab;

    // Array of blocks that are already placed.
    public GameObject[,] StaticBlocks = new GameObject[WIDTH, HEIGHT];

    // Array of blocks that are affected by gravity and controls.
    public GameObject[,] MovingBlocks = new GameObject[WIDTH, HEIGHT];

    // Array of blocks that were affected by gravity and controls and will be seen in the next update.
    public GameObject[,] NextFrameMovingBlocks = new GameObject[WIDTH, HEIGHT];

	void Start () {
        SpawnNextPiece();
	}

    //////////////
    // SPAWNING //
    //////////////

    private bool SpawnBlock(int x, int y)
    {
        // Invalid parameter handling
        if (x < 0 || 9 < x)
        {
            Debug.Log("Error: Tried to SpawnBlock at invalid X coordinate: " + x + ".Out of range.");
            return false;
        }
        else if (y < 0 || 21 < y)
        {
            Debug.Log("Error: Tried to SpawnBlock at invalid Y coordinate: " + y + ".Out of range.");
            return false;
        }
        else if (StaticBlocks[x, y] != null)
        {
            Debug.Log("Error: Tried to SpawnBlock over Static Block.");
            return false;
        }
        else if (MovingBlocks[x, y] != null)
        {
            Debug.Log("Error: Tried to SpawnBlock where there's already a Moving Block.");
            return false;
        }

        // Instance at the specified coord as a moving block and store it
        Vector3 Position = CoordToWorldPos(x, y);

        MovingBlocks[x, y] = Instantiate(BlockPrefab, Position, Quaternion.identity);
        return true;
    }

    // ID   0-  ·#·   1-  ##·    2-  ·##   3-  #$##   4-  ##   5-  ··#   6-  #··
    //          #$#       ·$#        #$·       ····       $#       #$#       #$#

    private void SpawnPiece(int ID, int x, int y)
    {
        switch (ID)
        {
            case 0:
                SpawnBlock(x, y);
                SpawnBlock(x - 1, y);
                SpawnBlock(x, y + 1);
                SpawnBlock(x + 1, y);
                break;

            case 1:
                SpawnBlock(x, y);
                SpawnBlock(x - 1, y + 1);
                SpawnBlock(x, y + 1);
                SpawnBlock(x + 1, y);
                break;
            case 2:
                SpawnBlock(x, y);
                SpawnBlock(x + 1, y + 1);
                SpawnBlock(x, y + 1);
                SpawnBlock(x - 1, y);
                break;
            case 3:
                SpawnBlock(x, y);
                SpawnBlock(x - 1, y);
                SpawnBlock(x + 1, y);
                SpawnBlock(x + 2, y);
                break;
            case 4:
                SpawnBlock(x, y);
                SpawnBlock(x + 1, y);
                SpawnBlock(x, y + 1);
                SpawnBlock(x + 1, y + 1);
                break;
            case 5:
                SpawnBlock(x, y);
                SpawnBlock(x - 1, y);
                SpawnBlock(x + 1, y);
                SpawnBlock(x + 1, y + 1);
                break;
            case 6:
                SpawnBlock(x, y);
                SpawnBlock(x - 1, y);
                SpawnBlock(x + 1, y);
                SpawnBlock(x - 1, y + 1);
                break;
            default:
                Debug.Log("Error: Invalid SpawnPiece ID: " + ID);
                break;
        }
    }

    private void SpawnNextPiece()
    {
        int nextID = Random.Range(0, 6);
        SpawnPiece(nextID, 5, 18);
    }
    
    ////////////
    // MOVING //
    ////////////

    // Appends blocks to update queue, doesn't actually move them
    private bool MoveBlock(int x_from, int y_from, int x_to, int y_to)
    {
        // Invalid parameter handling
        if (x_from < 0 || WIDTH-1 < x_from)
        {
            Debug.Log("Error: Invalid MoveBlock x_from: " + x_from);
            return false;
        }
        else if (y_from < 0 || HEIGHT-1 < y_from)
        {
            Debug.Log("Error: Invalid MoveBlock y_from: " + y_from);
            return false;
        }
        else if (x_to < 0 || WIDTH-1 < x_to)
        {
            Debug.Log("Error: Invalid MoveBlock x_to: " + x_to);
            return false;
        }
        else if (HEIGHT-1 < y_to)
        {
            Debug.Log("Error: Invalid MoveBlock y_to: " + y_to);
            return false;
        }
        if (x_from == null && x_to == null && y_from == null && y_to == null)
        {
            Debug.Log("Error: Called MoveBlock with some null parameter.");
            return false;
        }

        // On falling collision convert all moving blocks to static blocks
        if (y_to < y_from)
        {
            if (StaticBlocks[x_to, y_to] != null || y_to < 0)
            {
                MovingBlocks.CopyTo(StaticBlocks, 0);
                ((IList)MovingBlocks).Clear();

                SpawnNextPiece();
            }
        }

        // Change NextFrame blocks
        NextFrameMovingBlocks[x_to, y_to] = MovingBlocks[x_from, y_from];

        return true;
    }

    // Move blocks from queue
    private void ApplyNextFrameBlocks()
    {
        // Clear previous array
        ((IList)MovingBlocks).Clear();

        // Take all pending blocks from NextFrameMovingBlocks to MovingBlocks
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (NextFrameMovingBlocks[x, y] != null)
                {
                    // Update arrays
                    MovingBlocks[x, y] = NextFrameMovingBlocks[x, y];
                    // Update transform of every block
                    MovingBlocks[x, y].transform.position = CoordToWorldPos(x, y);
                }
            }
        }
    }
    
    private void Fall()
    {

    }

    ///////////////
    // UTILITIES //
    ///////////////

    public Vector3 CoordToWorldPos(int x, int y)
    {
        float XPos = -4.344f + (0.958f * x);
        float YPos = -8.526f + (0.958f * y);
        float ZPos = +8.720f;
        Vector3 Position = new Vector3(XPos, YPos, ZPos);

        return Position;
    }
}