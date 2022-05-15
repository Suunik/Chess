using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public int PieceNumber;

    private Rect Hitbox;

    public bool PieceHeld = false;
    public bool AvailableMovesCheck = false;
    public bool PieceHasMoved = false;
    public bool CheckForEnemy = false;
    public bool PieceActive = true;

    public bool CastleTime = false;
    

    public Square currentSquare;

    private void Update()
    {
        PieceMovement();

        if(!PieceActive)
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
                if (Board.Instance.AvailableMoves.Count != 0)
                {
                    for (int i = 0; i < Board.Instance.AvailableMoves.Count; ++i)
                    {
                        //Z axise tagasi muutmiseks et ta kattuks laual asuva squarega
                        transform.position = new Vector3( Mathf.Clamp(Mathf.Round(transform.position.x -0.5f) +0.5f, -3.5f, 3.5f),
                                                          Mathf.Clamp(Mathf.Round(transform.position.y - 0.5f) +0.5f, -3.5f,3.5f),
                                                          -1);

                        if (transform.position == Board.Instance.AvailableMoves[i].transform.position)
                        {
                            CheckForEnemy = true;
                            currentSquare = Board.Instance.AvailableMoves[i];
                            //Vangerdus kuningale
                            if (PieceNumber == 5)
                            {
                                if (!PieceHasMoved)
                                {
                                    if (currentSquare.ReturnSquare() == "" + (char)97 + (char)(49 + 6))
                                    {
                                        CastleTime = true;
                                    }
                                }
                            }
                            PieceHasMoved = true;
                            Board.Instance.ClearAvailableMoves();

                            Highlight(-0.3f);
                            break;
                        }

                        if (i == Board.Instance.AvailableMoves.Count - 1)
                        {
                            transform.position = currentSquare.transform.position;
                            Board.Instance.ClearAvailableMoves();
                            Highlight(-0.3f);
                            break;
                        }
                    }
                }
                else
                {
                    transform.position = currentSquare.transform.position;
                    Board.Instance.ClearAvailableMoves();
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