using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;
using SGGeometry;

public class ProgramRatioVisualizer : MonoBehaviour {

    RectTransform rect;
    List<GameObject> bars;
    List<GameObject> texts;
    public float totalHGet;
    private void Awake()
    {
        rect = (RectTransform)transform;
    }
    public void SetRatio(List<ShapeObject> sos)
    {
        Dictionary<string, float> namedArea = new Dictionary<string, float>();
        Dictionary<string, Color> namedColors = new Dictionary<string, Color>();
        List<string> names = new List<string>();
        List<float> areas = new List<float>();
        List<Color> colors = new List<Color>();

        foreach(ShapeObject so in sos)
        {
            if (so.meshable.GetType() == typeof(Extrusion))
            {
                Extrusion ext = (Extrusion)so.meshable;
                float area = ext.polygon.Area();
                if (!namedArea.ContainsKey(so.name))
                {
                    namedArea[so.name] = area;
                    namedColors[so.name] = so.GetComponent<MeshRenderer>().material.color;
                }
                else namedArea[so.name] += area;
            }
        }
        foreach(KeyValuePair<string,float> kv in namedArea)
        {
            names.Add(kv.Key);
            areas.Add(kv.Value);
        }
        foreach(KeyValuePair<string, Color> kv in namedColors)
        {
            colors.Add(kv.Value);
        }

        SetRatio(areas.ToArray(), names.ToArray(), colors.ToArray());
    }

    public void SetRatio(float[] areas, string[] names, Color[] colors)
    {
        if (!gameObject.activeSelf) return;
        float total = 0;
        float[] ratios = new float[areas.Length];
        float[] heights = new float[areas.Length];
        foreach (int area in areas) total += area;

        float hTotal = rect.sizeDelta[1];
        totalHGet = hTotal;

        DefaultControls.Resources rc = SceneManager.uiResources;
        float lowest = 0;
        for (int i = 0; i < ratios.Length; i++)
        {
            #region cal ratio
            ratios[i] = areas[i] / total;
            float h = ratios[i] * hTotal;
            #endregion

            #region GameObject management
            if (bars == null) bars = new List<GameObject>();
            if (texts == null) texts = new List<GameObject>();
            int dif = bars.Count - ratios.Length;
            if (dif > 0) SGUtility.RemoveExtraGameObjects(ref bars, dif);
            if (dif > 0) SGUtility.RemoveExtraGameObjects(ref texts, dif);
            
            if (i >= bars.Count)
            {
                GameObject bar = DefaultControls.CreateImage(rc);
                bar.transform.parent = transform;
                int cIndex = i % SchemeColor.ColorSetDefault.Length;
                bars.Add(bar);
            }
            if (i >= texts.Count)
            {
                GameObject text = DefaultControls.CreateText(rc);
                text.transform.parent = transform;
                int cIndex = i % SchemeColor.ColorSetDefault.Length;
                text.GetComponent<Text>().text = "";
                texts.Add(text);
            }
            #endregion

            //bars[i].GetComponent<Image>().material.color = SchemeColor.ColorSetDefault[i];
            bars[i].GetComponent<Image>().color = colors[i];
            RectTransform barRect = (RectTransform)bars[i].transform;
            barRect.anchorMax = new Vector2(0, 1);
            barRect.anchorMin = new Vector2(0, 1);
            barRect.pivot = new Vector2(0, 1);
            barRect.localScale = new Vector3(1, 1, 1);
            barRect.sizeDelta = new Vector2(20, h);
            barRect.anchoredPosition = new Vector2(0, lowest);

            string message = string.Format("{0}: {1}% area={2}sqm",
                names[i],
                Mathf.Round(ratios[i] * 100),
                areas[i]);
            
            Text t = texts[i].GetComponent<Text>();
            t.text = message;
            t.fontSize = 8;
            t.color = Color.white;
            RectTransform textRect = (RectTransform) texts[i].transform;
            textRect.anchoredPosition = new Vector2(35, lowest);
            textRect.anchorMax = new Vector2(0, 1);
            textRect.anchorMin = new Vector2(0, 1);
            textRect.pivot = new Vector2(0, 1);
            textRect.localScale = new Vector3(1, 1, 1);
            textRect.sizeDelta = new Vector2(500, 30);
            textRect.anchoredPosition = new Vector2(30, lowest);

            lowest -= h;
        }

        
        



        
        
    }
}
