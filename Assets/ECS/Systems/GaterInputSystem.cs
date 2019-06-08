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

public class GaterInputSystem : ComponentSystem
{
    float speed = 20;

    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerTag>().ForEach((ref SteeringInput steerInput) => {
            steerInput = new SteeringInput
            {   
                pitch = Input.GetAxis("Vertical") * speed,
                roll = Input.GetAxis("Horizontal") * speed,
                yaw = 0
            };
        });
    }
}
