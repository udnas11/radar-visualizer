using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

static public class Constants
{
    public const float DisplayAreaSize = 476;
    public const float DisplayLockRange = 20;

    static public class RadarConfig
    {
        static public readonly Vector2 LRSRadarConeAngles = new Vector2(120f, 10f);
        static public readonly Vector2 TWSRadarConeAngles = new Vector2(60f, 10f);
        static public readonly Vector2 STTRadarConeAngles = new Vector2(2.5f, 2.5f);
        static public readonly Vector2 GimbalLimits = new Vector2(60f, 30f);
        static public readonly Vector2 GimbalLimitsSTT = new Vector2(60f, 60f);
    }

    static public class Colors
    {
        static public readonly Color EnemyDisplayInvisible = new Color(1f, 0f, 0f, 0.5f);
        static public readonly Color EnemyDisplayTWS = new Color(0f, 1f, 0f, 0.5f);
    }
}

public enum EEditModeType
{
    None,
    Create,
    Move,
    Delete
}