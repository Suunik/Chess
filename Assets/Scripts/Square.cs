using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    private char _row;
    private char _column;

    private SpriteRenderer spriterenderer;
    private Color SquareColor;
    
    public int team;
    //sets to 0 if empty
    public char pieceOnSquare;

    private Rect rect;

    private void Awake()
    {
        pieceOnSquare = '0';

        spriterenderer = GetComponent<SpriteRenderer>();
        
        SquareColor = spriterenderer.color;
        SquareColor.a = 0;
        spriterenderer.color = SquareColor;

        // 0.5 on pool ruudu pikkusest
        // Vaja selleks, et aru saada kas hiir on ruudu peal
        rect = new Rect(
               new Vector2(-0.5f + transform.position.x, -0.5f + transform.position.y),
               new Vector2(1.01f, 1.01f));
    }

    public void PaintTheSquare()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousepos.x > rect.xMin && mousepos.x < rect.xMax)
            {
                if (mousepos.y > rect.yMin && mousepos.y < rect.yMax)
                {
                    Debug.Log(ReturnSquare());
                    if (spriterenderer.color.a > 0)
                    {
                        SquareColor.a = 0f;
                        spriterenderer.color = SquareColor;
                    }
                    else
                    {
                        SquareColor.a = 0.39f;
                        spriterenderer.color = SquareColor;
                    }
                }
            }
        }
    }
    public void SetRowAndColumn(char row, char column)
    {
        _row = row;
        _column = column;
    }
    public void SetSquareName()
    {
        gameObject.name = "Square: " + _row + _column;
    }
    
  public string ReturnSquare()
    {
        return "" + _row + _column;
    }

    public void HighlightSquare()
    {
        SquareColor.a = 0.7f;
        spriterenderer.color = SquareColor;
    }
    public void TransparentSquare()
    {
        SquareColor.a = 0f;
        spriterenderer.color = SquareColor;
    }
}
