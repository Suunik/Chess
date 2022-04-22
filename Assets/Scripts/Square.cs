using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public char row;
    public char column;


    private void Awake()
    {
       
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gameObject.SetActive(false);
            Debug.Log("Deactivated");
        }

        if(Input.GetMouseButtonDown(1))
        {

            Debug.Log("Activated");
        }
    }
}
