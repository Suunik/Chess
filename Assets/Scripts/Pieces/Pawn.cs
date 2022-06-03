using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    Pawn()
    {
        firstMove = true;
    }

    public override void findAllInboundsAndNoCollisionMoves()
    {
        int row = ReturnRowColumn()[0];
        int column = ReturnRowColumn()[1];

        if (WithinBounds(row, column + team))
        {
            if (Chessboard.instance.squares[row, column + team].team == 0)
            {
                if (!availableMoves.Contains(Chessboard.instance.squares[row, column + team]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row, column + team]);
                }
            }

        }
        if (firstMove && WithinBounds(row, column + team))
        {
            if (Chessboard.instance.squares[row, column + team*2].team == 0)
            {
                if (!availableMoves.Contains(Chessboard.instance.squares[row, column + team*2]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row, column + team*2]);
                }
            }
        }
    }

    private void findPawnAttackSquares()
    {
        int row = ReturnRowColumn()[0];
        int column = ReturnRowColumn()[1];
        if (WithinBounds(row + team, column + team))
        {
            if (Chessboard.instance.squares[row + team, column + team].team == -team)
            {
                if (!availableMoves.Contains(Chessboard.instance.squares[row + team, column + team]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row + team, column + team]);
                }
            }
        }
        if (WithinBounds(row - team, column + team))
        {
            if (Chessboard.instance.squares[row - team, column + team].team == -team)
            {
                if (!availableMoves.Contains(Chessboard.instance.squares[row - team, column + team]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row - team, column + team]);
                }
            }
        }
    }

    public override List<Square> FindAvailableMoves()
    {
        findAllInboundsAndNoCollisionMoves();
        findPawnAttackSquares();
        return availableMoves;
    }
    public override void addPieceAttackingMovesToChessboard()
    {
        int row = ReturnRowColumn()[0];
        int column = ReturnRowColumn()[1];

        if (WithinBounds(row - team, column + team))
        {
            if (team == 1)
            {
                if (WithinBounds(row + team, column + team))
                {
                    if (!availableMoves.Contains(Chessboard.instance.squares[row - team, column + team]))
                    {
                        Chessboard.instance.allWhiteMoves.Add(Chessboard.instance.squares[row - team, column + team]);
                    }
                }
                if (WithinBounds(row - team, column + team))
                {
                    if (!availableMoves.Contains(Chessboard.instance.squares[row - team, column + team]))
                    {
                        Chessboard.instance.allWhiteMoves.Add(Chessboard.instance.squares[row - team, column + team]);
                    }
                }
            }
            else
            {
                if (WithinBounds(row + team, column + team))
                {
                    if (!availableMoves.Contains(Chessboard.instance.squares[row - team, column + team]))
                    {
                        Chessboard.instance.allBlackMoves.Add(Chessboard.instance.squares[row - team, column + team]);
                    }
                }
                if (WithinBounds(row - team, column + team))
                {
                    if (!availableMoves.Contains(Chessboard.instance.squares[row - team, column + team]))
                    {
                        Chessboard.instance.allBlackMoves.Add(Chessboard.instance.squares[row - team, column + team]);
                    }
                }
            }
        }
    }
}
