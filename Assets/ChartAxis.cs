using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartAxis : MonoBehaviour
{
	GameObject titleText;

    // Start is called before the first frame update
    void Start()
    {
	titleText = new GameObject();
	TextMesh t = titleText.AddComponent<TextMesh>();
	
	titleText.transform.rotation = Quaternion.identity;
	titleText.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void UpdateAxis(string title, List<string> cellDescriptions){
    	/*
    	// size
    	axis[0].transform.position = new Vector3(-zLen/2, 0, -xLen/2);
    	axis[0].transform.localScale = new Vector3(0,0, xLen+1);

    	axis[1].transform.position= new Vector3(-zLen/2, yLen/2, 0);
    	axis[1].transform.localScale = new Vector3(0, yLen, 0);

    	axis[2].transform.position = new Vector3(0,0,-xLen);
    	axis[2].transform.localScale = new Vector3(zLen,0,0);
    	
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
    	    
	titleText.name = "axis:"+title;

	TextMesh t = titleText.GetComponent<TextMesh>();
	t.text = title + " axis";
	t.fontSize = 24;
	t.color = Color.black;
	
//	t.transform.localEulerAngles += new Vector3(90, 0, 0);
//	t.transform.localPosition += new Vector3(56f, 3f, 40f);

     }
}
