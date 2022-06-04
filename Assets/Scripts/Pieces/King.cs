using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    King()
    {
        movementMatrix = new int[,] { { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 } };
    }

    public override List<Square> findAllInboundsAndNoCollisionMoves()
    {
        int row = ReturnRowColumn()[0];
        int column = ReturnRowColumn()[1];
        List<Square> result = new List<Square>();

        for (int i = 0; i < 8; i++)
        {
            if (WithinBounds(row + movementMatrix[i, 0], column + movementMatrix[i, 1]))
            {
                if (Chessboard.instance.squares[row + movementMatrix[i, 0], column + movementMatrix[i, 1]].team != team)
                {
                    if (!result.Contains(Chessboard.instance.squares[row + movementMatrix[i, 0], column + movementMatrix[i, 1]]))
                    {
                        result.Add(Chessboard.instance.squares[row + movementMatrix[i, 0], column + movementMatrix[i, 1]]);
                    }
                }

            }

        }
        return result;
    }

    public override void restrictMovements()
    {
        
        int testcounter = availableMoves.Count;
        if (team == 1)
        {
            for (int i = 0; i < testcounter;)
            {
                List<Square> enemyMoves = Chessboard.instance.allBlackMoves;
                //if availablemove gets deleted, everything gets shifted left
                if (enemyMoves.Contains(availableMoves[i]))
                {
                    availableMoves.Remove(availableMoves[i]);
                    testcounter--;
                }
                //if an availablemove does not get deleted, increment up
                else
                {
                    i++;
                }

            }
        }
        else
        {
            for (int i = 0; i < testcounter;)
            {
                List<Square> enemyMoves = Chessboard.instance.allWhiteMoves;
                //if availablemove gets deleted, everything gets shifted left
                if (enemyMoves.Contains(availableMoves[i]))
                {
                    availableMoves.Remove(availableMoves[i]);
                    testcounter--;
                }
                //if an availablemove does not get deleted, increment up
                else
                {
                    i++;
                }

            }
        }
        
    }

    public override List<Square> FindAvailableMoves()
    {
        availableMoves.AddRange(findAllInboundsAndNoCollisionMoves());
        if (team == 1)
        {
            Chessboard.instance.whiteKingSquare = currentSquare;
        }
        if (team == -1)
        {
            Chessboard.instance.blackKingSquare = currentSquare;
        }

        return availableMoves;
    }

}