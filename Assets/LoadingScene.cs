using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{


	public GameObject rotatingCube; 


    // Update is called once per frame
    void Update()
    {
     
        rotatingCube.transform.Rotate(0.8f, 0, 0, Space.Self);   
    }
}
