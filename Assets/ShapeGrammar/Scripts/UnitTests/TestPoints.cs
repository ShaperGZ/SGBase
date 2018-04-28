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

    // Use this for initialization
    void Start () {
        //PointGrid(10, 20);

        Vector3 p1 = new Vector3(0, 0, 0);
        Vector3 p2 = new Vector3(1, 0, 0);
        Vector3 p3 = new Vector3(1, 0, 2);
        Vector3 p4 = new Vector3(1, 0, 1);

        SGGeometry.Intersect.LineLine2D(p1, p2, p3, p4);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

   
}
