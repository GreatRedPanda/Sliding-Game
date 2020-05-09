using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stone : MonoBehaviour, IPointerClickHandler
{

    public event System.Action<Stone> OnStoneClick;
    public event System.Action<Stone, bool> OnStoneMoving;
    // * GenUtil.Parent.transform.localScale.x
    public static float Speed { get { return GenUtil.StoneSpeed; } }
    Vector2 shadowAnchoredPos;
    public static Vector2 OffsetMin;
    public static Vector2 OffsetMax;
    public static Vector2 PrefabSize;



    public bool Interactable = true;

    public RectTransform Shadow;

    RectTransform stoneRectTr;
    public Image TargetImage;
    public Color SelectedColor;
    public Color UnSelectedColor;
    bool isMoving = false;

    public Cell CellParent { get { return GetComponentInParent<Cell>(); } }

  public  Vector2 originalSize;


    Animator animator;

    Transform _newParent;

    private void Start()
    {
        //animator = GetComponent<Animator>();
        GameController.Instance.SubscribeToStoneEvent(this);

        Vector2 newSize = GetComponent<RectTransform>().rect.size;
        Vector2 coef = newSize / PrefabSize;

       
        Shadow.offsetMin *= coef;
        Shadow.offsetMax *= coef;

        shadowAnchoredPos = Shadow.anchoredPosition;


        stoneRectTr = GetComponent<RectTransform>();
         ResetAnimation();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
       if(Interactable)
        OnStoneClick?.Invoke(this);
    }

    public void ResetAnimation()
    {
        //if (animator != null)
        {
            if (Interactable)
                Shadow.anchoredPosition = shadowAnchoredPos;
            else
                Shadow.anchoredPosition = Vector2.zero;


        }

    }

    public void Appear(bool appear)
    {

        if (appear)
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one));
        }
        else
        {

            transform.localScale = Vector3.one;

            iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.zero, "oncomplete", "completeAnimation"));
        }
    }

    void completeAnimation()
    {
        gameObject.SetActive(false);

    }
    public void SetInteractable(bool interactable)
    {

        Interactable = interactable;
        GetComponent<Graphic>().raycastTarget = interactable;


    }
    public void StartMove(Transform newParent,  bool inPit,bool isBack)
    {
       // Selected(false);
        _newParent = newParent;
         transform.SetParent(_newParent.parent);


        if (isBack)
        {
            if (inPit)
            {
                if (!GameManager.FastStoneBackMove && !GameManager.FastStoneMove)
                    iTween.ValueTo(gameObject, iTween.Hash(
                      "from", Shadow.anchoredPosition,
                      "to", shadowAnchoredPos,
                      "time", 0.5f,
                      "onupdate", "itweenUpdate", "oncomplete", "startMove"
                      )
                      );
                else
                {
                    Shadow.anchoredPosition = shadowAnchoredPos;
                    endMove();
                }
            }
            else
            {
                if (!GameManager.FastStoneBackMove && !GameManager.FastStoneMove)
                {

                    startMove();
                }
                else
                {

                    endMove();
                }
            }
        }
        else
        {
            if (!GameManager.FastStoneMove)
                startMove();
            else
                endMove();
        }
    }



    void itweenUpdate(Vector2 val)
    {

        Shadow.anchoredPosition = val;
    }
    void posUpdate(Vector2 pos)
    {

        stoneRectTr.anchoredPosition = pos;
        //transform.localPosition= Vector3.Scale(transform.localPosition, GenUtil.Scale);
    }

    void startMove()
    {
        // isMoving = true;
        // OnStoneMoving?.Invoke(this, true);
        // iTween.MoveTo(gameObject, iTween.Hash(

        // "position", _newParent.position,
        // "speed", GenUtil.StoneSpeed, "easetype", iTween.EaseType.linear,
        //"oncomplete", "endMove", "onupdate", "posUpdate", "islocal", true
        // ));


        //iTween.ValueTo(gameObject, iTween.Hash(
        //"from", transform.position,
        //"to", _newParent.position,
        //"time", Vector2.Distance(transform.position, _newParent.position) / GenUtil.StoneSpeed,
        //"onupdate", "posUpdate", "oncomplete", "endMove"
        //));

        iTween.ValueTo(gameObject, iTween.Hash(
      "from", stoneRectTr.anchoredPosition,
      "to", _newParent.parent.GetComponent<RectTransform>().sizeDelta/2
         + _newParent.GetComponent<RectTransform>().anchoredPosition,
      "speed", GenUtil.StoneSpeed, "easetype", iTween.EaseType.linear,
      "onupdate", "posUpdate", "oncomplete", "endMove"
      ));

    }

    void endMove()
    {
                    transform.SetParent(_newParent);
                    transform.localPosition = Vector2.zero;
                    isMoving = false;
                    OnStoneMoving?.Invoke(this, false);
                    CellParent.StoneGotToCell();
                    if (!Interactable)
                        iTween.ValueTo(gameObject, iTween.Hash(
            "from", Shadow.anchoredPosition,
            "to", (!Interactable) ? Vector2.zero : shadowAnchoredPos,
            "time", 0.5f,
            "onupdate", "itweenUpdate", "oncomplete", "completeITweenAnimation"
            )
            );

       
    }

    void completeITweenAnimation()
    { }
    public void  Selected(bool selected)
    {
      Image img= GetComponent<Image>();

        if (TargetImage != null)
        {

            if (selected)
            {
                TargetImage.color = SelectedColor;

            }
            else
            {

                TargetImage.color = UnSelectedColor;
            }
        }
    }
}
