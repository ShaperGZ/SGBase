using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnSelect(int i);
public delegate void OnClick();

public class ExpandableList : MonoBehaviour {

    // Use this for initialization
    // Transform btTransform;
    RectTransform btAddTransform;
    RectTransform btSubTransform;
    RectTransform pnContent;
    List<GameObject> items = new List<GameObject>();
    Transform btHolder;
    Text selectedIndexText;
    public int selectedIndex = -1;
    public OnSelect onSelectCallbacks;
    public OnClick onAddClick;
    public OnClick onSubClick;
    private void Awake()
    {
        Load();
    }

    void Start () {
        //Load();
        
    }
    public void Load()
    {
        btHolder = transform.Find("BtHolder") as Transform;
        btAddTransform = transform.Find("BtHolder/BtAdd") as RectTransform;
        btSubTransform = transform.Find("BtHolder/BtSub") as RectTransform;
        pnContent = transform.Find("ContentPanel") as RectTransform;
        
        btAddTransform.GetComponent<Button>().onClick.AddListener(delegate { onAddClick(); });
        btSubTransform.GetComponent<Button>().onClick.AddListener(delegate { onSubClick(); });
        
        selectedIndexText = transform.Find("BtHolder/SelectIndex").GetComponent<Text>() as Text;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void AddItem(string name = null)
    {
        if (name == null) name = "item" + items.Count.ToString();
        //print("ContentPanel.deltaSize:" + pnContent.sizeDelta);
        //Vector2 size = ((RectTransform)transform).sizeDelta;
        //size.y += 20;
        Vector2 offset = new Vector2(0, 20);
        Vector3 offset3 = new Vector3(0, 20,0);
        pnContent.sizeDelta += offset;
        int i = items.Count;
        GameObject item = Instantiate(UICreator.GetDefaultButtonAsset(),pnContent);
        item.GetComponent<Button>().onClick.AddListener(delegate { SelectItem(i); });
        float pos = items.Count * 20 +5;
        RectTransform trans = item.transform as RectTransform;
        trans.anchoredPosition = new Vector2(0, -pos);
        Vector2 size = new Vector2(200, 20);
        trans.sizeDelta = size;
        trans.Find("Text").GetComponent<Text>().text = name;
        //trans.parent = pnContent;
        //trans.position = new Vector3(0, pos, 0);
        //print(trans.position);
        items.Add(item);
    }
    public void SubItem()
    {
        //Vector2 size = ((RectTransform)transform).sizeDelta;
        //size.y += 20;
        Vector2 offset = new Vector2(0, 20);
        Vector3 offset3 = new Vector3(0, 20, 0);
        pnContent.sizeDelta -= offset;
    }
    public void SelectItem(int i)
    {
        selectedIndex = i;
        selectedIndexText.text = selectedIndex.ToString();
        if(onSelectCallbacks!=null)
            onSelectCallbacks(i);
    }
    public void Clear()
    {
        items.Clear();
        selectedIndex = -1;
    }
}
