using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextWheeler : MonoBehaviour,IScrollHandler {
    public void OnScroll(PointerEventData eventData)
    {
        print("scrolling");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
}
