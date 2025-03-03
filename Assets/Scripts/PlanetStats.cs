using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct gas
{
    public double absorption;
    public double molarMass;

    public gas(double initAbsorption, double mass)
    {
        absorption = initAbsorption;
        molarMass = mass;
    }
}


public class PlanetStats : MonoBehaviour
{
    [Tooltip("The diameter of the planet in km")]
    public double diameter = 12756d;
    [Tooltip("The mass of the planet in kg")]
    public double mass = 5972190000000000000000000d;
    [Tooltip("Distance to the sun in km")]
    public double distToSun = 149000000;
    public double atmosMass = 5148000000000000000d;
    [Tooltip("The average molar mass of the atmosphere")]
    public double avgMolarMass = 28.97d;
    [Tooltip("How much energy the sun outputs as radiation")]
    public double energyFromSun = 385000000000000000000000000d;

    public double solarConst = 0;
    public double intEnergy = 0;

    //Temp absorption value for the mix of elements
    public double absorption = 0.78d;

    [Tooltip("The density of air expressed as kg/m^3")]
    public double airDensity = 1.225d;

    //Gravitational constant m^3 /(kg * s^2)
    private double gravConst = 0.0000000000667408d;
    private double crustDist = 0;
    //Ideal gas constant J/ (K * mol)
    private double idealGasConst = 8.314;
    //Ideal gas constant (pressure) (Latm/molK)
    //private double pressureConst= 0.0821d;
    //The Stefan-Boltzmann constant
    private double boltzConst = 0.00000005670374419d;
    //The albedo (reflectiveness) value for the planet
    private double albedo = 0.31d;
    //The change in temperature
    //private double tempChange = 1d;

    private double emissivity = 0d;
    public double gravAcc = 0;
    private double density = 0;
    public double sizeMult = 10d;

    //Reference to Planet to get certain stats
    [SerializeField]
    private Planet planet;
    [SerializeField]
    public Compound crustComp;
    public Compound atmosComp;

    //private double previousTemp = 0d;

    // private bool first = true;

    private void Start()
    {
        mass = planet.mass * Math.Pow(10d, 24d);
        diameter = planet.planetSize;
        //planet = GetComponent<Planet>();
        crustComp = planet.currentCrust;
        atmosComp = planet.currentAtmosphere;
        double radius = diameter / 2d;
        density = mass / (4d * Math.PI * Math.Pow(radius, 3));
        distToSun = planet.orbitDistance * 100000d;
        Debug.Log(distToSun);
        crustDist = diameter / 2d;
    }


    public void updateStats()
    {
        //distToSun = (double)(double)planet.orbitDistance * 100000d;
        Debug.Log(distToSun);

        if (planet.sizeCategory == 0)
        {
            crustDist = planet.planetSize * (1 / sizeMult) / 2;
        }
        else if (planet.sizeCategory == 1) 
        {
            crustDist = planet.planetSize / 2;
        }
        else
        {
            crustDist = planet.planetSize * sizeMult / 2;
        }
       // mass = calcMassFromVol(crustDist);

        crustComp = planet.currentCrust;
        atmosComp = planet.currentAtmosphere;

        double planetCircumference = 4d * Math.PI * Math.Pow(crustDist, 2d);
        //crustDist = 1000 * diameter / 2;

        Debug.Log(energyFromSun);
        Debug.Log(distToSun);
        //Determine the solar constant of a specific planet
        solarConst = energyFromSun / (4d * Math.PI * Math.Pow(distToSun * 1000d, 2d));
        //Debug.Log($"Solar Constant = {solarConst}");

        //Determine how much energy is intercepted from the sun expressed in watts (joules/s)
        intEnergy = solarConst * Math.PI * Math.Pow(crustDist, 2d);


        //Grav Acceleration formula
        gravAcc = ((gravConst * mass) / Math.Pow(crustDist * 1000d, 2d));
        //Debug.Log(gravForce);


        //Debug.Log($"EnergyFromSun = {intEnergy}");

        //Maybe just add 0.01 to emissivity value until at equilibrium
        double Temp = Math.Pow(solarConst * (1 - albedo) / (4 * boltzConst), (1d / 4d));

        double baseTemp = Math.Pow(solarConst * (1 - albedo) / (4 * boltzConst), (1d / 4d));
        Temp = Math.Pow((2d * baseTemp) / (boltzConst * (2d - absorption)), (1d / 4d));
        Debug.Log($"Temp of {planet.planetName}'s surface = {Temp}");




/*
        if (first) 
        {
            Debug.Log($"Temp of Earth's surface = {EarthTemp}");
            emissivity = calcEmissivity(5.35d, tempChange, 431d);
            previousTemp = EarthTemp;
            first = false;
        }
        else
        {
            if (emissivity < 2d)
            {
                
                
                tempChange = EarthTemp - baseEarthTemp;
                Debug.Log($"TempChange = {tempChange}\n");
                previousTemp = EarthTemp;
                emissivity = calcEmissivity(5.35d, tempChange, 431d);
            }
        }
*/        
        //Note: just temp stuff for now

        //Determine the velocity gas needs to escape the atmosphere
        double velEsc = (Math.Sqrt((2d * gravConst * mass) / crustDist));
        //Debug.Log($"Escape Velocity of the atmosphere at sea level is = {velEsc} m/s");

        //Calculate the velocity of gas at certain points
        double gasVel = (3d * idealGasConst * (Temp + 273.15d)) / avgMolarMass;
        //Debug.Log($"The velocity of the atmosphere at sea level = {gasVel}");
    }


    double calcMassFromVol(double diameter)
    {//This assumes the same density
        double radius = diameter / 2d;
        double volume = 4d * Math.PI * Math.Pow(radius, 3);

        return density * volume;
    }

    double calcEmissivity(double absorption, double tempChange, double ppm)
    {//A function to calculate the emissivity of a gas based on certain factors
        //Calculate the radiative forcing of a gas
        double emission = absorption * Math.Log(ppm, Math.E);
        //provides the emissivity based on the change in emissivity and the change in temperature
        if (emission != 0)
        {
            emission = tempChange / emission;
            Debug.Log($"The emission of Earth's atmosphere = {emission}");

            return emission;
        }
        else
        {
            return emissivity;
        }
    }
}
