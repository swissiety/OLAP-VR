using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Net.Sockets;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;


[System.Serializable]
public class ResultSet{
	public ResultCell[] cells = new ResultCell[0]; 
	public ResultAxis[] axes = new ResultAxis[0];
	
	public ResultAxis getAxis(string name){
		
		for(int i = 0; i < axes.GetLength(0); i++){
			if( string.Equals( name , axes[i].name) ){
				return axes[i];
			}
		}
		throw new Exception( name +"Not found");
	}
}

[System.Serializable]
public class ResultCell{

	public string formattedValue;
   	public float value;
   	public int ordinal;
   	
    	//  coordinates
    	// string error;
}
[System.Serializable]
public class ResultAxis{
	public int ordinal;
	public string name;
	public ResultPosition[] positions;
	
	public ResultPosition getPosition(string name){
	
	for(int i = 0; i < positions.GetLength(0); i++){
		if( string.Equals( name , positions[i].memberDimensionNames) ){
			return positions[i];
		}
	}
	throw new Exception( name +"Not found");
}
}
[System.Serializable]
public class ResultPosition{
	public string[] memberDimensionNames;
	public string[] memberDimensionCaptions;
	public ResultPositionMember[] positionMembers;
}
[System.Serializable]
public class ResultPositionMember{
	public string memberLevelName;
	public string memberLevelCaption;
	public string memberValue;
	public ResultPositionMember parentMember = null;
}     

public class RequestMgr : MonoBehaviour
{

	// server connection
	string host = "";
	int port = 8080;
	// chosen database connection of the server
	string connectionName = "";
	// chosen cube of connection
	public string cube;
	// chosen dimensions
	public string x = "";
	public string y = "";
	public string z = "";

	// cache the retrieved datamodel
	OLAPSchema Schema = null;

	
	public CubeState getCubeState(){
		CubeState c = new CubeState();
		c.x = new AxisState();
		c.x.dimension = x;
		c.y = new AxisState();
		c.y.dimension = y;
		c.z = new AxisState();
		c.z.dimension = z;
		return c;
	}
	
	
	public void SetCube(string cubename){
		this.cube =cubename;
	}
	
	public void setServerConnection(string ip, int port){
		this.host = ip;
		this.port = port;
	}
	
	
	string buildBaseUrl(){
		return "http://"+host+":"+port+"/";
	}
	

		
	public OLAPSchema getSchema(){
		loadSchema();
		return Schema;
	}
	
	
	public void setConnection(string name){
		this.connectionName = name;
	}
	
	public string getConnection(){
		return connectionName;
	}
	
	
	public IEnumerator loadValues( CubeState cube, Action<ResultSet> callback){
		string url = buildBaseUrl()+"query";
		
		string query = cube.buildQuery(Schema);
		string queryStr = "{ \"connectionName\" : \""+ connectionName +"\", \"query\" : "+ query ;
		Debug.Log(query);
		
		using ( var webRequest = CreatePostRequest( url, queryStr) ){
   			yield return webRequest.SendWebRequest();
	   		callback(JsonUtility.FromJson<ResultSet>(webRequest.downloadHandler.text));
   		}
	}
	
	
	
	public IEnumerator loadDimension(string dimensionName){
		loadSchema();
		
		// Dimension.hierarchy.table.name
		Debug.Log( dimensionName );
		Dimension dim = getCubesDimension(dimensionName);
		string tableName = dim.hierarchy[0].name; // table.name;
		string query = "{ \"connectionName\" : \""+ connectionName +"\", \"query\" : \"select { ["+ dim.name +"].MEMBERS } on columns from "+ cube +"\"}";
		Debug.Log(query);
		string url = buildBaseUrl()+"query";
		using ( var webRequest = CreatePostRequest( url, query) ){
   			yield return webRequest.SendWebRequest();
	   		// Debug.Log( webRequest.downloadHandler.text );
	   		ResultSet resultSet = JsonUtility.FromJson<ResultSet>(webRequest.downloadHandler.text);
	   		
	   		
   			
   			Debug.Log( "cells"+ resultSet.cells.GetLength(0) +  " axes"+ resultSet.axes.GetLength(0) );
			ResultAxis axis = resultSet.getAxis("COLUMNS");
			
			int levelCount = dim.hierarchy[0].levels.Count;
			// Debug.Log(levelCount);
			Dictionary<string, int> levelMap = new Dictionary<string, int>();
			for(int l = 0; l < levelCount; l++){
				membersOfLevelCache[ dim.hierarchy[0].levels[ l ] ] = new List<string>(); 
				levelMap.Add( dim.hierarchy[0].levels[ l ].levelName, l );
			}
			
			for(int i = 0; i < axis.positions.GetLength(0);i++){
				// Debug.Log("posname "+ axis.positions[i].memberDimensionNames);
				var pos = axis.positions[i];				
				var posMember = pos.positionMembers[0];
				do{
					int levelIdx;
					if( levelMap.ContainsKey(posMember.memberLevelCaption) ){
						levelIdx = levelMap[posMember.memberLevelCaption]	;				
					}else{
						levelIdx = 0;
					}
					
					membersOfLevelCache[ dim.hierarchy[0].levels[ levelIdx] ].Add( posMember.memberValue ); 	
					posMember = posMember.parentMember;
				}while(posMember != null);
				
			
			}
			
   		}   	

	}
	
