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

    /// <summary>
    /// Returns the vertical angle of the unit relative to 0 0
    /// </summary>
    /// <param name="posRelativeToPlayer">Advis giving position relative to player (aka substract player's heght)</param>
    /// <returns></returns>
    static public float GetPointVerticalAngle(Vector3 posRelativeToPlayer)
    {
        return Mathf.Atan(posRelativeToPlayer.y / posRelativeToPlayer.z) * Mathf.Rad2Deg;
    }
}
