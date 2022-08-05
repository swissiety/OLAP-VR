using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Net;
using System;
using System.Net.Sockets;
using System.Xml.Serialization;
using UnityEngine;

using System.Text.Json.Serialization;
using System.Net.Http;


[System.Serializable]
class Connections{
	public Dictionary < string, Connection > connections = new Dictionary < string, Connection > ();
}



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
    
		
	public List<Connection> listConnections()
	{   
		string URLString = buildBaseUrl()+"getConnections";
		Debug.Log("load schema: "+ URLString); 

		List<Connection> connections = new List<Connection>();
		

		//using var client = new HttpClient();
		//var data = await client.GetFromJsonAsync<Connections>(URLString);
		
		Connections conns = JsonUtility.FromJson<Connections>("{\n  \"test\" : {\n    \"JdbcDriver\" : \"org.hsqldb.jdbc.JDBCDriver\",\n    \"Jdbc\" : \"jdbc:hsqldb:res:test\",\n    \"Description\" : \"Main version of test connection\",\n    \"ConnectionDefinitionSource\" : \"jar:file:/home/smarkus/IdeaProjects/mondrian-rest/target/mondrian-rest-executable.war!/WEB-INF/classes/mondrian-connections.json\",\n    \"IsDemo\" : true,\n    \"JdbcDriverClass\" : false,\n    \"MondrianSchemaUrl\" : \"jar:file:/home/smarkus/IdeaProjects/mondrian-rest/target/mondrian-rest-executable.war!/WEB-INF/classes!/test.xml\"\n  },\n  \"foodmart\" : {\n    \"JdbcDriver\" : \"org.hsqldb.jdbc.JDBCDriver\",\n    \"Jdbc\" : \"jdbc:hsqldb:res:foodmart;set schema \\\"foodmart\\\"\",\n    \"Description\" : \"Pentaho/Hyde FoodMart Database\",\n    \"ConnectionDefinitionSource\" : \"jar:file:/home/smarkus/IdeaProjects/mondrian-rest/target/mondrian-rest-executable.war!/WEB-INF/classes/mondrian-connections.json\",\n    \"IsDemo\" : true,\n    \"JdbcDriverClass\" : false,\n    \"MondrianSchemaUrl\" : \"https://raw.githubusercontent.com/pentaho/mondrian/master/demo/FoodMart.xml\"\n  }\n}");

		    foreach (KeyValuePair<string, Connection> entry  in conns.connections)
		    {
			Debug.Log( "-> "+ entry.Key );
			connections.Add( new Connection( entry.Key, "Example Dataset" ) );

		    }

		connections.Add( new Connection("foodmart", "Example Dataset" ) );
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
        
    
    
    public void execute_pivot(){
       Debug.Log("Pivot"); 
    }
    
    



}
