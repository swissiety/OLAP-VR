using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EasyUI.Toast;


public class OLAPCube : MonoBehaviour
{
	public RequestMgr requests;
	public SceneSwitcher switcher;
	
	public TMP_Text measureText;	
	public Button undoButton;	
	public GameObject tableHolder;
	
	public GameObject chartHolder;
	// Reference to the Prefab. Drag a Prefab into this field in the Inspector.
     	public GameObject myPrefab;
     	public GameObject[] axis;
     
     	private CubeState cube;      	
	private GameObject[,,] grid = new GameObject[1,1,1];
	
	Stack<CubeState> undoStack = new Stack<CubeState>();
	
	bool isSliceMode = false;
	    int maxDescr = 0;

	
    void Awake(){
    
    	undoButton.onClick.AddListener(OnUndoButtonClick);

    } 	
	
    void OnEnable()
    {	
    	grid[0,0,0] = transform.Find("Cube").gameObject;
    	StartCoroutine(onLoadCube(requests.getCubeState()));
    	
    }

	IEnumerator onLoadCube( CubeState cs){
		// hacky: wait until gameobjects are resolved
		yield return new WaitForSeconds(0.5f);
		initCubeState(cs);
	}
    
	public void initCubeState( CubeState cs ){
		
		cube = cs;
	    	
	    	// reset target
	    	transform.localRotation = Quaternion.identity; // chartHolder.transform.rotation;
	    	target.transform.localRotation = Quaternion.identity; // chartHolder.transform.rotation; 
	    	
	    	maxDescr = Mathf.Max( requests.listMembersOfLevel( cube.x ).Count, Mathf.Max(requests.listMembersOfLevel( cube.y ).Count, requests.listMembersOfLevel( cube.z ).Count));
	    	
		cube.x.maxLevel = requests.GetMaxLevelDepth( cube.x.dimension );
		cube.y.maxLevel = requests.GetMaxLevelDepth( cube.y.dimension);
		cube.z.maxLevel = requests.GetMaxLevelDepth( cube.z.dimension);

	    	UpdateAxes();
		
		measureText.SetText( cube.measure );		
		UpdateResults();
	
	}
	
	
	Coroutine queryExecRoutine = null;
	void UpdateResults(){
	
		undoStack.Push( cube.Clone() );
		if( undoStack.Count > 1){
			undoButton.interactable = true;
		}
	
		Debug.Log("request results");
		drawTable(null);
		
		if(queryExecRoutine != null){
			StopCoroutine(queryExecRoutine);
		}
		// FIXME queryExecRoutine = StartCoroutine( requests.loadValues( cube, OnResult ) );
	}
	
	public void OnResult( ResultSet result ){
		queryExecRoutine = null;
		
		if( result == null ){
			Debug.Log("update results failed");
			return;
		}
			
		Debug.Log("update results");
		drawTable( result );		
	
	} 


	void OnUndoButtonClick(){
		if( undoStack.Count > 1){

			// remove the current state - (first) state gets pushed back after initCubeState
			undoStack.Pop();
			var newState = undoStack.Pop();
					
			if( undoStack.Count < 2){
				undoButton.interactable = false;
			}
			initCubeState(newState);

			Debug.Log("undo");
			Toast.Show("undo.", 0.5f, ToastPosition.BottomRight);

		}else{
			Debug.Log("nothing to undo "+ undoStack.Count);
		}
		
		Debug.Log("undostacksize "+ undoStack.Count);

	}

    
    
    void UpdateAxes(){
	  UpdateAxis( cube.x );
	  UpdateAxis( cube.y );
	  UpdateAxis( cube.z );
	  UpdateCoordinateSystem();
    }
    
    
    void UpdateAxis( AxisState axisState ){
	
	string axisMapping = axisState.dimension;
	int level = axisState.level;
	int axisIdx = cube.getAxisIdx(axisState);
	
	List<string> member = requests.listMembersOfLevel(axisState );
	ChartAxis axisScript = (ChartAxis) axis[ axisIdx].GetComponent(typeof(ChartAxis));
	
	string title;
	if(axisState.level > 0){
		title = requests.getCubesDimension(axisState.dimension).hierarchy[0].levels[axisState.level].levelName;
	}else{
		title = requests.getDimensionTitle(axisMapping);
	}
	
	Debug.Log( "maxdescr"+ maxDescr );
	axisScript.UpdateAxis( title, member, maxDescr);
    }
    
    
    
