using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
    
    	// title	
    	connectionText.text = "Connection '"+requests.getConnection()+"'";
    
        chooseButton.onClick.AddListener(TaskOnClick);

	// load and add data
	requests.loadSchema();
	
	
	// hint: these are only shared dimensions - cube specific dimensions are not retrieved that way..
	List<Dimension> list = requests.listDimensions();
	dropdowns[ 0 ].options.Clear ();
	dropdowns[ 1 ].options.Clear ();
	dropdowns[ 2 ].options.Clear ();
	foreach (Dimension d in list)
	{
		dropdowns[ 0 ].options.Add (new TMP_Dropdown.OptionData() {text=d.dimensionName});
		dropdowns[ 1 ].options.Add (new TMP_Dropdown.OptionData() {text=d.dimensionName});
		dropdowns[ 2 ].options.Add (new TMP_Dropdown.OptionData() {text=d.dimensionName});
	}


	StartCoroutine(AutoSelect());

    }
    
    void OnDisable(){
         StopCoroutine("AutoSelect");
    }
    
    private IEnumerator AutoSelect()
    {
	yield return new WaitForSeconds(0.6f);
     	// FIXME: remove in production!
	dropdowns[0].value = 0;
	dropdowns[1].value = 4;	
	dropdowns[2].value = 3;
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
		
		requests.setDimensions( dropdowns[0].value, dropdowns[1].value, dropdowns[2].value);
		
		switcher.switchTo(4);
	}
    
 
 
    
    
    
}
