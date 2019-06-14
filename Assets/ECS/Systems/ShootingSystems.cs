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

[UpdateInGroup(typeof(SpawningSystemGroup))]
public class ShootingSystem : ComponentSystem
{
    float shotXOffset = 3f;

    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation translation, ref Rotation rotation, ref Velocity velocity, ref LocalToWorld localToWorld, ref ShootWeapon shootWeapon) =>
        {
            if (shootWeapon.shoot && shootWeapon.cooldownTimer <= 0)
            {
                var spawnPos = translation.Value + localToWorld.Up * -1 + localToWorld.Forward * 1.1f + localToWorld.Right * shotXOffset;
                UnitEnityDefinitions.SetupShot(PostUpdateCommands, spawnPos, rotation.Value, localToWorld, velocity);
                shotXOffset = -shotXOffset;
                shootWeapon.shoot = false;
                shootWeapon.cooldownTimer = shootWeapon.cooldown;
            }
            else
            {
                shootWeapon.cooldownTimer -= Time.deltaTime;
            }
        });
    }
}
