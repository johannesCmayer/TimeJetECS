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

public enum ConvMode
{
    latlon_to_vec,
    vec_to_latlon
}

[ExecuteInEditMode]
public class LatlonRotation : MonoBehaviour
{
    public ConvMode convMode;

    public GameObject dirObj;
    public Vector2 latlon;

    float3 latlon_to_dir(float2 latlon)
    {
        float x = math.sin(latlon.y) * math.cos(latlon.x);
        float y = math.sin(latlon.x);
        float z = math.cos(latlon.y) * math.cos(latlon.x);        
        return new float3(x, y, z);
    }

    float catan2(float a, float b)
    {
        var ret = math.atan(a / b);
        if (b < 0)
            return ret + math.PI;
        return ret;
    }

    float2 dir_to_latlon(float3 dir)
    {
        return new float2(catan2(dir.y, math.length(new float2(dir.z, dir.x))), 
                          catan2(dir.x, dir.z));
    }

    void Update()
    {
        var test = latlon_to_dir(new float2(1, 1));

        print(math.length(test));

        if (dirObj != null)
        {
            switch (convMode)
            {
                case ConvMode.latlon_to_vec:
                    dirObj.transform.forward = latlon_to_dir(latlon);
                    break;
                case ConvMode.vec_to_latlon:
                    latlon = dir_to_latlon(dirObj.transform.forward);
                    break;
            }
        }

        Debug.DrawRay(dirObj.transform.position, dirObj.transform.forward);

        transform.rotation = quaternion.identity;
        transform.rotation = quaternion.Euler(0, latlon.y, 0);
        transform.RotateAround(transform.position, transform.right, -math.degrees(latlon.x));
    }
}
