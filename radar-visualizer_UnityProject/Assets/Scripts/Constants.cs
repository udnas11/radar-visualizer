using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

static public class Constants
{
    public const float DisplayAreaSize = 476;

    public const float AngelsToNmRatio = 0.164579f;

    static public float NMtoAngels(float nauticalMiles)
    {
        return nauticalMiles / AngelsToNmRatio;
    }

    static public float AngelsToNM(float angels)
    {
        return angels * AngelsToNmRatio;
    }

    /// <summary>
    /// Transforms distance and rotation from player into worldspace coordinates
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    static public Vector3 GetPoint(float distance, Vector2 rotation)
    {
        // float HD = Mathf.Sin(bRad) * AD;
        Vector3 result = Vector3.zero;
        result.z = distance;
        result.y = Mathf.Sin(rotation.y * Mathf.Deg2Rad) * distance;
        result = Quaternion.Euler(0f, rotation.x, 0f) * result;
        return result;
    }

    /// <summary>
    /// Returns the horizontal angle of the unit relative to the player. Ignores altitude.
    /// </summary>
    /// <param name="worldSpace">Worldspace position</param>
    /// <returns></returns>
    static public float GetPointHorizontalAngle(Vector3 worldSpace)
    {
        return Mathf.Atan(worldSpace.x / worldSpace.z) * Mathf.Rad2Deg;
    }

    static public class RadarConfig
    {
        static public readonly Vector2 LRSRadarConeAngles = new Vector2(120f, 10f);
        static public readonly Vector2 TWSRadarConeAngles = new Vector2(60f, 10f);
        static public readonly Vector2 STTRadarConeAngles = new Vector2(2.5f, 2.5f);
    }
}