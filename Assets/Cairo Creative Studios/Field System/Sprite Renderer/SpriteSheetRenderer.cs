using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Renderers
{
    [ExecuteAlways]
    public class SpriteSheetRenderer : MonoBehaviour
    {
        public AnimationProperties this[string animationName]
        {
            get => animations.Find(x => x.name == animationName);
            set
            {
                var index = animations.FindIndex(x => x.name == animationName);
                if(index != -1)
                {
                    animations[index] = value;
                }
                else
                {
                    animations.Add(value);
                }
            }
        }

        public AnimationProperties currentAnimation;
        public List<AnimationProperties> animations = new List<AnimationProperties>();
        public enum ShaderType
        {
            Lit,
            Unlit
        }
        public ShaderType shaderType = ShaderType.Lit;

        public Action<AnimationProperties> OnAnimationChange;


        [DoNotSerialize] public int Frame { get => _frameIndex; set => _frameIndex = value; }
        [DoNotSerialize] public int Speed { get => currentAnimation.speed; set => currentAnimation.speed = value; }
        [DoNotSerialize] public bool Loop { get => currentAnimation.loop; set => currentAnimation.loop = value; }
        [DoNotSerialize] public bool Flip { get => currentAnimation.flip; set => currentAnimation.flip = value; }

        private GameObject _planeObject;
        private MeshRenderer _renderer;
        private ShaderType _lastShaderType = ShaderType.Lit;
        [HideInInspector]
        public Material _litMaterial;
        [HideInInspector]
        public Material _unlitMaterial;
        [SerializeField]
        private int _frameIndex = 1;
        private float _timer = 0;
        private Material _generatedMaterial;
        private Vector2 _textureOffset = new Vector2(0, 0);
        private Vector2 _textureScale = new Vector2(1, 1);

        void Start()
        {
            var _planeTransform = transform.Find("Plane");
            _planeObject = _planeTransform == null ?  new GameObject("Plane") : _planeTransform.gameObject;
            
            _planeObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            _planeObject.transform.parent = transform;

            // Add Components
            MeshFilter meshFilter = _planeObject.AddComponent<MeshFilter>();
            _renderer = _planeObject.AddComponent<MeshRenderer>();

            // Create the Plane Mesh
            Mesh mesh = new Mesh();
            meshFilter.mesh = mesh;

            // Create the verts
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(-0.5f, 0, -0.5f),
                new Vector3(0.5f, 0, -0.5f),
                new Vector3(0.5f, 0, 0.5f),
                new Vector3(-0.5f, 0, 0.5f)
            };

            // Create the UV coordinates
            Vector2[] uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };

            // Create the tris
            int[] triangles = new int[]
            {
                0, 2, 1,
                0, 3, 2
            };

            // Assign verts and tris to the Mesh
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;


            // Set Material based on Shader Type
            if(shaderType == ShaderType.Lit)
            {
                _generatedMaterial = new Material(_litMaterial);
            }
            else
            {
                _generatedMaterial = new Material(_unlitMaterial);
            }
            _renderer.material = _generatedMaterial;
        }

        void Update()
        {
            if(_lastShaderType != shaderType)
            {
                ChangeShader();
                _lastShaderType = shaderType;
            }

            if(currentAnimation == null)
            {
                if(animations.Count > 0)
                    SetAnimation(animations[^1].name);
            }

            if(!Application.isPlaying)
            {
                UpdateFrame();
            }
        }

        void FixedUpdate()
        {
            UpdateFrame();
        }
        
        void ChangeShader()
        {
            if(shaderType == ShaderType.Lit)
            {
                _generatedMaterial = new Material(_litMaterial);
            }
            else
            {
                _generatedMaterial = new Material(_unlitMaterial);
            }
        }

        public void SetAnimation(string name)
        {
            var animation = animations.Find(x => x.name == name);
            if(animation != null)
            {
                if(currentAnimation != null)
                    currentAnimation.OnAnimationEnd?.Invoke(currentAnimation);

                currentAnimation = animation;
                _frameIndex = 1;
                SetupMaterial();
                OnAnimationChange?.Invoke(currentAnimation);
            }
        }

        void SetupMaterial()
        {
            if(_generatedMaterial != null)
            {
                _generatedMaterial.mainTexture = currentAnimation.spriteSheet;

                _textureScale.x = 1f / currentAnimation.columns;
                _textureScale.y = 1f / currentAnimation.rows;

                _generatedMaterial.SetTextureScale("_BaseMap", _textureScale);
            }
        }

        public void ForceUpdate()
        {
            SetupMaterial();
        }

        void UpdateFrame()
        {
            if(_generatedMaterial == null)
                ChangeShader();

            if(currentAnimation == null)
                return;

            _timer ++;

            if(currentAnimation.speed > 0 && _timer >= 60 / currentAnimation.speed)
            {
                _timer = 0;

                if(_frameIndex == 1)
                    currentAnimation.OnAnimationStart?.Invoke(currentAnimation);

                if(_frameIndex < currentAnimation.rows * currentAnimation.columns)
                {
                    _frameIndex++;
                }
                else{
                    if(currentAnimation.loop)
                    {
                        _frameIndex = 1;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            _textureOffset.x = (float)_frameIndex % currentAnimation.columns / currentAnimation.columns;
            _textureOffset.y = 1f - (float)(_frameIndex / currentAnimation.columns) / currentAnimation.rows;

            _generatedMaterial.SetTextureOffset("_BaseMap", _textureOffset);
            currentAnimation.OnAnimationUpdate?.Invoke(currentAnimation);
        }

        [Serializable]
        public class AnimationProperties
        {
            public string name;
            public Texture spriteSheet;
            private Vector2 _scale;
            public Vector2 scale {
                get{
                    return _scale;
                }
                set{
                    _scale = value;
                    renderer.transform.localScale = new Vector3(_scale.x, _scale.y, 1);
                }
            }
            [SerializeField]
            private int _rows;
            public int rows { get => Mathf.Clamp(_rows, 1, Mathf.Abs(_rows)); set => _rows = Mathf.Clamp(value, 1, Mathf.Abs(value));}
            [SerializeField]
            private int _columns;
            public int columns { get => Mathf.Clamp(_columns, 1, Mathf.Abs(_columns)); set => _columns = Mathf.Clamp(value, 1, Mathf.Abs(value));}
            public int speed;
            public bool loop;
            public bool flip;
            public Action<AnimationProperties> OnAnimationStart;
            public Action<AnimationProperties> OnAnimationUpdate;
            public Action<AnimationProperties> OnAnimationEnd;

            private SpriteSheetRenderer renderer;
        }
    }
}