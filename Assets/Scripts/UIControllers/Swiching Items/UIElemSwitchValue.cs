using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIElemSwitchValue : MonoBehaviour
{
    public bool State=true;

    public virtual void Switch(bool enabled)
    {

        State = enabled;

    }
    public virtual void Switch()
    {

        State = !State;

    }
}
