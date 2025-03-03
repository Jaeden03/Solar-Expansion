using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiTabs : MonoBehaviour
{
    public int selected;
    public GameObject ExpandablePannel;
    private bool active = false;
    public Compound[] options;
    public Compound currentlySelected;
    public ElementPercent mixElements;

    public bool exclusive = true;
    public UiTabs[] otherTabs;

    public TextMeshProUGUI currentText;
    private string[] sizes = new string[] { "Giant", "Planet", "Dwarf" };

    // Start is called before the first frame update
    void Start()
    {
        ToggleTab(true);
        currentText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        if (SolarSystem.currentlySelected)
        {
            if (options.Length > 0) 
            { 
                selected = SolarSystem.currentlySelected.crustNumber; 
                currentText.text = options[selected].names[0];
            }
            else 
            { 
                selected = SolarSystem.currentlySelected.sizeCategory;
                currentText.text = sizes[selected];
            }
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mixElements) { currentlySelected = mixElements.primaryPercent;}
        
        if (currentlySelected) 
        { currentText.text = currentlySelected.names[0]; }
        
    }

    public void ToggleTab(bool allFalse = false)
    {
        if (allFalse) { active = true; }
        active = !active;

        if (active && exclusive)
        { 
            foreach(UiTabs tab in otherTabs) 
            { tab.ToggleTab(true); } 
        }

        ExpandablePannel.SetActive(active);
    }

    public void Select(int val)
    {
        selected = val;

        if (SolarSystem.currentlySelected)
        {
            if (options.Length > 0) 
            { 
                currentlySelected = options[selected];
                //SolarSystem.currentlySelected.crustNumber = selected;
            }
            else 
            { 
                currentText.text = sizes[selected];
                SolarSystem.currentlySelected.sizeCategory = selected;
            }
        }
        
    }
}
