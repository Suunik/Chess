using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    private int PieceNumber = 2;
    Knight()
    {
        movementMatrix = new int[,]{ {1, 2}, { 2, 1}, { -1, 2}, { 1, -2}, { -1, -2}, { -2, -1}, { 2, -1}, { -2, 1} };
    }

    public override void allInBoundsMoves()
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

    public override void FindAvailableMoves()
    {
        allInBoundsMoves();
    }

    public void Update()
    {
        FindAvailableMoves();
        PieceMovement();
    }
}