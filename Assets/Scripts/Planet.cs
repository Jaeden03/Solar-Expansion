using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float mass = 1; 

    [HideInInspector]public string planetName = "";
    public Sprite UiIcon;

    public GameObject[] orbitingBodies = new GameObject[0];
    public List<Planet> currentBods = new List<Planet>();
    public List<Planet> activeBods = new List<Planet>();

    public GameObject extraBodyPrefab;
    //[HideInInspector]public int bodyCount;
    public int maxBodies;

    public GameObject uiTrackingPrefab;
    private UiTracking myTracker;

    public GameObject uiSliderPrefab;
    private UiTracking mySlider;

    public float minBodyDist;
    public float maxBodyDist;

    public bool hasRings = false;
    [HideInInspector] public Orbit myOrbit;
    public float orbitDistance;

    //public Sprite planetIcon;

    public int atmosphereNumber = 0;
    public Compound currentAtmosphere;
    public int crustNumber = 0;
    public Compound currentCrust;
    public int sizeCategory = 0;

    public double planetSize;
    public PlanetStats stats;
    


    void Start()
    {
        myOrbit = GetComponent<Orbit>();
        stats = GetComponent<PlanetStats>();
        if (myOrbit) { orbitDistance = myOrbit.semiMajorAxis; }

        foreach (GameObject p in orbitingBodies)
        {
            var newBody = Instantiate(p, transform.position, Quaternion.identity).GetComponent<Planet>();
            newBody.planetName = p.transform.name.ToUpper();
            newBody.OrbitAround(this);
            currentBods.Add(newBody);
            activeBods.Add(newBody);
            //bodyCount ++;
        }

        if (UiIcon)
        {
            if (uiTrackingPrefab) 
            {
                myTracker = Instantiate(uiTrackingPrefab, SolarSystem.UniversalCanvas).GetComponent<UiTracking>();
                myTracker.target = this;
                myTracker.haveRings = hasRings;
                myTracker.SetSprite(UiIcon);
            }

            if (uiSliderPrefab)
            {
                var newSlider = Instantiate(uiSliderPrefab, SolarSystem.UniversalCanvas).GetComponent<UiTracking>();
                newSlider.target = this;
                newSlider.haveRings = hasRings;
                //newSlider.slider = true;
                newSlider.SetSprite(UiIcon);
                SolarSystem.planetSliders.Add(newSlider);

                mySlider = newSlider;
            }
        }

        if (planetName == "") { planetName = "THE SUN"; }
        else
        { 
            currentAtmosphere = SolarSystem.myTabs[0].options[atmosphereNumber];
            currentCrust = SolarSystem.myTabs[0].options[crustNumber]; 
        }
        //sizeCategory = 1;
    }

    void OnEnable()
    {
        if (myTracker) { myTracker.gameObject.SetActive(true); }

        if(mySlider) { mySlider.gameObject.SetActive(true); }

        foreach (Planet p in activeBods) { p.gameObject.SetActive(true); }

    }

    void OnDisable()
    {
        if(myTracker) { myTracker.gameObject.SetActive(false); }
        if(mySlider) { mySlider.gameObject.SetActive(false); }

        foreach(Planet p in currentBods)
        {
            if(p) { p.gameObject.SetActive(false); }            
        }
    }

    // Update is called once per frame
    void Update()
    {
        sortBodyOrder();
    }

    public void OrbitAround(Planet target)
    {
        var myOrbit = GetComponent<Orbit>();
        if (myOrbit) { myOrbit.referenceBody = target; }
    }

    public void AddBody()
    {
        if (activeBods.Count < maxBodies)
        {
            if (activeBods.Count >= currentBods.Count) 
            {
                var newBody = Instantiate(extraBodyPrefab, transform.position, Quaternion.identity).GetComponent<Planet>();
                newBody.planetName = extraBodyPrefab.transform.name.ToUpper();
                newBody.OrbitAround(this);
                currentBods.Add(newBody);
                activeBods.Add(newBody);
            }
            else
            {
                for (int i = currentBods.Count -1; i >= 0; i--)
                {
                    if (!activeBods.Contains(currentBods[i])) 
                    { 
                        activeBods.Add(currentBods[i]);
                        currentBods[i].gameObject.SetActive(true);
                        break;
                    }
                }
                //currentBods[bodyCount].gameObject.SetActive(true);
                //activeBods.Add(currentBods[bodyCount]);
            }
            //bodyCount++;
        }

        //sortBodyOrder();    
    }

    public void RemoveBody()
    {
        if (activeBods.Count > 0)
        {
            var removePlanet = activeBods[0];

            if (SolarSystem.currentlySelected != null) 
            { removePlanet = SolarSystem.currentlySelected; SolarSystem.Background(); }
            
            var index = currentBods.IndexOf(removePlanet);         

            currentBods[index].gameObject.SetActive(false);
            activeBods.Remove(removePlanet);
        }
        
        //sortBodyOrder();
    }

    public void SetOrbitDistance(float value)
    {
        if (myOrbit)
        {
            var currentAngle = Vector3.Angle(myOrbit.referenceBody.transform.position - transform.position, transform.forward);
            
            //if (myOrbit.semiMajorAxis > value) {  }

            myOrbit.semiMajorAxis = value;
            orbitDistance = value;

            
            /*
            myOrbit.CalculatePosition();
            var newAngle = Vector3.Angle(myOrbit.referenceBody.transform.position - transform.position, transform.forward);

            var incAngle = .1f;
            var minAngleDif = 1f;
            int safety = 0;

            while (Mathf.Abs(currentAngle - newAngle) > minAngleDif && safety < 1000)
            {
                if (currentAngle < newAngle) { myOrbit.meanLongitude += incAngle; }
                else { myOrbit.meanLongitude -= incAngle; }

                myOrbit.CalculatePosition();

                newAngle = Vector3.Angle(myOrbit.referenceBody.transform.position - transform.position, transform.forward);      

                safety++;
                if(safety == 1000) { Debug.Log("Overflow"); Debug.Log(Mathf.Abs(currentAngle - newAngle)); }
            }
            */

            //myOrbit.referenceBody.sortBodyOrder();
        }
    }


    public void sortBodyOrder()
    {
        activeBods = activeBods.OrderByDescending (p => p.orbitDistance).ToList();
        currentBods = currentBods.OrderByDescending(p => p.orbitDistance).ToList();

        foreach(Planet p in activeBods)
        {
            if (p.mySlider) { p.mySlider.transform.SetSiblingIndex(SolarSystem.UiIconIndex +1); }
            if (p.myTracker) { p.myTracker.transform.SetSiblingIndex(1); }
        }
    }

}
