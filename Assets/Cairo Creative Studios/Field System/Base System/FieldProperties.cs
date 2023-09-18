using UnityEngine;

namespace FieldSystem
{
    [CreateAssetMenu(fileName = "FieldProperties", menuName = "Cairo Creative/Field System/Field Properties")]
    public class FieldProperties<TField> : ScriptableObject
    {
        [Tooltip("The Size of the Field in Tiles, is used when Generating the Field's Grid")]
        public Vector2Int fieldSize = new Vector2Int(10, 10);
        [Tooltip("The Offset from the center of the tile to the center of the object")]
        public Vector3 objectOffset = new Vector3(0, 0, 0);
    }
}