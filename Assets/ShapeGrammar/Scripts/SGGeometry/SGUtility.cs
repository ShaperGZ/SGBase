using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGGeometry
{
    public class SGUtility 
    {
        // Find the point of intersection between
        // the lines p1 --> p2 and p3 --> p4.
        public static void LineLineIntersection(
            Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4,
            out bool lines_intersect, out bool segments_intersect,
            out Vector2 intersection,
            out Vector2 close_p1, out Vector2 close_p2)
        {
            // Get the segments' parameters.
            float dx12 = p2.x - p1.x;
            float dy12 = p2.y - p1.y;
            float dx34 = p4.x - p3.x;
            float dy34 = p4.y - p3.y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((p1.x - p3.x) * dy34 + (p3.y - p1.y) * dx34)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Vector2(float.NaN, float.NaN);
                close_p1 = new Vector2(float.NaN, float.NaN);
                close_p2 = new Vector2(float.NaN, float.NaN);
                return;
            }
            lines_intersect = true;

            float t2 =
                ((p3.x - p1.x) * dy12 + (p1.y - p3.y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new Vector2(p1.x + dx12 * t1, p1.y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new Vector2(p1.x + dx12 * t1, p1.y + dy12 * t1);
            close_p2 = new Vector2(p3.x + dx34 * t2, p3.y + dy34 * t2);
        }
    }

    public class Intersect
    {
        public static void LineLine2D(Vector3 ap1, Vector3 ap2, Vector3 bp1, Vector3 bp2)
        {
            bool lines_Intersect;
            bool segments_intersect;
            Vector2 intersection;
            Vector2 close_p1, close_p2;

            Vector2 p1 = new Vector2(ap1.x, ap1.z);
            Vector2 p2 = new Vector2(ap2.x, ap2.z);
            Vector2 p3 = new Vector2(bp1.x, bp1.z);
            Vector2 p4 = new Vector2(bp2.x, bp2.z);

            SGUtility.LineLineIntersection(p1, p2, p3, p4, out lines_Intersect, out segments_intersect, out intersection, out close_p1, out close_p2);

            Vector3 intersectionV3=new Vector3(intersection.x,0,intersection.y);
            Vector3 close_p1V3 = new Vector3(close_p1.x, 0, close_p1.y);
            Vector3 close_p2V3 = new Vector3(close_p2.x, 0, close_p2.y);

            Debug.Log("lines_intersect:" + lines_Intersect);
            Debug.Log("segment_intersect:" + segments_intersect);
            Debug.Log("intersection:" + intersectionV3);
            Debug.Log("clase_p1:" + close_p1V3);
            Debug.Log("clase_p2:" + close_p2V3);

        }
    }

}
