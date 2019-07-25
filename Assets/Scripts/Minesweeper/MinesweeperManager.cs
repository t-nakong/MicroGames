using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinesweeperManager : MonoBehaviour
{
    public RectTransform TileParent;
    public Image[] Tiles;
    public Sprite[] NumberSprites;
    public Sprite[] IconSprites;
    public Button ToggleFlagButton;
    public bool isFlagging;

    public void ToggleFlagging()
    {
        isFlagging = !isFlagging;
        if(isFlagging)
        {
            ToggleFlagButton.image.sprite = IconSprites[2];
        }
        else
        {
            ToggleFlagButton.image.sprite = IconSprites[0];
        }
    }

    #region Helper
    
    // The Grid itself
    public static int w = 10;
    public static int h = 10;
    public static Element[,] elements = new Element[w, h];
    public static double density = 0.05;
    public static int score;
    public static int resetBoard = 0;
    public static string status = "Empty"; // TODO: Make this enumerable
    public static string enemyStatus = "Default"; // TODO: Make this enumerable
    public static bool isOpened; // Ensures players only potentially earn 1 point per round
    public static bool gameOver = false; // Set true on timer end
    public static float totalTime = 180;
    // Uncover all Mines

    public void onClick(GameObject tile)
    {
        Element e = tile.GetComponent<Element>();

        if (!gameOver)
        {
           if (isFlagging) // In flagging mode
            {
                if (e.flag)
                {
                    e.flag = false;
                } else
                {
                    e.flag = true;
                }
            }
            else // In normal mode
            {
                // It's a mine
                if (e.mine)
                {
                    status = "Boom!";
                    // uncover all mines
                    uncoverMines();

                    // game over
                    gameOver = true;
                    print("you lose");

                }
                // It's not a mine
                else
                {
                    status = "Empty";
                    // show adjacent mine number
                    int x = (int)transform.position.x;
                    int y = (int)transform.position.y;

                    // uncover area without mines
                    FFuncover(x, y, new bool[w, h]);

                    // find out if the game was won now
                    if (isFinished() && !(isOpened))
                    {
                        print("you win");
                        score += 1;
                        status = "Clear!";
                        isOpened = true;

                    }
                }
            }

            
            //else if (GetComponent<SpriteRenderer>().sprite.texture.name == "flag")
            //{
            //    // Do nothing
            //}
            //else if (GetComponent<SpriteRenderer>().sprite.texture.name.Contains("empty"))
            //{
            //    // Logic for left clicking number
            //    int x = (int)transform.position.x;
            //    int y = (int)transform.position.y;
            //    Chord(x, y);

            //    if (isFinished() && !(isOpened))
            //    {
            //        print("you win");
            //        score += 1;
            //        status = "Clear!";
            //        isOpened = true;

            //    }
            //}
            //if (Input.GetMouseButtonDown(1))
            //{
            //    ToggleFlag();
            //}
        }
    }
    //    private static void uncoverMines()
    //    {
    //        foreach (Element elem in elements)
    //        {
    //            if (elem.mine) elem.loadTexture(0);
    //        }
    //        isOpened = true;
    //    }

    //    private static void sendAction(int time)
    //    {
    //        // TODO sending interactions to enemy
    //        enemyStatus = "Affected";
    //    }

    //    private static bool isFinished()
    //    {
    //        // Try to find a covered element that is no mine
    //        foreach (Element elem in elements)
    //            if (elem.isCovered() && !elem.mine)
    //                return false;
    //        // There are none => all are mines => game won.
    //        gameOver = true;
    //        return true;
    //    }

    private void ReRoll(Element elem)
    {
        // Randomly decide if it's a mine or not
        elem.mine = Random.value < density;
        elem.flag = false;
        elem.covered = true;
        // Register in Grid
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;
        GetComponent<SpriteRenderer>().sprite = defaultTexture;

        PlayField.elements[x, y] = this;
    }

    private static bool flagAt(int x, int y)
    {
        // Coordinates in range? Then check for flag.
        if (x >= 0 && y >= 0 && x < w && y < h)
            return elements[x, y].flag;
        return false;
    }

    // Find out if a mine is at the coordinates
    private static bool mineAt(int x, int y)
    {
        // Coordinates in range? Then check for mine.
        if (x >= 0 && y >= 0 && x < w && y < h)
            return elements[x, y].mine;
        return false;
    }

    // Count adjacent flags for an element
    private static int adjacentFlags(int x, int y)
    {
        int count = 0;

        if (flagAt(x, y + 1)) ++count; // top
        if (flagAt(x + 1, y + 1)) ++count; // top-right
        if (flagAt(x + 1, y)) ++count; // right
        if (flagAt(x + 1, y - 1)) ++count; // bottom-right
        if (flagAt(x, y - 1)) ++count; // bottom
        if (flagAt(x - 1, y - 1)) ++count; // bottom-left
        if (flagAt(x - 1, y)) ++count; // left
        if (flagAt(x - 1, y + 1)) ++count; // top-left

        return count;
    }

    // Count adjacent mines for an element
    private static int adjacentMines(int x, int y)
    {
        int count = 0;

        if (mineAt(x, y + 1)) ++count; // top
        if (mineAt(x + 1, y + 1)) ++count; // top-right
        if (mineAt(x + 1, y)) ++count; // right
        if (mineAt(x + 1, y - 1)) ++count; // bottom-right
        if (mineAt(x, y - 1)) ++count; // bottom
        if (mineAt(x - 1, y - 1)) ++count; // bottom-left
        if (mineAt(x - 1, y)) ++count; // left
        if (mineAt(x - 1, y + 1)) ++count; // top-left

        return count;
    }

    // Flood Fill empty elements
    private static void FFuncover(int x, int y, bool[,] visited)
    {
        // Coordinates in Range?
        if (x >= 0 && y >= 0 && x < w && y < h)
        {
            // visited already?
            if (visited[x, y])
                return;

            // uncover element
            if (elements[x, y].isCovered())
                elements[x, y].loadTexture(adjacentMines(x, y));

            // close to a mine? then no more work needed here
            if (adjacentMines(x, y) > 0)
                return;

            // set visited flag
            visited[x, y] = true;

            // recursion
            FFuncover(x - 1, y - 1, visited);
            FFuncover(x, y - 1, visited);
            FFuncover(x + 1, y - 1, visited);
            FFuncover(x - 1, y, visited);
            FFuncover(x + 1, y, visited);
            FFuncover(x - 1, y + 1, visited);
            FFuncover(x, y + 1, visited);
            FFuncover(x + 1, y + 1, visited);
        }
    }

    //    public static void Chord(int x, int y)
    //    {
    //        if (adjacentMines(x, y) == adjacentFlags(x, y))
    //        {
    //            for (int i = x - 1; i <= x + 1; i++)
    //            {
    //                for (int j = y - 1; j <= y + 1; j++)
    //                {
    //                    if (i != x || j != y)
    //                    {
    //                        if (i >= 0 && j >= 0 && i < w && j < h && elements[i, j].isCovered() && !elements[i, j].flag)
    //                        {
    //                            // Uncover element as long as it is covered and is a number
    //                            if (adjacentMines(i, j) > 0 && !elements[i, j].mine)
    //                            {
    //                                elements[i, j].loadTexture(adjacentMines(i, j));
    //                            }
    //                            else if (adjacentMines(i, j) > 0 && elements[i, j].mine)
    //                            {
    //                                // uncovered a mine!
    //                                status = "Boom!";
    //                                // uncover all mines
    //                                uncoverMines();

    //                                // game over
    //                                gameOver = true;
    //                            }
    //                            else if (elements[i, j].mine)
    //                            {
    //                                // uncovered a mine!
    //                                status = "Boom!";
    //                                // uncover all mines
    //                                uncoverMines();

    //                                // game over
    //                                gameOver = true;
    //                            }
    //                            else
    //                            {
    //                                FFuncover(i, j, new bool[w, h]);
    //                            }

    //                        }

    //                    }
    //                }
    //            }
    //        }
    //    }

    #endregion

    //    public void UpdateTexture(int index, int adjacentCount, Element elem)
    //    {
    //        if (elem.flag)
    //            Tiles[index].sprite = IconSprites[2];
    //        else if (elem.mine)
    //            Tiles[index].sprite = IconSprites[3];
    //        else
    //            Tiles[index].sprite = NumberSprites[adjacentCount];

    //    }

    //    private void ResetGrid()
    //    {
    //        foreach (Image tile in Tiles)
    //        {
    //            tile.sprite = IconSprites[0]; // set sprite to default
    //        }
    //    }

    //    private void SetParentDimensions()
    //    {
    //        float size = (int)(0.8f * Screen.height);
    //        TileParent.sizeDelta = new Vector2(size, size);
    //    }

    //    // Start is called before the first frame update
    //    void Start()
    //    {
    //        SetParentDimensions();
    //        ResetGrid();
    //    }

    //    // Update is called once per frame
    //    void Update()
    //    {

    //    }
}
