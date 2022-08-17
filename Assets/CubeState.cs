using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeState
{

	string host = "";
	int port = 8080;
	string connectionName = "";

	public string cubeName;		
	public string measure = "";

	public AxisState x;
	public AxisState y;
	public AxisState z;
	
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
	
	
       public string buildQuery(OLAPSchema schema){
		return "SELECT [Measures].["  + measure + "] ON 0, "
		 + x.buildQuery(this, schema) +" ON 1, " 
		 + y.buildQuery(this, schema) +" ON 2, "
		 + z.buildQuery(this, schema) +" ON 3 "
		 +" FROM ["+ cubeName +"]";
	}
	
	public void PivotLeft(){
		Swap(ref x, ref z);
	}
	
	public void PivotRight(){
		Swap(ref x, ref z);
	}
	
	public void PivotUpLeft(){
		Swap(ref x, ref y);
	}
	
	public void PivotUpRight(){
		Swap(ref y, ref z);
	}
	
	public void PivotDownLeft(){
		Swap(ref y, ref z);
	}
	
	public void PivotDownRight(){
		Swap( ref x, ref y);
	}	
	
   
    static void Swap<T>(ref T x, ref T y){
	     T t = y;
	     y = x;
	     x = t;
    }
	
}


public class AxisState
{
	public string dimension = "";
	public int hierarchy = 0;	// just use that for now  i.e. assume there is just one
	public int level = 0;
	
	public int maxLevel = Int32.MaxValue;		// correct value is set after loading
	
	
	// for slice (n dice)
	public int filterMemberMin = -1;
	public int filterMemberMax = Int32.MaxValue;		// FIXME: "unfiltered"
	
	public string buildQuery(CubeState cstate, OLAPSchema schema){
		string q = " [" + dimension + "]";
		List<Level> levels = schema.getCubesDimension(cstate.cubeName, dimension).hierarchy[0].levels;
		for(int i = 0; i <= level; i++){
			q += ".[" + levels[i].levelName + "]";
		}
		q += ".MEMBERS";
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
