using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets;
using System;

public class bottoms : MonoBehaviour
{
    private GUIStyle testStyle = new GUIStyle();

    public Texture BaiIvan;
    public Texture Sink;

    public Font myFont;
    public Font typeOfFiguresFont;

    private static bool isOutOfTheMatrix = false;
    private static bool isCorrectFigure = true;

    //public GUISkin customSkin = new GUISkin();

    public Texture2D bottomColor;
    string figureType = string.Empty;

    public static string rowToPlaceStr = "0";
    public static string colToPlaceStr = "0";
    public static int rowToPlace;
    public static int colToPlace;

    /// <summary>
    /// shortest length between the empty cells
    /// </summary>
    public static int bestLength;

    /// <summary>
    /// bestRow is the row in wich the new figure is going to be placed. It is the nearest empty place.
    /// </summary>
    public static int bestRow;

    /// <summary>
    /// bestCol is the col in wich the new figure is going to be placed. It is the nearest empty place.
    /// </summary>
    public static int bestCol;

    /// <summary>
    /// cells that are visited in recursion. We use this because we need to clean the cells for the next figure
    /// </summary>
    public static List<KeyValuePair<int, int>> currentVisitedCells = new List<KeyValuePair<int, int>>();

    Cell[,] matrix = new Cell[10, 10];

