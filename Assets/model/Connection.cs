using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;



[System.Serializable]
public class Connection
{
	public string connectionName;
		
	    public string JdbcDriver = "";
	    public string Jdbc = "";
	    public string Description = "";
	    public string ConnectionDefinitionSource = "";
	    public bool IsDemo = false;
	    public bool JdbcDriverClass = false;
	    public string MondrianSchemaUrl = "";
	    
	public Connection(string name, string description){
		this.connectionName = name;
		this.Description = description;
	}

}
