using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;

public class BSIDES
{
    const int SOUTH = 111;
    const int SOUTHTOP = 1111;
    const int SOUTHBOT = 1112;
    const int NORTH = 222;
    const int NORTHTOP = 2221;
    const int NORTHBOT = 2222;
    const int SOUNTHNORTH = 333;
    const int SOUNTHNORTHTOP = 3331;
    const int SOUNTHNORTHBOT = 3332;
    const int EAST = 444;
    const int EASTTOP = 4441;
    const int EASTBOT = 4442;
    const int WEST = 555;
    const int WESTTOP = 5551;
    const int WESTBOT = 5552;
    const int EASTWEST = 666;
    const int EASTWESTTOP = 6661;
    const int EASTWESTBOT = 6662;
    const int CORNER = 777;
    const int CORNERTOP = 7771;
    const int CORNERBOT = 7772;
}

public class ThemeIndice
{
    public List<int> top;
    public List<int> main;
    public List<int> bot;
    public ThemeIndice()
    {
        top = new List<int>();
        main = new List<int>();
        bot = new List<int>();
    }
}

public class ShapeObjectM : ShapeObject {

    List<ThemeIndice> themeIndices;
    List<GameObject> gameObjects;
    List<ShapeObject> components;
    
	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    public void Clear()
    {
        if (components != null)
        {
            foreach (ShapeObject som in components)
            {
                som.Clear();
            }
            components.Clear();
        }
        
        if (gameObjects != null)
        {
            foreach (GameObject o in gameObjects)
            {
                try
                {
                    Destroy(o);
                }
                catch { }
            }
            gameObjects.Clear();
        }
        else gameObjects = new List<GameObject>();
        if (themeIndices != null) themeIndices.Clear();
        else themeIndices = new List<ThemeIndice>();
    }
    private void OnDestroy()
    {
        Clear();
    }
    public List<GameObject> createGridCount(GameObject prefab, int uCount,int vCount, out List<ThemeIndice> indices)
    {
        List<GameObject> gos = new List<GameObject>();
        indices = new List<ThemeIndice>();
        indices.Add(new ThemeIndice());//primary theme
        indices.Add(new ThemeIndice());//secondary theme

        float stepU = 1f / (float)uCount;
        float stepV = 1f / (float)vCount;
        Vector3 unitLocalScale = new Vector3(stepU, stepV, 1);

        //float sizeU = size.x / uCount;
        //float sizeV = size.y / vCount;

        for (int i = 0; i < vCount; i++)
        {
            for (int j = 0; j < uCount; j++)
            {
                //if (i == 0 && j == 0) continue;
                GameObject o = Instantiate<GameObject>(prefab, transform);
                o.transform.localScale = unitLocalScale;
                o.transform.localPosition = new Vector3(j * stepU, i * stepV, 0);
                gos.Add(o);

                if(j==0 || j == uCount - 1)
                {
                    if (i > 0 && i < vCount - 1) indices[0].main.Add(gos.Count - 1);
                    else if(i==0) indices[0].bot.Add(gos.Count - 1);
                    else indices[0].top.Add(gos.Count - 1);
                }
            }
        }
        
        return gos;

    }
    public static ShapeObjectM CreateSchemeA(Form form)
    {
        GameObject container = new GameObject();
        ShapeObjectM som = container.AddComponent<ShapeObjectM>();
        GameObject prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh mesh = prefab.GetComponent<MeshFilter>().mesh;
        Vector3[] pts = mesh.vertices;
        for (int i = 0; i < pts.Length; i++)
        {
            pts[i] += new Vector3(0.5f, 0.5f, 0.5f);
        }
        mesh.vertices = pts;

        //first must move container in place
        BoundingBox bbox;
        if (form.bbox != null) bbox = form.bbox;
        else bbox = BoundingBox.CreateFromPoints(form.vertices);

        container.transform.position = bbox.position;
        container.transform.LookAt(bbox.vertices[3]);

        Debug.LogFormat("component count={0}", form.components.Count);
        foreach(Meshable m in form.components)
        {
            Polygon pg = (Polygon)m;
            //find and skip horizontal components
            float dot = Vector3.Dot(pg.GetNormal(), Vector3.up);
            Debug.LogFormat("dot={0}", dot);
            //if (dot == 1 || dot != -1) continue;
            if(dot<0.8 && dot > -0.8f)
            {
                //proces the vertical components
                ShapeObjectM ct = ShapeObjectM.CreateArrayDim(prefab, pg, 6, 3);
                ct.transform.parent = container.transform;
            }
            
        }

        
        prefab.SetActive(false);
        return som;
    }
    public static ShapeObjectM CreateArrayDim(GameObject prefab, Polygon pg, float w, float h)
    {
        if (pg.vertices.Length != 4) return null;
        Vector3 pos = pg.vertices[0];
        Vector3 normal = pg.GetNormal();
        float scaleX = (pg.vertices[1] - pg.vertices[0]).magnitude;
        float scaleY = (pg.vertices[3] - pg.vertices[0]).magnitude;
        Vector3 scale = new Vector3(scaleX, scaleY, 1);
        int uCount = (int)Mathf.Round( scaleX / w);
        int vCount = (int)Mathf.Round(scaleY/h);
        return CreateArrayCount(prefab, pos, normal, uCount, vCount, scale);
    }
    public static ShapeObjectM CreateArrayCount(GameObject prefab, Polygon pg, int uCount, int vCount)
    {
        if (pg.vertices.Length != 4) return null;
        Vector3 pos = pg.vertices[0];
        Vector3 normal = pg.GetNormal();
        float scaleX = (pg.vertices[1] - pg.vertices[0]).magnitude;
        float scaleY = (pg.vertices[3] - pg.vertices[0]).magnitude;
        Vector3 scale = new Vector3(scaleX, scaleY, 1);
        return CreateArrayCount(prefab, pos, normal, uCount, vCount, scale);


    }
    public static ShapeObjectM CreateArrayCount(GameObject prefab, Vector3 position, Vector3 forward, int uCount, int vCount,Vector3 size)
    {
        List<GameObject> gos = new List<GameObject>();
        GameObject container = new GameObject();
        ShapeObjectM som = container.AddComponent<ShapeObjectM>();
        //GameObject unit = Instantiate<GameObject>(prefab,container.transform);
        //gos.Add(unit);
        float stepU = 1f / (float)uCount;
        float stepV = 1f / (float)vCount;
        Vector3 unitLocalScale = new Vector3(stepU, stepV,1);

        //float sizeU = size.x / uCount;
        //float sizeV = size.y / vCount;

        for(int i = 0; i < vCount; i++)
        {
            for (int j = 0; j < uCount; j++)
            {
                //if (i == 0 && j == 0) continue;
                GameObject o = Instantiate<GameObject>(prefab, container.transform);
                o.transform.localScale = unitLocalScale;
                o.transform.localPosition = new Vector3(j*stepU, i*stepV,0);
                gos.Add(o);
            }
        }
        som.gameObjects = gos;
        som.transform.localScale = size;
        som.transform.position = position;
        som.transform.LookAt(position + forward);
       
        return som;

    }
    
    private void OnRenderObject()
    { }

}
