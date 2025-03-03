using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compound : MonoBehaviour
{

    public string[] names;
    public int state = 0; // 0 = solid, // 1 = liquid // 2 = gas
    public float mass;
    public float transmission;
    [Tooltip("Specific Heat in J/MolK")]
    public float specificHeat;
    public float boilingPoint;
    public float meltingPoint;

    public void setState(double energy, double mass)
    {
        double temp = energy / (mass * specificHeat);

        if (temp > boilingPoint)
        {
            state = 2;
        }
        else if (temp > meltingPoint)
        {
            state = 1;
        }
        else
        {
            state = 0;
        }
    }
}
