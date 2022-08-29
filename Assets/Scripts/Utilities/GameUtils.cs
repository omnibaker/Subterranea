using System;
using UnityEngine;

/// <summary>
/// Quick reference script for frequently used methods
/// </summary>
public static class GameUtils
{
    public const int DEFAULT_DP = 3;
    private const string TIME_FORMAT_MM_SS = "m\\:ss"; // 0:00

    /// <summary>
    /// Returns float time formatted to "mm:ss" string
    /// </summary>
    public static string GetTimeInFormattedString(float timeLeftInSecond)
    {
        return TimeSpan.FromSeconds(timeLeftInSecond).ToString(TIME_FORMAT_MM_SS);
    }

    /// <summary>
    /// Returns float to a designated decimal points (uo to 5dp)
    /// </summary>
    public static string FormatFloats(float rawNumber, int dp = DEFAULT_DP)
    {
        string prefix = rawNumber < 0 ? "-" : " ";

        string formatPattern = "";
        switch (dp)
        {
            case 0: formatPattern = "{0:0}"; break;
            case 1: formatPattern = "{0:0.0}"; break;
            case 2: formatPattern = "{0:0.00}"; break;
            case 3: formatPattern = "{0:0.000}"; break;
            case 4: formatPattern = "{0:0.0000}"; break;
            case 5: formatPattern = "{0:0.00000}"; break;
            default: formatPattern = "{0:0.000}"; break;
        }

        string formatted = string.Format(formatPattern, Mathf.Abs(rawNumber));
        return $"{prefix}{formatted}";
    }

    /// <summary>
    /// Get Unity Color object from hexadecimal string
    /// </summary>
    public static Color GetColorFromHexadecimal(string hexadecimalColor)
    {
        if (ColorUtility.TryParseHtmlString($"#{hexadecimalColor}", out Color rgbaColor))
        {
            return rgbaColor;
        }

        Debug.LogError($"ERR: Could not get color from #{hexadecimalColor}");
        return Color.gray;
    }

    /// <summary>
    /// Return string with +/- prefex
    /// </summary>
    public static string LimitDP(float rawNumber, int dp = 3, bool sign = true)
    {
        string prefix = "";
        if (sign)
        {
            prefix = rawNumber < 0 ? "-" : " ";
        }
        return $"{prefix}{Mathf.Abs((float)Math.Round((decimal)rawNumber, dp))}";
    }

    /// <summary>
    /// Return Vector2 values in formatted string version with flexible decimal point
    /// </summary>
    public static string GetVector2String(string label, Vector2 v2, int dp = DEFAULT_DP)
    {
        label = string.IsNullOrWhiteSpace(label) ? "" : $"{label}: ";
        return $"{label}{FormatFloats(v2.x, dp)}, {FormatFloats(v2.y, dp)}";
    }

    /// <summary>
    /// Return Vector3 values in formatted string version with flexible decimal point
    /// </summary>
    public static string GetVector3String(string label, Vector3 v3, int dp = DEFAULT_DP)
    {
        label = string.IsNullOrWhiteSpace(label) ? "" : $"{label}: ";
        return $"{label}{FormatFloats(v3.x, dp)}, {FormatFloats(v3.y, dp)}, {FormatFloats(v3.z, dp)}";
    }

    /// <summary>
    /// Return Vector3Int values in formatted string version
    /// </summary>
    public static string GetVector3IntString(string label, Vector3Int v3)
    {
        label = string.IsNullOrWhiteSpace(label) ? "" : $"{label}: ";
        return $"{label}{v3.x}, {v3.y}, {v3.z}";
    }
}
