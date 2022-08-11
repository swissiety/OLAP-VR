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

public class RequestMgr : MonoBehaviour
{

	string host = "";
	int port = 8080;
	string connectionName = "";
	// chosen cube of connection
	public string cube;
	// chosen dimensions
	public int x = -1;
	public int y = -1;
	public int z = -1;

	// 
	OLAPSchema Schema;

	
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
	
	public IEnumerator loadDimensions(int x, int y, int z, Action action){
		this.x = x;
		this.y = y;
		this.z = z;
		
		string url = buildBaseUrl()+"query";
		string query = "{ \"connectionName\" : \"foodmart\", \"query\" : \"select { [Measures].MEMBERS } on columns from Warehouse\"}";
		var webRequest = CreatePostRequest( url, query);
   		       	
   		 yield return webRequest.SendWebRequest();
   		
//   		var data = JsonUtility.FromJson<Todo>(webRequest.downloadHandler.text);

   		
   		Debug.Log( webRequest.downloadHandler.text );
   		
   		// FIXME: set maxLevel of AxisState
   		
   		
   		// FIXME: query real members!
   		membersOfLevelCache[ Schema.dimensions[ 0 ].hierarchy[0].levels[0].levelName ] = new List<string>(){"bla", "bli", "blupp"}; 
	   	membersOfLevelCache[ Schema.dimensions[ 4 ].hierarchy[0].levels[0].levelName ] = new List<string>(){"Paderborn", "Lippe", "Höxter", "München", "Berlin", "NY", "Hamburg"};
   		membersOfLevelCache[ Schema.dimensions[ 3 ].hierarchy[0].levels[0].levelName ] = new List<string>(){"Montag", "Dienstag", "Mittwoch", "Donnerstag","Freitag", "Wochenende"};
   		
   		action();
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
   
    
    
    	Dictionary<string, List<string>> membersOfLevelCache = new Dictionary<string, List<string>>();
   	public List<string> listMembersOfLevel( AxisState axis ){
   		loadSchema();
   		int dimension = axis.dimension;
   		int levelIdx = axis.level; 
   		
   		Debug.Log("dim "+ dimension + " lvl "+ levelIdx );
   		Debug.Log("retrieve members of: "+ Schema.dimensions[ dimension ].hierarchy[0].levels[levelIdx].levelName );
   		// FIXME incorporate   		
   		
   		string key = Schema.dimensions[ dimension ].hierarchy[0].levels[levelIdx].levelName;
   		if( membersOfLevelCache.ContainsKey( key) ){
   			return membersOfLevelCache[ key ];
   		}
   		Debug.Log("Error could not listMembersOfLevel! ["+ dimension + "] "+ levelIdx );   		
   		
   		//return membersOfLevelCache[ key ];
   		return new List<string>();
   	}
   	
   	public string getDimensionTitle( int dimension ){
   		loadSchema();
   		return Schema.dimensions[ dimension ].dimensionName;
   	}
   
   
    
      UnityWebRequest CreateGetRequest(string url)
    {
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        return request;
    }
    
    
    UnityWebRequest CreatePostRequest(string url, string dataRaw )
    {
       
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
        
        	request.SetRequestHeader("Content-Type", "application/json");
        	request.downloadHandler = new DownloadHandlerBuffer();
		request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(dataRaw));
       	return request;      
        }
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

/*
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
	        return Schema.dimensions;
        }
        


}
