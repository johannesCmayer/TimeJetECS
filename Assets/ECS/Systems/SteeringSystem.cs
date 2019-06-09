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
        Entities.WithNone<AngularVelocity>().ForEach((ref Rotation rotation, ref SteeringInput steerInput) => {
            var forward = rotation.Value.forward();
            var up = rotation.Value.up();
            var left = rotation.Value.right();

            var r = new float3(steerInput.pitch * Time.deltaTime, steerInput.yaw * Time.deltaTime, steerInput.roll * Time.deltaTime);
            rotation.Value *= Quaternion.Euler(r);
        });

        Entities.ForEach((ref Rotation rotation, ref SteeringInput steerInput, ref AngularVelocity angularVelocity) => {
            var forward = rotation.Value.forward();
            var up = rotation.Value.up();
            var left = rotation.Value.right();

            angularVelocity.Value = new float3(
                angularVelocity.Value.x * steerInput.pitch * Time.deltaTime,
                angularVelocity.Value.y * steerInput.yaw * Time.deltaTime,
                angularVelocity.Value.z * steerInput.roll * Time.deltaTime) * 0.01f;
            rotation.Value *= Quaternion.Euler(angularVelocity.Value);
        });
    }
}
