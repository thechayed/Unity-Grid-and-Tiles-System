using System;
using System.Collections.Generic;
using System.Linq;
using FieldSystem;
using Unity.VisualScripting;

namespace MeshFieldBase
{
    public class MeshFieldBase : Field<MeshFieldBase, MeshTileBase, MeshObjectBase, MeshFieldPropertiesBase>
    {
        public int tileLength = 10;
        public override void GenerateField()
        {
            base.GenerateField();
        }
    }
    public class MeshField<TField, TTile> : MeshFieldBase where TField : MeshFieldBase where TTile : MeshTileBase
    {
        /// <summary>
        /// Get the Tiles at the Given Z Index
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public TTile[] GetTilesAtZ(int z)
        {
            return GetTileArray().Where(x => x.Z == z).OfType<TTile>().ToArray();
        }

        /// <summary>
        /// Gets Tiles Adjacent to the Given Tile position according to the Z Axis index given
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public TTile[] SearchTilesOnZ(int x, int y, int z)
        {
            var tile = GetTile(x, y);
            var adjacentTiles = GetAdjacentTiles(tile).ToList();
            List<TTile> tiles = new();

            while(adjacentTiles.Count > 0)
            {
                foreach(var adjacentTile in adjacentTiles.ToArray())
                {
                    adjacentTiles.Remove(adjacentTile);
                    if(adjacentTile.Z == z && !tiles.Contains(adjacentTile)){
                        tiles.Add((TTile)adjacentTile);
                        adjacentTiles.AddRange(GetAdjacentTiles(tile));
                    }
                }
            }

            return tiles.ToArray();
        }

        /// <summary>
        /// Gets the Tiles on the given Row that are on the given Z Index
        /// </summary>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public TTile[] GetRowOnZ(int y, int z)
        {
            return GetRow(y).Where(x => x.Z == z).OfType<TTile>().ToArray();
        }

        /// <summary>
        /// Get's the Tiles on the given Column that are on the given Z Index
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public TTile[] GetColumnOnZ(int x, int z)
        {
            return GetColumn(x).Where(x => x.Z == z).OfType<TTile>().ToArray();
        }

        /// <summary>
        /// Get's a Range of Tiles that are on the given Z Index
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public TTile[] GetRangeOnZ(int x, int y, int z, int length)
        {
            return GetRange(x, y, length).Where(x => x.Z == z).OfType<TTile>().ToArray();
        }

        /// <summary>
        /// Searches all the Tiles in the Given Z Range, stepping through the Z Range and aquiring all adjacent Tiles that match.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public TTile[] SearchTilesOnZRange(int x, int y, int z, int range)
        {
            var tile = GetTile(x, y);
            var adjacentTiles = GetAdjacentTiles(tile).ToList();
            List<TTile> tiles = new();

            while(adjacentTiles.Count > 0)
            {
                foreach(var adjacentTile in adjacentTiles.ToArray())
                {
                    adjacentTiles.Remove(adjacentTile);
                    if(adjacentTile.Z >= z - range && adjacentTile.Z <= z + range && !tiles.Contains(adjacentTile)){
                        tiles.Add((TTile)adjacentTile);
                        adjacentTiles.AddRange(GetAdjacentTiles(tile));
                    }
                }
            }

            return tiles.ToArray();
        }

        /// <summary>
        /// Searches the Field for Tiles that match the given Predicate, starting from the given position and limited to the given Z Range
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="range"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TTile[] SearchTilesOnZRange(int x, int y, int z, int range, Predicate<TTile> predicate)
        {
            var tile = GetTile(x, y);
            var adjacentTiles = GetAdjacentTiles(tile).ToList();
            List<TTile> tiles = new();

            while(adjacentTiles.Count > 0)
            {
                foreach(var adjacentTile in adjacentTiles.ToArray())
                {
                    adjacentTiles.Remove(adjacentTile);
                    if(adjacentTile.Z >= z - range && adjacentTile.Z <= z + range && predicate((TTile)adjacentTile) && !tiles.Contains(adjacentTile)){
                        tiles.Add((TTile)adjacentTile);
                        adjacentTiles.AddRange(GetAdjacentTiles(tile));
                    }
                }
            }

            return tiles.ToArray();
        }
    }
}