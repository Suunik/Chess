using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    King()
    {
        movementMatrix = new int[,] { { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 } };
        pieceLetter = (team == 1) ? 'K' : 'k';
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
        //remember all enemy attack squares
        List<Square> enemyAttackSquares = new List<Square>();
        //remember what moves should be deleted
        List<Square> movesToDelete = new List<Square>();

        //remove king from currentsquare
        currentSquare.team = 0;
        
        foreach (Square kingMove in availableMoves)
        {
            //Save squares current piece
            int previousTeam = kingMove.team;
            //Place king on new square
            kingMove.team = team;
            //save enemy moves list while king is placed on new square
            enemyAttackSquares.AddRange(Chessboard.instance.allTeamCoveredSquares(-team));
            //remember all squares where king cannot be
            if (enemyAttackSquares.Contains(kingMove))
            {
                movesToDelete.Add(kingMove);
            }
            //return previous piece on square
            kingMove.team = previousTeam;
        }
        //remove all the moves where king cannot go
        foreach (Square move in movesToDelete)
        {
            availableMoves.Remove(move);
        }
        //return king to original position
        currentSquare.team = team;
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