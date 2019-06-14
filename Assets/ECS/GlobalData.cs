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

public enum TrailID
{
    NotSet,
    Missile,
    Shot
}

public class GlobalData : MonoBehaviour
{
    [Header("Units")]
    public Mesh friendlyPlaneMesh;
    public Material friendlyPlaneMaterial;

    public Mesh EnemyPlaneMesh;
    public Material enemyPlaneMaterial;

    [Header("Wepons")]
    public Mesh missileMesh;
    public Material missileMaterial;

    public Mesh shotMesh;
    public Material shotMaterial;

    [Header("Effects")]
    public Mesh explosionMesh;
    public Material explosionMaterial;

    [Header("Weapons")]
    public Mesh missileTrailMesh;
    public Material missileTrailMaterial;

    [Header("Effects/Prebafs")]
    [SerializeField] private GameObject missileTrailPrefab;
    [SerializeField] private GameObject shotTrailPrefab;

    [Header("Deubg")]
    public Mesh SphereColliderDisplayMesh;
    public Material SphereColliderDisplayMaterial;

    public static GlobalData instance;

    public GameObject GetTrail(TrailID trailID)
    {
        switch (trailID)
        {
            case TrailID.Missile:
                return missileTrailPrefab;
            case TrailID.Shot:
                return shotTrailPrefab;
            default:
                throw new System.Exception($"Trail Id not set or return not specified. value = {trailID}");
        }
    }

    private void Awake()
    {
        instance = this;
    }
}