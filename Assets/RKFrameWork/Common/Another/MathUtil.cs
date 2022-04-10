using System;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil
{
    public const float PI = 3.14159274f;
    public const float Deg2Rad = PI / 180f;
    public const float EPSILON = 0.001f;

    public static bool FlootEqual(float f1, float f2)
    {
        float c = Mathf.Abs(f1 - f2);
        return c <= 0.00001;
    }

    public static bool RectWithRectIntersect(Vector3 p1, float width1, float height1, Vector3 p2, float width2, float height2)
    {
        float distance_X = Mathf.Abs(p1.x - p2.x);
        float distance_Z = Mathf.Abs(p1.z - p2.z);
        float halfSum_X = (width1 + width2) * 0.5f;
        float halfSum_Z = (height1 + height2) * 0.5f;
        bool isRectIntersect = distance_X < halfSum_X && distance_Z < halfSum_Z;
        return isRectIntersect;
    }

    /// 判断两直线是否有交点(只考虑x与z坐标)
    /// </summary>
    /// <returns><c>true</c>, 相交, <c>false</c> 不相交.</returns>
    /// <param name="v1">第一条直接起始点.</param>
    /// <param name="v2">第一条直接起终点.</param>
    /// <param name="v3">第二条直接起始点.</param>
    /// <param name="v4">第二条直接起终点.</param>
    static public bool CrossPoint(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        if (Mathf.Max(v1.x, v2.x) >= Mathf.Min(v3.x, v4.x) &&
            Mathf.Max(v3.x, v4.x) >= Mathf.Min(v1.x, v2.x) &&
            Mathf.Max(v1.z, v2.z) >= Mathf.Min(v3.z, v4.z) &&
            Mathf.Max(v3.z, v4.z) >= Mathf.Min(v1.z, v2.z) &&
            mulptis(v1, v2, v3) * mulptis(v1, v2, v4) <= 0 &&
            mulptis(v3, v4, v1) * mulptis(v3, v4, v2) <= 0)
            return true;
        else
            return false;
    }
    static private float mulptis(Vector3 ps, Vector3 pe, Vector3 p)
    {
        float m;
        m = (pe.x - ps.x) * (p.z - ps.z) - (p.x - ps.x) * (pe.z - ps.z);
        return m;
    }
    /// <summary>
    /// 检测 p点在v0, v1, v2, v3构成的矩形中（只考虑x,z）
    /// </summary>
    /// <param name="p">P.</param>
    /// <param name="leftTop">矩形左上角的点</param>
    /// <param name="rightBottom">矩形右下角的点</param>
    /// <param name="canInLine">是否可以在边线上</param>
    static public bool IsPointInRectAngle(Vector3 p, Vector3 leftTop, Vector3 rightBottom, bool canInLine = true)
    {
       if(canInLine)
        {
            return p.x >= leftTop.x && p.x <= rightBottom.x && p.z >= rightBottom.z && p.z <= leftTop.z;
        }
        return p.x > leftTop.x && p.x < rightBottom.x && p.z > rightBottom.z && p.z < leftTop.z;
    }
    
    /// <summary>
    /// 检测 p点在v0, v1, v2, v3构成的四边形中（只考虑x,z）
    /// </summary>
    /// <returns><c>true</c>, if rect was ined, <c>false</c> otherwise.</returns>
    /// <param name="p">P.</param>
    /// <param name="v0">V0.</param>
    /// <param name="v1">V1.</param>
    /// <param name="v2">V2.</param>
    /// <param name="v3">V3.</param>

    static public bool isPointInRect(Vector3 p, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
    {//检测 p点在v0, v1, v2, v3构成的四边形中
        bool rst = false;
        int cnt = 0; //交点计数
        Vector3 p1 = p + new Vector3(50, 0, 0);
        if (CrossPoint(p, p1, v0, v1))
        {
            cnt++;
        }
        if (CrossPoint(p, p1, v1, v2))
        {
            cnt++;
        }
        if (CrossPoint(p, p1, v2, v3))
        {
            cnt++;
        }
        if (CrossPoint(p, p1, v3, v0))
        {
            cnt++;
        }
        if (cnt == 1)
        {
            rst = true;
        }

        //Debug.LogError("cnt:" + cnt);
        return rst;
    }

    /// <summary>
    /// 返回 角色前方，指定范围内的所有对象(转换成矩型的方式)
    /// </summary>
    /// <returns>The entities front line new.</returns>
    /// <param name="t">T.</param>
    /// <param name="length">Length.</param>
    /// <param name="direction">Direction.</param>
    /// <param name="width">Width.</param>
    /// <param name="offsetX">Offset x.</param>
    /// <param name="offsetY">Offset y.</param>
    /// <param name="angleOffset">Angle offset.</param>
    /// <param name="layerMask">Layer mask.</param>

    static public List<int> GetEntitiesFrontLine_Rect(Dictionary<int, Vector3> target, Vector3 pos, Vector3 dir, float distance, float width, float offsetX = 0, float offsetY = 0)
    {
        List<int> list = new List<int>();

        Vector3 b = new Vector3(0, 1, 0);
        Vector3 dir_left = Vector3.Cross(dir, b);


        Vector3 left = pos + dir_left * (width + offsetX) / 2;
        Debug.DrawLine(pos, left, Color.red, 2f);

        Vector3 right = pos + (-dir_left) * (width + offsetX) / 2;
        Debug.DrawLine(pos, right, Color.red, 2f);

        Vector3 leftEnd = left + dir * (distance + offsetY);
        Debug.DrawLine(left, leftEnd, Color.red, 2f);

        Vector3 rightEnd = right + dir * (distance + offsetY);
        Debug.DrawLine(right, rightEnd, Color.red, 2f);
        Debug.DrawLine(leftEnd, rightEnd, Color.red, 2f);

        List<int> temp = new List<int>(target.Keys);
        for (int i = 0; i < target.Count; i++)
        {
            //Debug.LogError("target:" + temp[i]);
            if (isPointInRect(target[temp[i]], leftEnd, rightEnd, right, left))
            {
                list.Add(temp[i]);
            }
        }
        SortByDistance(pos, list, target);
        return list;
    }

    /// <summary>
    /// 返回角色周围指定半径范围内的所有对象。
    /// </summary>
    /// <param name="t"></param>
    /// <param name="radius"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>

    static public List<int> GetEntitiesInRange(Dictionary<int, Vector3> target, Vector3 pos, Vector3 dir, float radius)
    {
        List<int> list = new List<int>();

        List<int> temp = new List<int>(target.Keys);
        for (int i = 0; i < target.Count; i++)
        {
            if ((pos - target[temp[i]]).magnitude > radius)
            {
                continue;
            }
            list.Add(temp[i]);
        }
        SortByDistance(pos, list, target);
        return list;
    }

    /// <summary>
    /// 返回角色周围指定扇形范围内的所有对象。
    /// </summary>
    /// <param name="t"></param>
    /// <param name="radius"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static List<int> GetEntitiesInSector(Dictionary<int, Vector3> target, Vector3 pos, Vector3 dir, float radius, float angle, float inradius = 0f)
    {
        List<int> list = new List<int>();

        foreach (KeyValuePair<int, Vector3> iter in target)
        {
            float t_distance = (pos - iter.Value).magnitude;

            if (t_distance > radius || t_distance < inradius)
            {
                continue;
            }

            float a = Mathf.Asin(0.5f / (iter.Value - pos).magnitude);
            float b = Vector3.Angle((iter.Value - pos), dir);
            bool result = (b - a) > angle * 0.5f;
            if (result) continue;

            list.Add(iter.Key);
        }

        if (list.Count < 2)
        {
            return list;
        }
        SortByDistance(pos, list, target);

        return list;
    }

    /// <summary>
    /// 由近到远排序
    /// </summary>
    /// <param name="t"></param>
    /// <param name="gos"></param>
    /// <param name="count">返回数量</param>
    /// <returns></returns>
    static public void SortByDistance(Vector3 pos, List<int> gos, Dictionary<int, Vector3> target)
    {
        gos.Sort(delegate (int a, int b)
        {
            Vector3 aPos = target[a];
            Vector3 bPos = target[b];
            if (Vector3.Distance(pos, aPos) >= Vector3.Distance(pos, bPos)) return 1;
            else return -1;
        });

    }

    //判断圆与扇形是否相交
    public static bool IsCircleWithSector(Vector3 center,float minRadius, float maxRadius,Vector3 dir, float degree, Vector3 checkPos, float checkRadius)
    {
        if (checkRadius < 0.0f)
        {
            return false;
        }
        float distance = GetDistanceXZ(center, checkPos);
        if (distance < (minRadius - checkRadius) || distance > (maxRadius + checkRadius))
        {
            return false;
        }
        {
            //left
            Quaternion quaternion = Quaternion.AngleAxis(-degree * 0.5f, Vector3.up);
            Vector3 leftDir = quaternion * dir;
            leftDir.y = 0.0f;
            leftDir.Normalize();
            if (IsIntersectWithSegment(center + leftDir * minRadius, center + leftDir * maxRadius, checkPos, checkRadius))
            {
                return true;
            }
        }

        {
            //right
            Quaternion quaternion = Quaternion.AngleAxis(degree * 0.5f, Vector3.up);
            Vector3 rightDir = quaternion * dir;
            rightDir.y = 0.0f;
            rightDir.Normalize();
            if (IsIntersectWithSegment(center + rightDir * minRadius, center + rightDir * maxRadius, checkPos, checkRadius))
            {
                return true;
            }
        }

        Vector3 dirXZ = dir;
        dirXZ.y = 0.0f;
        dirXZ.Normalize();
        Vector3 checkDir = checkPos - center;
        checkDir.y = 0.0f;
        checkDir.Normalize();
        if (checkDir.sqrMagnitude < EPSILON)
        {
            return true;
        }
        float checkDegree = GetAngle(dirXZ, checkDir);
        if (checkDegree >= -degree * 0.5f && checkDegree <= degree * 0.5f)
        {
            return true;
        }
        return false;
    }
    //判断圆与线段是否相交
    public static bool IsIntersectWithSegment(Vector3 start, Vector3 end, Vector3 checkPos, float checkRadius)
    {
        Vector3 dir = end - start;
        dir.y = 0.0f;
        if (dir.sqrMagnitude < EPSILON)
        {
            return GetDistanceXZ(start, checkPos) <= checkRadius;
        }
        dir.Normalize();
        Vector3 dirPerpendicular = new Vector3(-dir.z, 0.0f, dir.x);
        dirPerpendicular.Normalize();
        if (Mathf.Abs(Vector3.Dot(dirPerpendicular, checkPos) - Vector3.Dot(dirPerpendicular, start)) > checkRadius)
        {
            return false;
        }

        float valueMin = Vector3.Dot(dir, start);
        float valueMax = Vector3.Dot(dir, end);
        if (valueMin > valueMax)
        {
            float temp = valueMin;
            valueMin = valueMax;
            valueMax = temp;
        }
        float valueCheck = Vector3.Dot(dir, checkPos);
        if (valueCheck >= valueMin && valueCheck <= valueMax)
        {
            return true;
        }
        return (GetDistanceXZ(start, checkPos) <= checkRadius || GetDistanceXZ(end, checkPos) <= checkRadius);
    }

    //判断圆与矩形是否相交
    public static bool IsCircleWithRectangle(Vector3 center, Vector3 dir, Vector3 extends, Vector3 _checkPos, float checkRadius)
    {
        if (checkRadius < 0.0f || extends.x < 0.0f || extends.z < 0.0f)
        {
            return false;
        }
        Vector3 dirNormalize = dir;
        dirNormalize.y = 0.0f;
        dirNormalize.Normalize();

        Vector3 checkPos = _checkPos;
        checkPos.y = 0.0f;

        float lineMin = Vector3.Dot(dirNormalize, center - dirNormalize * extends.z);
        float lineMax = Vector3.Dot(dirNormalize, center + dirNormalize * extends.z);
        if (lineMin > lineMax)
        {
            float temp = lineMin;
            lineMin = lineMax;
            lineMax = temp;
        }
        lineMin -= checkRadius;
        lineMax += checkRadius;
        float checkValue = Vector3.Dot(dirNormalize, checkPos);
        if (checkValue < lineMin || checkValue > lineMax)
        {
            return false;
        }

        Vector3 dirPerpendicular = new Vector3(-dirNormalize.z, 0.0f, dirNormalize.x);
        lineMin = Vector3.Dot(dirPerpendicular, center - dirPerpendicular * extends.x);
        lineMax = Vector3.Dot(dirPerpendicular, center + dirPerpendicular * extends.x);
        if (lineMin > lineMax)
        {
            float temp = lineMin;
            lineMin = lineMax;
            lineMax = temp;
        }
        lineMin -= checkRadius;
        lineMax += checkRadius;
        checkValue = Vector3.Dot(dirPerpendicular, checkPos);
        if (checkValue < lineMin || checkValue > lineMax)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// Calculates the area of a triangle.
    /// 计算三个点之间三角形的面积
    /// <returns>The triangle area.</returns>
    public static float TriangleArea(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2)
    {
        var dx = p1 - p0;
        var dy = p2 - p0;
        
        double data = System.Math.Sin(Vector3.Angle(dx, dy) * Deg2Rad) * Vector3.Magnitude(dy);
        return Vector3.Magnitude(dx) * (float)data * 0.5f;
    }
    public static float GetDistanceXZ(Vector3 pos1, Vector3 pos2)
    {
        float xOffset = pos1.x - pos2.x;
        float zOffset = pos1.z - pos2.z;
        float distance = Mathf.Sqrt(xOffset * xOffset + zOffset * zOffset);
        return distance;
    }
    public static float GetAngle(Vector3 dir1, Vector3 dir2)
    {
        float angle = Quaternion.Angle(Quaternion.LookRotation(dir1), Quaternion.LookRotation(dir2));
        if (angle < 0.0f)
        {
            angle = -angle;
        }
        if (angle > 180.0f)
        {
            int iAngle = (int)angle;
            iAngle /= 180;
            iAngle *= 180;
            angle -= iAngle;
        }
        return angle;
    }
    /// <summary>
    /// Restricts the angle between -360 and 360 degrees.
    /// </summary>
    /// <param name="angle">The angle to restrict.</param>
    /// <returns>An angle between -360 and 360 degrees.</returns>
    public static float RestrictAngle(float angle)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return angle;
    }

    /// <summary>
    /// Restricts the angle between -180 and 180 degrees.
    /// </summary>
    /// <param name="angle">The angle to restrict.</param>
    /// <returns>An angle between -180 and 180 degrees.</returns>
    public static float RestrictInnerAngle(float angle)
    {
        if (angle < -180)
        {
            angle += 360;
        }
        if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }
    //----------
    public static float Distance2D(float x1, float y1, float x2, float y2)
    {
        float x = x1 - x2;
        float y = y1 - y2;

        return Mathf.Sqrt(x * x + y * y);
    }
    public static int RoundToInt(float val)
    {
        return (int)Math.Round(val, MidpointRounding.AwayFromZero);
    }
    public static int RoundToInt(double val)
    {
        return (int)Math.Round(val, MidpointRounding.AwayFromZero);
    }
    public static long RoundToLong(float val)
    {
        return (long)Math.Round(val, MidpointRounding.AwayFromZero);
    }

    public static long RoundToLong(double val)
    {
        return (long)Math.Round(val, MidpointRounding.AwayFromZero);
    }
    /// <summary>
    /// http://stackoverflow.com/questions/4061576/finding-points-on-a-rectangle-at-a-given-angle
    /// </summary>
    /// <returns>根据角度计算矩形上的点，center为0时，返回的点的坐标的原点是矩形的左下角，center设置为size/2，则返回的坐标的原点是矩形中心.</returns>
    /// <param name="center">Center.</param>
    /// <param name="size">Size.</param>
    /// <param name="radius">Radius.</param>
    public static Vector2 FindPointOnRectangle(Vector2 center, Vector2 size, float radius)
    {
        float twoPI = Mathf.PI * 2;

        while (radius < -Mathf.PI)
        {
            radius += twoPI;
        }

        float rectAtan = Mathf.Atan2(size.y, size.x);
        float tanRadius = Mathf.Tan(radius);
        int region;

        if ((radius > -rectAtan) && (radius <= rectAtan))
        {
            region = 1;
        }
        else if ((radius > rectAtan) && (radius <= (Mathf.PI - rectAtan)))
        {
            region = 2;
        }
        else if ((radius > (Math.PI - rectAtan)) || (radius <= -(Mathf.PI - rectAtan)))
        {
            region = 3;
        }
        else
        {
            region = 4;
        }

        Vector2 edgePoint = new Vector2(size.x / 2, size.y / 2);
        edgePoint -= center;
        int xFactor = 1;
        int yFactor = 1;

        switch (region)
        {
            case 1: xFactor = 1; yFactor = 1; break;
            case 2: xFactor = 1; yFactor = 1; break;
            case 3: xFactor = -1; yFactor = -1; break;
            case 4: xFactor = -1; yFactor = -1; break;
        }

        if ((region == 1) || (region == 3))
        {
            edgePoint.x += (xFactor * (size.x / 2f));                                     // "Z0"
            edgePoint.y += (yFactor * (size.x / 2f) * tanRadius);
        }
        else
        {
            edgePoint.x += (xFactor * (size.y / (2f * tanRadius)));                        // "Z1"
            edgePoint.y += (yFactor * (size.y / 2f));
        }

        return edgePoint;
    }
    public static bool IsApproximatelyEqualTo(double initialValue, double value)
    {
        return IsApproximatelyEqualTo(initialValue, value, 0.00001);
    }

    public static bool IsApproximatelyEqualTo(double initialValue, double value, double maximumDifferenceAllowed)
    {
        // Handle comparisons of floating point values that may not be exactly the same
        return (Math.Abs(initialValue - value) < maximumDifferenceAllowed);
    }
    public static Vector3 GetXZPosition(Vector3 position)
    {
        return new Vector3(position.x, 0f, position.z);
    }
    public static float DistanceXZ(Vector3 from, Vector3 to)
    {
        float x = from.x - to.x;
        float z = from.z - to.z;

        return Mathf.Sqrt(x * x + z * z);
    }
    public static float SqrDistanceXZ(Vector3 from, Vector3 to)
    {
        float x = from.x - to.x;
        float z = from.z - to.z;

        return x * x + z * z;
    }

    public static float SqrDistanceUI(Vector3 from, Vector3 to)
    {
        float x = from.x - to.x;
        float y = from.y - to.y;

        return x * x + y * y;
    }
    public static Quaternion LookRotationXZ(Vector3 direction)
    {
        return Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    }
    public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 translate;
        translate.x = matrix.m03;
        translate.y = matrix.m13;
        translate.z = matrix.m23;
        return translate;
    }
    public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }
    public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }

    
    public static void DecomposeMatrix(ref Matrix4x4 matrix, out Vector3 localPosition, out Quaternion localRotation, out Vector3 localScale)
    {
        localPosition = ExtractTranslationFromMatrix(ref matrix);
        localRotation = ExtractRotationFromMatrix(ref matrix);
        localScale = ExtractScaleFromMatrix(ref matrix);
    }
    public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix)
    {
        transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
        transform.localRotation = ExtractRotationFromMatrix(ref matrix);
        transform.localScale = ExtractScaleFromMatrix(ref matrix);
    }

    public static readonly Quaternion IdentityQuaternion = Quaternion.identity;

    public static readonly Matrix4x4 IdentityMatrix = Matrix4x4.identity;


    public static Matrix4x4 TranslationMatrix(Vector3 offset)
    {
        Matrix4x4 matrix = IdentityMatrix;
        matrix.m03 = offset.x;
        matrix.m13 = offset.y;
        matrix.m23 = offset.z;
        return matrix;
    }

    public static bool IsLineInsection(ref Vector2 v1, ref Vector2 v2, ref Vector2 v3, ref Vector2 v4)
    {
        float d = (v4.y - v3.y) * (v2.x - v1.x) - (v4.x - v3.x) * (v1.y - v1.y);
        float u = (v4.x - v3.x) * (v1.y - v3.y) - (v4.y - v3.y) * (v1.x - v3.x);
        float v = (v2.x - v1.x) * (v1.y - v3.y) - (v2.y - v1.y) * (v1.x - v3.x);
        if (d < 0)
        {
            d = -d;
            u = -u;
            v = -v;
        }
        return (u >= 0 && u <= d) && (v >= 0 && v <= d);
    }

    private static float CrossVal(ref Vector2 u, ref Vector2 v)
    {
        return u.x * v.y - u.y * v.x;
    }

    public static bool IsPointInTriangle(ref Vector2 a, ref Vector2 b, ref Vector2 c, ref Vector2 p)
    {
        Vector2 v0 = c - a;
        Vector2 v1 = b - a;
        Vector2 v2 = p - a;
        float u = CrossVal(ref v2, ref v0);
        float v = CrossVal(ref v1, ref v2);
        float d = CrossVal(ref v1, ref v0);
        if (d < 0)
        {
            d = -d;
            u = -u;
            v = -v;
        }
        return u >= 0 && v >= 0 && (u + v) <= d;
    }

    public static bool IsTriangleInsect(Vector2 t10, Vector2 t11, Vector2 t12, Vector2 t20, Vector2 t21, Vector2 t22)
    {
        if (IsLineInsection(ref t10, ref t11, ref t20, ref t21) ||
            IsLineInsection(ref t10, ref t11, ref t20, ref t22) ||
            IsLineInsection(ref t10, ref t11, ref t21, ref t22) ||
            IsLineInsection(ref t10, ref t12, ref t20, ref t21) ||
            IsLineInsection(ref t10, ref t12, ref t20, ref t22) ||
            IsLineInsection(ref t10, ref t12, ref t21, ref t22) ||
            IsLineInsection(ref t11, ref t12, ref t20, ref t21) ||
            IsLineInsection(ref t11, ref t12, ref t20, ref t22) ||
            IsLineInsection(ref t11, ref t12, ref t21, ref t22) ||
            IsPointInTriangle(ref t10, ref t11, ref t12, ref t20) ||
            IsPointInTriangle(ref t10, ref t11, ref t12, ref t21) ||
            IsPointInTriangle(ref t10, ref t11, ref t12, ref t22) ||
            IsPointInTriangle(ref t20, ref t21, ref t22, ref t10) ||
            IsPointInTriangle(ref t20, ref t21, ref t22, ref t11) ||
            IsPointInTriangle(ref t20, ref t21, ref t22, ref t12))
        {
            return true;
        }
        return false;
    }
}

public static class MathExtensions
{
    public static bool IsRectIntEquals(this RectInt rect1, RectInt rect2)
    {
        return rect1.position == rect2.position && rect1.height == rect2.height && rect1.width == rect2.width;
    }

    public static Vector3 Add(this Vector3 v1, Vector2 v2)
    {
        return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z);
    }

    public static Vector3 Subtract(this Vector3 v1, Vector2 v2)
    {
        return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z);
    }
}
