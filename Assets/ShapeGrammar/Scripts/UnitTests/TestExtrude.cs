using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;

public class TestExtrude : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector3[] pts = new Vector3[]
        {
            new Vector3(),
            new Vector3(100,0,0),
            new Vector3(100,0,100),
            new Vector3(0,0,100)
        };


        Meshable ext = new Extrusion(pts, 100);
        ext.bbox = BoundingBox.CreateFromPoints(ext.vertices, new Vector3(1, 0, 0));
        Meshable[] splits = ext.SplitByPlane(new Plane(Vector3.up, new Vector3(0, 20, 0)));

        ShapeObject A = ShapeObject.CreateMeshable(splits[0]);
        ShapeObject B = ShapeObject.CreateMeshable(splits[1]);
        A.name = "A";
        B.name = "B";

        Debug.Log("A=" + A.meshable.ToString() + " " + A.meshable.GetType().ToString() + A.meshable.bbox.position);
        Debug.Log("B=" + B.meshable.ToString() + " " + B.meshable.GetType().ToString() + B.meshable.bbox.position);

        splits = SGUtility.DivideFormToLength(B.meshable, 4, 1);
        foreach (Meshable m in splits)
        {
            ShapeObject.CreateMeshable(m);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
