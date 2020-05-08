using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageSwitching : UIElemSwitchValue
{

    public Image ImageToSwitch;
    public Color EnabledColor;
    public Color DisabledColor;


    public override void Switch()
    {
        
        base.Switch();
        if (ImageToSwitch!=null)
        {
            if (State)

            ImageToSwitch.color = EnabledColor;
            else

                ImageToSwitch.color = DisabledColor;
        }
    

    }
    public override void Switch(bool enabled)
    {

        base.Switch(enabled);
        if (ImageToSwitch != null)
        {
            if (State)

                ImageToSwitch.color = EnabledColor;
            else

                ImageToSwitch.color = DisabledColor;
        }


    }
}
