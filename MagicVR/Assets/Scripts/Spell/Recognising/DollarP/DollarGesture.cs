using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class DollarGesture {

    public DollarPoint[] points;
    public string gestureName = "";
    const int res = 16;

    public DollarGesture(DollarPoint[] points, string gestureName = "")
    {
        this.gestureName = gestureName;


        this.points = Scale(points);
        this.points = Translate(this.points, Center(this.points));
        this.points = Resample(this.points, res);
    }



    public DollarPoint Center(DollarPoint[] DollarPoints)
    {
        float cX = 0, cY = 0;
        foreach(DollarPoint point in DollarPoints)
        {
            cX += point.x;
            cY += point.y;
        }
        DollarPoint centerPoint = new DollarPoint(cX / DollarPoints.Length, cY / DollarPoints.Length, 0);
        return centerPoint;
    }

    // Translate
    DollarPoint[] Translate(DollarPoint[] DollarPoints, DollarPoint p)
    {
        DollarPoint[] newPoints = new DollarPoint[DollarPoints.Length];
        for (int i = 0; i < DollarPoints.Length; i++)
            newPoints[i] = new DollarPoint(DollarPoints[i].x - p.x, DollarPoints[i].y - p.y, DollarPoints[i].iD);

        return newPoints;

     }

    // Scale normalisation
    DollarPoint[] Scale(DollarPoint[] DollarPoints)
    {
        float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
        for (int i = 0; i < DollarPoints.Length; i++)
        {
            if (minX > DollarPoints[i].x) minX = DollarPoints[i].x;
            if (minY > DollarPoints[i].y) minY = DollarPoints[i].y;
            if (maxX < DollarPoints[i].x) maxX = DollarPoints[i].x;
            if (maxY < DollarPoints[i].y) maxY = DollarPoints[i].y;
        }
        DollarPoint[] newPoints = new DollarPoint[DollarPoints.Length];
        float scale = Mathf.Max(maxX - minX, maxY - minY);

        for (int i = 0; i < DollarPoints.Length; i++)
            newPoints[i] = new DollarPoint((DollarPoints[i].x - minX) / scale, (DollarPoints[i].y - minY) / scale, DollarPoints[i].iD);
        return newPoints;
    }

    float PathLength(DollarPoint[] DollarPoints)
    {
        float length = 0;
        for (int i = 1; i < DollarPoints.Length; i++)
            if (DollarPoints[i].iD == DollarPoints[i - 1].iD)
                length += DollarGeometry.EuclideanDistance(DollarPoints[i - 1], DollarPoints[i]);
        return length;
    }


    #region COPIEDCODE

    public DollarPoint[] Resample(DollarPoint[] DollarPoints, int n)
    {
        DollarPoint[] newPoints = new DollarPoint[n];
        newPoints[0] = new DollarPoint(DollarPoints[0].x, DollarPoints[0].y, DollarPoints[0].iD);
        int numPoints = 1;

        float I = PathLength(DollarPoints) / (n - 1); // computes interval length
        float D = 0;
        for (int i = 1; i < DollarPoints.Length; i++)
        {
            if (DollarPoints[i].iD == DollarPoints[i - 1].iD)
            {
                float d = DollarGeometry.EuclideanDistance(DollarPoints[i - 1], DollarPoints[i]);
                if (D + d >= I)
                {
                    DollarPoint firstPoint = DollarPoints[i - 1];
                    while (D + d >= I)
                    {
                        // add interpolated point
                        float t = Math.Min(Math.Max((I - D) / d, 0.0f), 1.0f);
                        if (float.IsNaN(t)) t = 0.5f;
                        newPoints[numPoints++] = new DollarPoint(
                            (1.0f - t) * firstPoint.x + t * DollarPoints[i].x,
                            (1.0f - t) * firstPoint.y + t * DollarPoints[i].y,
                            DollarPoints[i].iD
                        );

                        // update partial length
                        d = D + d - I;
                        D = 0;
                        firstPoint = newPoints[numPoints - 1];
                    }
                    D = d;
                }
                else D += d;
            }
        }

        if (numPoints == n - 1) // sometimes we fall a rounding-error short of adding the last point, so add it if so
            newPoints[numPoints++] = new DollarPoint(DollarPoints[DollarPoints.Length - 1].x, DollarPoints[DollarPoints.Length - 1].y, DollarPoints[DollarPoints.Length - 1].iD);
        return newPoints;
    }

    #endregion
    #region COPIEDCODELICENSE
    /**
     * The $P Point-Cloud Recognizer (.NET Framework 4.0 C# version)
     *
     * 	    Radu-Daniel Vatavu, Ph.D.
     *	    University Stefan cel Mare of Suceava
     *	    Suceava 720229, Romania
     *	    vatavu@eed.usv.ro
     *
     *	    Lisa Anthony, Ph.D.
     *      UMBC
     *      Information Systems Department
     *      1000 Hilltop Circle
     *      Baltimore, MD 21250
     *      lanthony@umbc.edu
     *
     *	    Jacob O. Wobbrock, Ph.D.
     * 	    The Information School
     *	    University of Washington
     *	    Seattle, WA 98195-2840
     *	    wobbrock@uw.edu
     *
     * The academic publication for the $P recognizer, and what should be 
     * used to cite it, is:
     *
     *	Vatavu, R.-D., Anthony, L. and Wobbrock, J.O. (2012).  
     *	  Gestures as point clouds: A $P recognizer for user interface 
     *	  prototypes. Proceedings of the ACM Int'l Conference on  
     *	  Multimodal Interfaces (ICMI '12). Santa Monica, California  
     *	  (October 22-26, 2012). New York: ACM Press, pp. 273-280.
     *
     * This software is distributed under the "New BSD License" agreement:
     *
     * Copyright (c) 2012, Radu-Daniel Vatavu, Lisa Anthony, and 
     * Jacob O. Wobbrock. All rights reserved.
     *
     * Redistribution and use in source and binary forms, with or without
     * modification, are permitted provided that the following conditions are met:
     *    * Redistributions of source code must retain the above copyright
     *      notice, this list of conditions and the following disclaimer.
     *    * Redistributions in binary form must reproduce the above copyright
     *      notice, this list of conditions and the following disclaimer in the
     *      documentation and/or other materials provided with the distribution.
     *    * Neither the names of the University Stefan cel Mare of Suceava, 
     *	    University of Washington, nor UMBC, nor the names of its contributors 
     *	    may be used to endorse or promote products derived from this software 
     *	    without specific prior written permission.
     *
     * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
     * IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
     * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
     * PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Radu-Daniel Vatavu OR Lisa Anthony
     * OR Jacob O. Wobbrock BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
     * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
     * OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
     * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
     * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
     * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
     * SUCH DAMAGE.
    **/
    #endregion
}
