using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGGeometry;
using SGCore;

public class TestIntersection : MonoBehaviour {
    public string message = "";
    private void OnGUI()
    {
        GUI.Label(new Rect(100, 100, 300, 300), message);
    }
    public ShapeObject so;
    public Vector3[] pts = new Vector3[3];
    public ShapeObject xp;
    // Use this for initialization
    void Start () {
        so = SOPoint.CreatePoint(new Vector3(0, 0, 0));
        xp = SOPoint.CreatePoint(new Vector3(1000, 1000, 10000));

        pts[0] = new Vector3(0, 0, 0);
        pts[1] = new Vector3(50, 0, 0);
        pts[2] = new Vector3(50, 0, 50);
        //sos[3] = SOPoint.CreatePoint(new Vector3(20, 0, 30));

	}
	
	// Update is called once per frame
	void Update ()
    {
        xp.Position = SGGeometry.SGUtility.PolylineClosesPoint(pts, so.Position);
    }
    private void OnRenderObject()
    {
        SGGeometry.GLRender.Polyline(pts,true,null, Color.black);
        
    }
}
