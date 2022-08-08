using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ChartAxis : MonoBehaviour
{

	public GameObject myPrefab; 
	 GameObject[] axisDescr; 

    // Start is called before the first frame update
    void Start()
    {
	axisDescr = new GameObject[0];
    }

    // Update is called once per frame
    void Update()
    {
	
    }


    public void UpdateAxis(string title, List<string> cellDescriptions){
        	
    	// cleanup
          for(int i = 0; i < axisDescr.GetLength(0); i++) {
                Destroy(axisDescr[i]);
          }
          
	
	// draw axis title
	name = "axis_"+ title;
    	    
	TMP_Text[] t = GetComponentsInChildren<TMP_Text>();
	t[0].SetText(title);
	
	// draw texts of cells
	axisDescr = new GameObject[ cellDescriptions.Count ];
	float height = cellDescriptions.Count*1.1f/2;
	
	
	// find holder
	Transform descrHolder = null;
	foreach (Transform child in transform)
        {
            if(string.Equals(child.name, "cellDescrHolder")){
		descrHolder = child;
		Debug.Log("descr holder found");
            }
        }
        
        // adjust holder -> max UI.LayoutElement.preferredWidth 

		
	int y = 0;
	foreach( string cellDescrStr in cellDescriptions ){
		// Instantiate at position, rotation
		axisDescr[y] = Instantiate(myPrefab, descrHolder);
		//cube.GetComponent<Renderer>().material.color = Random.ColorHSV();
//		axisDescr[y].transform.SetParent(descrHolder);
		axisDescr[y].transform.position = new Vector3(0.2f, y*1.1f-height, -2 );
		axisDescr[y].name = "descr_"+cellDescrStr;
		
		TMP_Text[] descrText = axisDescr[y].GetComponentsInChildren<TMP_Text>();
		descrText[0].SetText(cellDescrStr);
		y++;
	}
	

     }
}
