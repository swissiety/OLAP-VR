using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Connection
{
	
    public string JdbcDriver{ get; set; }
    public string Jdbc { get; set; }
    public string Description { get; set; }
    public string ConnectionDefinitionSource { get; set; }
    public bool IsDemo { get; set; }
    public bool JdbcDriverClass{ get; set; }
    public string MondrianSchemaUrl { get; set; } 
     
}
