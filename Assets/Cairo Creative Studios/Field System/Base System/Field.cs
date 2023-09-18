using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FieldSystem
{
    [ExecuteAlways]
    public class Field<TField, TTile, TObject, TProperties> : MonoBehaviour, IGrid<FieldTile<TField, TTile, TObject, TProperties>> where TTile : FieldTile<TField, TTile, TObject, TProperties> where TObject : FieldObject<TField, TTile, TObject, TProperties> where TField : Field<TField, TTile, TObject, TProperties> where TProperties : FieldProperties<TField>
    {
        public TProperties properties;
        public UnityEvent<TObject, TTile> OnObjectAdded = new();
        public UnityEvent<TObject, TTile> OnObjectRemoved = new();
        public UnityEvent<TObject, TTile> OnObjectMoved = new();
        public UnityEvent<FieldTile<TField, TTile, TObject, TProperties>, object[]> OnTileModified = new();

        public TObject[] this[int x, int y] => GetFieldObjectsAtPosition(x, y);
        public TObject[] this[Vector2Int position] => GetFieldObjectsAtPosition(position.x, position.y);
        public TObject[] this[int index] => GetFieldObjectsAtPosition(x: index % Grid.width, y: index / Grid.width);

        [SerializeField]
        private Grid<FieldTile<TField, TTile, TObject, TProperties>> _grid = new(10, 10);
        public Grid<FieldTile<TField, TTile, TObject, TProperties>> Grid { get => _grid; private set => _grid = value; }
        public Dictionary<TTile, List<TObject>> objects = new();
        public Vector2 tileSize = new Vector2(40, 24);
        protected bool generated = false;

        void Update()
        {
            Render();
        }

        void OnValidate()
        {
            Array.ForEach(Grid.ToArray(), x => x.value?.OnValidate()); // Array
        }

        [Button("Generate Properties")]
        public void GenerateProperties()
        {
            properties = ScriptableObject.CreateInstance<TProperties>();
            #if UNITY_EDITOR
            AssetDatabase.CreateAsset(properties, $"Assets/Field Properties/{typeof(TProperties).Name}.asset");
            Selection.activeObject = properties;
            #endif
        }
        
        [Button("Generate Field")]
        public virtual void GenerateField()
        {
            if(properties == null)
            {
                Debug.LogError("The Field Properties have not been set, please set them before attempting to generate the Field");
                return;
            }

            Grid.OnItemAdded.AddListener((index, gridNode) =>
            {
                gridNode.value.field = (TField)this;
                gridNode.value.gridNode = gridNode;
            });
            for(int tileIndex = 0; tileIndex < Grid.width * Grid.height; tileIndex++)
            {
                var i = tileIndex % Grid.width;
                var j = tileIndex / Grid.width;
                var tile = AddTile(i, j);
                objects.Add(tile, new List<TObject>());
            }
            generated = true;
        }

        public virtual void Render()
        {
            foreach(var tile in Grid.ToArray())
            {
                if(tile != null && tile.value != null)
                    tile.value.Render();
                else
                    Grid.Remove(tile);

                foreach(var obj in objects[(TTile)tile.value])
                {
                    obj.position = tile.value.position;
                    obj.Render();
                }
            }
        }

        TTile AddTile(int x, int y)
        {
            var tile = new GameObject("Tile").AddComponent<TTile>();
            tile.transform.parent = transform;
            tile.index = x + y * Grid.width;
            tile.position = new Vector2Int(x, y);
            ((IGrid<FieldTile<TField, TTile, TObject, TProperties>>)this).SetValueAt(x, y, tile);
            return tile;
        }

        /// <summary>
        /// Gets the Tile at the specified position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public TTile GetTile(int x, int y)
        {
            return ((IGrid<TTile>)this).GetValueAt(x, y);
        }

        /// <summary>
        /// Creates an Object at the specified position
        /// </summary>
        /// <returns></returns>
        public TObject CreateFieldObject(int x, int y)
        {
            var obj = new GameObject(nameof(TObject)).AddComponent<TObject>();
            OnObjectAdded?.Invoke(obj, GetTile(x, y));
            return obj;
        }

        /// <summary>
        /// Gets the Field Object matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TObject[] GetFieldObjectsAtPosition(Predicate<TObject> predicate)
        {
            return objects.SelectMany(x => x.Value).Where(x => predicate(x)).ToArray();
        }

        public TObject[] GetFieldObjectsAtPosition(int x, int y)
        {
            return objects[GetTile(x, y)].ToArray();
        }

        /// <summary>
        /// Removes the given Object from the Field
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveObject(TObject obj)
        {
            objects.Where(x => x.Value.Contains(obj)).FirstOrDefault().Value.Remove(obj);
            OnObjectRemoved?.Invoke(obj, obj.tile);
        }

        /// <summary>
        /// Adds the given Object to the Field at the specified position
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddObject(TObject obj, int x, int y)
        {
            var tile = GetTile(x, y);
            if(tile == null)
                return;
            if(!objects.ContainsKey(tile))
                objects.Add(tile, new List<TObject>());
            objects[tile].Add(obj);
            obj.tile = tile;
            OnObjectAdded?.Invoke(obj, tile);
        }

        public void MoveObject(TObject obj, int x, int y)
        {
            RemoveObject(obj);
            AddObject(obj, x, y);
            OnObjectMoved?.Invoke(obj, GetTile(x, y));
        }

        /// <summary>
        /// Get the Field's Tiles as an Array
        /// </summary>
        /// <returns></returns>
        public TTile[] GetTileArray()
        {
            return Grid.ToArray().Select(x => (TTile)x.value).ToArray();
        }

        /// <summary>
        /// Get's all the Tiles adjacent to the given Tile
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public TTile[] GetAdjacentTiles(TTile tile)
        {
            var adjacentTiles = new List<TTile>();
            var x = tile.position.x;
            var y = tile.position.y;
            if(x > 0)
                adjacentTiles.Add(GetTile(x - 1, y));
            if(x < Grid.width - 1)
                adjacentTiles.Add(GetTile(x + 1, y));
            if(y > 0)
                adjacentTiles.Add(GetTile(x, y - 1));
            if(y < Grid.height - 1)
                adjacentTiles.Add(GetTile(x, y + 1));
            return adjacentTiles.ToArray();
        }

        /// <summary>
        /// Searches the Field for all tiles that match the given predicate, starting from the given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TTile[] SearchTilesFromPointMatchingPredicate(int x, int y, Predicate<TTile> predicate)
        {
            var tile = GetTile(x, y);
            var adjacentTiles = GetAdjacentTiles(tile).ToList();
            List<TTile> tiles = new();

            while(adjacentTiles.Count > 0)
            {
                foreach(var adjacentTile in adjacentTiles.ToArray())
                {
                    adjacentTiles.Remove(adjacentTile);
                    if(predicate(adjacentTile) && !tiles.Contains(adjacentTile)){
                        tiles.Add(adjacentTile);
                        adjacentTiles.AddRange(GetAdjacentTiles(tile));
                    }
                }
            }

            return tiles.ToArray();
        }

        /// <summary>
        /// Gets all the Tiles in the given Row of the Field Grid
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public TTile[] GetRow(int y)
        {
            return Grid.GetRow(y).Select(x => (TTile)x.value).ToArray();
        }

        /// <summary>
        /// Returns all Tiles in the given Row that match the given Predicate
        /// </summary>
        /// <param name="y"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TTile[] GetRowMatchingPredicate(int y, Predicate<TTile> predicate)
        {
            return Grid.GetRow(y).Select(x => (TTile)x.value).Where(x => predicate(x)).ToArray();
        }

        /// <summary>
        /// Gets all the Tiles in the given Column of the Field Grid
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public TTile[] GetColumn(int x)
        {
            return Grid.GetColumn(x).Select(x => (TTile)x.value).ToArray();
        }

        /// <summary>
        /// Returns all Tiles in the given Column that match the given Predicate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TTile[] GetColumnMatchingPredicate(int x, Predicate<TTile> predicate)
        {
            return Grid.GetColumn(x).Select(x => (TTile)x.value).Where(x => predicate(x)).ToArray();
        }

        /// <summary>
        /// Gets all the Tiles within the given Range from the given position in the Field Grid
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public TTile[] GetRange(int x, int y, int range)
        {
            return Grid.GetRange(x, y, range).Select(x => (TTile)x.value).ToArray();
        }

        /// <summary>
        /// Returns all Tiles in the given Range that match the given Predicate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="range"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TTile[] GetRangeMatchingPredicate(int x, int y, int range, Predicate<TTile> predicate)
        {
            return Grid.GetRange(x, y, range).Select(x => (TTile)x.value).Where(x => predicate(x)).ToArray();
        }
    }
}