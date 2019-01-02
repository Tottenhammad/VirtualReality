using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DollarGeometry
{
    public static float SqrEuclideanDistance(DollarPoint a, DollarPoint b)
    {
        return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
    }

    public static float EuclideanDistance(DollarPoint a, DollarPoint b)
    {
        return (float)Math.Sqrt(SqrEuclideanDistance(a, b));
    }
}
