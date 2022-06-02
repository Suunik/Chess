using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    Rook()
    {
        pieceNumber = 1;
    }

    public override void allInBoundsMoves()
    {

        int row = ReturnRowColumn()[0];
        int column = ReturnRowColumn()[1];

        //Upwards
        for (int i = 1; i < 8; ++i)
        {   
            //Is the tile within the borders of the game
            if (WithinBounds(row + i, column))
            {
                //If team, then break. If enemy, then add and break.
                if (Chessboard.instance.squares[row + i, column].team != 0)
                {   
                    //Its your team
                    if (Chessboard.instance.squares[row + i, column].team == team)
                    {
                        break;
                    }
                    //Must be enemy
                    else
                    {
                        availableMoves.Add(Chessboard.instance.squares[row + i, column]);
                        break;
                    }
                }
                availableMoves.Add(Chessboard.instance.squares[row + i, column]);
            }    
        }

        //Downwards
        for (int i = -1; i > -8; --i)
        {
            //Is the tile within the borders of the game
            if (WithinBounds(row + i, column))
            {
                //If team, then break. If enemy, then add and break.
                if (Chessboard.instance.squares[row + i, column].team != 0)
                {
                    //Its your team
                    if (Chessboard.instance.squares[row + i, column].team == team)
                    {
                        break;
                    }
                    //Must be enemy
                    else
                    {
                        availableMoves.Add(Chessboard.instance.squares[row + i, column]);
                        break;
                    }
                }
                availableMoves.Add(Chessboard.instance.squares[row + i, column]);
            }
        }

        //To the right
        for (int i = 1; i < 8; ++i)
        {
            //Is the tile within the borders of the game
            if (WithinBounds(row, column + i))
            {
                //If team, then break. If enemy, then add and break.
                if (Chessboard.instance.squares[row, column + i].team != 0)
                {
                    //Its your team
                    if (Chessboard.instance.squares[row, column + i].team == team)
                    {
                        break;
                    }
                    //Must be enemy
                    else
                    {
                        availableMoves.Add(Chessboard.instance.squares[row, column + i]);
                        break;
                    }
                }
                availableMoves.Add(Chessboard.instance.squares[row, column + i]);
            }
        }

        //To the left
        for (int i = -1; i > -8; --i)
        {
            //Is the tile within the borders of the game
            if (WithinBounds(row, column + i))
            {
                //If team, then break. If enemy, then add and break.
                if (Chessboard.instance.squares[row, column + i].team != 0)
                {
                    //Its your team
                    if (Chessboard.instance.squares[row, column + i].team == team)
                    {
                        break;
                    }
                    //Must be enemy
                    else
                    {
                        availableMoves.Add(Chessboard.instance.squares[row, column + i]);
                        break;
                    }
                }
                availableMoves.Add(Chessboard.instance.squares[row, column + i]);
            }
        }
    }
    public override void FindAvailableMoves()
    {
        //All possible moves + collisions
        allInBoundsMoves();

    }
    
    void Update()
    {
        FindAvailableMoves();
        PieceMovement();
    }
}