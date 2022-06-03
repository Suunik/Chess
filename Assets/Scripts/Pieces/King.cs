using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    King()
    {
        movementMatrix = new int[,] { { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 } };
    }

    public override void findAllInboundsAndNoCollisionMoves()
    {
        int row = ReturnRowColumn()[0];
        int column = ReturnRowColumn()[1];

        for (int i = 0; i < 8; i++)
        {
            if (WithinBounds(row + movementMatrix[i, 0], column + movementMatrix[i, 1]))
            {
                if (Chessboard.instance.squares[row + movementMatrix[i, 0], column + movementMatrix[i, 1]].team != team)
                {
                    if (!availableMoves.Contains(Chessboard.instance.squares[row + movementMatrix[i, 0], column + movementMatrix[i, 1]]))
                    {
                        availableMoves.Add(Chessboard.instance.squares[row + movementMatrix[i, 0], column + movementMatrix[i, 1]]);
                    }
                }

            }

        }
    }

    public override void restrictMovements()
    {
        int testcounter = availableMoves.Count;
        if (team == 1)
        {
            for (int i = 0; i < testcounter;)
            {
                List<Square> enemyMoves = Chessboard.instance.allBlackMoves;
                Debug.Log("Check if array contains: " + availableMoves[i]);
                if (enemyMoves.Contains(availableMoves[i]))
                {
                    Debug.Log("Successful remove attempt: " + availableMoves[i]);
                    availableMoves.Remove(availableMoves[i]);
                    testcounter--;
                }
                else
                {
                    i++;
                }

            }
        }
        else
        {
            for (int i = 0; i < availableMoves.Count; i++)
            {
                List<Square> enemyMoves = Chessboard.instance.allWhiteMoves;
                if (enemyMoves.Contains(availableMoves[i]))
                {
                    availableMoves.Remove(availableMoves[i]);
                }
            }
        }

    }

    public override List<Square> FindAvailableMoves()
    {
        findAllInboundsAndNoCollisionMoves();
        return availableMoves;
    }

}