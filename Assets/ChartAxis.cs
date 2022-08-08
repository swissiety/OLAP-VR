using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ChartAxis : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
	
    }

    // Update is called once per frame
    void Update()
    {
	
    }


    public void UpdateAxis(string title, List<string> cellDescriptions){
    
/*
    	
    	// cleanup
    	for(int cx = 0; cx < axisDescr.GetLength(0); cx++) {
         for(int cy = 0; cy < axisDescr.GetLength(1); cy++) {
            for(int cz = 0; cz < axisDescr.GetLength(2); cz++) {
                Destroy(axisDescr[cx,cy,cz]);
            }
         }
        }
*/
    	// TODO: draw texts of cells
    	
    	// axisDescr = new GameObject[,,]; 
    	// for ... GameObject.CreatePrimitive(PrimitiveType.Cube);
	name = "axis_"+ title;
    	    
	TMP_Text[] t = GetComponentsInChildren<TMP_Text>();
	t[0].SetText(title);


//	t.transform.localEulerAngles += new Vector3(90, 0, 0);
//	t.transform.localPosition += new Vector3(56f, 3f, 40f);

     }
}
