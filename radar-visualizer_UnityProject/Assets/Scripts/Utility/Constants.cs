﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

static public class Constants
{
    public const float DisplayAreaSize = 476;

    static public class RadarConfig
    {
        static public readonly Vector2 LRSRadarConeAngles = new Vector2(120f, 10f);
        static public readonly Vector2 TWSRadarConeAngles = new Vector2(60f, 10f);
        static public readonly Vector2 STTRadarConeAngles = new Vector2(2.5f, 2.5f);
    }

    static public class Colors
    {
        static public readonly Color EnemyDisplayInvisible = new Color(1f, 0f, 0f, 0.5f);
    }
}