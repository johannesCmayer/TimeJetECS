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

//TODO Jobify
public class SteeringSystem : ComponentSystem
{
    const float additionalForceWhenBreaking = 0;

    const float maxAngularVelocity = 100;

    protected override void OnUpdate()
    {
        // Normal Steer
        Entities.WithNone<AngularVelocity>().ForEach((ref Rotation rotation, ref SteeringInput steerInput) => {
            var r = new float3(steerInput.pitch * Time.deltaTime, steerInput.yaw * Time.deltaTime, steerInput.roll * Time.deltaTime);
            rotation.Value *= Quaternion.Euler(r);
        });

        // Angular Velocity Steer
        Entities.ForEach((ref Rotation rotation, ref SteeringInput steerInput, ref AngularVelocity angularVelocity) => {
            var previousAngularVelocity = angularVelocity;

            var rotationChange = new float3(steerInput.pitch * Time.deltaTime, steerInput.yaw * Time.deltaTime, steerInput.roll * Time.deltaTime) * 0.05f;
            angularVelocity.Value += rotationChange;

            float angularAcceleration = math.length(angularVelocity.Value) - math.length(previousAngularVelocity.Value);

            if (angularAcceleration < 0)
                angularVelocity.Value += rotationChange * additionalForceWhenBreaking;

            angularVelocity.Value = new float3(
                math.min(maxAngularVelocity, math.abs(angularVelocity.Value.x)) * math.sign(angularVelocity.Value.x), 
                math.min(maxAngularVelocity, math.abs(angularVelocity.Value.y)) * math.sign(angularVelocity.Value.y), 
                math.min(maxAngularVelocity, math.abs(angularVelocity.Value.z)) * math.sign(angularVelocity.Value.z));

            rotation.Value *= Quaternion.Euler(angularVelocity.Value);
        });
    }
}
