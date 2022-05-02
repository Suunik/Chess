using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    SpriteRenderer spriterenderer;
    private Rect rect;
    private bool pieceheld = false;

    public Square currentSquare;

    private void Awake()
    {
        spriterenderer = GetComponent<SpriteRenderer>();  
        
    }
    private void Update()
    {
        PieceMovement();
    }

    private void PieceMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rect = new Rect(
               new Vector2(-0.4f + transform.position.x, -0.4f + transform.position.y),
               new Vector2(0.81f, 0.81f));

            if (mousepos.x > rect.xMin && mousepos.x < rect.xMax)
            {
                if (mousepos.y > rect.yMin && mousepos.y < rect.yMax)
                {
                    pieceheld = true;

                    Debug.Log("Current square: " + currentSquare.returnSquare());
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            pieceheld = false;
        }

        if (pieceheld)
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
    }
    private void Highlight()
    {
        transform.localScale = transform.localScale * 1.2f;
    }
}