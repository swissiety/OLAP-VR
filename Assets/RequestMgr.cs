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
	string ip = "";
	int port = 8080;
	string connectionName = "";
	string x = "";
	string y = "";
	string z = "";
	OLAPSchema schema;
	
	
	public void setServerConnection(string ip, int port){
		this.ip = ip;
		this.port = port;
	}
	
	string buildBaseUrl(){
		return "http://"+ip+":"+port+"/";
	}
	
	public void setConnection(string name){
		this.connectionName = name;
	}
	
	public string getConnection(){
		return connectionName;
	}
	
	public void setDimensions(string x, string y, string z){
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
    	
    		if( schema != null ){
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
			this.schema = (OLAPSchema) serializer.Deserialize(reader);

			Debug.Log("schema loaded for "+ URLString); 
			return true;
		}
		
        }
        
        public List<Dimension> listDimensions(){
	        loadSchema();
	        return schema.dimensions;
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
