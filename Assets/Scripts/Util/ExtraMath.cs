using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraMath : MonoBehaviour {

    static public float Map(float value, float fromA, float fromB, float toA, float toB)
    {
        // Map from an to ranges that start at 0
        if (fromA == 0 && toA == 0)
        {
            float percent_value = value / fromB;
            float second_value = percent_value * toB;
            Debug.Log(second_value);
            return second_value;
        }

        return 0;

        // First range
        //float range_from = fromB - fromA;

        // Percent of the value in the first range
        //float range_percent = 
    }
}
