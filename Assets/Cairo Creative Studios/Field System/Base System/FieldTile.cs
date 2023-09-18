using UnityEngine;

namespace FieldSystem
{
    [ExecuteAlways]
    public class FieldTile<TField, TTile, TObject, TProperties> : MonoBehaviour, IFieldReference<TField, TTile, TObject, TProperties> where TTile : FieldTile<TField, TTile, TObject, TProperties> where TObject : FieldObject<TField, TTile, TObject, TProperties> where TProperties : FieldProperties<TField> where TField : Field<TField, TTile, TObject, TProperties>
    {
        public GridNode<FieldTile<TField, TTile, TObject, TProperties>> gridNode;
        public int index { get; set; }
        public TField field { get; set; }
        public Vector2Int position { get; set; }
        
        /// <summary>
        /// Determines how the tile is rendered, this is usually called every frame
        /// </summary>
        public virtual void Render()
        {
        }

        /// <summary>
        /// A virtual OnValidate that updates the tile when the Field is updated
        /// </summary>
        public virtual void OnValidate()
        {
        }

        /// <summary>
        /// Extend to resolve a position directly above the tile
        /// </summary>
        /// <returns></returns>
        public virtual Vector3 GetObjectPosition()
        {
            return transform.position + field.properties.objectOffset;
        }
    }
}