using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    public override List<Square> findAllInboundsAndNoCollisionMoves()
    {
        int row = ReturnRowColumn()[0];
        int column = ReturnRowColumn()[1];
        List<Square> result = new List<Square>();
        //to the right and down
        for (int i = 1; i < 8; ++i)
        {   
            //Is the tile within the borders of the game
            if (WithinBounds(row - i, column + i))
            {
                //If team, then break. If enemy, then add and break.
                if (Chessboard.instance.squares[row - i, column + i].team != 0)
                {   
                    //Its your team
                    if (Chessboard.instance.squares[row - i, column + i].team == team)
                    {
                        break;
                    }
                    //Must be enemy
                    else
                    {
                        if (!result.Contains(Chessboard.instance.squares[row - i, column + i]))
                        {
                            result.Add(Chessboard.instance.squares[row - i, column + i]);
                        }
                        break;
                    }
                }
                if (!result.Contains(Chessboard.instance.squares[row - i, column + i]))
                {
                    result.Add(Chessboard.instance.squares[row - i, column + i]);
                }
                
            }    
        }

        //To the left and up
        for (int i = -1; i > -8; --i)
        {
            //Is the tile within the borders of the game
            if (WithinBounds(row - i, column + i))
            {
                //If team, then break. If enemy, then add and break.
                if (Chessboard.instance.squares[row - i, column + i].team != 0)
                {
                    //Its your team
                    if (Chessboard.instance.squares[row - i, column + i].team == team)
                    {
                        break;
                    }
                    //Must be enemy
                    else
                    {
                        if (!result.Contains(Chessboard.instance.squares[row - i, column + i]))
                        {
                            result.Add(Chessboard.instance.squares[row - i, column + i]);
                        }
                        break;
                    }
                }
                if (!result.Contains(Chessboard.instance.squares[row - i, column + i]))
                {
                    result.Add(Chessboard.instance.squares[row - i, column + i]);
                }
            }
        }

        //To the right and up
        for (int i = 1; i < 8; ++i)
        {
            //Is the tile within the borders of the game
            if (WithinBounds(row + i, column + i))
            {
                //If team, then break. If enemy, then add and break.
                if (Chessboard.instance.squares[row + i, column + i].team != 0)
                {
                    //Its your team
                    if (Chessboard.instance.squares[row + i, column + i].team == team)
                    {
                        break;
                    }
                    //Must be enemy
                    else
                    {
                        if (!result.Contains(Chessboard.instance.squares[row + i, column + i]))
                        {
                            result.Add(Chessboard.instance.squares[row + i, column + i]);
                        }
                        break;
                    }
                }
                if (!result.Contains(Chessboard.instance.squares[row + i, column+i]))
                {
                    result.Add(Chessboard.instance.squares[row + i, column+i]);
                }
            }
        }

        //To the left and down
        for (int i = -1; i > -8; --i)
        {
            //Is the tile within the borders of the game
            if (WithinBounds(row + i, column + i))
            {
                //If team, then break. If enemy, then add and break.
                if (Chessboard.instance.squares[row + i, column + i].team != 0)
                {
                    //Its your team
                    if (Chessboard.instance.squares[row + i, column + i].team == team)
                    {
                        break;
                    }
                    //Must be enemy
                    else
                    {
                        if (!result.Contains(Chessboard.instance.squares[row + i, column + i]))
                        {
                            result.Add(Chessboard.instance.squares[row + i, column + i]);
                        }
                        break;
                    }
                }
                if (!result.Contains(Chessboard.instance.squares[row + i, column + i]))
                {
                    result.Add(Chessboard.instance.squares[row + i, column + i]);
                }
            }
        }
        return result;
    }

    public override List<Square> FindAvailableMoves()
    {
        availableMoves.AddRange(findAllInboundsAndNoCollisionMoves());
        return availableMoves;
    }
}
