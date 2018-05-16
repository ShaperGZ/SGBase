using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class TestUtility : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector3[] pts = new Vector3[]
        {
            new Vector3(),
            new Vector3(30,0,-5),
            new Vector3(50,0,0),
            new Vector3(50,0,20),
            new Vector3(0,0,20),

        };

        Polygon pg = new Polygon(pts);
        ShapeObject.CreateMeshable(pg);
        Extrusion ext = pg.Extrude(new Vector3(0, 20, 0));

        ShapeObject refo = ShapeObject.CreateMeshable(ext);
        refo.transform.Translate(new Vector3(0, 20, 0));

        ext.bbox = BoundingBox.CreateFromPoints(ext.vertices, new Vector3(1, 0, 0));

        //ShapeObject.CreateMeshable(ext);
        //Meshable[] mbs = SGUtility.DivideFormByCount(ext, 3, 0);
        Meshable[] mbs = SGUtility.DivideFormToLength(ext, 10, 0);
        int counter = 0;
        foreach(Meshable mb in mbs)
        {
            if(mb.bbox==null)throw new System.Exception("null bbox");
            SGUtility.ScaleForm(mb, new Vector3(0.9f,0.3f,0.9f),Alignment.Center3D);
            ShapeObject so=ShapeObject.CreateMeshable(mb);
            
            //so.transform.Translate(new Vector3(counter*1, 10, 0));
            
            counter += 1;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
