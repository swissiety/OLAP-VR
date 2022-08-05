using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using EasyUI.Toast;


public class ConnectionScene : MonoBehaviour
{

	public SceneSwitcher switcher;
 	public TMP_Dropdown dropdown;
 	public RequestMgr requests;
	
    // Start is called before the first frame update
    void Start()
    {
    	// indicate loading: loader + dropdown.SetActive( false );
    	List<Connection> connections = requests.listConnections();
    	
    	if( connections == null || connections.Count <= 0 ){
    		Toast.Show("Loading connections failed.", 5.0f);
    		switcher.switchTo(1);
    		return;
    	}
    	
	dropdown.options.Clear();
	// dropdown.options.Add (new TMP_Dropdown.OptionData() {text=""});
	foreach (Connection conn in connections)
	{
		dropdown.options.Add (new TMP_Dropdown.OptionData() {text=conn.connectionName});
	}    	
    	
    	if( connections.Count == 1){
    	    	dropdown.value  = 0;
    	    	OnSelect();
    	}

    	
    }

     public void OnSelect()
    {

    	string cname = dropdown.options[dropdown.value].text; 
    	if( string.IsNullOrEmpty(cname)){
    		return;
    	}
    	
	Debug.Log(cname + " was selected");
    	
    	requests.setConnection( cname );
    	switcher.switchTo(2);
    }
    
}
