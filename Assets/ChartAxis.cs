using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ChartAxis : MonoBehaviour
{

	public GameObject myPrefab; 
	 GameObject[] axisDescr; 
	 int axisType = 0;

    // Start is called before the first frame update
    void Awake()
    {
	axisDescr = new GameObject[0];
	axisType = name[0]-'x';
    }

    // Update is called once per frame
    void Update()
    {
	
    }


    public void UpdateAxis(string title, List<string> cellDescriptions, int maxDimension){
        	
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
		break;
            }
        }
        
        // TODO: adjust holder position -> max preferredWidth <-> avoid text overlap with axis 

	int y = 0;
	float maxpWidth = 0;
	foreach( string cellDescrStr in cellDescriptions ){
		// Instantiate at position, rotation
		axisDescr[y] = Instantiate(myPrefab, descrHolder, false);
		//cube.GetComponent<Renderer>().material.color = Random.ColorHSV();
		// 
		
		axisDescr[y].transform.forward = descrHolder.forward;
		axisDescr[y].transform.localScale = new Vector3(0.5f, 0.05f, 0.25f );
		// axisDescr[y].transform.position = descrHolder.position + descrHolder.rotation * new Vector3(2.2f, y*1.1f-height, 0 );
		axisDescr[y].transform.rotation = descrHolder.rotation;
		
		axisDescr[y].name = "descr_"+cellDescrStr;
		
		TMP_Text[] descrText = axisDescr[y].GetComponentsInChildren<TMP_Text>();
		descrText[0].SetText(cellDescrStr);
		
		maxpWidth = Mathf.Max(descrText[0].preferredWidth, maxpWidth);
		y++;
	}
	
	
	Debug.Log("maxpw "+maxpWidth);
	Debug.Log("maxDim "+maxDimension);
	
	for( int i = 0; i < axisDescr.GetLength(0); i++){
		Vector3 translateV;
		
		if(axisType == 0 ){
			// x
			translateV = new Vector3(2-maxpWidth, i*1.1f-height, -maxDimension/2);			
		}else if( axisType == 1 ){
			translateV = new Vector3(2-maxpWidth, i*1.1f-height/2, 0 );
		}else{
			translateV = new Vector3( 2+ maxDimension/2 , i*1.1f-height, 0);
		}

		axisDescr[i].transform.position = descrHolder.position + descrHolder.rotation * translateV;
	}
	
	

     }
}
