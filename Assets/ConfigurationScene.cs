using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EasyUI.Toast;

public class ConfigurationScene : MonoBehaviour
{
  	public Button connectButton;
  	public TMP_InputField ip;
  	public TMP_InputField port;
  	public SceneSwitcher switcher;
  	public RequestMgr requests;


  	void Start(){
  		// connectButton.interactable = false;
  		connectButton.onClick.AddListener(TaskOnClick);
  		// ip.onValueChanged.AddListener(TaskTextChanged);
  		
  		// laod last successful server login if it exists
  		if(PlayerPrefs.HasKey("connectionIp")){
  			ip.text = PlayerPrefs.GetString("connectionIp");
  			port.text = PlayerPrefs.GetString("connectionPort");
  		}else{
  			ip.text = "127.0.0.1";
  			port.text = "8080";
  		}
  		
		if(!SceneSwitcher.production){
			StartCoroutine(AutoSelect());		
		}
    }
    
    void OnDisable(){
         StopCoroutine("AutoSelect");
    }
    
    private IEnumerator AutoSelect()
	    {
  	
		yield return new WaitForSeconds(0.1f);
     	  	
		TaskOnClick();
	}

	void TaskTextChanged(){
		Toast.Show ("text changed", 1.0f);

		// connectButton.interactable = ValidateIP(ip.text); 
		// TODO: allow domains as well..
		
	}


	async void TaskOnClick(){
		
		if( string.IsNullOrEmpty(ip.text.ToString()) || string.IsNullOrEmpty(port.text.ToString()) ){
			Toast.Show ("Invalid connection configuration.");
			return;
		}
		
		
		switcher.showLoadingScene(true);
		bool successful = await Task.Run( () => { return requests.tryConnect(ip.text.ToString(), Int32.Parse(port.text.ToString()));} );
		switcher.showLoadingScene(false);
		
		// TODO: enable/disable connect button
		//connectButton.interactable = successful;
		
		if( successful ){

			// remember last successful server login
			PlayerPrefs.SetString("connectionIp", ip.text.ToString());
			PlayerPrefs.SetString("connectionPort", port.text.ToString());
			PlayerPrefs.Save();
			
			requests.setServerConnection(ip.text.ToString(), Int32.Parse(port.text.ToString()));
			switcher.switchTo(1);
			Debug.Log("connect to "+  ip.text.ToString() + ":"+ port.text.ToString() +" was successful.");
		}else{
			Toast.Show ("Connection to "+  ip.text.ToString() + ":"+ port.text.ToString() +" failed.", 2.0f);
		}
	}
    
 
 
 	public static bool ValidateIP(string strIP)
	{
	    IPAddress result = null;
	    return !string.IsNullOrEmpty(strIP) && IPAddress.TryParse(strIP, out result);
	}
 
}
