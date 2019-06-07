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

public class GlobalData : MonoBehaviour
{
    [Header("Units")]
    public Mesh friendlyPlaneMesh;
    public Material friendlyPlaneMaterial;

    public Mesh EnemyPlaneMesh;
    public Material enemyPlaneMaterial;

    public Mesh missileMesh;    
    public Material missileMaterial;

    [Header("Effects")]
    public Mesh explosionMesh;
    public Material explosionMaterial;

    public Mesh missileTrailMesh;
    public Material missileTrailMaterial;

    [Header("Effects/Prebafs")]
    public GameObject TrailRenderer;

    public static GlobalData instance;

    private void Awake()
    {
        instance = this;
    }
}
