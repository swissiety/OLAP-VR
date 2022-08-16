using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EasyUI.Toast;

public class DimensionScene : MonoBehaviour
{
	public SceneSwitcher switcher;
  	public Button chooseButton;
  	public TMP_Text connectionText;

 	public TMP_Dropdown[] dropdowns;
 	public RequestMgr requests;

 
 
    // Start is called before the first frame update
    async void OnEnable()
    {
    
    	// title	
    	connectionText.text = "Cube '"+requests.cube+"'";
    
        chooseButton.onClick.AddListener(TaskOnClick);

	// load and add data
	switcher.showLoadingScene(true);
	List<Dimension> list = await Task.Run( () => { return requests.listDimensions();});
	switcher.showLoadingScene(false);
	
	// hint: these are only shared dimensions - cube specific dimensions are not retrieved that way..

	dropdowns[ 0 ].options.Clear ();
	dropdowns[ 1 ].options.Clear ();
	dropdowns[ 2 ].options.Clear ();
	foreach (Dimension d in list)
	{
		dropdowns[ 0 ].options.Add (new TMP_Dropdown.OptionData() {text=d.name});
		dropdowns[ 1 ].options.Add (new TMP_Dropdown.OptionData() {text=d.name});
		dropdowns[ 2 ].options.Add (new TMP_Dropdown.OptionData() {text=d.name});
	}


	// StartCoroutine(AutoSelect());

    }
    
    void OnDisable(){
         StopCoroutine("AutoSelect");
    }
    
    private IEnumerator AutoSelect()
    {
     	// FIXME: remove in production!
	dropdowns[0].value = 0;
	dropdowns[1].value = 4;	
	dropdowns[2].value = 3;

	yield return new WaitForSeconds(0.1f);
	     	
	ScreenCapture.CaptureScreenshot("DimensionScene.png", 4);
	yield return new WaitForSeconds(0.1f);

	TaskOnClick();
	
    }
    
    
    
	void TaskOnClick(){

		// check if all dropdowns have a (valid) selected option
		if(dropdowns[0].value < 0 || dropdowns[1].value < 0 || dropdowns[2].value < 0){
			Toast.Show ("You have to choose all three dimensions", 2.0f);
			return;
		}else if(dropdowns[0].value == dropdowns[1].value || dropdowns[1].value == dropdowns[2].value || dropdowns[0].value == dropdowns[2].value){
			Toast.Show ("You can't choose a dimension more than once.", 2.0f);
			return;
		}

		string dd1 = dropdowns[0].options[dropdowns[0].value].text;
		string dd2 = dropdowns[1].options[dropdowns[1].value].text;
		string dd3 = dropdowns[2].options[dropdowns[2].value].text;
		
		Debug.Log("Dimensions ["+dd1+", "+ dd2 + ", "+ dd3 +"] were chosen.");
		
		switcher.showLoadingScene(true);    		
		StartCoroutine(requests.loadDimensions( dd1, dd2, dd3, delegate{nextScreen();} ));
		
	}
	
	
	public void nextScreen(){
	    	switcher.showLoadingScene(false);		
		switcher.switchTo(4);
	}
    
 
 
    
    
    
}
