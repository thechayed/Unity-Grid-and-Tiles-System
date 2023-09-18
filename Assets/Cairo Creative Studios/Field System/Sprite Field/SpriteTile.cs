using FieldSystem;
using Renderers;
using UnityEngine;

namespace SpriteFieldBase
{
    [RequireComponent(typeof(SpriteSheetRenderer))]
    public class SpriteTile : FieldTile<SpriteField, SpriteTile, SpriteObject, FieldProperties<SpriteField>>
    {
        public SpriteSheetRenderer spriteRenderer;
        private Vector3 scale = new Vector3(1, 1, 1);

        void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteSheetRenderer>();
        }

        public override void Render()
        {
            if(spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteSheetRenderer>();

            scale.x = field.tileSize.x;
            scale.z = field.tileSize.y;
            spriteRenderer.transform.localScale = scale;
            transform.position = new Vector3(gridNode.x * field.tileSize.x, 0, gridNode.y * field.tileSize.y);
        }

        override public void OnValidate()
        {
            if(spriteRenderer != null)
                spriteRenderer.ForceUpdate();
        }
    }
}