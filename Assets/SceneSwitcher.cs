using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{

	public GameObject[] scenes; 
	public GameObject rotatingCube; 
	
	public GameObject loadingScene; 


	int currentSceneIdx = 0;
	
    // Start is called before the first frame update
    void Start()
    {
    	/*
    	foreach( var scene in scenes){
    		scene.SetActive(false);
    	}*/
     	switchTo(0);
    }
    
    
    public void setLoadingScene(bool flag){
    	loadingScene.SetActive(flag);
    	
    	// possibility to abort on screen?
    	
    }
    
    public void switchTo( int idx){
   	
    	if(currentSceneIdx >= 0 && idx < scenes.Length){
    		scenes[currentSceneIdx].SetActive(false);    	
	    	scenes[idx].SetActive(true);
		currentSceneIdx = idx;
		
		// hide in CubeScene
		rotatingCube.SetActive( idx != 3 );
		
    	}else{
    		Debug.Log("SceneSwitcher.switchTo() got an illegal Parameter: "+ idx);
    	}
    }
    
    
    
    void Update()
    {

        rotatingCube.transform.Rotate(0.5f, 0, 0, Space.Self);
    }
}
