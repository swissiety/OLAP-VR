using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using EasyUI.Toast;


public class OLAPCube : MonoBehaviour
{
	public RequestMgr requests;
	public SceneSwitcher switcher;
	
	public GameObject chartHolder;
	// Reference to the Prefab. Drag a Prefab into this field in the Inspector.
     	public GameObject myPrefab;
     	public GameObject[] axis;
     
     	private CubeState cube;      	
	private GameObject[,,] grid = new GameObject[0,0,0];
	
	
    void OnEnable()
    {	
    	cube = requests.getCubeState();
    		  	
    	UpdateAxis();
	UpdateCoordinateSystem();
    	Debug.Log("olapcube enabled");
    	
    }
    
    int[] currentMemberSize = new int[3];
    int maxDescr = 0;
    void UpdateAxis(){
	  UpdateAxis( cube.x );
	  UpdateAxis( cube.y );
	  UpdateAxis( cube.z );
	  UpdateCoordinateSystem();
    }
    
    
    void UpdateAxis( AxisState axisState ){
	
	int axisMapping = axisState.dimension;
	int level = axisState.level;
	int axisIdx = cube.getAxisIdx(axisState);
	
	List<string> member = requests.listMembersOfLevel(axisState );
	ChartAxis axisScript = (ChartAxis) axis[ axisIdx].GetComponent(typeof(ChartAxis));
	currentMemberSize[axisIdx] = member.Count;
	maxDescr = Mathf.Max( currentMemberSize[0], Mathf.Max(currentMemberSize[1], currentMemberSize[2]));
	
	axisScript.UpdateAxis( requests.getDimensionTitle(axisMapping), member, maxDescr);
    }
    
    void UpdateCoordinateSystem(){
        UpdateCubeSize();
 	
    	float maxDim = (maxDescr+2) * 1.1f;
	float scale = maxDim/6;
	
	// center & scale chartholder
	chartHolder.transform.localPosition = new Vector3(-maxDim/4 , -maxDim/2, maxDim/2 );
	chartHolder.transform.localScale = new Vector3(scale , scale, scale );
    }

    
    
    public void zoomIn( int idx){
    	AxisState axis = cube.getAxis(idx);
	if( axis.DrillDown() ){
		Debug.Log( "x zoom in.");
		UpdateAxis( axis );
		UpdateCoordinateSystem();
	}else{
		Toast.Show("I guess x doesn't DrillDown anymore.", 1f);
	}
    	    	
    }


    public void zoomOut( int idx){
    	AxisState axis = cube.getAxis(idx);
	if( axis.DrillDown() ){
		Debug.Log( "zoom in.");
		UpdateAxis(  axis  );
		UpdateCoordinateSystem();
	}else{
		Toast.Show("I guess "+ cube.getAxisName(axis) +" doesn't RollUp anymore.", 1f);
	}
    	    	
    }

    
        
    
    public void UpdateCubeSize( ){
	
	var xMember = requests.listMembersOfLevel( cube.x );
    	var yMember = requests.listMembersOfLevel( cube.y );
    	var zMember = requests.listMembersOfLevel( cube.z ); 
    	    	
	int xH = xMember.Count;
	int yH = yMember.Count;
	int zH = zMember.Count;
		
	
	// cleanup possibly existing elements
	for(int cx = 0; cx < grid.GetLength(0); cx++) {
         for(int cy = 0; cy < grid.GetLength(1); cy++) {
            for(int cz = 0; cz < grid.GetLength(2); cz++) {
            	Destroy(grid[cx,cy,cz]);
            }
         }
        }
        
	
        grid = new GameObject[xH,yH,zH];
	
    	float xHeight = (xH-1)*1.1f/2;
    	float yHeight = (yH-1)*1.1f/2;
    	float zHeight = (zH-1)*1.1f/2;
	for(int x = 0; x < xH; x++){
	
		
		for(int y = 0; y < yH; y++){
			
			for(int z = 0; z < zH; z++){
			
				// Instantiate at position, rotation.
				GameObject cube = Instantiate(myPrefab, new Vector3(x*1.1f-xHeight, y*1.1f-yHeight, z*1.1f-zHeight), Quaternion.identity);
				 
				cube.GetComponent<Renderer>().material.color = new Color(0.246f, 0.059f, 0.106f, 1.000f);
				cube.transform.SetParent(transform);
 
				cube.name = "cell_"+x +"_"+ y +"_"+ z;
				grid[x,y,z] = cube;
								
			}		
		}
	}
       
    }
    
    

    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;
    private Vector3 previousMousePosition;
    private Vector3 mouseDelta;
    private float speed = 200f;
    public GameObject target;  
    bool dragRotate = false;  



