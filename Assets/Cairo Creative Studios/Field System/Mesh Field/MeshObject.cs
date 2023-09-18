using FieldSystem;

namespace MeshFieldBase
{
    public class MeshObjectBase : FieldObject<MeshFieldBase, MeshTileBase, MeshObjectBase, MeshFieldPropertiesBase>
    {

    }

    public class MeshObject<TObject> : MeshObjectBase where TObject : MeshObjectBase
    {
    }
}