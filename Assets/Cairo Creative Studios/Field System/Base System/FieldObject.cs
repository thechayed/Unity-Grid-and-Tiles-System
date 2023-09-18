using UnityEngine;

namespace FieldSystem
{
    [ExecuteAlways]
    public class FieldObject<TField, TTile, TObject, TProperties> : MonoBehaviour, IFieldReference<TField, TTile, TObject, TProperties> where TTile : FieldTile<TField, TTile, TObject, TProperties> where TObject : FieldObject<TField, TTile, TObject, TProperties> where TField : Field<TField, TTile, TObject, TProperties> where TProperties : FieldProperties<TField>
    {
        public int index { get; set; }
        public TField field { get; set; }
        public TTile tile { get; set; }
        public Vector2Int position { get; set; }
        public virtual void Render()
        {
            transform.position = new Vector3(tile.transform.position.x + field.properties.objectOffset.x, tile.transform.position.y + field.properties.objectOffset.y, tile.transform.position.z + field.properties.objectOffset.z);
        }
    }
}