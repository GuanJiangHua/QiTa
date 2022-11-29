using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondOrderBezierCurveTool 
{
    public static Vector3 GetBezierCurvePos(Vector3 startPos, Vector3 endPos, Vector3 midpoint, float t)
    {
        Vector3 p1 = Vector3.Lerp(startPos, midpoint, t);
        Vector3 p2 = Vector3.Lerp(midpoint, endPos, t);
        Vector3 pos = Vector3.Lerp(p1, p2, t);

        return pos;
    }

    public static Vector3 GetBezierCurveUniformSpeedPos(Vector3 startPos, Vector3 endPos, Vector3 midpoint, float t)
    {
        Vector3 p0 = startPos;
        Vector3 p1 = midpoint;
        Vector3 p2 = endPos;

        float ax = p0.x - 2 * p1.x + p2.x;
        float ay = p0.y - 2 * p1.y + p2.y;
        float az = p0.z - 2 * p1.z + p2.z;
        float bx = 2 * p1.x - 2 * p0.x;
        float by = 2 * p1.y - 2 * p0.y;
        float bz = 2 * p1.z - 2 * p0.z;

        float A = 4 * (ax * ax + ay * ay + az * az);
        float B = 4 * (ax * bx + ay * by + az * bz);
        float C = bx * bx + by * by + bz * bz;
        //曲线总长度:
        float totalLength = CurveLength(A,B,C,1);
        float l = t * totalLength;
        float t_ = InvertCurveLength(A, B, C, t, l);

        float x = (1 - t_) * (1 - t_) * p0.x + 2 * (1 - t_) * t_ * p1.x + t_ * t_ * p2.x;
        float y = (1 - t_) * (1 - t_) * p0.y + 2 * (1 - t_) * t_ * p1.y + t_ * t_ * p2.y;
        float z = (1 - t_) * (1 - t_) * p0.z + 2 * (1 - t_) * t_ * p1.z + t_ * t_ * p2.z;
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 曲线各点的速度函数
    /// </summary>
    private static float CurveSpeed(float A, float B, float C,float t)
    {
        return Mathf.Sqrt(A * t * t + B * t + C);
    }

    /// <summary>
    /// 求曲线长度的函数
    /// </summary>
    private static float CurveLength(float A, float B, float C, float t)
    {
        float temp1 = Mathf.Sqrt(C + t * (B + A * t));
        float temp2 = (2 * A * t * temp1 + B * (temp1 - Mathf.Sqrt(C)));
        float temp3 = Mathf.Log(B + 2 * Mathf.Sqrt(A) * Mathf.Sqrt(C));
        float temp4 = Mathf.Log(B + 2 * A * t + 2 * Mathf.Sqrt(A) * temp1);
        float temp5 = 2 * Mathf.Sqrt(A) * temp2;
        float temp6 = (B * B - 4 * A * C) * (temp3 - temp4);
        return (temp5 + temp6) / (8 * Mathf.Pow(A, 1.5f));
    }
    public static float CurveLength(Vector3 startPos, Vector3 endPos, Vector3 midpoint)
    {
        Vector3 p0 = startPos;
        Vector3 p1 = midpoint;
        Vector3 p2 = endPos;

        float ax = p0.x - 2 * p1.x + p2.x;
        float ay = p0.y - 2 * p1.y + p2.y;
        float az = p0.z - 2 * p1.z + p2.z;
        float bx = 2 * p1.x - 2 * p0.x;
        float by = 2 * p1.y - 2 * p0.y;
        float bz = 2 * p1.z - 2 * p0.z;

        float A = 4 * (ax * ax + ay * ay + az * az);
        float B = 4 * (ax * bx + ay * by + az * bz);
        float C = bx * bx + by * by + bz * bz;

        return CurveSpeed(A, B, C, 1);
    }
    /// <summary>
    /// 长度函数的反函数
    /// </summary>
    /// 使用牛顿切线法求解,求出t'带入贝塞尔曲线函数可得到匀速条件下对应的点
    /// <returns></returns>
    private static float InvertCurveLength(float A, float B, float C, float t, float l)
    {
        float t1 = t, t2;

        do
        {
            t2 = t1 - (CurveLength(A, B, C, t1) - l) / CurveSpeed(A,B,C,t1);
            if (Mathf.Abs(t1 - t2) < 0.00001) break;
            t1 = t2;
        } while (true);

        return t2;
    }
}
