using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#region Psuedo
// https://depts.washington.edu/madlab/proj/dollar/pdollar.pdf
#endregion
public class DollarPointCloud : MonoBehaviour
{
    static DollarResult Classify(DollarGesture checkee, DollarGesture[] checkers)
    {
        float minDist = float.MaxValue;
        string gesture = "";
        foreach(DollarGesture checker in checkers)
        {
            float dist = GreedyMatch(checkee.points, checker.points);
            if(dist < minDist)
            {
                minDist = dist;
                gesture = checker.gestureName;
            }
        }
        return gesture == "" ? new DollarResult() { Value = 0, Gesture = "None Found"} : new DollarResult() { Value = Mathf.Max((minDist - 2.0f) / -2.0f, 0.0f)};
    }


    static float GreedyMatch(DollarPoint[] DollarPoints1, DollarPoint[] DollarPoints2)
    {
        int n = DollarPoints1.Length;
        float searchTries = 0.5f;
        int step = (int)Math.Floor(Math.Pow(n, 1 - searchTries));
        float minDist = float.MaxValue;
        for(int i = 0; i < n; i += step)
        {
            float dist1 = CloudDistance(DollarPoints1, DollarPoints2, i);
            float dist2 = CloudDistance(DollarPoints2, DollarPoints1, i);
            minDist = Math.Min(minDist, Math.Min(dist1, dist2));
        }

        return minDist;
    }


    static float CloudDistance(DollarPoint[] DollarPoints1, DollarPoint[] DollarPoints2, int startIndex)
    {
        int n = DollarPoints1.Length;
        bool[] equalTo = new bool[n];
        Array.Clear(equalTo, 0, n);

        float sum = 0;
        int i = startIndex;

        do
        {
            int index = -1;
            float minDist = float.MaxValue;
            for (int j = 0; j < n; j++)
            {
                if (!equalTo[j])
                {
                    float dist = DollarGeometry.SqrEuclideanDistance(DollarPoints1[i], DollarPoints2[j]);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        index = j;
                    }
                }
            }

            equalTo[index] = true;
            float weight = 1 - ((i - startIndex + n) % n) / (n);
            sum += weight * minDist;
            i = (i + 1) % n;
        } while (i != startIndex);


        return sum;
    }
}
