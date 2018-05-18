using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class DrawDimension : MonoBehaviour {

    ShapeObject so;
	// Use this for initialization
	void Start () {
        so = GetComponent<ShapeObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnPostRender()
    {
        BoundingBox bbox = so.meshable.bbox;
        float offset = 3;

        Vector3 p1 = bbox.vertices[0];
        Vector3 p2 = bbox.vertices[1];

        Vector3 n = bbox.vects[2] * -1 * offset;

        Vector3 p3 = p1 + n;
        Vector3 p4 = p2 + n;

        SGGeometry.GLRender.Polyline(new Vector3[] { p1, p3, p4, p2 },false,null,Color.white);
    }
}