    // Update is called once per frame
    void Update()
    {        
    
        
        // Axis' Stuff
	if(Input.GetMouseButtonDown(0)){
	     var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	     RaycastHit hit;
	      
	      if(Physics.Raycast(ray,out hit)){
		   if(hit.collider.gameObject == axis[0]){
		   }else if(hit.collider.gameObject == axis[1]){
		   } else if(hit.collider.gameObject == axis[2]){
		   }else{
		   	firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		   	dragRotate = true;
		   }
	      }else{
	           firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
	      	   dragRotate = true;
	      }
      }
      
              
        DragEnd();
        
		if ( Input.GetMouseButton(0))
		{
			if(dragRotate){
		    // while the mouse is held down the cube can be moved around its central axis to provide visual feedback
		    mouseDelta = Input.mousePosition - previousMousePosition;
		    mouseDelta *= 0.2f; // reduction of rotation speed
		    transform.rotation = Quaternion.Euler(mouseDelta.y, -mouseDelta.x, 0) * transform.rotation;
		    }
		}
		else
		{
		    // automatically move to the target position
		    if (transform.rotation != target.transform.rotation)
		    {
		        var step = speed * Time.deltaTime;
		        transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, step);
		    }       
		}
        
      previousMousePosition = Input.mousePosition;

        
    }

    void DragEnd()
    {

        if (dragRotate && Input.GetMouseButtonUp(0))
        {
        dragRotate = false;
            // get the 2D poition of the second mouse click
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //create a vector from the first and second click positions
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

		// threshold 
		if (  currentSwipe.sqrMagnitude > 10000 ){

		    //normalize the 2d vector
		    currentSwipe.Normalize();

		    if (LeftSwipe(currentSwipe))
		    {
		        target.transform.Rotate(0, 90, 0, Space.World);
		        Debug.Log("left");
		        cube.PivotLeft();
		        UpdateAxis( cube.x);
     			UpdateAxis( cube.z);
		    }
		    else if (RightSwipe(currentSwipe))
		    {
		        target.transform.Rotate(0, -90, 0, Space.World);
		    	Debug.Log("right");
			cube.PivotRight();
		        UpdateAxis(cube.x);
     			UpdateAxis(cube.z);
		    }
		    else if (UpLeftSwipe(currentSwipe))
		    {
		        target.transform.Rotate(90, 0, 0, Space.World);
		    	Debug.Log("upLeft");
		    	cube.PivotUpLeft();
		        UpdateAxis(cube.x);
     			UpdateAxis(cube.y);
		    }
		    else if (UpRightSwipe(currentSwipe))
		    {
		        target.transform.Rotate(0, 0, -90, Space.World);
		    	Debug.Log("upRight");
		    	cube.PivotUpRight();
	        	UpdateAxis(cube.y);
		        UpdateAxis(cube.z);
		    }
		    else if (DownLeftSwipe(currentSwipe))
		    {
		     	 target.transform.Rotate(0, 0, 90, Space.World);
		    	Debug.Log("downLeft");
			cube.PivotDownLeft();
		        UpdateAxis(cube.y);
		        UpdateAxis(cube.z);
		    }
		    else if (DownRightSwipe(currentSwipe))
		    {
		        target.transform.Rotate(-90, 0, 0, Space.World);
		    	Debug.Log("downRight");
		    	cube.PivotDownRight();
     			UpdateAxis(cube.x);
     			UpdateAxis(cube.y);
		    }
	      }else{
	      		// reset
	      		Debug.Log("RESET swipe");
	      		target.transform.Rotate(0, 0, 0, Space.World);	
	      }
        }
    }

    bool LeftSwipe(Vector2 swipe)
    {
        return currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f;
    }

    bool RightSwipe(Vector2 swipe)
    {
        return currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f;
    }

    bool UpLeftSwipe(Vector2 swipe)
    {
        return currentSwipe.y > 0 && currentSwipe.x < 0f;
    }

    bool UpRightSwipe(Vector2 swipe)
    {
        return currentSwipe.y > 0 && currentSwipe.x > 0f;
    }

    bool DownLeftSwipe(Vector2 swipe)
    {
        return currentSwipe.y < 0 && currentSwipe.x < 0f;
    }

    bool DownRightSwipe(Vector2 swipe)
    {
        return currentSwipe.y < 0 && currentSwipe.x > 0f;
    }   
    
    
    static void Swap<T>(ref T x, ref T y){
	     T t = y;
	     y = x;
	     x = t;
    }
}
