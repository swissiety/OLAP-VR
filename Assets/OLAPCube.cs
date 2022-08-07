using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OLAPCube : MonoBehaviour
{
	//public GameObject parentLayer;
	
	// Reference to the Prefab. Drag a Prefab into this field in the Inspector.
     	public GameObject myPrefab;
	
	private GameObject[,,] grid = new GameObject[0,0,0];
	


	List<string> xAxis = new List<string>{ "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag" };
	List<string> yAxis = new List<string>{ "Paderborn", "Bielefeld", "München", "Rotterdam", "NY", "Berlin", "Zürich"};
	List<string> zAxis = new List<string>{ "Hosen","Schuhe", "Socken", "Tops" };
	

    // Start is called before the first frame update
    void OnEnable()
    {	
	    CreateCube(xAxis.Count, yAxis.Count, zAxis.Count);    
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

    	Debug.Log("cube generation starteed");
    	int x = 0;
    	float xHeight = xH*1.1f/2f;
    	float yHeight = yH*1.1f/2f;
    	float zHeight = zH*1.1f/2f;
	foreach( string xName in xAxis){
	
		int y = 0;
		foreach( string yName in yAxis){
			int z = 0;
			foreach( string zName in zAxis){
			
			
				// Instantiate at position, rotation.
				GameObject cube = Instantiate(myPrefab, new Vector3(x*1.1f-xHeight, y*1.1f-yHeight, z*1.1f-zHeight), Quaternion.identity);
				 
				cube.GetComponent<Renderer>().material.color = Random.ColorHSV();
				cube.transform.SetParent(transform);
 
				cube.name = "cell_"+x +"_"+ y +"_"+ z;
				// cube.AddComponent();
								
			z++;
			}		
		y++;
		}
	x++;
	}
	
	
        
           
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
            Debug.Log(firstPressPos);
        }
        if (Input.GetMouseButtonUp(0))
        {
            // get the 2D poition of the second mouse click
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //create a vector from the first and second click positions
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
            //normalize the 2d vector
            currentSwipe.Normalize();

            if (LeftSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 90, 0, Space.World);
                Debug.Log("left");
            }
            else if (RightSwipe(currentSwipe))
            {
                target.transform.Rotate(0, -90, 0, Space.World);
            	Debug.Log("right");
            }
            else if (UpLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(90, 0, 0, Space.World);
            	Debug.Log("upLeft");
            }
            else if (UpRightSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 0, -90, Space.World);
            	Debug.Log("upRight");
            }
            else if (DownLeftSwipe(currentSwipe))
            {
             	 target.transform.Rotate(0, 0, 90, Space.World);
            	Debug.Log("downLeft");
            }
            else if (DownRightSwipe(currentSwipe))
            {
                target.transform.Rotate(-90, 0, 0, Space.World);
            	Debug.Log("downRight");
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
 
}
