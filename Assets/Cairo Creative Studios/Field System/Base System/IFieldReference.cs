using UnityEngine;

namespace FieldSystem
{
    public interface IFieldReference<TField, TTile, TObject, TProperties> where TTile : FieldTile<TField, TTile, TObject, TProperties> where TObject : FieldObject<TField, TTile, TObject, TProperties> where TField : Field<TField, TTile, TObject, TProperties> where TProperties : FieldProperties<TField>
    {
        public int index { get; set; }
        public TField field { get; set; }
        public Vector2Int position { get; set; }
    }
}