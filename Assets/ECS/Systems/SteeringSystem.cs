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

public class SteeringSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation, ref SteeringInput steerInput) => {
            var forward = rotation.Value.forward();
            var up = rotation.Value.up();
            var left = rotation.Value.left();
            //var q = quaternion.identity;
            //q *= (Quaternion)quaternion.AxisAngle(left, steerInput.pitch * Time.deltaTime) ;
            //q *= (Quaternion)quaternion.AxisAngle(up, steerInput.yaw * Time.deltaTime) ;
            //q *= (Quaternion)quaternion.AxisAngle(forward, steerInput.roll * Time.deltaTime) ;
            var r = new float3(steerInput.pitch * Time.deltaTime, steerInput.yaw * Time.deltaTime, steerInput.roll * Time.deltaTime);
            rotation.Value *= Quaternion.Euler(r);
        });
    }
}
