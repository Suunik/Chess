using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Square SquarePrefab;

    void Start()
    {
        for (int row = 0; row < 8; ++row)
        {
            for (int column = 0 ; column < 8; ++column)
            {
                Square square =
                    GameObject.Instantiate<Square>(SquarePrefab,new Vector3((-2.1f + column * 0.6f), (-2.1f + row * 0.6f),-1), Quaternion.identity);

                square.row = (char)(97 + row);
                square.column = (char)(49 + column);
            }
        }
    }
}
