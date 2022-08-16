using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using EasyUI.Toast;


public class CubeChooseScene : MonoBehaviour
{

	public SceneSwitcher switcher;
 	public TMP_Dropdown dropdown;
 	public RequestMgr requests;
	
	async void OnEnable(){

	    	// indicate loading
		switcher.showLoadingScene(true);
	    	OLAPSchema schema = await Task.Run( () => { return requests.getSchema(); });
	    	switcher.showLoadingScene(false);
	    	    		    	
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
	    	
	    	/* FIXME
	    	if( schema.cubes.Count == 1){
	    	    	dropdown.value  = 0;
	    	    	OnSelect();
	    		return;
	    	}
	    	*/
	    	
	    	if(!switcher.production){
			StartCoroutine("AutoSelect");
		}
    }
    
    void OnDisable(){
         StopCoroutine("AutoSelect");
    }
    
	private IEnumerator AutoSelect(){
	  	dropdown.value = 2;
	  	dropdown.Show();
		yield return new WaitForSeconds(0.2f);
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
    	
	Debug.Log("cube "+ cname + " was selected");
    	requests.SetCube( cname );
    	
    	switcher.switchTo(3);
    }
    
}
