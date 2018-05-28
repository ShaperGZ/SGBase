using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnMouseOverCalCell : MonoBehaviour {
    public PlaningScheme scheme = null;
    public string message = null;
    public Vector3 labelPos = Vector3.zero;
    public Camera calCamera;
    public GameObject textObject;


    RectTransform typeABar,typeBBar,typeCBar, ttlSalesBar;


    // Use this for initialization
    void Start () {
        textObject = GameObject.Find("CalCellInfo");
        typeABar = (RectTransform)GameObject.Find("TypeACountBar").transform;
        typeBBar = (RectTransform)GameObject.Find("TypeBCountBar").transform;
        typeCBar = (RectTransform)GameObject.Find("TypeCCountBar").transform;
        ttlSalesBar = (RectTransform)GameObject.Find("TtlSalesBar").transform;
    }
	
	// Update is called once per frame
	void Update () {
        //labelPos = calCamera.WorldToScreenPoint(transform.position);
	}

    private void OnMouseOver()
    {
        message = scheme.Format();

        typeABar.sizeDelta = new Vector2(scheme.counts[0] * 10, 15);
        typeBBar.sizeDelta = new Vector2(scheme.counts[1] * 10, 15);
        typeCBar.sizeDelta = new Vector2(scheme.counts[2] * 10, 15);
        ttlSalesBar.sizeDelta = new Vector2(scheme.TotalSales*0.001f, 15);
        textObject.GetComponent<Text>().text = message;
    }
    private void OnMouseExit()
    {
        message = null;
        textObject.GetComponent<Text>().text = "";
    }
}
