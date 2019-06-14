﻿using System.Collections;
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

public class UnitEnityDefinitions
{
    public static Entity playerPrefab;

    public static EntityArchetype playerPlaneArechetype;
    public static EntityArchetype friendlyPlaneArechetype;
    public static EntityArchetype enemyPlaneArechetype;
    public static EntityArchetype missileArechetype;
    public static EntityArchetype shotArechetype;

    static bool isSetup;
    static EntityManager entityManager;

    public static void Setup()
    {
        Debug.Log($"setup {typeof(UnitEnityDefinitions)}");
        if (isSetup)
            return;
        isSetup = true;

        entityManager = World.Active.EntityManager;

        playerPlaneArechetype = entityManager.CreateArchetype(
            typeof(PlayerTag),
            typeof(Prefab),
            typeof(ShootWeapon),
            typeof(Alive),
            typeof(Respawn),
            typeof(UseUnscaledDeltatime),

            typeof(MoveSpeed),
            typeof(Velocity),
            typeof(AngularVelocity),
            typeof(SteeringInput),

            typeof(Translation),
            typeof(Rotation),
            typeof(Scale),
            typeof(LocalToWorld),

            typeof(SphereCollider),
            typeof(RenderMesh)
        );

        playerPrefab = SetupPlanes(playerPlaneArechetype, 1, GlobalData.instance.friendlyPlaneMesh, GlobalData.instance.friendlyPlaneMaterial)[0];
        entityManager.SetSharedComponentData(playerPrefab, new RenderMesh
        {
            mesh = GlobalData.instance.friendlyPlaneMesh,
            material = GlobalData.instance.friendlyPlaneMaterial,
            layer = 9
        });
        entityManager.SetComponentData(playerPrefab, new Respawn {
            pos = new float3(0, 0, -100),
            prefab = playerPrefab
        });

        friendlyPlaneArechetype = entityManager.CreateArchetype(
            typeof(FriendlyAITag),
            typeof(Respawn),
            typeof(Alive),
            typeof(ShootWeapon),

            typeof(MoveSpeed),
            typeof(RotationSpeed),
            typeof(Velocity),
            typeof(TargetSelection),
            typeof(TurnTowardsTarget),

            typeof(Translation),
            typeof(Rotation),
            typeof(Scale),
            typeof(LocalToWorld),

            typeof(SphereCollider),
            typeof(RenderMesh)
        );

        enemyPlaneArechetype = entityManager.CreateArchetype(
            typeof(EnemyAITag),
            typeof(Respawn),
            typeof(Alive),
            typeof(ShootWeapon),

            typeof(MoveSpeed),
            typeof(RotationSpeed),
            typeof(Velocity),
            typeof(TargetSelection),
            typeof(TurnTowardsTarget),

            typeof(Translation),
            typeof(Rotation),
            typeof(Scale),
            typeof(LocalToWorld),

            typeof(SphereCollider),
            typeof(RenderMesh)
        );

        missileArechetype = entityManager.CreateArchetype(
            typeof(MissileTag),
            typeof(HasTrail),

            typeof(MoveSpeed),
            typeof(Velocity),
            typeof(RotationSpeed),
            typeof(TargetSelection),
            typeof(TurnTowardsTarget),

            typeof(Translation),
            typeof(Rotation),
            typeof(Scale),
            typeof(LocalToWorld),

            typeof(RenderMesh),

            typeof(SphereCollider)
        );

        shotArechetype = entityManager.CreateArchetype(
            typeof(ProjectileTag),
            typeof(HasTrail),

            typeof(MoveSpeed),
            typeof(Velocity),

            typeof(Translation),
            typeof(Rotation),
            typeof(Scale),
            typeof(LocalToWorld),

            typeof(RenderMesh),

            typeof(SphereCollider),
            typeof(DestroyAfterTime)
        );
    }

    public static NativeArray<Entity> SetupPlanes(EntityArchetype a, int amount, Mesh mesh, Material material)
    {
        NativeArray<Entity> planes = new NativeArray<Entity>(amount, Allocator.Temp);
        entityManager.CreateEntity(a, planes);
        for (int i = 0; i < amount; i++)
        {
            var plane = planes[i];
            var r = 20;
            var rpos = new float3(R.Range(-r, r), R.Range(-r, r), R.Range(-r, r));
            entityManager.SetComponentData(plane, new Translation { Value = rpos });
            entityManager.SetComponentData(plane, new Rotation { Value = quaternion.identity });
            entityManager.SetComponentData(plane, new Scale { Value = 1f });
            entityManager.SetComponentData(plane, new SphereCollider { size = 1f });
            entityManager.SetComponentData(plane, new MoveSpeed { Value = new float3(0, 0, 0f) });
            //entityManager.SetComponentData(plane, new Velocity { Value = new float3(0, 0, 1f) });
            entityManager.SetComponentData(plane, new ShootWeapon { cooldown = 0.05f });
            entityManager.SetSharedComponentData(plane, new RenderMesh
            {
                mesh = mesh,
                material = material
            });
        }
        return planes;
    }

    public static void SetupMissile(float3 pos, quaternion rotation, Velocity v, Entity target)
    {
        var missile = entityManager.CreateEntity(missileArechetype);
        entityManager.SetComponentData(missile, new Translation { Value = pos });
        entityManager.SetComponentData(missile, new Rotation { Value = rotation });
        entityManager.SetComponentData(missile, new SphereCollider { size = 1f });
        entityManager.SetComponentData(missile, new MoveSpeed { Value = new float3(0, 0, 10f) });
        entityManager.SetComponentData(missile, new Velocity { Value = v.Value });
        entityManager.SetComponentData(missile, new RotationSpeed { Value = 4 });
        entityManager.SetComponentData(missile, new Scale { Value = 1f });
        entityManager.SetSharedComponentData(missile, new RenderMesh
        {
            mesh = GlobalData.instance.missileMesh,
            material = GlobalData.instance.missileMaterial
        });
        entityManager.SetComponentData(missile, new Scale { Value = 0.3f });
        entityManager.SetComponentData(missile, new TargetSelection { target = target });
        entityManager.SetComponentData(missile, new HasTrail { trailID = TrailID.Missile });
    }

    public static void SetupShot(EntityCommandBuffer buffer, float3 pos, quaternion rotation, LocalToWorld localToWorld,  Velocity v)
    {        
        var shot = buffer.CreateEntity(shotArechetype);
        buffer.SetComponent(shot, new Translation { Value = pos });
        buffer.SetComponent(shot, new Rotation { Value = rotation });
        buffer.SetComponent(shot, new SphereCollider { size = 0.2f });
        buffer.SetComponent(shot, new MoveSpeed { Value = new float3(0, 0, 0) });
        buffer.SetComponent(shot, new Velocity { Value = v.Value + localToWorld.Forward * 150 });
        //buffer.SetSharedComponent(shot, new RenderMesh
        //{
        //    mesh = GlobalData.instance.shotMesh,
        //    material = GlobalData.instance.shotMaterial
        //});
        buffer.SetComponent(shot, new Scale { Value = 1f });
        buffer.SetComponent(shot, new DestroyAfterTime { timeToDestruction = 10 });
        buffer.SetComponent(shot, new HasTrail { trailID = TrailID.Shot });
    }
}
