using UnityEngine;
using System.Collections;

public class SetAsListChild : MonoBehaviour
{

   

    void Update()
    {
        if (transform.GetSiblingIndex() != transform.parent.childCount-1)
        {

            transform.SetAsLastSibling();
        }
    }
}
