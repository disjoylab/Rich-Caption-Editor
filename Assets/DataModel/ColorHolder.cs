using System;
using UnityEngine;

[Serializable]
public struct ColorHolder
{
    public float R;
    public float G;
    public float B;
    public float A;

    // Constructor
    public ColorHolder(float r, float g, float b, float a = 1.0f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
     
    public static explicit operator Color(ColorHolder cv)
    {
        return new Color(cv.R, cv.G, cv.B, cv.A);
    } 

    public static explicit operator ColorHolder(Color c)
    {
        return new ColorHolder(c.r, c.g, c.b, c.a);
    }
    public override string ToString()
    {
        return $"RGBA({R:F2}, {G:F2}, {B:F2}, {A:F2})";
    }

}