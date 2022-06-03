using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public override void findAllInboundsAndNoCollisionMoves()
    {
        int row = ReturnRowColumn()[0];
        int column = ReturnRowColumn()[1];

        //Bishop moves
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
                        if (!availableMoves.Contains(Chessboard.instance.squares[row - i, column + i]))
                        {
                            availableMoves.Add(Chessboard.instance.squares[row - i, column + i]);
                        }
                        break;
                    }
                }
                if (!availableMoves.Contains(Chessboard.instance.squares[row - i, column + i]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row - i, column + i]);
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
                        if (!availableMoves.Contains(Chessboard.instance.squares[row - i, column + i]))
                        {
                            availableMoves.Add(Chessboard.instance.squares[row - i, column + i]);
                        }
                        break;
                    }
                }
                if (!availableMoves.Contains(Chessboard.instance.squares[row - i, column + i]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row - i, column + i]);
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
                        if (!availableMoves.Contains(Chessboard.instance.squares[row + i, column + i]))
                        {
                            availableMoves.Add(Chessboard.instance.squares[row + i, column + i]);
                        }
                        break;
                    }
                }
                if (!availableMoves.Contains(Chessboard.instance.squares[row + i, column + i]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row + i, column + i]);
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
                        if (!availableMoves.Contains(Chessboard.instance.squares[row + i, column + i]))
                        {
                            availableMoves.Add(Chessboard.instance.squares[row + i, column + i]);
                        }
                        break;
                    }
                }
                if (!availableMoves.Contains(Chessboard.instance.squares[row + i, column + i]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row + i, column + i]);
                }
            }
        }

        //Rook moves:
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
                        if (!availableMoves.Contains(Chessboard.instance.squares[row + i, column]))
                        {
                            availableMoves.Add(Chessboard.instance.squares[row + i, column]);
                        }
                        break;
                    }
                }
                if (!availableMoves.Contains(Chessboard.instance.squares[row + i, column]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row + i, column]);
                }
                
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
                        if (!availableMoves.Contains(Chessboard.instance.squares[row + i, column]))
                        {
                            availableMoves.Add(Chessboard.instance.squares[row + i, column]);
                        }
                        break;
                    }
                }
                if (!availableMoves.Contains(Chessboard.instance.squares[row + i, column]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row + i, column]);
                }
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
                        if (!availableMoves.Contains(Chessboard.instance.squares[row, column + i]))
                        {
                            availableMoves.Add(Chessboard.instance.squares[row, column + i]);
                        }
                        break;
                    }
                }
                if (!availableMoves.Contains(Chessboard.instance.squares[row, column+i]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row, column+i]);
                }
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
                        if (!availableMoves.Contains(Chessboard.instance.squares[row, column + i]))
                        {
                            availableMoves.Add(Chessboard.instance.squares[row, column + i]);
                        }
                        break;
                    }
                }
                if (!availableMoves.Contains(Chessboard.instance.squares[row, column + i]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row, column + i]);
                }
            }
        }
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
                        if (!availableMoves.Contains(Chessboard.instance.squares[row + i, column]))
                        {
                            availableMoves.Add(Chessboard.instance.squares[row + i, column]);
                        }
                        break;
                    }
                }
                if (!availableMoves.Contains(Chessboard.instance.squares[row + i, column]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row + i, column]);
                }

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
                        if (!availableMoves.Contains(Chessboard.instance.squares[row + i, column]))
                        {
                            availableMoves.Add(Chessboard.instance.squares[row + i, column]);
                        }
                        break;
                    }
                }
                if (!availableMoves.Contains(Chessboard.instance.squares[row + i, column]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row + i, column]);
                }
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
                        if (!availableMoves.Contains(Chessboard.instance.squares[row, column + i]))
                        {
                            availableMoves.Add(Chessboard.instance.squares[row, column + i]);
                        }
                        break;
                    }
                }
                if (!availableMoves.Contains(Chessboard.instance.squares[row, column + i]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row, column + i]);
                }
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
                        if (!availableMoves.Contains(Chessboard.instance.squares[row, column + i]))
                        {
                            availableMoves.Add(Chessboard.instance.squares[row, column + i]);
                        }
                        break;
                    }
                }
                if (!availableMoves.Contains(Chessboard.instance.squares[row, column + i]))
                {
                    availableMoves.Add(Chessboard.instance.squares[row, column + i]);
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
