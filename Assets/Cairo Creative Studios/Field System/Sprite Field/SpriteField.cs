using System;
using System.Linq;
using FieldSystem;
using Renderers;
using UnityEngine;

namespace SpriteFieldBase
{
    [ExecuteAlways]
    public class SpriteField : Field<SpriteField, SpriteTile, SpriteObject, FieldProperties<SpriteField>>
    {
        public SpriteSheetRenderer.AnimationProperties tileSprite = new();

        public override void Render()
        {
            if(Grid.Count != Grid.width * Grid.height){
                var children = transform.GetComponentsInChildren<SpriteTile>().Select(x => x.gameObject);
                Array.ForEach(children.ToArray(), x => DestroyImmediate(x));
                GenerateField();
            }

            if(!generated && tileSprite != null && tileSprite.spriteSheet != null)
            {
                GenerateField();
                generated = true;
            }

            if(tileSprite.spriteSheet != null)
            {
                tileSprite.spriteSheet.name = "Tile Sprite";
                tileSprite.rows = (int)(tileSprite.spriteSheet.height / tileSize.y);
                tileSprite.columns = (int)(tileSprite.spriteSheet.width / tileSize.x);
            }
            base.Render();
        }

        public override void GenerateField()
        {
            base.GenerateField();
            for(int tileIndex = 0; tileIndex < Grid.width * Grid.height; tileIndex++)
            {
                var i = tileIndex % Grid.width;
                var j = tileIndex / Grid.width;

                var tile = Grid[i, j];

                var spriteTile = (SpriteTile)tile.value;
                spriteTile.spriteRenderer = spriteTile.GetComponent<SpriteSheetRenderer>();
                spriteTile.spriteRenderer.currentAnimation = tileSprite;
                spriteTile.OnValidate();
            }
        }
    }
}