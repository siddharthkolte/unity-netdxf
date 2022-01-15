using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace netDxf.Math
{
    public static class Mathd
    {
        #region Properties

        public const float DegToGrad = 10.0f / 9.0f;
        public const float GradToDeg = 9.0f / 10.0f;

        public static readonly Vector3 V3UnitX = new Vector3(1.0f, 0.0f, 0.0f);
        public static readonly Vector2 V2UnitX = new Vector2(1.0f, 0.0f);
        public static readonly Vector3 V3UnitY = new Vector3(0.0f, 1.0f, 0.0f);
        public static readonly Vector2 V2UnitY = new Vector2(0.0f, 1.0f);
        public static readonly Vector3 V3UnitZ = new Vector3(0.0f, 0.0f, 1.0f);

        #endregion

        #region Extention Methods

        public static bool IsEquals(this Vector2 u, Vector2 other)
        {
            return u.IsEquals(other, Mathf.Epsilon);
        }

        public static bool IsEquals(this Vector2 u, Vector2 other, float threshold)
        {
            return IsEqual(other.x, u.x, threshold) && IsEqual(other.y, u.y, threshold);
        }

        public static bool IsEquals(this Vector2 u, object other)
        {
            if (other is Vector2 vector)
            {
                return u.Equals(vector);
            }
            return false;
        }

        public static List<Vector2> ToVector2List (this List<Vector3> u)
        {
            List<Vector2> result = new List<Vector2>();

            if (u.Count == 0) return null;

            foreach (Vector3 v in u)
            {
                result.Add(new Vector2(v.x, v.y));
            }

            return result;
        }

        public static float[] ToArray( this Vector3 v)
        {
            return new[] { v.x, v.y, v.z };
        }

        #endregion

        #region Math Functions

        public static float Cross(Vector2 u, Vector2 v)
        {
            return u.x * v.y - u.y * v.x;
        }

        public static bool IsNaN(Vector2 u)
        {
            return float.IsNaN(u.x) || float.IsNaN(u.y);
        }

        public static bool IsNaN(Vector3 u)
        {
            return float.IsNaN(u.x) || float.IsNaN(u.y) || float.IsNaN(u.z);
        }

        public static float PointLineDistance(Vector3 p, Vector3 origin, Vector3 dir)
        {
            float t = Vector3.Dot(dir, p - origin);
            Vector3 pPrime = origin + t * dir;
            Vector3 vec = p - pPrime;
            float distanceSquared = Vector3.Dot(vec, vec);
            return Mathf.Sqrt(distanceSquared);
        }

        public static float PointLineDistance(Vector2 p, Vector2 origin, Vector2 dir)
        {
            float t = Vector2.Dot(dir, p - origin);
            Vector2 pPrime = origin + t * dir;
            Vector2 vec = p - pPrime;
            float distanceSquared = Vector2.Dot(vec, vec);
            return Mathf.Sqrt(distanceSquared);
        }

        public static Vector2 MidPoint(Vector2 u, Vector2 v)
        {
            return new Vector2((v.x + u.x) * 0.5f, (v.y + u.y) * 0.5f);
        }

        public static Vector3 MidPoint(Vector3 u, Vector3 v)
        {
            return new Vector3((v.x + u.x) * 0.5f, (v.y + u.y) * 0.5f, (v.z + u.z) * 0.5f);
        }

        public static bool IsOne(float number)
        {
            return IsOne(number, Mathf.Epsilon);
        }

        public static bool IsOne(float number, float threshold)
        {
            return IsZero(number - 1, threshold);
        }

        public static bool IsZero(float number)
        {
            return IsZero(number, Mathf.Epsilon);
        }

        public static bool IsZero(float number, float threshold)
        {
            return number >= -threshold && number <= threshold;
        }

        public static bool IsEqual(float a, float b)
        {
            return IsEqual(a, b, Mathf.Epsilon);
        }

        public static bool IsEqual(float a, float b, float threshold)
        {
            return IsZero(a - b, threshold);
        }

        public static bool ArePerpendicular(Vector3 u, Vector3 v)
        {
            return ArePerpendicular(u, v, Mathf.Epsilon);
        }

        public static bool ArePerpendicular(Vector3 u, Vector3 v, float threshold)
        {
            return IsZero(Vector3.Dot(u, v), threshold);
        }

        public static bool AreParallel(Vector3 u, Vector3 v)
        {
            return AreParallel(u, v, Mathf.Epsilon);
        }

        public static bool AreParallel(Vector3 u, Vector3 v, float threshold)
        {
            Vector3 cross = Vector3.Cross(u, v);

            if (!IsZero(cross.x, threshold))
            {
                return false;
            }

            if (!IsZero(cross.y, threshold))
            {
                return false;
            }

            if (!IsZero(cross.z, threshold))
            {
                return false;
            }

            return true;
        }

        public static float NormalizeAngle(float angle)
        {
            float normalized = angle % 360.0f;
            if (IsZero(normalized) || IsEqual(Mathf.Abs(normalized), 360.0f))
            {
                return 0.0f;
            }

            if (normalized < 0)
            {
                return 360.0f + normalized;
            }

            return normalized;
        }

        public static float Angle(Vector2 u)
        {
            float angle = Mathf.Atan2(u.y, u.x);
            if (angle < 0)
            {
                return (2 * Mathf.PI) + angle;
            }
            return angle;
        }

        public static float Angle(Vector2 u, Vector2 v)
        {
            Vector2 dir = v - u;
            return Angle(dir);
        }

        public static Vector2 Rotate(Vector2 u, float angle)
        {
            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);
            return new Vector2(u.x * cos - u.y * sin, u.x * sin + u.y * cos);
        }

        public static Vector2 Polar(Vector2 u, float distance, float angle)
        {
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            return u + dir * distance;
        }

        public static Vector2 FindIntersection(Vector2 point0, Vector2 dir0, Vector2 point1, Vector2 dir1)
        {
            return FindIntersection(point0, dir0, point1, dir1, Mathf.Epsilon);
        }

        public static Vector2 FindIntersection(Vector2 point0, Vector2 dir0, Vector2 point1, Vector2 dir1, float threshold)
        {
            // test for parallelism.
            if (AreParallel(dir0, dir1, threshold))
            {
                return new Vector2(float.NaN, float.NaN);
            }

            // lines are not parallel
            Vector2 v = point1 - point0;
            float cross = Mathd.Cross(dir0, dir1);
            float s = (v.x * dir1.y - v.y * dir1.x) / cross;
            return point0 + s * dir0;
        }

        public static float RoundToNearest(float number, float roundTo)
        {
            float multiplier = Mathf.Round(number / roundTo);
            return multiplier * roundTo;
        }

        public static Vector3 RotateAboutAxis(Vector3 v, Vector3 axis, float angle)
        {
            Vector3 q = new Vector3();
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            q.x += (cos + (1 - cos) * axis.x * axis.x) * v.x;
            q.x += ((1 - cos) * axis.x * axis.y - axis.z * sin) * v.y;
            q.x += ((1 - cos) * axis.x * axis.z + axis.y * sin) * v.z;

            q.y += ((1 - cos) * axis.x * axis.y + axis.z * sin) * v.x;
            q.y += (cos + (1 - cos) * axis.y * axis.y) * v.y;
            q.y += ((1 - cos) * axis.y * axis.z - axis.x * sin) * v.z;

            q.z += ((1 - cos) * axis.x * axis.z - axis.y * sin) * v.x;
            q.z += ((1 - cos) * axis.y * axis.z + axis.x * sin) * v.y;
            q.z += (cos + (1 - cos) * axis.z * axis.z) * v.z;

            return q;
        }

        public static int PointInSegment(Vector3 p, Vector3 start, Vector3 end)
        {
            Vector3 dir = end - start;
            Vector3 pPrime = p - start;
            double t = Vector3.Dot(dir, pPrime);
            if (t < 0)
            {
                return -1;
            }
            double dot = Vector3.Dot(dir, dir);
            if (t > dot)
            {
                return 1;
            }
            return 0;
        }

        public static int PointInSegment(Vector2 p, Vector2 start, Vector2 end)
        {
            Vector2 dir = end - start;
            Vector2 pPrime = p - start;
            double t = Vector2.Dot(dir, pPrime);
            if (t < 0)
            {
                return -1;
            }
            double dot = Vector2.Dot(dir, dir);
            if (t > dot)
            {
                return 1;
            }
            return 0;
        }

        #endregion

        #region Generic Functions

        public static void Swap<T>(ref T obj1, ref T obj2)
        {
            T tmp = obj1;
            obj1 = obj2;
            obj2 = tmp;
        }

        #endregion
    }
}
