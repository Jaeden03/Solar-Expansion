using System.Collections;
using System.Collections.Generic;
using System.Linq;

//using System.Numerics;
using UnityEngine;

//https://www.youtube.com/watch?v=Ie5L8Nz1Ns0
public class Orbit : MonoBehaviour
{
    public float speed = 1;
    private float currentPoint;
    public float semiMajorAxis = 1; //distance in km * 10000
    [Range(0, 0.9f)] public float eccentricity;
    public float inclination;
    private float longitudeOfAscendingNode;
    public float degreeOfAscendingNode;
    private float argumentOfPeriapsis;
    public float degreeOfPeriapsis;
    public float meanLongitude;

    public Planet referenceBody;

    //Semi constant variables
    private float meanAnomaly;
    private float mu;
    private float n;
    private float trueAnomalyConst;
    private float cosLOAN;
    private float sinLOAN;
    private float cosI;
    private float sinI;

    public GameObject PathPrefab;
    private LineRenderer orbitLine;
    private float minScale;
    private float maxScale;
    [Range(1, 200)]public int lineResolution = 10;

    public float F(float E, float e, float M)
    {
        return M - E + e * Mathf.Sin(E);
    }

    public float DF(float E, float e)
    {
        return (-1f) + e * Mathf.Cos(E);
    }

    // Start is called before the first frame update
    void Start()
    {
        orbitLine = Instantiate(PathPrefab, transform).GetComponent<LineRenderer>();
        minScale = orbitLine.startWidth / 10;
        maxScale = orbitLine.startWidth * 10;
    }

    // Update is called once per frame
    void Update()
    {
        longitudeOfAscendingNode = degreeOfAscendingNode * 6.3f / 360f;
        argumentOfPeriapsis = degreeOfPeriapsis * 6.3f / 360f;

        CalculatePosition();
    }

    void CalculateSemiConstants() //variables that only change when orbit is adjusted
    {
        mu = SolarSystem.gravitationalConstant * referenceBody.mass;
        n = Mathf.Sqrt(mu / Mathf.Pow(semiMajorAxis, 3));
        trueAnomalyConst = Mathf.Sqrt((1 + eccentricity) / (1 - eccentricity));
        cosLOAN = Mathf.Cos(longitudeOfAscendingNode);
        sinLOAN = Mathf.Sin(longitudeOfAscendingNode);

        var incScaled = inclination * (1.5708f / 90f);
        cosI = Mathf.Cos(incScaled);
        sinI = Mathf.Sin(incScaled);
    }

    public void CalculatePosition()
    {
        CalculateSemiConstants();

        currentPoint += speed * Time.deltaTime * SolarSystem.simulationSpeed;

        meanAnomaly = n * (currentPoint - meanLongitude);

        //Kepler
        float E1 = meanAnomaly; //initial guess
        float difference = 1f;
        float accuracyTolerance = 0.000001f;
        int maxIterations = 5;

        for (int i = 0; difference > accuracyTolerance && i < maxIterations; i++)
        {
            float E0 = E1;
            E1 = E0 - F(E0, eccentricity, meanAnomaly) / DF(E0, eccentricity);
            difference = Mathf.Abs(E1 - E0);
        }

        float EccentricAnomaly = E1;

        float trueAnomaly = 2 * Mathf.Atan(trueAnomalyConst * Mathf.Tan(EccentricAnomaly / 2));
        float distance = semiMajorAxis * (1 - eccentricity * Mathf.Cos(EccentricAnomaly));

        //Set 3D position
        float cosAOPPlusTA = Mathf.Cos(argumentOfPeriapsis + trueAnomaly);
        float sinAOPPlusTA = Mathf.Sin(argumentOfPeriapsis + trueAnomaly);

        float x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
        float y = distance * (sinI * sinAOPPlusTA);
        float z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));

        transform.position = new Vector3(x, y, z) + referenceBody.transform.position;

        CalculatePoints();
    }

    void CalculatePoints()
    {
        float orbitFraction = 1f / lineResolution;

        float bodyMAModulo = meanAnomaly % (Mathf.PI * 2);
        if (Mathf.Sign(bodyMAModulo) == -1f) 
        { bodyMAModulo = (Mathf.PI * 2) - Mathf.Abs(bodyMAModulo); }

        //var laterPoints = orbitPoints;
        var laterPoints = new List<Vector3>();
        var prevPoints = new List<Vector3>();
        var orderedPoints = new Vector3[lineResolution +2];
        orbitLine.positionCount = lineResolution +2;


        for(int i = 0; i < lineResolution; i ++)
        {
            float EccentricAnomaly = i * orbitFraction * Mathf.PI * 2;

            float trueAnomaly = 2 * Mathf.Atan(trueAnomalyConst * Mathf.Tan(EccentricAnomaly / 2));
            float distance = semiMajorAxis * (1 - eccentricity * Mathf.Cos(EccentricAnomaly));

            float cosAOPPlusTA = Mathf.Cos(argumentOfPeriapsis + trueAnomaly);
            float sinAOPPlusTA = Mathf.Sin(argumentOfPeriapsis + trueAnomaly);

            float x = (cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI);
            float y = sinI * sinAOPPlusTA;
            float z = (sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI);

            var point = (new Vector3(x,y,z) * distance) + referenceBody.transform.position;

            float pointAnomaly = EccentricAnomaly - eccentricity * Mathf.Sin(EccentricAnomaly);

            if (pointAnomaly > bodyMAModulo) 
            { prevPoints.Add(point); }
            else { laterPoints.Add(point); }
        }
        
        orderedPoints[0] = transform.position;
        
        for (int i = 0; i < prevPoints.Count; i++)
        { orderedPoints[i+1] = prevPoints[i]; }

        for (int i = 0; i < laterPoints.Count; i++)
        { orderedPoints[i+1+prevPoints.Count] = laterPoints[i]; }
               
        orderedPoints[lineResolution +1] = transform.position;

        if (speed < 0) { orderedPoints = orderedPoints.Reverse().ToArray(); }

        orbitLine.SetPositions(orderedPoints);
        orbitLine.startWidth = Mathf.Lerp(minScale, maxScale, SolarSystem.screenScale);
        if (SolarSystem.currentlySelected)
        {
            if (SolarSystem.currentlySelected.myOrbit == this)
            { orbitLine.startWidth *= 4;}
        }
        

    }
}
