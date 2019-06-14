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

public class EnemyTargetingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<EnemyAITag>().ForEach((ref TargetSelection targetSelection, ref Translation translation, ref LocalToWorld localToWorld, ref ShootWeapon shootWeapon) => {
            var targetPos = World.Active.EntityManager.GetComponentData<Translation>(targetSelection.target).Value;
            if (math.dot(math.normalize(targetPos - translation.Value), localToWorld.Forward) > 0.9)
            {
                shootWeapon.shoot = true;
            }
        });
    }
}
