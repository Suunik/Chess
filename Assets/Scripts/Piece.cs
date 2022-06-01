using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public int PieceNumber;

    public char PieceColor;

    private Rect Hitbox;

    public bool PieceHeld = false;
    public bool AvailableMovesCheck = false;
    public bool PieceHasMoved = false;
    public bool CheckForEnemy = false;
    public bool PieceActive = true;

    public bool CastleTime = false;
    public bool PawnTransform = false;

    public bool EnPassant = false;
    private int EnPassantTurn = 0;
    public Square EnpassantSquare;
    public bool EnPassantDone = false;

    private string previousSquare;
    public Square currentSquare;

    private void Update()
    {
        if(PieceColor == 'w' && (Board.instance.turn_counter % 2) == 0)
            PieceMovement();

        if (PieceColor == 'b' && (Board.instance.turn_counter % 2) != 0)
            PieceMovement();

        if (!Board.instance.enPassantCheck)
        {
            //Annab yhe k2igu en passanti jaoks
            if (EnPassant)
            {
                if (EnPassantTurn != Board.instance.turn_counter)
                {
                    EnPassant = false;
                }
            }
        }

        if (!PieceActive)
        {
            Destroy(gameObject);
            currentSquare = null;
        }
    }

    private void PieceMovement()
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
                    PieceHeld = true;
                    AvailableMovesCheck = true;

                    Highlight(0.3f);
                }
            }
        }
        //Liigutab nuppu hiire j2rgi
        if (PieceHeld)
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //position of the mouse 
            mousepos = new Vector3(mousepos.x, mousepos.y, -2);

            transform.position = mousepos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //salvestab hiire lahti laskmisel nupule uue ruudu
            if (PieceHeld)
            {
                if (Board.instance.availableMoves.Count != 0)
                {
                    for (int i = 0; i < Board.instance.availableMoves.Count; ++i)
                    {
                        //Z axise tagasi muutmiseks et ta kattuks laual asuva squarega
                        transform.position = new Vector3( Mathf.Clamp(Mathf.Round(transform.position.x -0.5f) +0.5f, -3.5f, 3.5f),
                                                          Mathf.Clamp(Mathf.Round(transform.position.y - 0.5f) +0.5f, -3.5f,3.5f),
                                                          -1);

                        if (transform.position == Board.instance.availableMoves[i].transform.position)
                        {

                            CheckForEnemy = true;
                            previousSquare = currentSquare.ReturnSquare();
                            currentSquare = Board.instance.availableMoves[i];
                            //Vangerdus kuningale
                            if (PieceNumber == 5)
                            {
                                if (!PieceHasMoved)
                                {
                                    if (PieceColor == 'w')
                                    {
                                        if (currentSquare.ReturnSquare() == "" + (char)97 + (char)(49 + 6))
                                        {
                                            CastleTime = true;
                                        }
                                        if (currentSquare.ReturnSquare() == "" + (char)97 + (char)(49 + 2))
                                        {
                                            CastleTime = true;
                                        }
                                    }
                                    if (PieceColor == 'b')
                                    {
                                        if (currentSquare.ReturnSquare() == "" + (char)(97 + 7) + (char)(49 + 6))
                                        {
                                            CastleTime = true;
                                        }
                                        if (currentSquare.ReturnSquare() == "" + (char)(97 + 7) + (char)(49 + 2))
                                        {
                                            CastleTime = true;
                                        }
                                    }
                                }
                            }
                            //Etturi Special moved
                            if(PieceNumber == 0)
                            {
                                //Etturi l6ppu j6udmine
                                for (int z = 0; z < 8; ++z)
                                {
                                    if (PieceColor == 'w')
                                    {
                                        if (currentSquare == Board.instance.squares[z, 7])
                                        {
                                            PawnTransform = true;
                                        }
                                    }
                                    if (PieceColor == 'b')
                                    {
                                        if (currentSquare == Board.instance.squares[z, 0])
                                        {
                                            PawnTransform = true;
                                        }
                                    }
                                }
                                //En passant
                                if (PieceColor == 'w' && currentSquare.ReturnSquare() == "" + previousSquare[0] + (char)(previousSquare[1] + 2))
                                {
                                    //Kui ettur k2is kaks edasi aktiveerib en passanti v6imaluse
                                    EnPassant = true;
                                    EnPassantTurn = Board.instance.turn_counter + 1;
                                }
                                if(PieceColor == 'b' && currentSquare.ReturnSquare() == "" + previousSquare[0] + (char)(previousSquare[1] - 2))
                                {
                                    EnPassant = true;
                                    EnPassantTurn = Board.instance.turn_counter + 1;
                                }
                                if(EnpassantSquare == currentSquare)
                                {
                                    EnPassantDone = true;
                                }
                            }

                            PieceHasMoved = true;
                            Board.instance.ClearAvailableMoves();

                            Highlight(-0.3f);
                            Board.instance.turn_counter = Board.instance.turn_counter + 1;
                            break;
                        }

                        if (i == Board.instance.availableMoves.Count - 1)
                        {
                            transform.position = currentSquare.transform.position;
                            Board.instance.ClearAvailableMoves();
                            Highlight(-0.3f);
                            break;
                        }
                    }
                }
                else
                {
                    transform.position = currentSquare.transform.position;
                    Board.instance.ClearAvailableMoves();
                    Highlight(-0.3f);
                }
            }
            PieceHeld = false;
        }
    }
    private void Highlight(float value)
    {
        transform.localScale = new Vector2(transform.localScale.x + value, transform.localScale.y + value);
    }
}