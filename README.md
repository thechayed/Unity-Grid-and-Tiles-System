# Unity Grid and Tiles System
A Field System that enhances gameplay in Unity projects by facilitating the dynamic management of objects and tiles within grid-based environments.
This is good for games that require a Grid Based Battle System, as it vastly simplifies the approach for setting up unique grids and interactions.

## Installation
Download the .UnityPackage and import it into your project. UPM version coming soon.

## Features
- An extensive suite of Grid based Tiled Game System functionality.
- Fields (extends IGrid, see [Unity Generic Grid](https://github.com/thechayed/Unity-Generic-Grid)): The Grid that is used to as the basis for generating the Tiles and Field Objects. Tiles and Objects are created as children of the Field.
  - Extend the Field, MeshField, or SpriteField to add additional functionality.
  - Fields are created via the Add Component button in the GameObject Inspector.
  - Field Generation is done via the "Generate Field" button in the Field's Inspector (Field Properties must be set).
  - Contains OnTileModified, OnObjectAdded, OnObjectRemoved, and OnObjectMoved Events, as well as Unity-Generic-Grid events for Tiles including: OnItemAdded, OnItemMoved, and OnItemRemoved.
``` c#
// This demonstrates how to set up a custom Field that set's Tile Text values to "Hello, World!" on Generation.
public class MyField : Field<MyField, MyTile, MyObject, MyProperties>
{
  public override void GenerateField()
  {
    foreach(var tile in Grid)
    {
      if(tile.index % 2 == 0)
        tile.text = "Hello, World!";
      tile.transform.position = new Vector3(tile.position.x * properties.tileSize.x, 0, tile.position.y * properties.tileSize.z);
    }
  }
}
```
- Field Tiles: The Field's Grid consists of Grid Nodes who's Values are Field Tiles. The Fields have many methods for interacting with these Tiles by default.
  - Extend Field Tiles to add unique properties to them, and customize the way in which they are rendered.
  - Tiles are generated automatically by the Field when the "Generate Field" button is clicked.
``` c#
// This demonstrates how to set up a custom Field Tile with a custom Property, and how it's Rendering can be customized
public class MyTile : FieldTile<MyField, MyTile, MyObject, MyProperties>
{
  public string text = "";

  void Start()
  {
    text = field.properties.defaultText;
  }

  public override void Render()
  {
    tile.transform.eulerAngles = new Vector3(0, Mathf.Sin(Time.time) * 15, 0);
  }

// You can also override ModifyTile to add some standard modification functionality to the Tile
  public override void ModifyTile(params object[] args)
  {
    base.ModifyTile(args);
    text = (string)args[0];
  }
}
```
- Field Properties: Field Properties allow you to store Data for the Field
  - Extend Field Properties to add more data for the Field
  - You can also simply use the "Generate Properties" button in the Field's Inspector to create the Properties Asset, but as it extends Scriptable Object, you can also use the [CreateAssetMenu] attribute.
``` c#
// This demonstrates how to set up Properties for your custom Field.

[CreateAssetMenu(fileName = "MyFieldProperties", menuName = "My Field System/My Field Properties")]
public class MyFieldProperties : MyFieldProperties<MyField>
{
  public string defaultText = "FooBar";
  public GameObject fieldObjectPrefab;
}
```
- Field Objects: Field Objects are rendered above the Tiles on the Field. this can be any number of things, from units in a TBS game, or marker in a Grid Based map, or the selection in a Menu, and so on.
  - Extend Field Objects to customize how the Objects should be rendered.
  - To Create an Object, you must call the Field's CreateFieldObject(x, y) method. This will create the Object above the Tile at the given Position, and return it.
``` c#
// This demonstrates how to create a Field Object that instantiates a Prefab to use for rendering the Object, 
// and then moving it on Render.

public class MyFieldObject : FieldObject<MyField, MyTile, MyObject, MyProperties>
{
  GameObject instance;

  void start()
  {
    instance = GameObject.Instantiate(field.properties.fieldObjectPrefab);
    instance.transform.parent = transform;
  }

  public override void Render()
  {
    instance.transform.localPosition = tile.GetObjectPosition() + new Vector3(0, Mathf.Sin(Time.time),0);
  }
}
```

There are many features in the Field System, and I will document more in the future. Please let me know if you have any issues, and [follow me on Twitter if you want to talk](https://twitter.com/TheChayed)
