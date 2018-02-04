// Made by Alexandru Romanciuc <sanromanciuc@gmail.com>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityRandom = UnityEngine.Random;
using UnityEngine.Analytics;

static public class AnalyticsController
{
	static public class EventNames
    {
        public const string LockTWS = "LockTWS";
        public const string UnlockTWS = "UnlockTWS";
        public const string LockSTT = "LockSTT";
        public const string UnlockSTT = "UnlockSTT";
        public const string ToggleTWS = "ToggleTWS";
        public const string ZoomIn = "ZoomIn";
        public const string ZoomOut = "ZoomOut";
        public const string EnemyCreate = "EnemyCreate";
        public const string EnemyMove = "EnemyMove";
        public const string EnemyDelete = "EnemyDelete";
        public const string ToggleHiddenEnemies = "ToggleHiddenEnemies";
    }


    static public void OnLockTWS() { Analytics.CustomEvent(EventNames.LockTWS); }
    static public void OnUnlockTWS() { Analytics.CustomEvent(EventNames.UnlockTWS); }
    static public void OnLockSTT() { Analytics.CustomEvent(EventNames.LockSTT); }
    static public void OnUnlockSTT() { Analytics.CustomEvent(EventNames.UnlockSTT); }

    static public void OnToggleTWS() { Analytics.CustomEvent(EventNames.ToggleTWS); }

    static public void OnZoomIn() { Analytics.CustomEvent(EventNames.ZoomIn); }
    static public void OnZoomOut() { Analytics.CustomEvent(EventNames.ZoomOut); }

    static public void OnEnemyCreate() { Analytics.CustomEvent(EventNames.EnemyCreate); }
    static public void OnEnemyMove() { Analytics.CustomEvent(EventNames.EnemyMove); }
    static public void OnEnemyDelete() { Analytics.CustomEvent(EventNames.EnemyDelete); }

    static public void OnToggleHiddenEnemies() { Analytics.CustomEvent(EventNames.ToggleHiddenEnemies); }
}
