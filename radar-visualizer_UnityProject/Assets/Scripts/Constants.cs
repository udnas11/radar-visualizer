using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

static public class Constants
{
    public const float AngelsToNmRatio = 0.164579f;

    static public float NMtoAngels(float nauticalMiles)
    {
        return nauticalMiles / AngelsToNmRatio;
    }

    static public float AngelsToNM(float angels)
    {
        return angels * AngelsToNmRatio;
    }

    static public Vector3 GetPoint(float distance, Vector2 rotation)
    {
        // float HD = Mathf.Sin(bRad) * AD;
        Vector3 result = Vector3.zero;
        result.z = distance;
        result.y = Mathf.Sin(rotation.y * Mathf.Deg2Rad) * distance;
        result = Quaternion.Euler(0f, rotation.x, 0f) * result;
        return result;
    }

    static public class RadarConfig
    {
        static public readonly Vector2 LRSRadarConeAngles = new Vector2(120f, 10f);
        static public readonly Vector2 TWSRadarConeAngles = new Vector2(60f, 10f);
        static public readonly Vector2 STTRadarConeAngles = new Vector2(2.5f, 2.5f);
    }
}