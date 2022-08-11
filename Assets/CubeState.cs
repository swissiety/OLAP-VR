using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeState
{

	public string Cube;		
	string host = "";
	int port = 8080;
	string connectionName = "";

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
	
	
       public string buildQuery(){
		return "SELECT " + x.buildQuery() +" ON ROWS + " + y.buildQuery() + " ON COLUMNS " ;
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
	public int dimension = -1;
	public int hierarchy = 0;	// just use that for now  i.e. assume there is just one
	public int level = 0;
	
	public int maxLevel = 0;
	
	
	// for slice (n dice)
	public int filterMemberMin = -1;
	public int filterMemberMax = Int32.MaxValue;		// "unfiltered"
	
	public string buildQuery(){
		return "";
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
