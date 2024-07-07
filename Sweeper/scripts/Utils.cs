using System;
using System.Linq;
using UnityEngine;

namespace Util
{
    public static class Utils
    {
        public const float RADS = (float)(Math.PI / 180);
        public const float EULER = (float)(180f / Math.PI);

        public static void QuitGame()
        {
            Application.Quit();
        }
        public static bool IsInt(string str)
        {
            return int.TryParse(str, out _);
        }
        public static bool IsInt(string str, out int outInteger)
        {
            return int.TryParse(str, out outInteger);
        }
        public static bool IsFloat(string str)
        {
            return float.TryParse(str, out _);
        }
        public static bool IsFloat(string str, out float outFloat)
        {
            return float.TryParse(str, out outFloat);
        }
        public static int Clamp(int value, int minimun, int maximum)
        {
            if (minimun.Equals(null))
                minimun = -2000000000;
            if (maximum.Equals(null))
                maximum = 2000000000;

            if (minimun.Equals(404))
                minimun = -2000000000;
            if (maximum.Equals(404))
                maximum = 2000000000;

            return Mathf.Clamp(value, minimun, maximum);
        }
        public static int ClampMin(int value, int minimun)
        {
            if (minimun.Equals(null))
                minimun = -2000000000;


            if (minimun.Equals(404))
                minimun = -2000000000;

            int maximum = 2000000000;

            return Mathf.Clamp(value, minimun, maximum);
        }
        public static int ClampMax(int value, int maximum)
        {
            int minimun = -2000000000;

            if (maximum.Equals(null))
                maximum = 2000000000;
            if (maximum.Equals(404))
                maximum = 2000000000;

            return Mathf.Clamp(value, minimun, maximum);
        }

        public static float Clamp(float value, float minimun, float maximum)
        {
            if (minimun.Equals(null))
                minimun = -2000000000;
            if (maximum.Equals(null))
                maximum = 2000000000;

            if (minimun.Equals(404))
                minimun = -2000000000;
            if (maximum.Equals(404))
                maximum = 2000000000;

            return Mathf.Clamp(value, minimun, maximum);
        }
        public static float ClampMin(float value, float minimun)
        {
            if (minimun.Equals(null))
                minimun = -2000000000;
            if (minimun.Equals(404))
                minimun = -2000000000;

            int maximum = 2000000000;

            return Mathf.Clamp(value, minimun, maximum);
        }
        public static float ClampMax(float value, float maximum)
        {
            int minimun = -2000000000;

            if (maximum.Equals(null))
                maximum = 2000000000;
            if (maximum.Equals(404))
                maximum = 2000000000;

            return Mathf.Clamp(value, minimun, maximum);
        }


        public static Vector3 MousePosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        public static Vector3 MousePosition(Camera cam)
        {
            return cam.ScreenToWorldPoint(Input.mousePosition);
        }

        public static float InvertAngle(float angle, bool invertX, bool invertY)
        {
            int x = invertX ? 180 : 0;
            int y = invertX ? -1 : 1;
            if (invertY) { y *= -1; }

            return y * angle - x;
        }
        public static float AngleBetweenPositions2D(Vector3 position1, Vector3 position2)
        {
            position2.z = 0;
            position1.z = 0;

            return AngleBetweenPositions3D(position1, position2);
        }
        public static float AngleBetweenPositions3D(Vector3 position1, Vector3 position2)
        {
            Vector3 diff = position2 - position1;
            float hypot = diff.magnitude;
            return AcosEuler(diff.x / hypot, diff);
        }
        public static float AngleBetweenPositionAndMouse(Vector3 position)
        {
            return AngleBetweenPositions2D(position, MousePosition());
        }
        public static float AngleBetweenPositionAndMouse(Vector3 position, Camera cam)
        {
            return AngleBetweenPositions2D(position, MousePosition(cam));
        }

