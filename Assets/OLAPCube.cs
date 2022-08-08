using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OLAPCube : MonoBehaviour
{
	public RequestMgr requests;
	
	// Reference to the Prefab. Drag a Prefab into this field in the Inspector.
     	public GameObject myPrefab;
     	public GameObject[] axis;
     
     	public OLAPSchema schema; 	

	// FIXME: incorporate rotating/pivoting the levels as well! 
     	int xLevel = 0;
     	int yLevel = 0;
     	int zLevel = 0;
     
     	int xAxisMap;
    	int yAxisMap;
    	int zAxisMap;
     	
	private GameObject[,,] grid = new GameObject[0,0,0];
	private TextMeshProUGUI[,,] axisDescr;
	
	private List<string> xMembers = new List<string>();
	private List<string> yMembers = new List<string>();
	private List<string> zMembers = new List<string>();
	

    // Start is called before the first frame update
    void OnEnable()
    {	
    	// init
    	xAxisMap = requests.x;
    	yAxisMap = requests.y;
    	zAxisMap = requests.z;
    	
    	// FIXME
    	// hint: there could be multiple hierarchys.. ignore for now.
    	xMembers = requests.listMembersOf( schema.dimensions[ xAxisMap ].hierarchy[0].levels[xLevel].levelName );
	yMembers = requests.listMembersOf(schema.dimensions[ xAxisMap ].hierarchy[0].levels[yLevel].levelName);
	zMembers = requests.listMembersOf(schema.dimensions[ xAxisMap ].hierarchy[0].levels[zLevel].levelName);
	
    
    	// FIXME: incorporate OLAPDimension
	CreateCube(5, 7, 4);
	    
    	// TODO: make axis drawn text clickable if there is more hierarchy
    	
	     
    }
        
    
    public void UpdateAxis(){

    	int xLen = xMembers.Count;
    	int yLen = yMembers.Count;
    	int zLen = zMembers.Count;
    	
    	// size
    	axis[0].transform.position = new Vector3(-zLen/2, 0, -xLen/2);
    	axis[0].transform.localScale = new Vector3(0,0, xLen+1);

    	axis[1].transform.position= new Vector3(-zLen/2, yLen/2, 0);
    	axis[1].transform.localScale = new Vector3(0, yLen, 0);

    	axis[2].transform.position = new Vector3(0,0,-xLen);
    	axis[2].transform.localScale = new Vector3(zLen,0,0);
    	
    	// TODO: set dimension text 
    	//axis[0].GetComponent<TMPro>().text = "x Axis"
    	//axis[1].GetComponent<TMPro>().text = "y Axis"
    	//axis[2].GetComponent<TMPro>().text = "z Axis"
    	
    	
    	// cleanup
    	for(int cx = 0; cx < axisDescr.GetLength(0); cx++) {
         for(int cy = 0; cy < axisDescr.GetLength(1); cy++) {
            for(int cz = 0; cz < axisDescr.GetLength(2); cz++) {
                Destroy(axisDescr[cx,cy,cz]);
            }
         }
        }

    	// TODO: draw texts of cells
    	
    	// axisDescr = new GameObject[,,]; 
    	// for ... GameObject.CreatePrimitive(PrimitiveType.Cube);
    	    
    }
    
    
   
    void AxisOnClick(GameObject axis){
    	Debug.Log("axis clicked");
    		
    }
    
    
    public void CreateCube( int xH, int yH, int zH ){
  	myPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);    	
	
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
				 
				cube.GetComponent<Renderer>().material.color = Random.ColorHSV();
				cube.transform.SetParent(transform);
 
				cube.name = "cell_"+x +"_"+ y +"_"+ z;
								
			}		
		}
	}
	
	UpdateAxis();
        
        
        myPrefab.SetActive(false);   
    }


    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;
    private Vector3 previousMousePosition;
    private Vector3 mouseDelta;
    private float speed = 400f;
    public GameObject target;    



    // Update is called once per frame
    void Update()
    {        
        Swipe();
        Drag();
        
        
        // Axis' Stuff
	if(Input.GetMouseButtonDown(0)){
	     var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	     RaycastHit hit;
	      
	      if(Physics.Raycast(ray,out hit)){
		   if(hit.collider.gameObject == axis[0]){
		        //hit.collider.gameObject now refers to the 
		        //cube under the mouse cursor if present
		        AxisOnClick(axis[0]);
		   }else            if(hit.collider.gameObject == axis[1]){
		        //hit.collider.gameObject now refers to the 
		        //cube under the mouse cursor if present
		        AxisOnClick(axis[1]);
		   } else            if(hit.collider.gameObject == axis[2]){
		        //hit.collider.gameObject now refers to the 
		        //cube under the mouse cursor if present
		        AxisOnClick(axis[2]);
		   }
	      }
      }

        
    }

    void Drag()
    {
        if (Input.GetMouseButton(0))
        {
            // while the mouse is held down the cube can be moved around its central axis to provide visual feedback
            mouseDelta = Input.mousePosition - previousMousePosition;
            mouseDelta *= 0.2f; // reduction of rotation speed
            transform.rotation = Quaternion.Euler(mouseDelta.y, -mouseDelta.x, 0) * transform.rotation;
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

    void Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // get the 2D position of the first mouse click
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            // Debug.Log(firstPressPos);
        }
        if (Input.GetMouseButtonUp(0))
        {
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
		        SwapDimension(ref xAxisMap,ref  zAxisMap);
		        UpdateAxis();
		    }
		    else if (RightSwipe(currentSwipe))
		    {
		        target.transform.Rotate(0, -90, 0, Space.World);
		    	Debug.Log("right");
		        SwapDimension(ref xAxisMap,ref  zAxisMap);
		        UpdateAxis();
		    }
		    else if (UpLeftSwipe(currentSwipe))
		    {
		        target.transform.Rotate(90, 0, 0, Space.World);
		    	Debug.Log("upLeft");
		        SwapDimension(ref yAxisMap, ref zAxisMap);
		        UpdateAxis();
		    }
		    else if (UpRightSwipe(currentSwipe))
		    {
		        target.transform.Rotate(0, 0, -90, Space.World);
		    	Debug.Log("upRight");
		        SwapDimension(ref xAxisMap, ref yAxisMap);
		        UpdateAxis();
		    }
		    else if (DownLeftSwipe(currentSwipe))
		    {
		     	 target.transform.Rotate(0, 0, 90, Space.World);
		    	Debug.Log("downLeft");
		        SwapDimension(ref yAxisMap, ref zAxisMap);
		        UpdateAxis();
		    }
		    else if (DownRightSwipe(currentSwipe))
		    {
		        target.transform.Rotate(-90, 0, 0, Space.World);
		    	Debug.Log("downRight");
		        SwapDimension(ref xAxisMap, ref yAxisMap);
     			UpdateAxis();
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
    
    
    static void SwapDimension<T>(ref T x, ref T y)
{
     T t = y;
     y = x;
     x = t;
}
}
