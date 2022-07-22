using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPiece : MonoBehaviour
{
    public Square currentSquare;
    protected int[,] movementMatrix;
    protected int pieceNumber;
    public List<Square> availableMoves = new List<Square>();

    public char pieceLetter;

    private Rect Hitbox;
    private bool pieceHeld;
    public bool firstMove;

    public int team;
    private void Start()
    {

    }

    //This is for getting all moves if there were no king restrictions
    public abstract List<Square> FindAvailableMoves();
    public abstract List<Square> findAllInboundsAndNoCollisionMoves();
    public virtual void restrictMovements()
    {
        checkIfKingUnderCheck();
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


    //Get no collision and in bounds tiles covered by a piece and return it
    public virtual List<Square> findPieceAttackingMoves()
    {
        List<Square> attackingMoves = new List<Square>();
        
        //Default attackable tiles for every piece is that they can also attack where they can legally move
        attackingMoves.AddRange(findAllInboundsAndNoCollisionMoves());

        return attackingMoves;
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
                            //remember moves made
                            Chessboard.instance.moveList.Add(new Square[]{ currentSquare, availableMoves[i] });
                            //change currentsquare team then assign new square to piece
                            currentSquare.team = 0;
                            //remove piece letter from square
                            currentSquare.pieceOnSquare = '0';
                            //assign new current square
                            currentSquare = availableMoves[i];
                            //If the tile the piece went to was assigned to the enemy, destroy the piece there
                            if (team == -currentSquare.team)
                            {
                                if (team == 1)
                                {
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
                            //assign piece to a new square
                            currentSquare.pieceOnSquare = pieceLetter;
                            firstMove = false;
                            availableMoves.Clear();
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

    private void checkIfKingUnderCheck()
    {
        //remember all the squares to remove from availablemoves list
        List<Square> movesToDelete = new List<Square>();
        //remember the squares that the king is being attacked from
        List<Square> kingAttackerSquare = new List<Square>();
        //take piece off of current square
        currentSquare.team = 0;
        //loop through every available move
        foreach (Square pieceMoves in availableMoves)
        {
            //save squares previous team
            int previousTeam = pieceMoves.team;
            //set piece on the test square
            pieceMoves.team = team;
            //check if enemy can attack king while piece is on a temporary square
            if (team == 1)
            {
                //Find enemy piece that is attacking the king
                foreach (ChessPiece blackPiece in Chessboard.instance.blackPieces)
                {
                    if (blackPiece.findPieceAttackingMoves().Contains(Chessboard.instance.whiteKingSquare))
                    {
                        //remember the move to delete later
                        movesToDelete.Add(pieceMoves);
                        //add the piece position where king is being attacked from only once
                        if (!kingAttackerSquare.Contains(blackPiece.currentSquare))
                        {
                            kingAttackerSquare.Add(blackPiece.currentSquare);
                        }
                    }
                }
            }
            if (team == -1)
            {
                //Find enemy piece that is attacking the king
                foreach (ChessPiece whitePiece in Chessboard.instance.whitePieces)
                {
                    if (whitePiece.findPieceAttackingMoves().Contains(Chessboard.instance.blackKingSquare))
                    {
                        //remember the move to delete later
                        movesToDelete.Add(pieceMoves);
                        //add the piece position where king is being attacked from only once
                        if (!kingAttackerSquare.Contains(whitePiece.currentSquare))
                        {
                            kingAttackerSquare.Add(whitePiece.currentSquare);
                        }
                    }
                }
            }
            //reset test squares previous team
            pieceMoves.team = previousTeam;
        }
        //You can only move the king if there is more than one attacker
        //If more than one king attacker piece remove all available moves
        if (kingAttackerSquare.Count > 1)
        {
            availableMoves.Clear();
        }
        //delete the saved moves
        foreach (Square removeSquare in movesToDelete)
        {
            if (availableMoves.Contains(removeSquare))
            {
                //skip deleteting if it is king attacker square
                if (!kingAttackerSquare.Contains(removeSquare))
                {
                    availableMoves.Remove(removeSquare);
                }
            }
        }
        //set the piece back to original position
        currentSquare.team = team;
    }
    private void Highlight(float value)
    {
        transform.localScale = new Vector2(transform.localScale.x + value, transform.localScale.y + value);
    }
    public void killYourself()
    {
        Destroy(gameObject);
        Chessboard.instance.pieceKilled = true;
    }
}