using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    private char _row;
    private char _column;

    private SpriteRenderer spriterenderer;
    private Color SquareColor;

    Rect rect;

    private void Awake()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
        SquareColor = spriterenderer.color;
        SquareColor.a = 0;
        spriterenderer.color = SquareColor;

        // 0.3 on pool ruudu pikkusest
        // Vaja selleks, et aru saada kas hiir on ruudu peal
        rect = new Rect(
               new Vector2(-0.3f + transform.position.x, -0.3f + transform.position.y),
               new Vector2(0.61f, 0.61f));
    }

    private void Update()
    {
        PaintTheSquare();
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
                    Debug.Log("Square: " + _row + _column);
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
}
