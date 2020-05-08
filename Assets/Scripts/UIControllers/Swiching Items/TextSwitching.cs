using UnityEngine;
using System.Collections;
using TMPro;

public class TextSwitching : UIElemSwitchValue
{

    public TextMeshProUGUI TextToSwitch;

    public string KeyEnabled;
    public string KeyDisabled;

    public override void Switch(bool enabled)
    {
        base.Switch(enabled);
        Localize();
    }


    public void Localize()
    {

         if (TextToSwitch != null)
        {
            if (LocalizationManager.Instance != null)
            {
                if (State)
                    TextToSwitch.text =UIController.getLocal(KeyEnabled, KeyEnabled);
                else
                    TextToSwitch.text = UIController.getLocal(KeyDisabled, KeyDisabled);
            }

        }


        ImageSwitching iSw = GetComponent<ImageSwitching>();
        if (iSw != null)
            iSw.Switch(State);

   }
}
