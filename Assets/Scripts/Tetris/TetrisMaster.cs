using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TetrisMaster : MonoBehaviour {

    /* Square width and height = +0.958
     *                   scale = +1.023592
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
    public GameObject HollowBlockPrefab;

    // Array of blocks that are already placed.
    public GameObject[,] StaticBlocks = new GameObject[WIDTH, HEIGHT];

    // Array of blocks that are affected by gravity and controls.
    public GameObject[,] MovingBlocks = new GameObject[WIDTH, HEIGHT];

    // Array of blocks that were affected by gravity and controls and will be seen in the next update.
    public GameObject[,] NextFrameMovingBlocks = new GameObject[WIDTH, HEIGHT];

    // Array of hollow blocks that are used in the falling simulation
    public GameObject[,] HollowBlocks = new GameObject[WIDTH, HEIGHT];

    // Coordinate of the piece that is currently moving
    public int PiecePivotX, PiecePivotY;

    // Rates at which player falls
    public float defaultFallSpeed = 0.5f;
    public float fastFallSpeed = 0.05f;

    // Quick fall type: fast fall = false, teleport = true
    public bool instantFall = false;

    // Player died flag
    private bool killPlayer = false;

    // Next piece ID
    public int currentPieceID = 0;

    // Rotation Matrix
    public int[,] RotationMatrix = { { 0, -1 },
                                    { 1, 0 } };

    public int[,] RotationMatrixInversed = { { 0, 1 },
                                            { -1, 0 } };

    void Start () {
        // First piece spawn
        SpawnNextPiece();

        // Start falling at default speed
        InvokeRepeating("Fall", 0.2f, 0.5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Gizmos.DrawSphere(CoordToWorldPos(PiecePivotX, PiecePivotY), 0.2f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }
    }

    //////////////
    // SPAWNING //
    //////////////

    // Spawns a block at the given coordinates, if it can't, the player dies
    private void SpawnBlock(int x, int y)
    {
        // Invalid parameter handling
        if (x < 0 || 9 < x)
        {
            Debug.Log("Error: Tried to SpawnBlock at invalid X coordinate: " + x + ".Out of range.");
            return;
        }
        else if (y < 0 || 21 < y)
        {
            Debug.Log("Error: Tried to SpawnBlock at invalid Y coordinate: " + y + ".Out of range.");
            return;
        }
        else if (MovingBlocks[x, y] != null)
        {
            Debug.Log("Error: Tried to SpawnBlock where there's already a Moving Block.");
            return;
        }

        // Player dies if there's a static block at the spawning location
        else if (StaticBlocks[x, y] != null)
        {
            killPlayer = true;
            return;
        }

        // Instance at the specified coord as a moving block and store it
        Vector3 Position = CoordToWorldPos(x, y);

        MovingBlocks[x, y] = Instantiate(BlockPrefab, Position, Quaternion.identity);
        return;
    }

    // ID   0-  ·#·   1-  ##·    2-  ·##   3-  #$##   4-  ##   5-  ··#   6-  #··
    //          #$#       ·$#        #$·       ····       $#       #$#       #$#

    // Spawns a piece block by block
    private void SpawnPiece(int ID, int x, int y)
    {
        // Try to spawn blocks to form piece
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
        
        // Updates piece pivot coord
        PiecePivotX = x;
        PiecePivotY = y;

    }

    // Spawns the next piece choosing it randomly
    private void SpawnNextPiece()
    {
        currentPieceID = Random.Range(0, 6);
        SpawnPiece(currentPieceID, 5, 18);

        // Handle flag
        if (killPlayer)
        {
            Die();
            return;
        }

        FallSimulation();
    }
    
    ////////////
    // MOVING //
    ////////////

    // Appends blocks to update queue, doesn't actually move them
    private void MoveBlock(int x_from, int y_from, int x_to, int y_to)
    {
        // Invalid parameter handling
        if (x_from < 0 || WIDTH < x_from)
        {
            Debug.Log("Error: Invalid MoveBlock x_from: " + x_from);
            return;
        }
        else if (y_from < 0 || HEIGHT < y_from)
        {
            Debug.Log("Error: Invalid MoveBlock y_from: " + y_from);
            return;
        }
        else if (x_to < 0 || WIDTH < x_to)
        {
            Debug.Log("Error: Invalid MoveBlock x_to: " + x_to);
            return;
        }
        else if (y_to < 0 || HEIGHT < y_to)
        {
            Debug.Log("Error: Invalid MoveBlock y_to: " + y_to);
            return;
        }

        
        // If the moving block is the pivot
        if (x_from == PiecePivotX && y_from == PiecePivotY)
        {
            PiecePivotX = x_to;
            PiecePivotY = y_to;
        }

        // Change NextFrame blocks
        NextFrameMovingBlocks[x_to, y_to] = MovingBlocks[x_from, y_from];
    }

    // Apply movement in queue and update 
    private void ApplyNextFrameBlocks()
    {
        // Take all pending blocks from NextFrameMovingBlocks to MovingBlocks
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                MovingBlocks[x, y] = null;

                // Apply next frame blocks
                if (NextFrameMovingBlocks[x, y] != null)
                {
                    // Update arrays
                    MovingBlocks[x, y] = NextFrameMovingBlocks[x, y];
                    // Update transform of every block
                    MovingBlocks[x, y].transform.position = CoordToWorldPos(x, y);

                    // Delete object reference
                    NextFrameMovingBlocks[x, y] = null;
                }
            }
        }
    }

    // Convert MovingBlocks to StaticBlocks
    private void FallingCollision()
    {

        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (StaticBlocks[x, y] == null)
                {
                    StaticBlocks[x, y] = MovingBlocks[x, y];
                    MovingBlocks[x, y] = null;
                }
            }
        }

        SpawnNextPiece();
    }
    
    // Move all MovingBlocks one block down and handle collision
    private void Fall()
    {
        // If any moving block has a StaticBlock underneath it, detect collision
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (MovingBlocks[x, y] != null)
                {
                    if (y == 0)
                    {
                        FallingCollision();
                        return;
                    }
                    else if (StaticBlocks[x, y-1] != null)
                    {
                        FallingCollision();
                        return;
                    }
                }
            }
        }

        // Otherwise, move all blocks one block down
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (MovingBlocks[x, y] != null)
                    MoveBlock(x, y, x, y - 1);
            }
        }


        // Update blocks
        ApplyNextFrameBlocks();

        CheckFullLines();
    }
    
    // Simulate and update where the piece will fall
    private void FallSimulation()
    {
        // Clean simulation
        CleanArray(HollowBlocks);

        // Instantiate hollow blocks where current piece is
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (MovingBlocks[x, y] != null)
                {
                    Vector3 Position = CoordToWorldPos(x, y);

                    HollowBlocks[x, y] = Instantiate(HollowBlockPrefab, Position, Quaternion.identity);
                }
            }
        }

        // Cancel simulation if after setup HollowBlocks is empty
        bool allNull = true;
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (HollowBlocks[x, y] != null)
                {
                    allNull = false;
                }
            }
        }
        if (allNull)
            return;

        // Make hollow blocks fall until hit static blocks
        while (true)
        {
            // If any block has a StaticBlock underneath it, detect collision
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    if (HollowBlocks[x, y] != null)
                    {
                        if (y == 0)
                        {
                            return;
                        }
                        else if (StaticBlocks[x, y - 1] != null)
                        {
                            return;
                        }
                    }
                }
            }

            GameObject[,] nextFrame = new GameObject[WIDTH, HEIGHT];
            // Otherwise, move all blocks one block down
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    if (HollowBlocks[x, y] != null)
                    {
                        nextFrame[x, y - 1] = HollowBlocks[x, y];

                        nextFrame[x, y - 1].transform.position = CoordToWorldPos(x, y - 1);
                    }
                }
            }
            HollowBlocks = nextFrame;
        }
    }

    // Teleport piece down
    private void InstantFall()
    {
        CleanArray(MovingBlocks);

        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (HollowBlocks[x, y] != null)
                {
                    SpawnBlock(x, y);
                }
            }
        }
    }
    
    //////////////
    // ROTATION //
    //////////////

    public void Rotate(bool clockwise = true)
    {
        // Cancel rotation if currentPiece is a square
        if (currentPieceID == 4)
            return;

        // Move piece's pivot point to (0, 0) for the matrix multiplication
        Vector2[,] relativeMovingBlocks = new Vector2[WIDTH, HEIGHT];
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (MovingBlocks[x, y] != null)
                {
                    // Save the default relative position
                    relativeMovingBlocks[x, y] = new Vector2(x - PiecePivotX, y - PiecePivotY);

                    // Rotate the default relative position
                    relativeMovingBlocks[x, y] = RotationMatrixTimes(relativeMovingBlocks[x, y], clockwise);

                    // Convert it to absolute position
                    float absX = relativeMovingBlocks[x, y].x + PiecePivotX;
                    float absY = relativeMovingBlocks[x, y].y + PiecePivotY;
                    relativeMovingBlocks[x, y] = new Vector2(absX, absY);

                    // If new absolute position is out of bounds, cancel rotation
                    if (absX < 0 || 9 < absX)
                    {
                        return;
                    }
                    else if (absY < 0 || 21 < absY)
                    {
                        return;
                    }
                }
            }
        }

        // If new absolute positions aren't out of bounds, move blocks to new position
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (relativeMovingBlocks[x, y] != null)
                {
                    int newX = (int)relativeMovingBlocks[x, y].x;
                    int newY = (int)relativeMovingBlocks[x, y].y;
                    MoveBlock(x, y, newX, newY);
                }
            }
        }

        ApplyNextFrameBlocks();

        FallSimulation();
    }

    ////////////
    // PLAYER //
    ////////////

    // Cleans screen
    private void Die()
    {
        killPlayer = false;

        CleanArray(MovingBlocks);
        CleanArray(StaticBlocks);
        CleanArray(NextFrameMovingBlocks);
        CleanArray(HollowBlocks);
    }

    // From player input
    public void MoveLeft()
    {
        // If any moving block has a StaticBlock to its left, cancel movement
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (MovingBlocks[x, y] != null)
                {
                    if (x == 0)
                    {
                        return;
                    }
                    else if (StaticBlocks[x - 1, y] != null)
                    {
                        return;
                    }
                }
            }
        }

        // Otherwise, move all blocks one block to the left
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (MovingBlocks[x, y] != null)
                    MoveBlock(x, y, x - 1, y);
            }
        }

        // Update blocks
        ApplyNextFrameBlocks();

        // And simulate maximum falling
        FallSimulation();
    }
    public void MoveRight()
    {
        // If any moving block has a StaticBlock to its right, cancel movement
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (MovingBlocks[x, y] != null)
                {
                    if (x == WIDTH - 1)
                    {
                        return;
                    }
                    else if (StaticBlocks[x + 1, y] != null)
                    {
                        return;
                    }
                }
            }
        }

        // Otherwise, move all blocks one block to the right
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (MovingBlocks[x, y] != null)
                    MoveBlock(x, y, x + 1, y);
            }
        }

        // Update blocks
        ApplyNextFrameBlocks();

        // And simulate maximum falling
        FallSimulation();
    }
    public void StartQuickFall()
    {
        if (instantFall)
        {
            InstantFall();
            Fall();
        }
        else
        {
            CancelInvoke();
            InvokeRepeating("Fall", 0f, fastFallSpeed);
        }
    }
    public void StopQuickFall()
    {
        // Set fall speed to default if not instant falling
        if (!instantFall)
        {
            CancelInvoke();
            InvokeRepeating("Fall", 0f, defaultFallSpeed);
        }
    }

    // Delete completed lines and award XP
    public void CheckFullLines()
    {
        int deletedLines = 0;

        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (StaticBlocks[x, y] == null)
                {
                    x = 100;
                }
                
                if (x == 9)
                {
                    deletedLines++;
                    DeleteLine(y);
                    y--;
                }
            }
        }


    }

    public void DeleteLine(int deletedY)
    {
        for (int y = deletedY; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (y == deletedY)
                {
                    // Clean line
                    Destroy(StaticBlocks[x, y]);
                    StaticBlocks[x, y] = null;
                }
                else
                {
                    // Move line below
                    StaticBlocks[x, y - 1] = StaticBlocks[x, y];
                    StaticBlocks[x, y] = null;

                    StaticBlocks[x, y - 1].transform.position = CoordToWorldPos(x, y - 1);
                }
            }
        }
    }

    ///////////////
    // UTILITIES //
    ///////////////

    // Converts tetris map coords into Unity Vector3
    public Vector3 CoordToWorldPos(int x, int y)
    {
        float XPos = -4.344f + (0.958f * x);
        float YPos = -8.526f + (0.958f * y);
        float ZPos = +8.720f;
        Vector3 Position = new Vector3(XPos, YPos, ZPos);

        return Position;
    }

    // Cleans an array and destroys its objects
    public void CleanArray(GameObject[,] array)
    {
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (array[x, y] != null)
                {
                    Destroy(array[x, y]);
                    array[x, y] = null;
                }
            }
        }
    }

    // Matrix multiplication
    public Vector2 RotationMatrixTimes(Vector2 oldPosition, bool clockwise = true)
    {
        int x, y;
        if (clockwise)
        {
            x = RotationMatrix[0, 0] * (int)oldPosition.x + RotationMatrix[1, 0] * (int)oldPosition.y;
            y = RotationMatrix[0, 1] * (int)oldPosition.x + RotationMatrix[1, 1] * (int)oldPosition.y;
        }
        else
        {
            x = RotationMatrixInversed[0, 0] * (int)oldPosition.x + RotationMatrixInversed[1, 0] * (int)oldPosition.y;
            y = RotationMatrixInversed[0, 1] * (int)oldPosition.x + RotationMatrixInversed[1, 1] * (int)oldPosition.y;
        }
        
        Vector2 newPosition = new Vector2(x, y); // Devil's anus
        return newPosition;
    }
}