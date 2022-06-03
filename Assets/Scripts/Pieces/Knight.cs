using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    Knight()
    {
        movementMatrix = new int[,]{ {1, 2}, { 2, 1}, { -1, 2}, { 1, -2}, { -1, -2}, { -2, -1}, { 2, -1}, { -2, 1} };
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

    public override List<Square> FindAvailableMoves()
    {
        findAllInboundsAndNoCollisionMoves();
        return availableMoves;
    }
}