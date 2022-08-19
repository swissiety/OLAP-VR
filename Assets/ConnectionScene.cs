using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using EasyUI.Toast;


// TODO: OnEnable? Start is only called once -> multiple "window" accesses w/different connections will fail..

public class ConnectionScene : MonoBehaviour
{

	public SceneSwitcher switcher;
 	public TMP_Dropdown dropdown;
 	public RequestMgr requests;
	
	async void OnEnable(){
		
	    	// indicate loading: loader + dropdown.SetActive( false );
	    	
	    	switcher.showLoadingScene(true);
	    	
	    	StartCoroutine(requests.loadCubeConnections(( Dictionary< string, string > d) => OnLoaded(d) ));
	    	
    }
    
    public void OnLoaded( Dictionary< string, string > connections ){
    	    	switcher.showLoadingScene(false);
	    	
	    	if( connections == null || connections.Count <= 0 ){
	    		Toast.Show("Loading connections failed.", 5.0f);
	    		Debug.Log("Loading connections failed.");
	    		switcher.switchTo(0);
	    		return;
	    	}
	    	
		dropdown.value = 0;	
		dropdown.options.Clear();
		dropdown.options.Add (new TMP_Dropdown.OptionData() {text=""});
		foreach (KeyValuePair < string, string > conn in connections)
		{
			dropdown.options.Add (new TMP_Dropdown.OptionData() {text=conn.Key});
		}    	
	    	
	    	/* FIXME 
	    	if( connections.Count == 1){
	    	    	dropdown.value  = 0;
	    	    	OnSelect();
	    		return;
	    	}
	    	*/
	    	
	    	    	
		if(!SceneSwitcher.production){
			StartCoroutine("AutoSelect");
		}
		
    }
    
    
    void OnDisable(){
         StopCoroutine("AutoSelect");
    }
    
	private IEnumerator AutoSelect(){
		dropdown.value = 2;		
		dropdown.Show();
	    	yield return new WaitForSeconds(0.1f);
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
    	
    	requests.setConnection( cname );
    	    	
    	switcher.switchTo(2);
    }
    
}
