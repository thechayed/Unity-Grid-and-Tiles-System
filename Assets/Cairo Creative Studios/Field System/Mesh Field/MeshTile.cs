using FieldSystem;
using UnityEngine;
using UnityEngine.Events;

namespace MeshFieldBase
{
    public class MeshTileBase : FieldTile<MeshFieldBase, MeshTileBase, MeshObjectBase, MeshFieldPropertiesBase>
    {
        public UnityEvent<MeshTileBase> OnTileZChanged = new();
        private int z = 0;
        /// <summary>
        /// Setting the Z Property will Invoke the OnTileZChanged Event
        /// </summary>
        public int Z
        {
            get => z;
            set
            {
                z = value;
                OnTileZChanged.Invoke(this);
            }
        }
        private Vector3 targetPosition = Vector3.zero;

        void OnEnable()
        {
            targetPosition = new Vector3(gridNode.x * field.tileSize.x, 0, gridNode.y * field.tileSize.y);
            transform.position = targetPosition;
        }

        public override void Render()
        {
            base.Render();
            targetPosition.y = z;
            transform.localPosition = targetPosition;
        }   
    }
    public class MeshTile<TTile> : MeshTileBase where TTile : MeshTileBase
    {
    }
}