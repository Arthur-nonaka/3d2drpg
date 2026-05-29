using System;
using UnityEngine;

public static class LevelFormula
{
    public static int XPForLevel(int level) => (int)(50 * Mathf.Pow(level, 1.5f));
}
