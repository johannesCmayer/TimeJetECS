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

public class UnitEnityDefinitions
{
    public static EntityArchetype playerPlaneArechetype;
    public static EntityArchetype friendlyPlaneArechetype;
    public static EntityArchetype enemyPlaneArechetype;
    public static EntityArchetype missileArechetype;
    //public static EntityArchetype meshChild;
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
            typeof(MoveSpeed),
            typeof(SteerInput),

            typeof(Translation),
            typeof(Rotation),
            typeof(Scale),
            typeof(LocalToWorld),

            typeof(SphereCollider),
            typeof(RenderMesh)
        );

        friendlyPlaneArechetype = entityManager.CreateArchetype(
            typeof(FriendlyAITag),
            typeof(MoveSpeed),
            typeof(SteerInput),

            typeof(Translation),
            typeof(Rotation),
            typeof(Scale),
            typeof(LocalToWorld),

            typeof(SphereCollider),
            typeof(RenderMesh)
        );

        enemyPlaneArechetype = entityManager.CreateArchetype(
            typeof(EnemyAITag),
            typeof(MoveSpeed),
            typeof(SteerInput),

            typeof(Translation),
            typeof(Rotation),
            typeof(Scale),
            typeof(LocalToWorld),

            typeof(SphereCollider),
            typeof(RenderMesh)
        );

        missileArechetype = entityManager.CreateArchetype(
            typeof(MissileTag),
            typeof(HasTrailTag),

            typeof(MoveSpeed),
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

        //meshChild = entityManager.CreateArchetype(
        //    typeof(RenderMesh),

        //    typeof(LocalToWorld),
        //    typeof(LocalToParent),
        //    typeof(Parent),

        //    typeof(Translation),
        //    typeof(Rotation),
        //    typeof(Scale)
        //);
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
            entityManager.SetComponentData(plane, new MoveSpeed { Value = 1f });
            entityManager.SetSharedComponentData(plane, new RenderMesh
            {
                mesh = mesh,
                material = material
            });
        }
        return planes;
    }

    public static void SetupMissile(float3 pos, quaternion rotation, Entity target)
    {
        var missile = entityManager.CreateEntity(UnitEnityDefinitions.missileArechetype);
        entityManager.SetComponentData(missile, new Translation { Value = pos });
        entityManager.SetComponentData(missile, new Rotation { Value = rotation });
        entityManager.SetComponentData(missile, new SphereCollider { size = 1f });
        entityManager.SetComponentData(missile, new MoveSpeed { Value = 10 });
        entityManager.SetComponentData(missile, new Scale { Value = 1f });
        entityManager.SetSharedComponentData(missile, new RenderMesh
        {
            mesh = GlobalData.instance.missileMesh,
            material = GlobalData.instance.missileMaterial
        });
        entityManager.SetComponentData(missile, new Scale { Value = 0.3f });
        entityManager.SetComponentData(missile, new TargetSelection { target = target });
        entityManager.SetComponentData(missile, new RotationSpeed { Value = 1 });
    }
}
