using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SolarSystem : MonoBehaviour
{
    public static float gravitationalConstant = 6.6f;

    public static float simulationSpeed = 1f;
    //private float currentSimulationSpeed = 1f;

    public static float screenScale = 0.5f;

    public static Transform UniversalCanvas;

    public static Planet currentlySelected = null;

    public static AnimationCurve zoomWeight;
    public AnimationCurve zoomScaleLine;

    public GameObject optionMenu;
    private static GameObject SunOptions;
    public GameObject PlanetEditUi;
    public static GameObject EditUi;
    public static GameObject PlanetOptions;
    private static TextMeshProUGUI PlanetTitleText;
    private static TextMeshProUGUI PlanetDistanceText;

    public static Slider[] UiSliders;

    public GameObject sunPrefab;
    public static Planet theSun;

    public static List<UiTracking> planetSliders = new List<UiTracking>();

    public TextMeshProUGUI pauseButtonText;
    public static string pauseTxt = "||";

    //public RectTransform planetsLine;
    //public RectTransform AddButton;
    //public RectTransform RemoveButton;

    public static int UiIconIndex;

    private CamControl myCam;
    public static UiTabs[] myTabs;

    //private bool firstTime = true;

    public Compound waterPrefab;
    public static Compound water;
    public Compound oxygenPrefab;
    public static Compound oxygen;


    // Start is called before the first frame update
    void Start()
    {
        
        UniversalCanvas = transform;
        zoomWeight = zoomScaleLine;
        EditUi = PlanetEditUi;
        SunOptions = PlanetEditUi.transform.Find("Sun Options").gameObject;
        PlanetOptions = PlanetEditUi.transform.Find("Planet Options").gameObject;
        myTabs = GetComponentsInChildren<UiTabs>();

        water = waterPrefab;
        oxygen = oxygenPrefab;

        if (EditUi) 
        { 
            UiSliders = GetComponentsInChildren<Slider>();
            if (UiSliders[0]) { PlanetDistanceText = UiSliders[0].transform.GetComponentInChildren<TextMeshProUGUI>(); }

            PlanetTitleText = EditUi.transform.GetComponentInChildren<TextMeshProUGUI>();
            EditUi.SetActive(false);
        }

        theSun = Instantiate(sunPrefab, Vector3.zero, Quaternion.identity).GetComponent<Planet>();

        myCam = Camera.main.GetComponent<CamControl>();
    }

    // Update is called once per frame
    void Update()
    {
        //UiIconIndex = pauseButtonText.transform.parent.GetSiblingIndex();
    }

    void LateUpdate()
    {
        UiIconIndex = pauseButtonText.transform.parent.GetSiblingIndex();
        //Debug.Log(UiIconIndex);

        if (currentlySelected)
        {       
            if (!EditUi.activeSelf) { InitializeSelectionUi(); }

            if (PlanetDistanceText) 
            { 
                if (currentlySelected.planetName == "THE SUN") { PlanetDistanceText.text = ""; }
                else 
                { 
                    var dist = (int)Vector3.Distance(currentlySelected.transform.position, Vector3.zero);
                    PlanetDistanceText.text = dist + " million KMs from The Sun.";
                }
            }
        }

        //UpdateIconOrder();
        pauseButtonText.text = pauseTxt;
    }

    public static void Background()
    {
        currentlySelected = null;
        if (EditUi.activeSelf) 
        { 
            EditUi.SetActive(false);
            if (simulationSpeed == 0f) { TogglePause(); }

            //foreach (UiTabs tab in myTabs) { tab.ToggleTab(true); }
        }
        
    }
    public void AddPlanet()
    {
        theSun.AddBody();

        //UpdateIconOrder();

        //foreach (UiTabs tab in myTabs) { tab.ToggleTab(true); }
    }

    public void RemovePlanet()
    {
        theSun.RemoveBody();

        //UpdateIconOrder();

        //foreach (UiTabs tab in myTabs) { tab.ToggleTab(true); }
    }

    public static void TogglePause()
    {
        bool pause = true;
        if (simulationSpeed == 0f) { pause = false; }
        if (pause)
        {
            simulationSpeed = 0f; pauseTxt = ">";


            for (int i = 0; i < theSun.activeBods.Count(); i++)
            {
                if (theSun.activeBods[i].stats != null)
                    theSun.activeBods[i].stats.updateStats();
            }
        }
        else { simulationSpeed = 1f; pauseTxt = "||"; }

        foreach (UiTabs tab in myTabs) { tab.ToggleTab(true); }
        
    }

    public void ToggleOptionsMenu()
    {
        if (optionMenu.activeSelf) { optionMenu.SetActive(false); }
        else { optionMenu.SetActive(true); }
    }

    public void SelectSun()
    {
        currentlySelected = theSun;
        InitializeSelectionUi();
    }

    public static void InitializeSelectionUi()
    {
        EditUi.SetActive(true);
        PlanetTitleText.text = currentlySelected.planetName;

        if (currentlySelected.planetName == "THE SUN") 
        { PlanetOptions.SetActive(false); SunOptions.SetActive(true); }
        else { PlanetOptions.SetActive(true); SunOptions.SetActive(false);}

        if(simulationSpeed != 0f) { TogglePause(); }

        foreach (UiTabs tab in myTabs) { tab.ToggleTab(true); }

    }

/*
    public static float MessyGuessy(float min, float max, float desiredValue, float inc = 0.01f)
    {
        var guess = 1f; 

        
        for (float f = 0; f < 1f; f+= inc)
        {
            var currentGuess = guess * zoomWeight.Evaluate(guess); 
            var newGuess = f * zoomWeight.Evaluate(f);
            var d1 = Mathf.Lerp(min, max, currentGuess);
            var d2 = Mathf.Lerp(min, max, newGuess);
            if (Mathf.Abs(desiredValue - d1) > Mathf.Abs(desiredValue - d2))
            { guess = f; }
        }
        
        return guess;
    }


    public void ChangePlanetDistance()
    {
        var scaledSlider = UiSliders[0].value * zoomWeight.Evaluate(UiSliders[0].value);
        float newValue = Mathf.Lerp(theSun.minBodyDist, theSun.maxBodyDist, scaledSlider);
        var oldValue = currentlySelected.myOrbit.semiMajorAxis;
        currentlySelected.SetOrbitDistance(newValue);


        UpdateIconOrder();

        foreach (UiTabs tab in myTabs) { tab.ToggleTab(true); }
    }
    

    public void UpdateIconOrder()
    {

        var buffer = new Vector3(Screen.width / 10f,0,0);
        //var lineMin = RemoveButton.position + buffer;
        //var lineMax = AddButton.position - buffer;

        
        for (int i = 0; i < planetSliders.Count; i ++)
        {
            if (theSun.activeBods.Contains(planetSliders[i].target) || i == 0) 
            {
                planetSliders[i].SetColor(Color.white);
                

                //var distPercent = planetSliders[i].target.orbitDistance / theSun.maxBodyDist;
                //var scaledDist = distPercent * zoomWeight.Evaluate(distPercent);

                //var distPercent = 0f;
                //if(theSun.activeBods.Count > 0f)
                //{ distPercent =  planetSliders[i].target.orbitDistance / theSun.activeBods[0].orbitDistance; } 

                //planetSliders[i].transform.position = Vector3.Lerp(lineMin, lineMax, distPercent);
                
            }
            else { planetSliders[i].SetColor(Color.clear); }
        }
        

    }
*/

    public void CameraZoomSlider()
    {
        myCam.ChangeZoomSlideValue();

        //foreach (UiTabs tab in myTabs) { tab.ToggleTab(true); }
    }

}
