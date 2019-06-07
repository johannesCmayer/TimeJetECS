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

public struct PlayerTag : IComponentData { }
public struct FriendlyAITag : IComponentData { }
public struct EnemyAITag : IComponentData { }
public struct MissileTag : IComponentData { }
public struct HasTrailTag : IComponentData { }
public struct ExplosionTag : IComponentData { }

public struct TurnTowardsTarget : IComponentData { }

public struct TargetSelection : IComponentData
{
    public Entity target;
}

public struct MoveSpeed : IComponentData
{
    public float Value;
}

public struct RotationSpeed : IComponentData
{
    public float Value;
}

public struct SteerInput : IComponentData
{
    public float pitch;
    public float tilt;
    public float yaw;
}

public struct FireWeapon : IComponentData
{
    public float cooldown;
}

public struct SphereCollider : IComponentData
{
    public float size;
}

public struct DestroyAfterTime : IComponentData
{
    public float timeToDestruction;
}
