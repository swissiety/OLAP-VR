using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellDescr : MonoBehaviour
{

    Renderer m_Renderer;

    void Start()
    {
        //Fetch the Renderer component of the GameObject
        m_Renderer = GetComponent<Renderer>();
    }

    //Run your mouse over the GameObject to change the Renderer's material color to clear
    void OnMouseOver()
    {
       //  m_Renderer.material.color = Color.clear;
    }

    //Change the Material's Color back to white when the mouse exits the GameObject
    void OnMouseExit()
    {
     //   m_Renderer.material.color = Color.white;
    }
    
    void OnMouseDown()
    {
	Debug.Log("celldescr down");
    }
    
       void OnMouseUp()
    {
	Debug.Log("celldescr up");
    }
    
    void OnMouseUpAsButton(){
    
    	Debug.Log("cell descr  OnMouseupasButton");
    }

}
