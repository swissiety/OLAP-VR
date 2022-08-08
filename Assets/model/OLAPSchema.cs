using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.Xml.Serialization;


[XmlRoot("Schema")]
public class OLAPSchema
{
	[XmlAttribute("name")]
	public string schemaName;
	
    	[XmlElement("Dimension")]
	public List<Dimension> dimensions;
	
	[XmlElement("Cube")]
	public List<Cube> cubes;
	
	// List<VirtualCube> vcubes;
	// List<Role> roles;
	// CalculatedMember cmembers;	
	
}

public class Measure{
 
	[XmlAttribute("name")]
	public string measureName;
	[XmlAttribute]
	public string column;
	[XmlAttribute]
	public string aggregator;
	[XmlAttribute]
	public string formatString;
}

public class Dimension
{
	[XmlAttribute("name")]
	public string dimensionName;
	
	[XmlElement("Hierarchy")]
	public List<Hierarchy> hierarchy;
	
	
	[XmlAttribute("type")]
	public string dimensionType;
}

public class Cube
{
	[XmlAttribute("name")]
	public string cubeName;
	[XmlAttribute]
	public string defaultMeasure;
	[XmlElement("Measure")]
	public List<Measure> measures;
	
	
}

public class Hierarchy 
{

	[XmlAttribute("name")]
	public string name;

	[XmlElement("Table")]
	public Table table;				// there may be multiple table elements?
	
	[XmlElement("Level")]
	public List<Level> levels;

	[XmlAttribute]
	public bool hasAll;
	
	[XmlAttribute]
	public string primaryKey;
	
	[XmlAttribute]
	public string primaryKeyTable;

	// [XmlIgnore("View")]	
	//[XmlElement("Join")]
	//public Join join;
	
	// [XmlIgnore("View")]
	// public View view;
}

public class Table{
	[XmlAttribute("name")]
	public string name;
	
	[XmlElement("AggExclude")]
	public AggExclude aggExclude;
}

public class AggExclude{
	[XmlAttribute("name")]
	public string aggExcludeName; 
}

public class Join{
	[XmlAttribute]
	public string leftKey;
	[XmlAttribute]
	public string rightKey;
	
	[XmlElement("Table")]
	public List<Table> tables;
}

public class Level
{
	[XmlAttribute("name")]
	public string levelName;
	[XmlAttribute]
	public string table;
	
	[XmlAttribute]
	public string column;
	[XmlAttribute]
	public bool uniqueMembers;
	
	[XmlAttribute]
	public string levelType;
	
	[XmlAttribute]
	public string type;
	
	[XmlElement("Property")]
	public List<Property> properties;
	
	[XmlElement("KeyExpression")]
	public KeyExpression keyExpression;
	
}

public class KeyExpression{
	[XmlElement("SQL")]
	public List<SQL> sql;
}

public class SQL{
	[XmlAttribute]
	public string dialect;
	[XmlTextAttribute]
	public string value;
}


public class Property
{
	[XmlAttribute("name")]
	public string propertyName;
	[XmlAttribute]
	public string column;
	[XmlAttribute]
	public string type;
}


