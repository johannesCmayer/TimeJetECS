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

public class TimeWarpSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerTag>().ForEach((ref SteeringInput steeringInput, ref MoveSpeed moveSpeed) => {
            var targetTimescale = math.max(0.01f, math.min(1, math.abs(steeringInput.pitch) + math.abs(steeringInput.roll) + math.abs(steeringInput.yaw) + math.length(moveSpeed.Value)));
            UIDataManager.instance.timscaleIndicator.value = targetTimescale;
            Time.timeScale = targetTimescale;
        });
    }
}
