using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class Math
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

    /// <summary>
    /// Transforms distance and rotation from player into worldspace coordinates
    /// </summary>
    /// <param name="distance">Distance stays constant with dynamic vertical rotation</param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    static public Vector3 GetPoint(float distance, Vector2 rotation)
    {
        Vector3 result = GetPointForVerticalAngleAtGroundDistance(distance, rotation.y);
        result = Quaternion.Euler(0f, rotation.x, 0f) * result; // rotating sideways
        return result;
    }

    static public Vector3 GetPointForVerticalAngleAtGroundDistance(float distance, float angle)
    {
        Vector3 result = Vector3.zero;
        result.z = distance;
        result.y = Mathf.Tan(angle * Mathf.Deg2Rad) * distance;
        return result;
    }

    /// <summary>
    /// Returns the horizontal angle of the unit relative to the player. Ignores altitude.
    /// </summary>
    /// <param name="worldSpace">Worldspace position</param>
    /// <returns></returns>
    static public float GetAngleHorizontalForPoint(Vector3 worldSpace)
    {
        return Mathf.Atan(worldSpace.x / worldSpace.z) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Returns the vertical angle of the unit relative to 0 0
    /// </summary>
    /// <returns></returns>
    static public float GetAngleVerticalForPoint(Vector3 pos)
    {
        return Mathf.Atan(pos.y / pos.z) * Mathf.Rad2Deg;
    }
}
