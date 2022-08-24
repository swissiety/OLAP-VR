using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CubeState
{
	public string cubeName = "";		
	public string measure = "";

	public AxisState x;
	public AxisState y;
	public AxisState z;
	
	public CubeState Clone()
	     {
		CubeState c = new CubeState();
		c.cubeName = cubeName;
		c.measure = measure;
		
		c.x = new AxisState();
		c.x.dimension = x.dimension;
		c.x.level = x.level;
		c.y = new AxisState();
		c.y.dimension = y.dimension;
		c.y.level = y.level;
		c.z = new AxisState();
		c.z.dimension = z.dimension;
		c.z.level = z.level;
		return c;
	     }
	
	public AxisState getAxis(int idx){
		switch( idx){
		case 0: 	return x;
		case 1: 	return y;
		case 2: 	return z;
		}
		
		throw new Exception();
	}
	
	
	public int getAxisIdx(AxisState axis){
		if( x == axis){
			return 0;	
		}else 
		if( y == axis){
			return 1;	
		}else if( z == axis){
			return 2;	
		}
		
		throw new Exception();
	}
	
	public string getAxisName(AxisState axis){
		if( x == axis){
			return "x";	
		}else 
		if( y == axis){
			return "y";	
		}else if( z == axis){
			return "z";	
		}
		
		throw new Exception();
	}
	
	public void PivotLeft(){
		Swap(ref x, ref z);
	}
	
	public void PivotRight(){
		Swap(ref x, ref z);
	}
	
	public void PivotUpLeft(){
		Swap(ref z, ref y);
	}
		
	public void PivotDownLeft(){
		Swap(ref y, ref z);
	}
	
	public void PivotUpRight(){
		Swap(ref y, ref x);
	}

	public void PivotDownRight(){
		Swap( ref x, ref y);
	}	
	
   
    static void Swap<T>(ref T x, ref T y){
	     T t = y;
	     y = x;
	     x = t;
    }
	
       public string buildQuery(OLAPSchema schema){
		return "SELECT "
		 + " "+x.buildQuery(this, schema) +" ON 0, " 
		 + " "+y.buildQuery(this, schema) +" ON 1, "
		 + " "+z.buildQuery(this, schema) +" ON 2 "
		 +" FROM ["+ cubeName +"]" 
		 + "WHERE [Measures].["  + measure + "] ";
		
		/*
		return "SELECT "
		 + "NON EMPTY "+x.buildQuery(this, schema) +" ON 0, " 
		 + "NON EMPTY "+y.buildQuery(this, schema) +" *  "
		 + " "+z.buildQuery(this, schema) +" ON 1 "
		 +" FROM ["+ cubeName +"]"
		 + "WHERE [Measures].["  + measure + "] ";*/		 
		 
	}
	
}

[Serializable]
public class AxisState
{
	public string dimension = "";
	public int hierarchy = 0;	// just use that for now  i.e. assume there is just one
	public int level = 0;
	
	public int maxLevel = Int32.MaxValue;		// correct value is set after loading
	
	
	// for slice (n dice)
	public HashSet<int> wantedMembers = new HashSet<int>();
	
	public string buildQuery(CubeState cstate, OLAPSchema schema){
		string q = " [" + dimension + "]";
		List<Level> levels = schema.getCubesDimension(cstate.cubeName, dimension).hierarchy[0].levels;
		for(int i = 0; i < level; i++){
			q += ".[" + levels[i].levelName + "]";
		}
		if( level < maxLevel){
			q += ".MEMBERS";
		}
		
		
		
		/*
			for(int i = 0; i < level; i++){
			q += ".[" + levels[i].levelName + "]";
			}
			if( level < maxLevel){
				q += ".MEMBERS";
			}
		
		*/
		
		return q;
	}
	
	public bool DrillDown(){
		if(level < maxLevel){
			level++;
			return true;
		}else{
			return false;
		}
		
	}
	
	public bool RollUp(){
		if(level > 0){
			level--;
			return true;
		}else{
			return false;
		}
		
	}

}
