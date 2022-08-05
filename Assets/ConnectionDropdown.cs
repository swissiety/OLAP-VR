using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class ConnectionDropdown : MonoBehaviour, ISelectHandler// required interface when using the OnSelect method.
{
	public ConnectionScene scene; 


    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
	    scene.OnSelect();
    }
}

