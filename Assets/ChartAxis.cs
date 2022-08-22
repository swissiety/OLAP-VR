using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ChartAxis : MonoBehaviour
{

	public GameObject olapcube;
	public GameObject myPrefab; 
	 GameObject[] axisDescr = new GameObject[0]; 
	 private int axisType = -1;

    // Start is called before the first frame update
    void Awake()
    {
	axisType = name[0]-'x';
    }
    
    Renderer m_Renderer;
    void Start()
    {
        //Fetch the Renderer component of the GameObject
        m_Renderer = GetComponent<Renderer>();
	oldColor = m_Renderer.material.color; 
    }

	void OnEnable(){
		if( ctitle != null ){
			UpdateAxis(ctitle, ccellDescriptions, cmaxDimension);
		}
	}



	private float temps;
	private bool click;
	private bool longClickDone = false;

    // Update is called once per frame
    void Update()
    {
 	    if ( click && !longClickDone && (Time.time - temps) > 0.3 ) {
		// long click effect
		longClickDone = true;
	       //  Debug.Log("longclick effect");
	        m_Renderer.material.color = new Color(0.446f, 0.099f, 0.206f, 1.000f); 
	   }
    }
    
    Color oldColor;
    void OnMouseDown(){
   		temps = Time.time ;
		click = true ;
		longClickDone = false ;
		   
	       // Debug.Log("clickdown");
	       // make brighter
	        m_Renderer.material.color = new Color(0.246f, 0.059f, 0.106f, 1.000f);
    }
    
    void OnMouseUp(){
	  click = false ;
	   m_Renderer.material.color = oldColor;
	       if ( (Time.time - temps) < 0.3 ){
		    // short click 
		    OnShortClick();
	    }else{
	    	// long click
		    OnLongClick();
	    }
    }
    
	
    void OnLongClick(){
	Debug.Log("longclick "+name );
	OLAPCube cubescript = (OLAPCube) olapcube.GetComponent(typeof(OLAPCube));
	cubescript.zoomOut( axisType );
    }    

    void OnShortClick(){	
	Debug.Log("shortclick "+name );
	OLAPCube cubescript = (OLAPCube) olapcube.GetComponent(typeof(OLAPCube));
	cubescript.zoomIn( axisType );
    }    



	string ctitle;
	List<string> ccellDescriptions;
	 int cmaxDimension;
    public void UpdateAxis(string title, List<string> cellDescriptions, int maxDimension){
        // could not been enabled when already called from cube	
        if(axisType < 0){
        	ctitle = title;
        	cellDescriptions = ccellDescriptions;
        	cmaxDimension = maxDimension;
        	return; 
        }
        
    	// cleanup
          for(int i = 0; i < axisDescr.GetLength(0); i++) {
                Destroy(axisDescr[i]);
          }
          
	
	// draw axis title
	name = "axis_"+ title;
    	    
	TMP_Text[] t = transform.parent.GetComponentsInChildren<TMP_Text>();
	t[0].SetText(title);
	
	// draw texts of cells
	axisDescr = new GameObject[ cellDescriptions.Count ];
	float height = cellDescriptions.Count*1.1f/2;
	
	
	// find holder
	Transform descrHolder = null;
	foreach (Transform child in transform.parent.transform)
        {
            if(string.Equals(child.name, "cellDescrHolder")){
		descrHolder = child;
		break;
            }
        }
        
        if(descrHolder == null){
        	throw new UnityException("no holder found.");
        }
        
        // hint: adjust holder position -> max preferredWidth <-> avoid text overlap with long members names and axis 

	int y = 0;
	foreach( string cellDescrStr in cellDescriptions ){
		// Instantiate at position, rotation
		GameObject ad = Instantiate(myPrefab, descrHolder, false);
		axisDescr[y] = ad;

		//cube.GetComponent<Renderer>().material.color = Random.ColorHSV();
		// 
		
		ad.transform.localScale = new Vector3(0.5f, 0.05f, 0.25f );
		ad.transform.rotation = descrHolder.rotation;
		ad.name = "descr_"+cellDescrStr;
		
		TMP_Text[] descrText = axisDescr[y].GetComponentsInChildren<TMP_Text>();
		descrText[0].SetText(cellDescrStr);
		if(axisType != 2 ){
			descrText[0].alignment = TextAlignmentOptions.TopRight;
		}
				
		y++;
	}
		
	for( int i = 0; i < axisDescr.GetLength(0); i++){
		Vector3 translateV;
		
		if(axisType == 0 ){
			// x
			translateV = new Vector3( 0 , i*1.1f-height/2, 0);			
		}else if( axisType == 1 ){
			translateV = new Vector3( 0, i*1.1f-height/2, 0 );
		}else{
			translateV = new Vector3( 0 , i*1.1f-height/2, 0);
		}

		axisDescr[i].transform.position = descrHolder.position + descrHolder.rotation * translateV;

	}
	
	

     }
     
     
     

     
}
