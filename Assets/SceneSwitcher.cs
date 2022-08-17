using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{

	static public bool production = false;


	public GameObject[] scenes; 
	public GameObject rotatingCube; 
	
	public GameObject loadingScene; 
	bool loading = false;

	int currentSceneIdx = 0;
	
    // Start is called before the first frame update
    void Awake()
    {
    	/*
    	foreach( var scene in scenes){
    		scene.SetActive(false);
    	}*/
     	switchTo(0);
    }
    
    
    public void showLoadingScene(bool flag){
	
	loadingScene.SetActive(flag);
//	var render = scenes[currentSceneIdx].GetComponentInChildren<MeshRenderer>(); 	render.enabled = false;
 
	loading = flag;
    	// add possibility to abort on loading screen?
    	
    }
    
    public void switchTo( int idx){
   	
    	if(currentSceneIdx >= 0 && idx < scenes.Length){
    	
    	//	scenes[currentSceneIdx].SetActive(false);    	
		foreach( var scene in scenes ){
			scene.SetActive(false);    	
		}
		
	    	scenes[idx].SetActive(true);
		currentSceneIdx = idx;
		
		// hide in CubeScene
		rotatingCube.SetActive( idx != 4 );
		
    	}else{
    		Debug.Log("SceneSwitcher.switchTo() got an illegal Parameter: "+ idx);
    	}
    }
    
    
    int shots = 0;
    void Update()
    {

        rotatingCube.transform.Rotate(0.5f, 0, 0, Space.Self);
        
        if (Input.GetKeyDown(KeyCode.Space)){
 	       ScreenCapture.CaptureScreenshot("shot_"+ (shots++) +".png", 4);
	}
        
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
        	if(loading){
        		showLoadingScene(false);
        	}
        	switchTo(0);
        }
    }
}   
    
 
