using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{

	public GameObject[] scenes; 
	public GameObject rotatingCube; 

	int currentSceneIdx = 0;
	
    // Start is called before the first frame update
    void Start()
    {
     	switchTo(0);
    }
    
    
    public void switchTo( int idx){
   	
    	if(currentSceneIdx >= 0 && idx < scenes.Length){
    		scenes[currentSceneIdx].SetActive(false);    	
	    	scenes[idx].SetActive(true);
		currentSceneIdx = idx;
		
		// hide in CubeScene
		// rotatingCube.SetActive( idx != 4 );
		
    	}else{
    		Debug.Log("SceneSwitcher.switchTo() got an illegal Parameter: "+ idx);
    	}
    }
    
    
    
    void Update()
    {

        rotatingCube.transform.Rotate(0.3f, 0, 0, Space.Self);
    }
}
