using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public int PieceNumber;

    SpriteRenderer spriterenderer;
    private Rect Hitbox;

    public bool PieceHeld = false;
    public bool AvailableMovesCheck = false;
    public bool CheckForEnemy = false;
    public bool PieceActive = true;

    public Square currentSquare;


    private void Awake()
    {
    }
    private void Update()
    {
        PieceMovement();

        if(!PieceActive)
        {
            gameObject.SetActive(false);
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

            //Selleks, et nupp ruudul 6igesse kohta snapiks   
            mousepos = new Vector3(
                Mathf.Round(mousepos.x - 0.5f) + 0.5f,
                Mathf.Round(mousepos.y - 0.5f) + 0.5f,
                -2);

            //Selleks, et nupp ei saaks laua piiretest v2lja minna
            transform.position = new Vector3(
             Mathf.Clamp(mousepos.x, -3.5f, +3.5f),
             Mathf.Clamp(mousepos.y, -3.5f, +3.5f),
             mousepos.z);

        }

        if (Input.GetMouseButtonUp(0))
        {
            //salvestab hiire lahti laskmisel nupule uue ruudu
            if (PieceHeld)
            {
                for(int i = 0; i < Board.Instance.AvailableMoves.Count; ++i)
                {
                    //Z axise tagasi muutmiseks et ta kattuks laual asuva squarega
                    transform.position = new Vector3(transform.position.x, transform.position.y, -1);

                    if (transform.position == Board.Instance.AvailableMoves[i].transform.position)
                    {
                        CheckForEnemy = true;
                        currentSquare = Board.Instance.AvailableMoves[i];

                        Board.Instance.ClearAvailableMoves();

                        Debug.Log(currentSquare.ReturnSquare());
                        Highlight(-0.3f);
                        break;
                    }

                    if (i == Board.Instance.AvailableMoves.Count -1)
                    {
                        transform.position = currentSquare.transform.position;
                        Board.Instance.ClearAvailableMoves();
                        Highlight(-0.3f);
                        Debug.Log("Kaka");
                        break;
                    }
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