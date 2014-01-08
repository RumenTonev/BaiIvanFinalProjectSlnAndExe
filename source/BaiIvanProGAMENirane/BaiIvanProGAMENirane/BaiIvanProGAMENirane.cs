
//-----------------------------------------DEV TEAM --------------------------------------//
//-----Nikolay Zhelyazkov Zhelyazkov -------------------------- zhelyazkovn@gmailcom ------//
//-----Rumen Kirchev Tonev ------------------------------------ Rumen.tonev@yahoo.com -----//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace BaiIvanProGAMENirane
{
    class BaiIvanProGAMENirane
    {
        /// <summary>
        /// here comes the coordinates for row of the new figure
        /// </summary>
        public static int rowToPlace;

        /// <summary>
        /// here comes the coordin ates for col of the new figure
        /// </summary>
        public static int colToPlace;

        /// <summary>
        /// shortest length between the empty cells
        /// </summary>
        public static int bestLength;

        /// <summary>
        /// bestRow is the row in wich the new figure is going to be placed. It is the nearest empty place.
        /// If no such place it returns -1.
        /// </summary>
        public static int bestRow;

        /// <summary>
        /// bestCol is the col in wich the new figure is going to be placed. It is the nearest empty place.
        /// If no such place it returns -1.
        /// </summary>
        public static int bestCol;

        /// <summary>
        /// cells that are visited in recursion. We use this because we need to clean the cells for the next figure
        /// </summary>
        public static List<KeyValuePair<int, int>> currentVisitedCells = new List<KeyValuePair<int, int>>();

        static void Main()
        {
            Cell[,] matrix = new Cell[500, 500];

            FillCellMatrix(matrix);

            //-------------------console input for new figures-------------//

            ushort n = ushort.Parse(Console.ReadLine()); //read the number of the figures that will be passed
            string[] currentCommand;

            for (int i = 0; i < n; i++)// input the figures and their possitions
            {
                currentCommand = Console.ReadLine().Split(' ');
                OccupyCells(currentCommand, matrix);
            }
        }

        ///<summary>
        /// Split the command and get the current position of the figure and its kind.
        /// Find the nearest free place in the matrix to put the figure.
        /// </summary>
        /// <param name="currentCommand">Command on the current input line.</param>
        public static void OccupyCells(string[] currentCommand, Cell[,] matrix)
        {
            rowToPlace = int.Parse(currentCommand[1]);
            colToPlace = int.Parse(currentCommand[2]);

            bestRow = rowToPlace;//just to give some value to the fields
            bestCol = colToPlace;


            MakeVisitedCellsInTheCurrentMethodUnvisited(matrix);
            currentVisitedCells.Clear(); //restart visited in previous recursion cells

            bestLength = int.MaxValue; //restart for every new figure
            switch (currentCommand[0])
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

                    Console.WriteLine(bestRow + " " + bestCol); //output
                    break;
                case "plus":
                    FillPlus(matrix, rowToPlace, colToPlace);
                    matrix[bestRow, bestCol].isFree = false;
                    matrix[bestRow + 1, bestCol].isFree = false;
                    matrix[bestRow - 1, bestCol].isFree = false;
                    matrix[bestRow, bestCol + 1].isFree = false;
                    matrix[bestRow, bestCol - 1].isFree = false;

                    Console.WriteLine(bestRow + " " + bestCol); //output
                    break;
                case "hline":
                    FillHline(matrix, rowToPlace, colToPlace);
                    matrix[bestRow, bestCol].isFree = false;
                    matrix[bestRow + 1, bestCol].isFree = false;
                    matrix[bestRow - 1, bestCol].isFree = false;
                    Console.WriteLine(bestRow + " " + bestCol); //output
                    break;
                case "vline":
                    FillVline(matrix, rowToPlace, colToPlace);
                    matrix[bestRow, bestCol].isFree = false;
                    matrix[bestRow, bestCol - 1].isFree = false;
                    matrix[bestRow, bestCol + 1].isFree = false;
                    Console.WriteLine(bestRow + " " + bestCol); //output
                    break;
                case "angle-ur":
                    FillAngleUr(matrix, rowToPlace, colToPlace);
                    matrix[bestRow, bestCol].isFree = false;
                    matrix[bestRow, bestCol + 1].isFree = false;
                    matrix[bestRow - 1, bestCol].isFree = false;
                    Console.WriteLine(bestRow + " " + bestCol); //output
                    break;
                case "angle-dr":
                    FillAngleDr(matrix, rowToPlace, colToPlace);
                    matrix[bestRow, bestCol].isFree = false;
                    matrix[bestRow, bestCol + 1].isFree = false;
                    matrix[bestRow + 1, bestCol].isFree = false;
                    Console.WriteLine(bestRow + " " + bestCol); //output
                    break;
                case "angle-dl":
                    FillAngleDL(matrix, rowToPlace, colToPlace);
                    matrix[bestRow, bestCol].isFree = false;
                    matrix[bestRow, bestCol - 1].isFree = false;
                    matrix[bestRow + 1, bestCol].isFree = false;
                    Console.WriteLine(bestRow + " " + bestCol); //output
                    break;
                case "angle-ul":
                    FillAngleUl(matrix, rowToPlace, colToPlace);
                    matrix[bestRow, bestCol].isFree = false;
                    matrix[bestRow, bestCol - 1].isFree = false;
                    matrix[bestRow - 1, bestCol].isFree = false;
                    Console.WriteLine(bestRow + " " + bestCol); //output
                    break;
                default:
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
}
