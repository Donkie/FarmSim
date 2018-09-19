using System;
using System.Collections;
using Assets.Components;
using Assets.FarmSim;
using Assets.FarmSim.I3D;
using Pfim;
using UnityEngine;

namespace Assets
{
    public class Entity : MonoBehaviour
    {
        public Shape Shape;
        [HideInInspector]
        public Texture2D Tex;
        
        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                GetComponent<MeshRenderer>().enabled = value;
            }
        }

        private bool VisibleInnerCheck(GameObject part)
        {
            while (true)
            {
                if (part.GetComponent<I3DTransform>().Visibility == false)
                    return false;

                if (part.transform.parent == null)
                    return true;

                part = part.transform.parent.gameObject;
            }
        }

        private bool DeepIsVisible()
        {
            if (!Shape || Shape.NonRenderable)
                return false;

            if (gameObject.transform.parent == null)
                return true;

            return VisibleInnerCheck(gameObject.transform.parent.gameObject);
        }
        
        public void Setup()
        {
            //Assign material
            if (Shape != null)
            {
                foreach (I3DMaterial shapeMaterial in Shape.Materials)
                {
                    if (shapeMaterial.TextureFile == null)
                        continue;

                    Material mat = GetComponent<Renderer>().material;
                    mat.mainTextureScale = new Vector2(1, -1);
                    try
                    {
                        mat.mainTexture = TextureLoader.GetTexture(shapeMaterial.TextureFile.AbsolutePath);
                    }
                    catch (UnityException e)
                    {
                        Debug.LogError($"Failed to parse texture {shapeMaterial.TextureFile.AbsolutePath}\n{e.Message}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to parse texture {shapeMaterial.TextureFile.AbsolutePath}\n{e.Message}");
                    }

                    if (shapeMaterial.NormalMapFile != null)
                        mat.SetTexture("_BumpMap", TextureLoader.GetTexture(shapeMaterial.NormalMapFile.AbsolutePath));

                    if (shapeMaterial.AlphaBlending)
                    {
                        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        mat.SetInt("_ZWrite", 0);
                        mat.DisableKeyword("_ALPHATEST_ON");
                        mat.DisableKeyword("_ALPHABLEND_ON");
                        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        mat.renderQueue = 3000;
                    }

                    //if (shapeMaterial.ReflectionMap != null)
                    //    mat.SetTexture("_Cube", LoadTexture(shapeMaterial.ReflectionMap.AbsolutePath));
                }
            }

            //Assign name
            name = GetComponent<I3DTransform>().Name;

            //Check visibility
            Visible = DeepIsVisible();
            //Visible = !Scene.NonRenderable;
            //Visible = true;
        }
    }
}
