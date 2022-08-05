using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OLAPCube : MonoBehaviour
{
	//public GameObject parentLayer;
	
	List<string> xAxis;
	List<string> yAxis;
	List<string> zAxis;


    // Start is called before the first frame update
    void Start()
    {	
	foreach( string x in xAxis){
	
		foreach( string y in yAxis){
		
			foreach( string z in zAxis){
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.transform.position = new Vector3(0, 0.5f, 0);
				cube.transform.position = new Vector3(0, 0.5f, 0);
			//	cube.layer = parentLayer;
				
			
			}
				
		}
	
	}
	
	
        
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
