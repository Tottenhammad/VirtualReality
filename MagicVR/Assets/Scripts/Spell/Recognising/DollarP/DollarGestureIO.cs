using System.IO;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class DollarGestureIO : MonoBehaviour
{
    public static DollarGesture ReadGestureFromXML(string file)
    {
        XmlTextReader xmlReader = null;
        DollarGesture gesture = null;

        try
        {

            xmlReader = new XmlTextReader(new StringReader(file));
            gesture = ReadGesture(xmlReader);

        }
        finally
        {

            if (xmlReader != null)
                xmlReader.Close();
        }

        return gesture;
    }

    public static DollarGesture ReadGestureFromFile(string fileName)
    {

        XmlTextReader xmlReader = null;
        DollarGesture gesture = null;

        try
        {

            xmlReader = new XmlTextReader(File.OpenText(fileName));
            gesture = ReadGesture(xmlReader);

        }
        finally
        {

            if (xmlReader != null)
                xmlReader.Close();
        }

        return gesture;
    }


    #region CopiedCode
    private static DollarGesture ReadGesture(XmlTextReader xmlReader)
    {
        List<DollarPoint> points = new List<DollarPoint>();
        int currentStrokeIndex = -1;
        string gestureName = "";
        try
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element) continue;
                switch (xmlReader.Name)
                {
                    case "Gesture":
                        gestureName = xmlReader["Name"];
                        if (gestureName.Contains("~")) // '~' character is specific to the naming convention of the MMG set
                            gestureName = gestureName.Substring(0, gestureName.LastIndexOf('~'));
                        if (gestureName.Contains("_")) // '_' character is specific to the naming convention of the MMG set
                            gestureName = gestureName.Replace('_', ' ');
                        break;
                    case "Stroke":
                        currentStrokeIndex++;
                        break;
                    case "Point":
                        points.Add(new DollarPoint(
                            float.Parse(xmlReader["X"]),
                            float.Parse(xmlReader["Y"]),
                            currentStrokeIndex
                        ));
                        break;
                }
            }
        }
        finally
        {
            if (xmlReader != null)
                xmlReader.Close();
        }
        return new DollarGesture(points.ToArray(), gestureName);
    }

    /// <summary>
    /// Writes a multistroke gesture to an XML file
    /// </summary>
    public static void WriteGesture(PDollarGestureRecognizer.Point[] points, string gestureName, string fileName)
    {
        using (StreamWriter sw = new StreamWriter(fileName))
        {
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");
            sw.WriteLine("<Gesture Name = \"{0}\">", gestureName);
            int currentStroke = -1;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].StrokeID != currentStroke)
                {
                    if (i > 0)
                        sw.WriteLine("\t</Stroke>");
                    sw.WriteLine("\t<Stroke>");
                    currentStroke = points[i].StrokeID;
                }

                sw.WriteLine("\t\t<Point X = \"{0}\" Y = \"{1}\" T = \"0\" Pressure = \"0\" />",
                    points[i].X, points[i].Y
                );
            }
            sw.WriteLine("\t</Stroke>");
            sw.WriteLine("</Gesture>");
        }
    }
    #endregion

}
