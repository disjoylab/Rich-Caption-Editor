using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

[Serializable]
public struct Position //vector2 cant be jsonified
{
    public float X;
    public float Y;

    [JsonConstructor]
    public Position(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static explicit operator Position(Vector2 v) => new Position(v.x, v.y);
    public static explicit operator Vector2(Position p) => new Vector2(p.X, p.Y);
}