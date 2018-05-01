using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPoints : MonoBehaviour {

    void PointGrid(int wCount, int hCount)
    {
        float w = 10;
        float h = 10;

        for (int i = 0; i < wCount; i++)
        {
            for (int j = 0; j < hCount; j++)
            {
                float posX = w * (float)j;
                float posZ = h * (float)i;
                Vector3 p = new Vector3(posX, 0, posZ);
                SOPoint.CreatePoint(p);

            }
        }
    }
    public Vector3[] boundary;
    ShapeObject so;
    ShapeObject xp;
    public bool isInside = false;


    // Use this for initialization
    void Start () {

        //PointGrid(10, 20);

        //Vector3 p1 = new Vector3(0, 0, 0);
        //Vector3 p2 = new Vector3(1, 0, 0);
        //Vector3 p3 = new Vector3(1, 0, 2);
        //Vector3 p4 = new Vector3(1, 0, 1);

        //SGGeometry.Intersect.LineLine2D(p1, p2, p3, p4);

        boundary = new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(10,0,0),
            new Vector3(10,0,10),
            new Vector3(-2,0,12),
        };

        so = SOPoint.CreatePoint();
        xp = SOPoint.CreatePoint();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = so.Position;
        Vector3 csp = SGGeometry.SGUtility.PolylineClosesPoint(boundary, pos);
        xp.Position = csp;
        //Debug.Log(SGGeometry.SGUtility.PointInBoundaryA(boundary, pos));
	}

    private void OnRenderObject()
    {
        SGGeometry.GLRender.Polyline(boundary, true, null, Color.black);
    }
}