        public static Vector3 PointOnCircle(Vector3 center, float radius, float angleEuler)
        {
            return new(center.x + radius * MathF.Cos(angleEuler * MathF.PI / 180), center.y + radius * MathF.Sin(angleEuler * MathF.PI / 180), center.z);
        }
        public static Vector3 PointOnCircle(Vector3 center, float radius, float angleEuler, bool invertX, bool invertY)
        {
            int iX = 1, iY = 1;
            if (invertX) { iX = -1; }
            if (invertY) { iY = -1; }
            return new(iX * center.x + radius * MathF.Cos(angleEuler * MathF.PI / 180), iY * center.y + radius * MathF.Sin(angleEuler * MathF.PI / 180), center.z);
        }
        public static Vector3 ClampVector2D(Vector3 vectorStart, Vector3 VectorEnd, float minLength, float maxLength)
        {
            vectorStart.z = VectorEnd.z;
            if ((VectorEnd - vectorStart).magnitude < minLength)
                VectorEnd = PointOnCircle(vectorStart, minLength, AngleBetweenPositions2D(vectorStart, VectorEnd));
            if ((VectorEnd - vectorStart).magnitude > maxLength)
                VectorEnd = PointOnCircle(vectorStart, maxLength, AngleBetweenPositions2D(vectorStart, VectorEnd));
            return VectorEnd;
        }
        public static Vector3 ClampVector3D(Vector3 vectorStart, Vector3 VectorEnd, float minLength, float maxLength)
        {
            if ((VectorEnd - vectorStart).magnitude < minLength)
                VectorEnd = PointOnCircle(vectorStart, minLength, AngleBetweenPositions3D(vectorStart, VectorEnd));
            if ((VectorEnd - vectorStart).magnitude > maxLength)
                VectorEnd = PointOnCircle(vectorStart, maxLength, AngleBetweenPositions3D(vectorStart, VectorEnd));
            return VectorEnd;
        }
        public static Vector3 Clamp(Vector3 vector, float minX, float maxX, float minY, float maxY)
        {
            return new(Mathf.Clamp(vector.x, minX, maxX), Mathf.Clamp(vector.y, minY, maxY), vector.z);
        }
        public static Vector3 Clamp(Vector3 vector, float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            return new(Mathf.Clamp(vector.x, minX, maxX), Mathf.Clamp(vector.y, minY, maxY), Mathf.Clamp(vector.z, minZ, maxZ));
        }
        public static Vector3 Clamp(Vector3 vector, Vector2 minXY, Vector2 maxXY)
        {
            return new(Mathf.Clamp(vector.x, minXY.x, maxXY.x), Mathf.Clamp(vector.y, minXY.y, maxXY.y), vector.z);
        }
        public static Vector3 Clamp(Vector3 vector, Vector3 minXYZ, Vector3 maxXYZ)
        {
            return new(Mathf.Clamp(vector.x, minXYZ.x, maxXYZ.x), Mathf.Clamp(vector.y, minXYZ.y, maxXYZ.y), Mathf.Clamp(vector.z, minXYZ.z, maxXYZ.z));
        }

        public static float Acos(float value, Vector3 vectorInQuater)
        {
            if (vectorInQuater.y >= 0)
                return Mathf.Acos(value);
            else
                return -Mathf.Acos(value);
        }
        public static float Asin(float value, Vector3 vectorInQuater)
        {
            return Mathf.Asin(value);
        }
        public static float AcosEuler(float value, Vector3 vectorInQuater)
        {
            if (vectorInQuater.y >= 0)
                return Mathf.Acos(value) * EULER;
            else
                return -Mathf.Acos(value) * EULER;
        }
        public static float AsinEuler(float value, Vector3 vectorInQuater)
        {
            return Mathf.Asin(value) * EULER;
        }
        public static int Quater(Vector3 vectorInQuater)
        {
            if(vectorInQuater.x >= 0 && vectorInQuater.y >= 0)
                return 1;
            if (vectorInQuater.x <= 0 && vectorInQuater.y > 0)
                return 2;
            if (vectorInQuater.x < 0 && vectorInQuater.y < 0)
                return 3;
            if (vectorInQuater.x > 0 && vectorInQuater.y <= 0)
                return 4;
            return 0;
        }

        public static Vector2 FPS(Vector2 fpsTimer)
        {
            fpsTimer.x++;
            fpsTimer.y += Time.deltaTime;
            if (fpsTimer.y >= 1)
            {
                fpsTimer.y %= 1;
                Debug.Log(">>>> FPS: " + fpsTimer.x);
                fpsTimer.x = 0;
            }
            return fpsTimer;
        }
    }
}

