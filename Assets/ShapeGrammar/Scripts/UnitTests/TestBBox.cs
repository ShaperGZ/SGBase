using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;

public class TestBBox : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {

        GameObject prefab = Resources.Load("Components\\uinit") as GameObject;

        Vector3[] pts = new Vector3[4];
        Vector3 v1 = (new Vector3(2, 0, 1)).normalized;
        float scale = 2;

        pts[0] = Vector3.zero;
        pts[1] = pts[0] + v1 * scale;
        pts[2] = pts[1] + Vector3.up * scale * 2;
        pts[3] = pts[0] + Vector3.up * scale * 2;
        Polygon pg = new Polygon(pts);
        ShapeObject so = ShapeObject.CreateMeshable(pg, v1);

        BoundingBox bbox = so.meshable.bbox;
        print(bbox.vects[0]);
        print(bbox.vects[1]);
        print(bbox.vects[2]);
        print(bbox.vertices[0]);
        print(bbox.vertices[3]);
        GenerateObjects(prefab, bbox,30,30);

    }

    private static void GenerateObjects(GameObject prefab, BoundingBox bbox, int countW, int countH)
    {
        Vector3 org = bbox.position;
        float totalW = bbox.size[0];
        float totalH = bbox.size[1];
        float stepW = totalW / (float)countW;
        float stepH = totalH / (float)countH;
        print("stepW=" + stepW);
        print("stepH=" + stepH);

        Vector3 vectW = bbox.vects[0] * stepW;
        Vector3 vectH = bbox.vects[1] * stepH;
        Vector3 vectD = bbox.vects[2] * 1;

        for (int i = 0; i < countH; i++)
        {
            for (int j = 0; j < countW; j++)
            {
                Vector3 bp = org + (vectW * j) + (vectH * i);
                Vector3[] ptsi = new Vector3[2];
                ptsi[0] = bp;
                ptsi[1] = ptsi[0] + vectW + vectH + vectD;
                BoundingBox bboxi = BoundingBox.CreateFromPoints(ptsi, vectW);

                ShapeObject shpo = ShapeObject.CreateBasic(prefab);
                shpo.ConformToBBoxTransform(bboxi);
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
