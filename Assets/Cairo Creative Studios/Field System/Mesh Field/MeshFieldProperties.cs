using FieldSystem;
using UnityEngine;

namespace MeshFieldBase
{
    public class MeshFieldPropertiesBase : FieldProperties<MeshFieldBase>
    {
        [Tooltip("The Prefab used to generate the Mesh Tiles")]
        public GameObject tilePrefab;
        [Tooltip("The Prefab used to generate the Mesh Objects")]
        public GameObject objectPrefab;
    }
    
    [CreateAssetMenu(fileName = "MeshFieldProperties", menuName = "Cairo Creative/Field System/Mesh Field Properties")]
    public class MeshFieldProperties<TProperties> : MeshFieldPropertiesBase where TProperties : MeshFieldPropertiesBase
    {
    }
}