	public int GetMaxLevelDepth(string dimensionName){
		return getCubesDimension(dimensionName).hierarchy[0].levels.Count;
	}
	
	
	public IEnumerator loadDimensions(string x, string  y, string  z, Action callback){
		loadSchema();
		
		this.x = x;
		this.y = y;
		this.z = z;
			
		yield return loadDimension(x);
		yield return loadDimension(y);
		yield return loadDimension(z);			
	   		
   		// example filling for members of the first level of dimensions
   		//membersOfLevelCache[ Schema.dimensions[ 4 ].hierarchy[0].levels[0] ] = new List<string>(){"bla", "bli", "blupp"}; 
	   	// membersOfLevelCache[ Schema.dimensions[ 0 ].hierarchy[0].levels[0]] = new List<string>(){"Paderborn", "Lippe", "Höxter", "München", "Berlin", "NY", "Hamburg"};
   		// membersOfLevelCache[ Schema.dimensions[ 3 ].hierarchy[0].levels[0] ] = new List<string>(){"Montag", "Dienstag", "Mittwoch", "Donnerstag","Freitag", "Wochenende"};
   		
   		callback();
	}
	

    
    	Dictionary<Level, List<string>> membersOfLevelCache = new Dictionary<Level, List<string>>();
   	public List<string> listMembersOfLevel( AxisState axis ){
   		loadSchema();
   		Dimension dimension = getCubesDimension(axis.dimension);
   		int levelIdx = axis.level; 
   		
   		Debug.Log("dim "+ dimension + " lvl "+ levelIdx );
   		Debug.Log("retrieve members of: "+ dimension.hierarchy[0].levels[levelIdx].levelName );
   		
   		Level key = dimension.hierarchy[0].levels[levelIdx];
   		if( membersOfLevelCache.ContainsKey( key) ){
   			// Debug.Log("members: " + membersOfLevelCache[ key ] + " " + membersOfLevelCache[ key ][0] );
   			return membersOfLevelCache[ key ];
   		}

   		// throw new Exception("There is no data for that dimension/level");
   		return new List<string>(){"No Data."};
   	}
   	
   	public string getDimensionTitle( string dimensionName ){
   		loadSchema();
   		return getCubesDimension(dimensionName).name;
   	}
   
   
    
      UnityWebRequest CreateGetRequest(string url)
    {
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        return request;
    }
    
    
    UnityWebRequest CreatePostRequest(string url, string dataRaw )
    {
       
        UnityWebRequest request = new UnityWebRequest(url, "POST");
	request.downloadHandler = new DownloadHandlerBuffer();
	request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(dataRaw));
	request.SetRequestHeader("Content-Type", "application/json");
	return request;      
        
    }
    
	
	public IEnumerator loadCubeConnections( Action<Dictionary < string, string > > callback){
		string URLString = buildBaseUrl()+"getConnections";
		Debug.Log("load schema: "+ URLString); 

		UnityWebRequest webrequest = CreateGetRequest( URLString );
		
		
		yield return webrequest.SendWebRequest();
		string jsonstr = webrequest.downloadHandler.text;
		
		var values = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string,string>>>(jsonstr);
		
		Dictionary < string, string >  connections = new Dictionary<string, string>();
		foreach (KeyValuePair<string, Dictionary<string,string>> entry in values)
		{
		      string cname =  entry.Key;
		      string descr=  entry.Value["Description"];
		      connections.Add( cname, descr);
			// Debug.Log( "name: "+ cname + " descr: "+descr );
		}

//		connections.Add("foodmart", "Example Dataset" );
		callback( connections);
	}


	public bool loadSchema()
    	{   
    		if( Schema != null ){
    			return true;
    		}
    		
		string URLString = buildBaseUrl()+"getSchema?connectionName="+ connectionName;
		Debug.Log("load schema: "+ URLString); 

		XmlSerializer serializer = new XmlSerializer(typeof(OLAPSchema));

		/* DEBUG helping code
				serializer.UnknownAttribute += (sender, args) =>
		{
		    System.Xml.XmlAttribute attr = args.Attr;
		    Debug.Log($"Unknown attribute {attr.Name}=\'{attr.Value}\'");
		};
		serializer.UnknownNode += (sender, args) =>
		{
		    Debug.Log($"Unknown Node:{args.Name}\t{args.Text}");
		};
		serializer.UnknownElement += 
		    (sender, args) => 
			Debug.Log("Unknown Element:" 
			    + args.Element.Name + "\t" + args.Element.InnerXml);
			    
		serializer.UnreferencedObject += 
		    (sender, args) => 
			Debug.Log("Unreferenced Object:"
			    + args.UnreferencedId + "\t" + args.UnreferencedObject.ToString());

			*/	
		
		using (XmlTextReader reader = new XmlTextReader (URLString))
		{
			this.Schema = (OLAPSchema) serializer.Deserialize(reader);

			Debug.Log("schema loaded for "+ URLString); 
			return true;
		}
        }
        
        public List<Dimension> listDimensions(){
	        loadSchema();
		
		// cubes dimensions plus used shared dimensions
		var usedSharedDims = Schema.dimensions.FindAll( d => Schema.getCube(cube).dimensionUsages.Find( du => du.name == d.name) != null);
		usedSharedDims.AddRange(Schema.getCube(cube).dimensions);	       
	        return usedSharedDims;
        }
        
        public Dimension getCubesDimension( string name){
	        return listDimensions().Find( d => d.name == name );
        }
        
        
	public bool tryConnect(string ip, int port){
		Debug.Log("try to connect..");
		try{
			TcpClient socketConnection = new TcpClient( ip , port); 
			socketConnection.Close();
		  }
		  catch (ArgumentNullException e)
		  {
		    Debug.Log("ArgumentNullException:"+ e);
		  	return false;
		  }
		  catch (SocketException e)
		  {
		    Debug.Log("SocketException:"+ e);
		    return false;
		  }
    		return true;
    	}
   
    

}
