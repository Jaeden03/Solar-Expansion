using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UiTracking : MonoBehaviour, IPointerDownHandler
{
    public Planet target;


    private Slider mySlider;
    private float previousSliderValue;

    private Vector2 startScale;
    //private float minScale;
    //private float maxScale;

    [HideInInspector]public RectTransform myRect;
    private Image myImage;

    [HideInInspector]public bool haveRings = false;
    private RectTransform Rings;
    private float maxPlanetSize = 10f;

    // Start is called before the first frame update
    void Start()
    {
        //var startScale = transform.localScale.x;
        //minScale = startScale;
        //maxScale = startScale  * 2;

        myRect = GetComponent<RectTransform>();
        //minScale = myRect.rect.width;
        //maxScale = myRect.rect.width * 10f;

        mySlider = GetComponentInChildren<Slider>();
        if (mySlider) { MessyGuessy(); }

        //if (stationary) { minScale = 20f; maxScale = 40f; }
        //else { transform.SetSiblingIndex(1); }

        
    }

    public void MessyGuessy()
    {
        float min = SolarSystem.theSun.minBodyDist;
        float max = SolarSystem.theSun.maxBodyDist;
        float desiredValue = target.orbitDistance;
        float inc = 0.01f;

        var guess = 1f; 
        
        for (float f = 0; f < 1f; f+= inc)
        {
            var currentGuess = guess * SolarSystem.zoomWeight.Evaluate(guess); 
            var newGuess = f * SolarSystem.zoomWeight.Evaluate(f);
            var d1 = Mathf.Lerp(min, max, currentGuess);
            var d2 = Mathf.Lerp(min, max, newGuess);
            if (Mathf.Abs(desiredValue - d1) > Mathf.Abs(desiredValue - d2))
            { guess = f; }
        }
        
        mySlider.value = guess;
        previousSliderValue = mySlider.value;
    }

    public void ChangePlanetDistance()
    {
        var scaledSlider = mySlider.value * SolarSystem.zoomWeight.Evaluate(mySlider.value);
        float newValue = Mathf.Lerp(SolarSystem.theSun.minBodyDist, SolarSystem.theSun.maxBodyDist, scaledSlider);
        //var oldValue = currentlySelected.myOrbit.semiMajorAxis;
        //currentlySelected.SetOrbitDistance(newValue);

        previousSliderValue = mySlider.value;
        target.SetOrbitDistance(newValue);

        SelectThis();

        //UpdateIconOrder();

        //foreach (UiTabs tab in myTabs) { tab.ToggleTab(true); }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target)        
        {
            //var planetSizeScale = Mathf.Lerp(0.2f, 1.23f, target.transform.localScale.x / maxPlanetSize);
            if(mySlider)
            {
                if (mySlider.value != previousSliderValue)
                { ChangePlanetDistance(); }
            }
            else
            { 
                var screenSpot = Camera.main.WorldToScreenPoint(target.transform.position);
                screenSpot.z = 0f;
                myRect.transform.position = screenSpot;
                //var newScale = Mathf.Lerp(maxScale, minScale, SolarSystem.screenScale) * planetSizeScale;
                //var newScale = planetSizeScale * 69f;
                //myRect.sizeDelta = new Vector2(newScale, newScale);
            }

            SetIconSize(target.sizeCategory);

        }

    }

    public void OnPointerDown (PointerEventData eventData) 
    {
        if (mySlider) { SelectThis(); }
    }

    public void SelectThis()
    {
        SolarSystem.currentlySelected = target;
        SolarSystem.InitializeSelectionUi();
    }

    public void SetSprite(Sprite newSprite)
    {
        myImage = GetComponentInChildren<Image>();
        myImage.sprite = newSprite;
        startScale = myImage.rectTransform.sizeDelta;

        Rings = myImage.transform.Find("Rings").GetComponent<RectTransform>();
        if (!haveRings) { Rings.gameObject.SetActive(false); }
    }

    public void SetColor(Color c)
    {
        myImage.color = c;
    }


    public void SetIconSize(int sizeCategory)
    {

        float scale = 40f;
        if (sizeCategory == 0) { scale *= 1f; }
        if (sizeCategory == 1) { scale *= 0.5f; }
        if (sizeCategory == 2) { scale *= 0.25f; }
        if (target.planetName == "THE SUN") { scale *= 2f; }

        if (mySlider)
        {
           myImage.rectTransform.sizeDelta = new Vector2(scale, scale -20);
        }
        else
        {
            scale *= SolarSystem.zoomWeight.Evaluate(1.5f - SolarSystem.screenScale);
            myImage.rectTransform.sizeDelta = new Vector2(scale, scale);
        }

        if (haveRings) { Rings.sizeDelta = myImage.rectTransform.sizeDelta * 2; }
        

        
    }

    public float GetIconWidth()
    {
        return myImage.rectTransform.rect.width * 2; // * transform.localScale.x;
    }
}
