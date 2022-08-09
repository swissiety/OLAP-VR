using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Net;
using System;
using System.Net.Sockets;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;

public class RequestMgr : MonoBehaviour
{

	OLAPSchema Schema;
	public string Cube;
	
	string host = "";
	int port = 8080;
	string connectionName = "";
	public int x = -1;
	public int y = -1;
	public int z = -1;
	
	
	public void setServerConnection(string ip, int port){
		this.host = ip;
		this.port = port;
	}
	
	
	string buildBaseUrl(){
		return "http://"+host+":"+port+"/";
	}
	
	public void setCube(string name){
		this.Cube = name;
	}
	
	public string getCube(){
		return Cube;
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
	
	public void setDimensions(int x, int y, int z){
		this.x = x;
		this.y = y;
		this.z = z;
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
   
   	public List<string> listMembersOfLevel( int dimension, int levelIdx ){
   		loadSchema();
   		Debug.Log("dim "+ dimension + " lvl "+ levelIdx );
   		Debug.Log( "retrieve members of: "+ Schema.dimensions[ dimension ].hierarchy[0].levels[levelIdx].levelName );
   		// FIXME incorporate   		
   		
   		string key = Schema.dimensions[ dimension ].hierarchy[0].levels[levelIdx].levelName;
   		if( membersOfLevelCache.ContainsKey( key) ){
   			return membersOfLevelCache[ key ];
   		}
   		
   		
   		// FIXME: query real members!
   		if( dimension == 4){
   			membersOfLevelCache[ key ] = new List<string>(){"bla", "bli", "blupp"}; 
   		}else if(dimension == 0 ) {
	   		membersOfLevelCache[ key ] = new List<string>(){"Paderborn", "Lippe", "Höxter", "München", "Berlin", "NY", "Hamburg"};
   		}else{
   			membersOfLevelCache[ key ] = new List<string>(){"Montag", "Dienstag", "Mittwoch", "Donnerstag","Freitag", "Wochenende"};
   		}
   		
   		return membersOfLevelCache[ key ];
   	}
   	
   	public string getDimensionTitle( int dimension ){
   		loadSchema();
   		Debug.Log( "dimension title: "+ Schema.dimensions[ dimension ].dimensionName );
   		return Schema.dimensions[ dimension ].dimensionName;
   	}
   
   
    
      IEnumerator GetRequest(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();
            
            
            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    callback(webRequest);
                    break;
            }
            
        }
    }
    
		
	public Dictionary < string, string > listConnections()
	{   
		string URLString = buildBaseUrl()+"getConnections";
		Debug.Log("load schema: "+ URLString); 

		// var webrequest = GetRequest( URLString );
		
		
		// FIXME: get json data from the server! 

		
		string jsonstr = "{\n  \"test\" : {\n    \"JdbcDriver\" : \"org.hsqldb.jdbc.JDBCDriver\",\n    \"Jdbc\" : \"jdbc:hsqldb:res:test\",\n    \"Description\" : \"Main version of test connection\",\n    \"ConnectionDefinitionSource\" : \"jar:file:/home/smarkus/IdeaProjects/mondrian-rest/target/mondrian-rest-executable.war!/WEB-INF/classes/mondrian-connections.json\",\n    \"IsDemo\" : true,\n    \"JdbcDriverClass\" : false,\n    \"MondrianSchemaUrl\" : \"jar:file:/home/smarkus/IdeaProjects/mondrian-rest/target/mondrian-rest-executable.war!/WEB-INF/classes!/test.xml\"\n  },\n  \"foodmart\" : {\n    \"JdbcDriver\" : \"org.hsqldb.jdbc.JDBCDriver\",\n    \"Jdbc\" : \"jdbc:hsqldb:res:foodmart;set schema \\\"foodmart\\\"\",\n    \"Description\" : \"Pentaho/Hyde FoodMart Database\",\n    \"ConnectionDefinitionSource\" : \"jar:file:/home/smarkus/IdeaProjects/mondrian-rest/target/mondrian-rest-executable.war!/WEB-INF/classes/mondrian-connections.json\",\n    \"IsDemo\" : true,\n    \"JdbcDriverClass\" : false,\n    \"MondrianSchemaUrl\" : \"https://raw.githubusercontent.com/pentaho/mondrian/master/demo/FoodMart.xml\"\n  }\n}";
		
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
		return connections;
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
        
        
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }
    
    



}
