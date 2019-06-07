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

public class Boot : MonoBehaviour
{
    public Mesh playerPlaneMesh;
    public Mesh EnemyPlaneMesh;
    public Mesh missileMesh;
    public Material playerPlaneMaterial;
    public Material enemyPlaneMaterial;
    public Material missileMaterial;

    EntityManager entityManager;

    void Start()
    {
        entityManager = World.Active.EntityManager;

        UnitEnityDefinitions.Setup();

        var player = UnitEnityDefinitions.SetupPlanes(UnitEnityDefinitions.playerPlaneArechetype, 1, playerPlaneMesh, playerPlaneMaterial)[0];
        var enemyPlanes = UnitEnityDefinitions.SetupPlanes(UnitEnityDefinitions.enemyPlaneArechetype, 200, EnemyPlaneMesh, enemyPlaneMaterial);

        for (int i = 0; i < enemyPlanes.Length; i++)
        {
            UnitEnityDefinitions.SetupMissile(new float3(R.Range(-10, 10), R.Range(-10, 10), R.Range(-10, 10)) + new float3(0, 0, 100), math.normalize(new quaternion(0.5f,0,0.0f,0)), enemyPlanes[i], missileMesh, missileMaterial);
        }
    }
}

public class DebugMovePlayer : ComponentSystem
{
    protected override void OnUpdate()
    {
        var s = math.sin(Time.realtimeSinceStartup);
        var c = math.cos(Time.realtimeSinceStartup);
        Entities.WithAll<PlayerTag>().ForEach((ref Rotation rotation) =>
        {            
            rotation.Value = math.normalize(new quaternion(s, c, 0.0f, 1));
        });
    }
}