    public void zoomIn( int idx){
    	AxisState axis = cube.getAxis(idx);
	if( axis.DrillDown() ){
		Debug.Log( "x zoom in.");
		UpdateAxis( axis );
		UpdateCoordinateSystem();
		UpdateResults();
	}else{
		Toast.Show("I guess x doesn't DrillDown anymore.", 1f);
	}
    	    	
    }


    public void zoomOut( int idx){
    	AxisState axis = cube.getAxis(idx);
	if( axis.RollUp() ){
		Debug.Log( "zoom in.");
		UpdateAxis(  axis  );
		UpdateCoordinateSystem();
		UpdateResults();
	}else{
		Toast.Show("I guess "+ cube.getAxisName(axis) +" doesn't RollUp anymore.", 1f);
	}
    	    	
    }

    
        
    Color cellCol =  new Color(0.246f, 0.059f, 0.106f, 1.000f);
    public void UpdateCoordinateSystem( ){
	
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
	
    	
	float scale = 8;	    	
    	float spacing = 1.5f;
        float maxDim = maxDescr*spacing;

    	float xHeight = (xH-1)*spacing/2;
    	float yHeight = (yH-1)*spacing/2;
    	float zHeight = (zH-1)*spacing/2;

	
	for(int x = 0; x < xH; x++){
	
		
		for(int y = 0; y < yH; y++){
			
			for(int z = 0; z < zH; z++){
			
				// Instantiate at position, rotation.
				GameObject cube = Instantiate(myPrefab, new Vector3(transform.position.x+x*spacing-xHeight, transform.position.y+y*spacing-yHeight, transform.position.z+z*spacing-zHeight), Quaternion.identity);
				 
				cube.GetComponent<Renderer>().material.color = cellCol;
				cube.transform.SetParent(transform);
				cube.transform.rotation = Quaternion.identity;
 
				cube.name = "cell_"+x +"_"+ y +"_"+ z;
				cube.transform.localScale = new Vector3(1,1,1);
				grid[x,y,z] = cube;
								
			}		
		}
	}
       
        
	
	// transform.localPosition = new Vector3( -maxDim/2 , -maxDim/2, -maxDim/2  );
	transform.localScale = new Vector3(scale/maxDim , scale/maxDim, scale/maxDim );

	// target.transform.localPosition = transform.localPosition;
	target.transform.localScale = transform.localScale;

       
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
		   }else if(hit.collider.gameObject == axis[2]){
		   }else{
		   	if( isSliceMode ){
		   	
		   	}else{
			   	firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			   	dragRotate = true;
		   	}
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

		    if (isLeftSwipe(currentSwipe))
		    {
			LeftSwipe();
		    }
		    else if (isRightSwipe(currentSwipe))
		    {
		    	RightSwipe();
		    }
		    else if (isUpLeftSwipe(currentSwipe))
		    {
		    	UpLeftSwipe();
		    }
		    else if (isUpRightSwipe(currentSwipe))
		    {
		    	UpRightSwipe();
		    }
		    else if (isDownLeftSwipe(currentSwipe))
		    {
		    	DownLeftSwipe();
		    }
		    else if (isDownRightSwipe(currentSwipe))
		    {
		    	DownRightSwipe();
		    }
	      }else{
	      		// reset
	      		// Debug.Log("RESET swipe");
	      		target.transform.Rotate(0, 0, 0, Space.World);	
	      }
        }
    }



    void LeftSwipe()
    {
    		        target.transform.Rotate(0, 90, 0, Space.World);
		        Debug.Log("left");
		        cube.PivotLeft();
		        UpdateAxis( cube.x);
     			UpdateAxis( cube.z);
     			UpdateResults();
    }

    void RightSwipe()
    {
        
		    	target.transform.Rotate(0, -90, 0, Space.World);
		    	Debug.Log("right");
			cube.PivotRight();
		        UpdateAxis(cube.x);
     			UpdateAxis(cube.z);
     			UpdateResults();
    }

    void UpLeftSwipe()
    {
     		        target.transform.Rotate(90, 0, 0, Space.World);
		    	Debug.Log("upLeft");
		    	cube.PivotUpLeft();
		        UpdateAxis(cube.z);
     			UpdateAxis(cube.y);
     			UpdateResults();
   
    }


    void DownLeftSwipe()
    {
    
		     	target.transform.Rotate(-90, 0, 0, Space.World);
		    	Debug.Log("downLeft");
			cube.PivotDownLeft();
		        UpdateAxis(cube.y);
		        UpdateAxis(cube.z);
		        UpdateResults();
    }
    
    void UpRightSwipe()
    {
    
		        target.transform.Rotate(0, 0, 90, Space.World);
		    	Debug.Log("upRight");
		    	cube.PivotUpRight();
	        	UpdateAxis(cube.x);
		        UpdateAxis(cube.y);
		        UpdateResults();
    }

    void DownRightSwipe()
    {
    		        target.transform.Rotate(0, 0, -90, Space.World);
		    	Debug.Log("downRight");
		    	cube.PivotDownRight();
     			UpdateAxis(cube.x);
     			UpdateAxis(cube.y);
     			UpdateResults();
    }
    



    bool isLeftSwipe(Vector2 swipe)
    {
        return currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f;
    }

    bool isRightSwipe(Vector2 swipe)
    {
        return currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f;
    }

    bool isUpLeftSwipe(Vector2 swipe)
    {
        return currentSwipe.y > 0 && currentSwipe.x < 0f;
    }

    bool isUpRightSwipe(Vector2 swipe)
    {
        return currentSwipe.y > 0 && currentSwipe.x > 0f;
    }

    bool isDownLeftSwipe(Vector2 swipe)
    {
        return currentSwipe.y < 0 && currentSwipe.x < 0f;
    }

    bool isDownRightSwipe(Vector2 swipe)
    {
        return currentSwipe.y < 0 && currentSwipe.x > 0f;
    }   
    
    
    void drawTable ( ResultSet results ) {
		
		
	// cleanup
	 foreach (Transform child in tableHolder.transform) {
     		GameObject.Destroy(child.gameObject);
	 }

         
         GridLayoutGroup glg = tableHolder.GetComponent<GridLayoutGroup>();

         int colCount;
         if( results == null ){
	 
		glg.childAlignment = TextAnchor.MiddleCenter;

	 	GameObject obj =  new GameObject();
	 	TextMeshProUGUI emptyinfo = obj.AddComponent<TextMeshProUGUI>();
	 	emptyinfo.SetText("Calculate Results for "+ cube.measure +"..");
	 	emptyinfo.fontSize = 12;
	 	emptyinfo.color = Color.black;
 	 	obj.transform.SetParent(tableHolder.transform);
	 	// emptyinfo.alignment = AlignmentTypes.Center;

	 	colCount = 1;
		
	 }else{

 	 	colCount = results.getAxis("COLUMNS").positions.GetLength(0);
	 	
	 	float width = 1.0f;
	 	float height = 1.0f;

	
	 	colCount = 1;

		foreach( ResultCell cell in results.cells ){
			
			GameObject obj =  new GameObject();
		 	TextMeshProUGUI tcell = obj.AddComponent<TextMeshProUGUI>();
		 	tcell.SetText( cell.ordinal + " => "+ cell.formattedValue);
		 	tcell.fontSize = 10;
		 	tcell.color = Color.black;
	 	 	tcell.transform.SetParent(tableHolder.transform);
		 	// emptyinfo.alignment = AlignmentTypes.Center;

		}

/*
		// FIXME: incorporate texts
		 
		 // draw lines
		 for(int i = rows; i >= 0; i--){
		 	// width
		 }
		 
		 
		 for(int i = cols; i >= 0; i--){
		 	// height
		 }
		 */
		 
		 
         }
         
         
 	glg.constraintCount = colCount;
 	
 	
     }
     


    static void Swap<T>(ref T x, ref T y){
	     T t = y;
	     y = x;
	     x = t;
    }
}
