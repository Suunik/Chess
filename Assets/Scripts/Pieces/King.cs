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
    public List<Square> findCastleMoves()
    {
 
        List<Square> result = new List<Square>();
        int king_row = currentSquare.ReturnSquare()[0]- 97;
        int king_column = (team == 1) ? 0 : 7;
        //checks if it's kings first move
        if (firstMove)
        {
            //save the rook that hasnt moved yet
            List<ChessPiece> rooks_found = Chessboard.instance.findAvailableRookForCastleing();
            //loop through every not moved rook we found
            foreach (ChessPiece rook in rooks_found)
            {
                //remember the rooks row we are currently testing
                int rook_row = rook.currentSquare.ReturnSquare()[0] - 97;
                //check if the rook we found is in our team
                if (rook.team == team)
                {
                    //find out on which side the rook is positioned from the king
                    if ((king_row + 3) == rook_row)
                    {
                        //check if path to the rook is clear from pieces
                        if (Chessboard.instance.squares[king_row + 1, king_column].team == 0)
                        {
                            if (Chessboard.instance.squares[king_row + 2, king_column].team == 0)
                            {
                                result.Add(Chessboard.instance.squares[king_row + 2, king_column]);
                                Chessboard.instance.castleSquare.Add("" + (char)(king_row + 97 + 2) + (char)(king_column + 49));
                            }
                        }
                    }
                    //find out on which side the rook is positioned from the king
                    if (king_row - 4 == rook_row)
                    {
                        //check if path to the rook is clear from other pieces
                        if (Chessboard.instance.squares[king_row - 1, king_column].team == 0)
                        {
                            if (Chessboard.instance.squares[king_row - 2, king_column].team == 0)
                            {
                                if (Chessboard.instance.squares[king_row - 3, king_column].team == 0)
                                {
                                    result.Add(Chessboard.instance.squares[king_row - 2, king_column]);
                                    Chessboard.instance.castleSquare.Add("" + (char)(king_row + 97 - 2) + (char)(king_column + 49));
                                }
                            }
                        }
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
        availableMoves.AddRange(findCastleMoves());
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