using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderBehaviour : MonoBehaviour {
	Mesh mesh;
    int layer;
    Material material;
 
    const int countItems = 5;
 
    void Start () {
        mesh = GetComponent<MeshFilter> ().mesh;
        layer = gameObject.layer;
        material = GetComponent<MeshRenderer> ().material;
    }
 
    void Update () {
        for (var i = 0; i < countItems; ++i) {
            var cloned = Clone (material);
            var color = cloned.color;
            color.a = 0.5f * ((countItems - (float)i) / (float)countItems);
            cloned.color = color;
            {
				material.SetFloat ("_Mode", 2);
				material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt ("_ZWrite", 0);
				material.DisableKeyword ("_ALPHATEST_ON");
				material.EnableKeyword ("_ALPHABLEND_ON");
				material.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
			}
 
            Graphics.DrawMesh (mesh, Matrix4x4.Translate (new Vector3 ((i + 1.0f) * 1.5f, .0f, .0f)), cloned, layer);
        }
    }
 
    Material Clone (Material material) {
        var cloned = new Material (material.shader);
        // cloned.color = material.color;
        return cloned;
    }
}