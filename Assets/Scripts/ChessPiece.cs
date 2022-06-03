using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPiece : MonoBehaviour
{
    // Start is called before the first frame update
    public Square currentSquare;
    protected int[,] movementMatrix;
    protected int pieceNumber;
    public List<Square> availableMoves = new List<Square>();

    private Rect Hitbox;
    private bool pieceHeld;
    protected bool firstMove;

    public int team;

    // Update is called once per frame for every ChessPiece
    void Update()
    {

    }

    //This is for getting all moves if there were no king restrictions
    public abstract List<Square> FindAvailableMoves();
    public abstract void findAllInboundsAndNoCollisionMoves();
    public virtual void restrictMovements()
    {
        //testIfKingWillBeInCheck();
    }

    public int[] ReturnRowColumn()
    {
        string piece_coordinates = currentSquare.ReturnSquare();
        int[] value = new int[2];
        value[0] = piece_coordinates[0] - 97;
        value[1] = piece_coordinates[1] - 49;

        return value;
    }

    public bool WithinBounds(int x, int y)
    {
        if (x < 8 && x > -1)
        {
            if (y < 8 && y > -1)
            {
                return true;
            }
        }
        return false;
    }


    //Get no collision and in bounds tiles covered by a piece and add it to chessboard squares list
    public virtual void addPieceAttackingMovesToChessboard()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {

            if (team == 1)
            {
                if (!Chessboard.instance.allWhiteMoves.Contains(availableMoves[i]))
                {
                    Chessboard.instance.allWhiteMoves.Add(availableMoves[i]);
                }

            }
            else 
            {
                if (!Chessboard.instance.allBlackMoves.Contains(availableMoves[i]))
                {
                    Chessboard.instance.allBlackMoves.Add(availableMoves[i]);
                }
            }
        }
    }
    /*
    Every piece uses update() to check whether it has been clicked
    by calling this function. If this happens to be true, the object will follow
    the mouse location.
     */
    public void PieceMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Teeb nupule hitboxi
            Hitbox = new Rect(
               new Vector2(-0.4f + transform.position.x, -0.4f + transform.position.y),
               new Vector2(0.81f, 0.81f));

            if (mousepos.x > Hitbox.xMin && mousepos.x < Hitbox.xMax)
            {
                if (mousepos.y > Hitbox.yMin && mousepos.y < Hitbox.yMax)
                {
                    Highlight(0.3f);
                    pieceHeld = true;
                }
            }

        }
        //Liigutab nuppu hiire j2rgi
        if (pieceHeld)
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //position of the mouse 
            mousepos = new Vector3(mousepos.x, mousepos.y, -2);

            transform.position = mousepos;
            for (int i = 0; i < availableMoves.Count; i++)
            {
                availableMoves[i].HighlightSquare();
            }
        }

        //Looks at where the player tries to place the piece
        if (Input.GetMouseButtonUp(0))
        {
            //salvestab hiire lahti laskmisel nupule uue ruudu
            if (pieceHeld)
            {
                //Letting go of a piece resets all availablemoves
                //Get rid of highlights, clear availablemoves list
                for (int j = 0; j < availableMoves.Count; j++)
                {
                    availableMoves[j].TransparentSquare();
                }
                pieceHeld = false;
                if (availableMoves.Count != 0)
                {
                    //Go through all possible squares that a piece is allowed to go on
                    for (int i = 0; i < availableMoves.Count; ++i)
                    {
                        //Z axise tagasi muutmiseks et ta kattuks laual asuva squarega
                        transform.position = new Vector3(Mathf.Clamp(Mathf.Round(transform.position.x - 0.5f) + 0.5f, -3.5f, 3.5f),
                                                          Mathf.Clamp(Mathf.Round(transform.position.y - 0.5f) + 0.5f, -3.5f, 3.5f),
                                                          -1);

                        //The square that the piece was hovered over is in the availableMoves list
                        //Therefore transform it there.
                        //And clear availableMoves, otherwise it will remain available for the next move
                        if (transform.position == availableMoves[i].transform.position)
                        {
                            //change currentsquare team then assign new square to piece
                            currentSquare.team = 0;
                            currentSquare = availableMoves[i];
                            //If the tile the piece went to was assigned to the enemy, destroy the piece there
                            if (team == -currentSquare.team)
                                Debug.Log("Destroy 1st test");
                            {
                                if (team == 1)
                                {
                                    Debug.Log("Destroy 2nd test");
                                    for (int k = 0; k < Chessboard.instance.blackPieces.Count; k++)
                                    {
                                        if (Chessboard.instance.blackPieces[k].currentSquare == currentSquare)
                                        {
                                            Chessboard.instance.blackPieces[k].killYourself();
                                            Chessboard.instance.blackPieces.Remove(Chessboard.instance.blackPieces[k]);
                                            break;
                                        }
                                    }
                                }
                                if (team == -1)
                                {
                                    Debug.Log("Destroy 2nd test");
                                    for (int k = 0; k < Chessboard.instance.whitePieces.Count; k++)
                                    {
                                        if (Chessboard.instance.whitePieces[k].currentSquare == currentSquare)
                                        {
                                            Chessboard.instance.whitePieces[k].killYourself();
                                            Chessboard.instance.whitePieces.Remove(Chessboard.instance.whitePieces[k]);
                                            break;
                                        }
                                    }
                                }

                            }
                            currentSquare.team = team;
                            firstMove = false;
                            availableMoves.Clear();
                            Chessboard.instance.allBlackMoves.Clear();
                            Chessboard.instance.allWhiteMoves.Clear();
                            Chessboard.instance.turnCounter++;

                            Highlight(-0.3f);
                            break;
                        }

                        //The square a piece was hovered on is not among availableMoves -> Transform back to original square
                        if (i == availableMoves.Count - 1)
                        {
                            transform.position = currentSquare.transform.position;
                            Highlight(-0.3f);
                            break;
                        }
                    }
                }
                else
                {
                    transform.position = currentSquare.transform.position;
                    Highlight(-0.3f);
                }
            }

        }
    }


    protected void testIfKingWillBeInCheck()
    {
        //Change the piece to an available square then look at whether your team's king is in check
        //if it is in check, remove that availablemove
        Square previousSquare = currentSquare;
        List<Square> newAttackSquares;
        int arrayLength = availableMoves.Count;
        for (int i = 0; i < arrayLength;)
        {
            //Assign the piece a new square
            currentSquare = availableMoves[i];
            //Find new squares the enemy pieces can attack
            newAttackSquares = Chessboard.instance.allTeamMoves(-team);

            //If any of the new squares is the same as team king, then remove available move
            if (team == 1 && newAttackSquares.Contains(Chessboard.instance.whiteKingSquare))
            {
                availableMoves.Remove(availableMoves[i]);
                arrayLength--;
            }
            else
            {
                i++;
            }
            if (team == -1 && newAttackSquares.Contains(Chessboard.instance.blackKingSquare))
            {
                availableMoves.Remove(availableMoves[i]);
                arrayLength--;
            }
            else
            {
                i++;
            }
        }
    }
    private void Highlight(float value)
    {
        transform.localScale = new Vector2(transform.localScale.x + value, transform.localScale.y + value);
    }

    private void killYourself()
    {
        Destroy(gameObject);
    }
}