using System;
using UnityEngine;

// https://answers.unity.com/questions/478617/not-interpolate-quaternion-on-shortest-path.html
public static class QuaternionExtension
{
    public static Quaternion Lerp(Quaternion p, Quaternion q, float t, bool shortWay)
    {
        if (shortWay)
        {
            float dot = Quaternion.Dot(p, q);
            if (dot < 0.0f)
                return Lerp(ScalarMultiply(p, -1.0f), q, t, true);
        }
        else
        {
            float dot = Quaternion.Dot(p, q);
            if (dot > 0.0f)
                return Lerp(ScalarMultiply(p, -1.0f), q, t, false);
        }

        Quaternion r = Quaternion.identity;
        r.x = p.x * (1f - t) + q.x * (t);
        r.y = p.y * (1f - t) + q.y * (t);
        r.z = p.z * (1f - t) + q.z * (t);
        r.w = p.w * (1f - t) + q.w * (t);
        return r;
    }

    public static Quaternion Slerp(Quaternion p, Quaternion q, float t, bool shortWay)
    {
        float dot = Quaternion.Dot(p, q);
        if (shortWay)
        {
            if (dot < 0.0f)
                return Slerp(ScalarMultiply(p, -1.0f), q, t, true);
        }
        else
        {
            if (dot > 0.0f)
                return Slerp(ScalarMultiply(p, -1.0f), q, t, false);
        }

        float angle = Mathf.Acos(dot);
        Quaternion first = ScalarMultiply(p, Mathf.Sin((1f - t) * angle));
        Quaternion second = ScalarMultiply(q, Mathf.Sin((t) * angle));
        float division = 1f / Mathf.Sin(angle);
        return ScalarMultiply(Add(first, second), division);
    }


    public static Quaternion ScalarMultiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }

    public static Quaternion Add(Quaternion p, Quaternion q)
    {
        return new Quaternion(p.x + q.x, p.y + q.y, p.z + q.z, p.w + q.w);
    }
}