using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    Pawn()
    {
        firstMove = true;
    }

    public override List<Square> findAllInboundsAndNoCollisionMoves()
    {
        int row = ReturnRowColumn()[0];
        int column = ReturnRowColumn()[1];
        List<Square> result = new List<Square>();

        if (WithinBounds(row, column + team))
        {
            if (Chessboard.instance.squares[row, column + team].team == 0)
            {
                if (!result.Contains(Chessboard.instance.squares[row, column + team]))
                {
                    result.Add(Chessboard.instance.squares[row, column + team]);
                }
            }

        }
        if (firstMove && WithinBounds(row, column + team*2))
        {
            if (Chessboard.instance.squares[row, column + team*2].team == 0)
            {
                if (!result.Contains(Chessboard.instance.squares[row, column + team*2]))
                {
                    result.Add(Chessboard.instance.squares[row, column + team*2]);
                }
            }
        }
        return result;
    }

    private List<Square> findPawnAttackSquares()
    {
        int row = ReturnRowColumn()[0];
        int column = ReturnRowColumn()[1];
        List<Square> result = new List<Square>();
        if (WithinBounds(row + team, column + team))
        {
            if (Chessboard.instance.squares[row + team, column + team].team == -team)
            {
                availableMoves.Add(Chessboard.instance.squares[row + team, column + team]);
            }
        }
        if (WithinBounds(row - team, column + team))
        {
            if (Chessboard.instance.squares[row - team, column + team].team == -team)
            {
                availableMoves.Add(Chessboard.instance.squares[row - team, column + team]);
            }
        }
        return result;
    }

    public override List<Square> FindAvailableMoves()
    {
        availableMoves.AddRange(findAllInboundsAndNoCollisionMoves());
        availableMoves.AddRange(findPawnAttackSquares());
        return availableMoves;
    }
    public override List<Square> findPieceAttackingMoves()
    {
        int row = ReturnRowColumn()[0];
        int column = ReturnRowColumn()[1];
        List<Square> result = new List<Square>();
        if (WithinBounds(row - team, column + team))
        {
            if (team == 1)
            {
                if (WithinBounds(row + team, column + team))
                {

                    result.Add(Chessboard.instance.squares[row + team, column + team]);

                }
                if (WithinBounds(row - team, column + team))
                {

                    result.Add(Chessboard.instance.squares[row - team, column + team]);

                }
            }
            else
            {
                if (WithinBounds(row + team, column + team))
                {

                    result.Add(Chessboard.instance.squares[row + team, column + team]);

                }
                if (WithinBounds(row - team, column + team))
                {

                    result.Add(Chessboard.instance.squares[row - team, column + team]);

                }
            }
        }
        return result;
    }
}