	// Use this for initialization
	void Start () {
        FillCellMatrix(matrix);
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnGUI()
    {
        int leftFloat = 10;
        int topFloat = 0;
        for (int i = 1; i <= 100; i++)
        {
            if ((i == 11) || (i == 21) || (i == 31) || (i == 41) || (i == 51) || (i == 61) || (i == 71) || (i == 81) || (i == 91))
            {
                leftFloat = 10;
            }

            if (i < 11)
            {
                topFloat = 70;
            }
            else if (i < 21)
            {
                topFloat = 120;
            }
            else if (i < 31)
            {
                topFloat = 170;
            }
            else if (i < 41)
            {
                topFloat = 220;
            }
            else if (i < 51)
            {
                topFloat = 270;
            }
            else if (i < 61)
            {
                topFloat = 320;
            }
            else if (i < 71)
            {
                topFloat = 370;
            }
            else if (i < 81)
            {
                topFloat = 420;
            }
            else if (i < 91)
            {
                topFloat = 470;
            }
            else
            {
                topFloat = 520;
            }


            GUI.Box(new Rect(leftFloat, topFloat, 50, 50), string.Empty);
            leftFloat += 50;

            
        }
        GUIStyle textColor = new GUIStyle();
        textColor.font = typeOfFiguresFont;
        GUIStyle TitleStyle = new GUIStyle();
        TitleStyle.font = myFont;
        //------------Draw Explanation------------//

        GUI.Label(new Rect(70, 10, 100, 50), "                                        Old Ivan wants to \nFind the nearest empty place for new figure...", TitleStyle);
        GUI.Label(new Rect(530, 70, 200, 300), "Choose between: \nninetile, plus, angle-ul, \nangle-dl, angle-ur, angle-dr,\n vline and hline.", textColor);

        //--------------DRAW BAI IVAN and sink-----------//

        GUI.Button(new Rect(450, 280, BaiIvan.width, BaiIvan.height), BaiIvan, testStyle);
        GUI.Button(new Rect(660, 410, Sink.width, Sink.height), Sink, testStyle);

        //--------------DRAW AUTHORS--------------//

        GUI.Label(new Rect(530, 525, 100, 30), "Authors: \nzhelyazkovn@gmail.com \nRumen.tonev@yahoo.com", textColor);

        //-------------INPUT FIGURE----------------//

        

        GUI.Label(new Rect(530, 145, 100, 30), "Figure type: ", textColor);

        figureType = GUI.TextField(new Rect(680, 145, 100, 25), figureType, 8);

        GUI.Label(new Rect(530, 180, 150, 30), "Central row: ", textColor);

        rowToPlaceStr = GUI.TextField(new Rect(680, 180, 100, 25), rowToPlaceStr, 1);

        GUI.Label(new Rect(530, 215, 150, 30), "Central col: ", textColor);

        colToPlaceStr = GUI.TextField(new Rect(680, 215, 100, 25), colToPlaceStr, 1);

        if (GUI.Button(new Rect(530, 250, 100, 25), "Find place"))
        {
            rowToPlace = int.Parse(colToPlaceStr);
            colToPlace = int.Parse(rowToPlaceStr);
            CheckIsInMatrix(figureType, rowToPlace, colToPlace, matrix);
            if (!isOutOfTheMatrix)
            {
                OccupyCells(figureType, rowToPlace, colToPlace, matrix);
            }
                
        }

        if (isOutOfTheMatrix)
        {
            GUI.Box(new Rect(620, 310, 15, 15), string.Empty);
            GUI.Box(new Rect(650, 290, 20, 20), string.Empty);
            GUI.Box(new Rect(680, 290, 100, 100), "This \ncoords \nare out of my \nfield. \nTry again..");
            if (GUI.Button(new Rect(755, 290, 25, 25), "X"))
            {
                isOutOfTheMatrix = false;
            }
        }

        if (!isCorrectFigure)
        {
            GUI.Box(new Rect(620, 310, 15, 15), string.Empty);
            GUI.Box(new Rect(650, 290, 20, 20), string.Empty);
            GUI.Box(new Rect(680, 290, 100, 100), "This \nfigure \nis not one of \n these I use. \nTry again..");
            if (GUI.Button(new Rect(755, 290, 25, 25), "X"))
            {
                isCorrectFigure = true;
            }
        }

        if (GUI.Button(new Rect(680, 250, 100, 25), "Clear All"))
        {
            FillCellMatrix(matrix);
        }

        for (int i = 0, len = matrix.GetLength(0); i < len; i += 1)
        {
            for (int j = 0; j < len; j += 1)
            {
                if (!matrix[i, j].isFree)//cell is not free so we have to put figure in it
                {
                    GUI.Box(new Rect(i * 50 + 10, j * 50 + 70, 50, 50), string.Empty);
                    //Debug.Log("row: " + i + "  col: " + j);
                }
            }
        }

    }

    private void CheckIsInMatrix(string figureType, int rowToPlace, int colToPlace, Cell[,] matrix)
    {
        switch (figureType)
        {
            case "ninetile":
                if ((rowToPlace < 1) || (rowToPlace > matrix.GetLength(0) - 2) || (colToPlace < 1) || (colToPlace > matrix.GetLength(1) - 2))
                {
                    isOutOfTheMatrix = true;
                }
                break;
            case "plus":
                if ((rowToPlace < 1) || (rowToPlace > matrix.GetLength(0) - 2) || (colToPlace < 1) || (colToPlace > matrix.GetLength(1) - 2))
                {
                    isOutOfTheMatrix = true;
                }
                break;
            case "hline":
                if ((rowToPlace < 1) || (rowToPlace > matrix.GetLength(0) - 2) || (colToPlace < 0) || (colToPlace > matrix.GetLength(1) - 1))
                {
                    isOutOfTheMatrix = true;
                }
                break;
            case "vline":
                if ((rowToPlace < 0) || (rowToPlace > matrix.GetLength(0) - 1) || (colToPlace < 1) || (colToPlace > matrix.GetLength(1) - 2))
                {
                    isOutOfTheMatrix = true;
                }
                break;
            case "angle-ur":
                if ((rowToPlace < 1) || (rowToPlace > matrix.GetLength(0) - 1) || (colToPlace < 0) || (colToPlace > matrix.GetLength(1) - 2))
                {
                    isOutOfTheMatrix = true;
                }
                break;
            case "angle-dr":
                if ((rowToPlace < 0) || (rowToPlace > matrix.GetLength(0) - 2) || (colToPlace < 0) || (colToPlace > matrix.GetLength(1) - 2))
                {
                    isOutOfTheMatrix = true;
                }
                break;
            case "angle-dl":
                if ((rowToPlace < 0) || (rowToPlace > matrix.GetLength(0) - 2) || (colToPlace < 1) || (colToPlace > matrix.GetLength(1) - 1))
                {
                    isOutOfTheMatrix = true;
                }
                break;
            case "angle-ul":
                if ((rowToPlace < 1) || (rowToPlace > matrix.GetLength(0) - 1) || (colToPlace < 1) || (colToPlace > matrix.GetLength(1) - 1))
                {
                    isOutOfTheMatrix = true;
                }
                break;
            default:
                isCorrectFigure = false;
                break;
        }
    }

    ///<summary>
    /// Split the command and get the current position of the figure and its kind.
    /// Find the nearest free place in the matrix to put the figure.
    /// </summary>
    /// <param name="currentCommand">Command on the current input line.</param>
    public static void OccupyCells(string figureType, int rowToPlace, int colToPlace, Cell[,] matrix)
    {
        bestRow = rowToPlace;//just to give some value to the fields
        bestCol = colToPlace;


        MakeVisitedCellsInTheCurrentMethodUnvisited(matrix);
        currentVisitedCells.Clear(); //restart visited in previous recursion cells

        bestLength = int.MaxValue; //restart for every new figure
        switch (figureType)
        {
            case "ninetile":
                FillNinetile(matrix, rowToPlace, colToPlace);
                matrix[bestRow, bestCol].isFree = false;

                matrix[bestRow + 1, bestCol].isFree = false;
                matrix[bestRow - 1, bestCol].isFree = false;
                matrix[bestRow, bestCol + 1].isFree = false;
                matrix[bestRow, bestCol - 1].isFree = false;

                matrix[bestRow + 1, bestCol - 1].isFree = false;
                matrix[bestRow + 1, bestCol + 1].isFree = false;
                matrix[bestRow - 1, bestCol - 1].isFree = false;
                matrix[bestRow - 1, bestCol + 1].isFree = false;

                break;
            case "plus":
                FillPlus(matrix, rowToPlace, colToPlace);
                matrix[bestRow, bestCol].isFree = false;
                matrix[bestRow + 1, bestCol].isFree = false;
                matrix[bestRow - 1, bestCol].isFree = false;
                matrix[bestRow, bestCol + 1].isFree = false;
                matrix[bestRow, bestCol - 1].isFree = false;

                break;
            case "hline":
                FillHline(matrix, rowToPlace, colToPlace);
                matrix[bestRow, bestCol].isFree = false;
                matrix[bestRow + 1, bestCol].isFree = false;
                matrix[bestRow - 1, bestCol].isFree = false;

                break;
            case "vline":
                FillVline(matrix, rowToPlace, colToPlace);
                matrix[bestRow, bestCol].isFree = false;
                matrix[bestRow, bestCol - 1].isFree = false;
                matrix[bestRow, bestCol + 1].isFree = false;
                break;
            case "angle-ur":
                FillAngleUr(matrix, rowToPlace, colToPlace);
                matrix[bestRow, bestCol].isFree = false;
                matrix[bestRow, bestCol + 1].isFree = false;
                matrix[bestRow - 1, bestCol].isFree = false;

                break;
            case "angle-dr":
                FillAngleDr(matrix, rowToPlace, colToPlace);
                matrix[bestRow, bestCol].isFree = false;
                matrix[bestRow, bestCol + 1].isFree = false;
                matrix[bestRow + 1, bestCol].isFree = false;

                break;
            case "angle-dl":
                FillAngleDL(matrix, rowToPlace, colToPlace);
                matrix[bestRow, bestCol].isFree = false;
                matrix[bestRow, bestCol - 1].isFree = false;
                matrix[bestRow + 1, bestCol].isFree = false;

                break;
            case "angle-ul":
                FillAngleUl(matrix, rowToPlace, colToPlace);
                matrix[bestRow, bestCol].isFree = false;
                matrix[bestRow, bestCol - 1].isFree = false;
                matrix[bestRow - 1, bestCol].isFree = false;

                break;
            default: 
                isCorrectFigure = false;
                break;
        }
    }

    /// <summary>
    /// Find(recursively) the shortest empty place for the new figure angleUL and save it corrdinates in global field
    /// bestRow and bestCol
    /// </summary>
    /// <param name="matrix">the matrix field where the figures are placed</param>
    /// <param name="startRow">row cordinates of the new figure</param>
    /// <param name="startCol">col cordinates of the new figure</param>
    public static void FillAngleUl(Cell[,] matrix, int startRow, int startCol)
    {

        if ((startRow < 1) || (startRow > matrix.GetLength(0) - 1) || (startCol < 1) || (startCol > matrix.GetLength(1) - 1)) //borders of the matrix for angleUL
        {
            return;
        }

        if (matrix[startRow, startCol].isVisited) //if cell is visited return
        {
            return;
        }
        else
        {
            matrix[startRow, startCol].isVisited = true;

            currentVisitedCells.Add(new KeyValuePair<int, int>(startRow, startCol));
        }

        if (matrix[startRow, startCol].isFree && matrix[startRow, startCol - 1].isFree && matrix[startRow - 1, startCol].isFree)//if empty place found
        {
            int currenrBestRow = startRow;
            int currentBestCol = startCol;

            int currentLength = Math.Abs(startRow - rowToPlace) + Math.Abs(startCol - colToPlace);

            if (currentLength < bestLength) //fix only if current length is shorter than the shortest(bestLength)
            {
                bestLength = currentLength;
                bestRow = currenrBestRow;
                bestCol = currentBestCol;
            }
            return;
        }

        FillAngleUl(matrix, startRow + 1, startCol); //down direction

        FillAngleUl(matrix, startRow - 1, startCol); //up direction

        FillAngleUl(matrix, startRow, startCol - 1); //left direction

        FillAngleUl(matrix, startRow, startCol + 1); //right direction
    }

    private static void FillAngleDL(Cell[,] matrix, int startRow, int startCol)
    {
        if ((startRow < 0) || (startRow > matrix.GetLength(0) - 2) || (startCol < 1) || (startCol > matrix.GetLength(1) - 1)) //borders
        {
            return;
        }

        if (matrix[startRow, startCol].isVisited) //if cell is visited return
        {
            return;
        }
        else
        {
            matrix[startRow, startCol].isVisited = true;

            currentVisitedCells.Add(new KeyValuePair<int, int>(startRow, startCol));
        }

        if (matrix[startRow, startCol].isFree && matrix[startRow, startCol - 1].isFree && matrix[startRow + 1, startCol].isFree)//if empty place found
        {
            int currentBestRow = startRow;
            int currentBestCol = startCol;

            int currentLength = Math.Abs(startRow - rowToPlace) + Math.Abs(startCol - colToPlace);

            if (currentLength < bestLength) //fix only if current length is shorter than the shortest(bestLength)
            {
                bestLength = currentLength;
                bestRow = currentBestRow;
                bestCol = currentBestCol;
            }
            return;
        }

        FillAngleDL(matrix, startRow + 1, startCol); //down direction

        FillAngleDL(matrix, startRow - 1, startCol); //up direction

        FillAngleDL(matrix, startRow, startCol - 1); //left direction

        FillAngleDL(matrix, startRow, startCol + 1); //right direction
    }

    private static void FillAngleDr(Cell[,] matrix, int startRow, int startCol)
    {
        if ((startRow < 0) || (startRow > matrix.GetLength(0) - 2) || (startCol < 0) || (startCol > matrix.GetLength(1) - 2)) //borders
        {
            return;
        }

        if (matrix[startRow, startCol].isVisited) //if cell is visited return
        {
            return;
        }
        else
        {
            matrix[startRow, startCol].isVisited = true;

            currentVisitedCells.Add(new KeyValuePair<int, int>(startRow, startCol));
        }

        if (matrix[startRow, startCol].isFree && matrix[startRow, startCol + 1].isFree && matrix[startRow + 1, startCol].isFree)//if empty place found
        {
            int currenrBestRow = startRow;
            int currentBestCol = startCol;

            int currentLength = Math.Abs(startRow - rowToPlace) + Math.Abs(startCol - colToPlace);

            if (currentLength < bestLength) //fix only if current length is shorter than the shortest(bestLength)
            {
                bestLength = currentLength;
                bestRow = currenrBestRow;
                bestCol = currentBestCol;
            }
            return;
        }

        FillAngleDr(matrix, startRow + 1, startCol); //down direction

        FillAngleDr(matrix, startRow - 1, startCol); //up direction

        FillAngleDr(matrix, startRow, startCol - 1); //left direction

        FillAngleDr(matrix, startRow, startCol + 1); //right direction
    }

    private static void FillAngleUr(Cell[,] matrix, int startRow, int startCol)
    {
        if ((startRow < 1) || (startRow > matrix.GetLength(0) - 1) || (startCol < 0) || (startCol > matrix.GetLength(1) - 2)) //borders
        {
            return;
        }

        if (matrix[startRow, startCol].isVisited) //if cell is visited return
        {
            return;
        }
        else
        {
            matrix[startRow, startCol].isVisited = true;

            currentVisitedCells.Add(new KeyValuePair<int, int>(startRow, startCol));
        }

        if (matrix[startRow, startCol].isFree && matrix[startRow, startCol + 1].isFree && matrix[startRow - 1, startCol].isFree)//if empty place found
        {
            int currenrBestRow = startRow;
            int currentBestCol = startCol;

            int currentLength = Math.Abs(startRow - rowToPlace) + Math.Abs(startCol - colToPlace);

            if (currentLength < bestLength) //fix only if current length is shorter than the shortest(bestLength)
            {
                bestLength = currentLength;
                bestRow = currenrBestRow;
                bestCol = currentBestCol;
            }
            return;
        }

        FillAngleUr(matrix, startRow + 1, startCol); //down direction

        FillAngleUr(matrix, startRow - 1, startCol); //up direction

        FillAngleUr(matrix, startRow, startCol - 1); //left direction

        FillAngleUr(matrix, startRow, startCol + 1); //right direction
    }

    private static void FillVline(Cell[,] matrix, int startRow, int startCol)
    {
        if ((startRow < 0) || (startRow > matrix.GetLength(0) - 1) || (startCol < 1) || (startCol > matrix.GetLength(1) - 2)) //borders
        {
            return;
        }

        if (matrix[startRow, startCol].isVisited) //if cell is visited return
        {
            return;
        }
        else
        {
            matrix[startRow, startCol].isVisited = true;

            currentVisitedCells.Add(new KeyValuePair<int, int>(startRow, startCol));
        }

        if (matrix[startRow, startCol].isFree && matrix[startRow, startCol - 1].isFree && matrix[startRow, startCol + 1].isFree)//if empty place found
        {
            int currenrBestRow = startRow;
            int currentBestCol = startCol;

            int currentLength = Math.Abs(startRow - rowToPlace) + Math.Abs(startCol - colToPlace);

            if (currentLength < bestLength) //fix only if current length is shorter than the shortest(bestLength)
            {
                bestLength = currentLength;
                bestRow = currenrBestRow;
                bestCol = currentBestCol;
            }
            return;
        }

        FillVline(matrix, startRow + 1, startCol); //down direction

        FillVline(matrix, startRow - 1, startCol); //up direction

        FillVline(matrix, startRow, startCol - 1); //left direction

        FillVline(matrix, startRow, startCol + 1); //right direction
    }

    private static void FillHline(Cell[,] matrix, int startRow, int startCol)
    {
        if ((startRow < 1) || (startRow > matrix.GetLength(0) - 2) || (startCol < 0) || (startCol > matrix.GetLength(1) - 1)) //borders
        {
            return;
        }

        if (matrix[startRow, startCol].isVisited) //if cell is visited return
        {
            return;
        }
        else
        {
            matrix[startRow, startCol].isVisited = true;

            currentVisitedCells.Add(new KeyValuePair<int, int>(startRow, startCol));
        }

        if (matrix[startRow, startCol].isFree && matrix[startRow + 1, startCol].isFree && matrix[startRow - 1, startCol].isFree)//if empty place found
        {
            int currenrBestRow = startRow;
            int currentBestCol = startCol;

            int currentLength = Math.Abs(startRow - rowToPlace) + Math.Abs(startCol - colToPlace);

            if (currentLength < bestLength) //fix only if current length is shorter than the shortest(bestLength)
            {
                bestLength = currentLength;
                bestRow = currenrBestRow;
                bestCol = currentBestCol;
            }
            return;
        }

        FillHline(matrix, startRow + 1, startCol); //down direction

        FillHline(matrix, startRow - 1, startCol); //up direction

        FillHline(matrix, startRow, startCol - 1); //left direction

        FillHline(matrix, startRow, startCol + 1); //right direction
    }

    private static void FillPlus(Cell[,] matrix, int startRow, int startCol)
    {
        if ((startRow < 1) || (startRow > matrix.GetLength(0) - 2) || (startCol < 1) || (startCol > matrix.GetLength(1) - 2)) //borders
        {
            return;
        }

        if (matrix[startRow, startCol].isVisited) //if cell is visited return
        {
            return;
        }
        else
        {
            matrix[startRow, startCol].isVisited = true;

            currentVisitedCells.Add(new KeyValuePair<int, int>(startRow, startCol));
        }

        if (matrix[startRow, startCol].isFree && matrix[startRow + 1, startCol].isFree && matrix[startRow - 1, startCol].isFree
            && matrix[startRow, startCol + 1].isFree && matrix[startRow, startCol - 1].isFree)//if empty place found
        {
            int currenrBestRow = startRow;
            int currentBestCol = startCol;

            int currentLength = Math.Abs(startRow - rowToPlace) + Math.Abs(startCol - colToPlace);

            if (currentLength < bestLength) //fix only if current length is shorter than the shortest(bestLength)
            {
                bestLength = currentLength;
                bestRow = currenrBestRow;
                bestCol = currentBestCol;
            }
            return;
        }

        FillPlus(matrix, startRow + 1, startCol); //down direction

        FillPlus(matrix, startRow - 1, startCol); //up direction

        FillPlus(matrix, startRow, startCol - 1); //left direction

        FillPlus(matrix, startRow, startCol + 1); //right direction
    }

    private static void FillNinetile(Cell[,] matrix, int startRow, int startCol)
    {
        if ((startRow < 1) || (startRow > matrix.GetLength(0) - 2) || (startCol < 1) || (startCol > matrix.GetLength(1) - 2)) //borders
        {
            return;
        }

        if (matrix[startRow, startCol].isVisited) //if cell is visited return
        {
            return;
        }
        else
        {
            matrix[startRow, startCol].isVisited = true;

            currentVisitedCells.Add(new KeyValuePair<int, int>(startRow, startCol));
        }

        if (matrix[startRow, startCol].isFree && matrix[startRow + 1, startCol].isFree && matrix[startRow - 1, startCol].isFree
                             && matrix[startRow, startCol + 1].isFree && matrix[startRow, startCol - 1].isFree
                             && matrix[startRow + 1, startCol + 1].isFree && matrix[startRow + 1, startCol - 1].isFree
                             && matrix[startRow - 1, startCol + 1].isFree && matrix[startRow - 1, startCol - 1].isFree)//if empty place found
        {
            int currenrBestRow = startRow;
            int currentBestCol = startCol;

            int currentLength = Math.Abs(startRow - rowToPlace) + Math.Abs(startCol - colToPlace);

            if (currentLength < bestLength) //fix only if current length is shorter than the shortest(bestLength)
            {
                bestLength = currentLength;
                bestRow = currenrBestRow;
                bestCol = currentBestCol;
            }
            return;
        }

        FillNinetile(matrix, startRow + 1, startCol); //down direction

        FillNinetile(matrix, startRow - 1, startCol); //up direction

        FillNinetile(matrix, startRow, startCol - 1); //left direction

        FillNinetile(matrix, startRow, startCol + 1); //right direction
    }

    public static void FillCellMatrix(Cell[,] matrix)
    {
        for (int i = 0, len = matrix.GetLength(0); i < len; i += 1)
        {
            for (int j = 0; j < len; j += 1)
            {
                matrix[i, j] = new Cell();
            }
        }
    }

    public static void MakeVisitedCellsInTheCurrentMethodUnvisited(Cell[,] matrix)
    {
        foreach (var cell in currentVisitedCells)
        {
            matrix[cell.Key, cell.Value].isVisited = false;
        }
    }
}
