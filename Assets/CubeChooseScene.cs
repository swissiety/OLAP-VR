using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using EasyUI.Toast;


// TODO: OnEnable? Start is only called once -> multiple "window" accesses w/different connections will fail..

public class CubeChooseScene : MonoBehaviour
{

	public SceneSwitcher switcher;
 	public TMP_Dropdown dropdown;
 	public RequestMgr requests;
	
	void OnEnable(){

	    	// indicate loading: loader + dropdown.SetActive( false );
	    	OLAPSchema schema = requests.getSchema();
	    	
	    
	    	if( schema == null || schema.cubes.Count <= 0 ){
	    		Toast.Show("Loading OLAPSChema/Cube Data failed.", 5.0f);
    			Debug.Log("cube loading failed");
	    		switcher.switchTo(0);
	    		return;
	    	}
	    	
		dropdown.options.Clear();
		dropdown.options.Add (new TMP_Dropdown.OptionData() {text=""});
		foreach (Cube c in schema.cubes)
		{
			dropdown.options.Add (new TMP_Dropdown.OptionData() {text=c.cubeName});
		}    	
	    	
	    	if( schema.cubes.Count == 1){
	    	    	dropdown.value  = 0;
	    	    	OnSelect();
	    	}
	    	
	    	    	
		// FIXME: remove in production
		 StartCoroutine(AutoSelect());
		

    }
    
    void OnDisable(){
         StopCoroutine("AutoSelect");
    }
    
	private IEnumerator AutoSelect(){
	    	yield return new WaitForSeconds(0.3f);
	  	dropdown.value = 2;
	    	OnSelect();
	}
    
    
    
    void Start(){
        dropdown.onValueChanged.AddListener(delegate { OnSelect(); });
    }
    
    
     public void OnSelect()
    {
	if(dropdown.value < 1){
		return;
	}
    	string cname = dropdown.options[dropdown.value].text; 
    	if( string.IsNullOrEmpty(cname)){
    		return;
    	}
    	
	Debug.Log(cname + " was selected");
    	
    	requests.setCube( cname );
    	switcher.switchTo(3);
    	Debug.Log("cube chosen");
    }
    
}
