using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{

    public Canvas Canvas;
 public   RectTransform GamePanel;
    Vector2 originalSize;

    public float ZoomMin;
    public float ZoomMax;

    public float Speed;

    Vector2 pointerPrevPos;

   
 public   Vector2 BoundsMin;
    public Vector2 BoundsMax;


    Vector2 originalPos;// = (Vector2)GamePanel.transform.localPosition;
    // Start is called before the first frame update
    void Start()
    {
        originalSize = GamePanel.sizeDelta;
        originalPos = (Vector2)GamePanel.localPosition;
        BoundsMin = (Vector2)GamePanel.transform.localPosition - originalSize / 2;
        BoundsMax = (Vector2)GamePanel.transform.localPosition + originalSize / 2;
    
    }

    // Update is called once per frame
    void Update()
    {
        float mouse = Input.GetAxis("Mouse ScrollWheel");

        if (mouse != 0)
        {
            GamePanel.sizeDelta += (Vector2.one * mouse * Speed)/2;

            //GamePanel.offsetMin -=(Vector2.one * mouse * Speed) / 2;

            GamePanel.sizeDelta = new Vector2(Mathf.Clamp(GamePanel.sizeDelta.x,0, 200), Mathf.Clamp(GamePanel.sizeDelta.y, 0, 200));
             Vector2 size = GamePanel.rect.size;
        
            Vector2 sizeParent = Canvas.GetComponent<RectTransform>().rect.size;

            Vector2 delta = size- sizeParent;

            BoundsMin = originalPos - delta;
            BoundsMax = originalPos; // + delta ;

           
            FindObjectOfType<GameController>().Resize();


           

            //Debug.Log("DELTA" + delta + "ORIGINAL SIZE"+size +"    "+sizeParent);


        }
        if (Input.GetMouseButtonDown(2))
        {

            Vector2 mousePos = Input.mousePosition;

            pointerPrevPos = mousePos;

        }
            if (Input.GetMouseButton(2))
        {

            Vector2 mousePos = Input.mousePosition;
            Vector3 delta = mousePos - pointerPrevPos;
            Vector3 position = GamePanel.transform.localPosition + delta * 2;
           position = new Vector3(Mathf.Clamp(position.x, BoundsMin.x, BoundsMax.x), Mathf.Clamp(position.y, BoundsMin.y, BoundsMax.y), position.z);

            GamePanel.transform.localPosition=position;
            //Vector2 offsetMin = GamePanel.offsetMin + delta;
         
            //Vector2 offsetMax = GamePanel.offsetMax + delta;

      
            //offsetMin = new Vector2(Mathf.Clamp(offsetMin.x, -GamePanel.sizeDelta.x/2, GamePanel.sizeDelta.x/2),

            //   Mathf.Clamp(offsetMin.y, -GamePanel.sizeDelta.y / 2, GamePanel.sizeDelta.y / 2));


            //offsetMax = new Vector2(Mathf.Clamp(offsetMax.x, -GamePanel.sizeDelta.x/2, GamePanel.sizeDelta.x/2),

            //   Mathf.Clamp(offsetMax.y, -GamePanel.sizeDelta.y / 2, GamePanel.sizeDelta.y / 2));

            //GamePanel.offsetMin = offsetMin;
            //GamePanel.offsetMax= offsetMax;
            pointerPrevPos = mousePos;
        }
    }
}
