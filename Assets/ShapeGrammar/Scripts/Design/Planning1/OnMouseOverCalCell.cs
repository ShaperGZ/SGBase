using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnMouseOverCalCell : MonoBehaviour {
    public float countA = 0;
    public float countB = 0;
    public float countC = 0;
    public float dif = 0;
    public float gfa = 0;
    public float totalSales = 0;

    public string message = null;
    public Vector3 labelPos = Vector3.zero;
    public Camera calCamera;
    public GameObject textObject;


    RectTransform typeABar,typeBBar,typeCBar;


    // Use this for initialization
    void Start () {
        textObject = GameObject.Find("CalCellInfo");
        typeABar = (RectTransform)GameObject.Find("TypeACountBar").transform;
        typeBBar = (RectTransform)GameObject.Find("TypeBCountBar").transform;
        typeCBar = (RectTransform)GameObject.Find("TypeCCountBar").transform;
	}
	
	// Update is called once per frame
	void Update () {
        labelPos = calCamera.WorldToScreenPoint(transform.position);
	}

    private void OnMouseOver()
    {
        message = string.Format("A:{0},B:{1},C:{2},dif={3},gfa-{4}",countA,countB,countC,dif,gfa);

        typeABar.sizeDelta = new Vector2(countA * 10, 15);
        typeBBar.sizeDelta = new Vector2(countB * 10, 15);
        typeCBar.sizeDelta = new Vector2(countC * 10, 15);

        textObject.GetComponent<Text>().text = message;
    }
    private void OnMouseExit()
    {
        message = null;
        textObject.GetComponent<Text>().text = "";
    }
}
