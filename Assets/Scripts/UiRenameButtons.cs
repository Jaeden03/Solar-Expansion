using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiRenameButtons : MonoBehaviour
{

    public UiTabs myTabs;

    // Start is called before the first frame update
    void Awake()
    {
        if (myTabs == null)
        { myTabs = GetComponentInParent<UiTabs>(); }
    }

    void OnEnable()
    {
        if (myTabs)
        {
            var elementButtons = GetComponentsInChildren<TextMeshProUGUI>(true);
            var middle = elementButtons.Length / 2;
            for (int i = 0; i < elementButtons.Length; i++) 
            {
                elementButtons[i].text = myTabs.options[i].names[0];
                /*  
                if (i != middle)
                {
                    if (i > middle) { elementButtons[i].text = myTabs.options[i-1].names[0]; }
                    else { elementButtons[i].text = myTabs.options[i].names[0]; }
                }
                */
                
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
