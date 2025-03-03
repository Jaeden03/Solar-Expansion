using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElementPercent : MonoBehaviour
{
    public Slider oxygenSlider; 
    public TextMeshProUGUI oxygenText;
    [HideInInspector] public float oxygenPercent;

    public Slider waterSlider;
    public TextMeshProUGUI waterText;

    [HideInInspector] public float waterPercent;

    public Slider percentSlider;
    public TextMeshProUGUI atmosphereText;
    [HideInInspector] public float atmospherePercent;
    public TextMeshProUGUI crustText;
    [HideInInspector] public float crustPercent;

    public Compound Oxygen;
    public UiTabs AtmosphereTab;
    public Compound Water;
    public UiTabs CrustTab;
    [HideInInspector] public Compound primaryPercent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateValues()
    {
        if (oxygenSlider && oxygenText)
        { 
            oxygenPercent = oxygenSlider.value * 100;
            oxygenText.text = (int)(oxygenPercent) + "% Oxygen"; 
        }

        if (waterSlider && waterText)
        { 
            waterPercent = waterSlider.value * 100;
            waterText.text = "Water " + (int)(waterPercent) + "%"; 
        }

        if (percentSlider && atmosphereText && crustText)
        {
            atmospherePercent = percentSlider.value * 100;
            atmosphereText.text = "Atmosphere " + (int)(atmospherePercent) + "%";
            crustPercent = 100 - atmospherePercent;
            crustText.text = (int)crustPercent + "% Crust";
        }


        if (atmospherePercent > 50)
        {
            if (oxygenPercent >= 50) { primaryPercent = Oxygen; }
            else { primaryPercent = AtmosphereTab.currentlySelected; }
        }
        else 
        {
            if (waterPercent >= 50) { primaryPercent = Water; }
            else { primaryPercent = CrustTab.currentlySelected; }
        }
    }
}
