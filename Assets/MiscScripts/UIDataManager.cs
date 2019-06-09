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

public class UIDataManager : MonoBehaviour
{
    public static UIDataManager instance;

    public GameObject steerPos;

    void Awake()
    {
        instance = this;
    }
}
