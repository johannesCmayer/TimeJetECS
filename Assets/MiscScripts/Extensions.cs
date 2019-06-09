using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R = UnityEngine.Random;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public static class A
{
    public static float3 forward(this quaternion quat)
    {
        var a = math.normalize(new quaternion(0, 0, 0, 1));
        var r = math.mul(math.mul(quat.value, a), math.inverse(quat.value));
        return new float3(r.value.x, r.value.y, r.value.z);

        //var q = quat.value;
        //var x = 2 * (q.x * q.z + q.w * q.y);
        //var y = 2 * (q.y * q.z - q.w * q.x);
        //var z = 1 - 2 * (q.x * q.x + q.y * q.y);
        //return new float3(x, y, z) ;
    }

    public static float3 up(this quaternion quat)
    {
        var q = quat.value;
        var x = 2 * (q.x * q.y - q.w * q.z);
        var y = 1 - 2 * (q.x * q.x + q.z * q.z);
        var z = 2 * (y * q.z + q.w * x);
        return new float3(x, y, z);
    }

    public static float3 right(this quaternion quat)
    {
        var q = quat.value;
        var x = 1 - 2 * (q.y * q.y + q.z * q.z);
        var y = 2 * (x * q.y + q.w * q.z);
        var z = 2 * (x * q.z - q.w * q.y);
        return new float3(x, y, z);
    }

}
