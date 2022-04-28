using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    SpriteRenderer spriterenderer;
    private Rect rect;

    private void Awake()
    {
        spriterenderer = GetComponent<SpriteRenderer>();

        rect = new Rect(
               new Vector2(-0.3f + transform.position.x, -0.3f + transform.position.y),
               new Vector2(0.61f, 0.61f));
    }
    private void Update()
    {
      if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousepos.x > rect.xMin && mousepos.x < rect.xMax)
            {
                if (mousepos.y > rect.yMin && mousepos.y < rect.yMax)
                {
                    Highlight();
                }
            }
        }
    }

    private void Highlight()
    {
        transform.localScale = transform.localScale * 1.2f;
    }
}
