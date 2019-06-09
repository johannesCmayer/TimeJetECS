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

public class GaterInputSystem : ComponentSystem
{
    float pitchSpeed = 1600 * 6;
    float rollSpeed = 80 * 4;
    float moveSpeed = 100;

    float steerPosSensitivity = 9;
    float2 steerPos = float2.zero;

    protected override void OnCreate()
    {
        Cursor.lockState = CursorLockMode.Locked;        
    }

    protected override void OnUpdate()
    {
        steerPos += new float2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime * steerPosSensitivity;

        var cam = Camera.main;
        var tar = cam.transform.position +
            cam.transform.up * steerPos.y + cam.transform.right * steerPos.x + cam.transform.forward * 4f;
        Debug.DrawLine(cam.transform.position + cam.transform.forward * 4f, tar);

        Entities.WithAll<PlayerTag>().ForEach((ref SteeringInput steerInput, ref MoveSpeed moveSpeed) => {
            if (Input.GetMouseButton(0))
            {
                steerInput = new SteeringInput
                {
                    pitch = 0,
                    roll = 0,
                    yaw = 0
                };
                return;
            }

            float roll = 0;
            float pitch = 0;

            float threshhold = 0.1f;

            bool mouseNotMoved = Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0;

            void UpdateSteerPosX(float dirX, float dirY)
            {
                if (!mouseNotMoved)
                    return;
                dirX *= 0.01f;
                dirY *= 0.01f;
                float steerLen = math.abs(math.length(steerPos));
                var normSteerPos = math.normalize(steerPos);
                float cos = math.acos(normSteerPos.x);
                float sin = math.asin(normSteerPos.y);
                var coef = Time.deltaTime * 1.3f * rollSpeed;
                var newSteerDir = math.normalize(new float2(math.cos(cos + dirX * coef), math.sin(sin + dirY * coef)));

                steerPos = newSteerDir * steerLen;
            }

            if (steerPos.y > threshhold && steerPos.x < -threshhold)
            {
                roll = 1 * rollSpeed;
                UpdateSteerPosX(-1, 1);
            }
            else if (steerPos.y > threshhold && steerPos.x > threshhold)
            {
                roll = -1 * rollSpeed;
                UpdateSteerPosX(1, 1);
            }
            else if (steerPos.y < -threshhold && steerPos.x < -threshhold)
            {
                roll = -1 * rollSpeed;
                UpdateSteerPosX(-1, -1);
            }
            else if (steerPos.y < -threshhold && steerPos.x > threshhold)
            {
                roll = 1 * rollSpeed;
                UpdateSteerPosX(1, -1);
            }
            else if (steerPos.x < threshhold * 1.1f && steerPos.x > -threshhold * 1.1f &&
                (steerPos.y > threshhold * 1.1f || steerPos.y < -threshhold * 1.1f))
            {
                pitch = pitchSpeed * math.sign(steerPos.y);

                if (!mouseNotMoved)
                    return;

                float val = 0;
                if (steerPos.y > 0)                
                {
                    val = steerPos.y - (Time.deltaTime * pitchSpeed * 0.001f);
                }
                else if (steerPos.y < 0)
                {
                    Debug.Log("B");
                    val = steerPos.y + (Time.deltaTime * pitchSpeed * 0.001f);
                }
                Debug.Log("F");

                steerPos = new float2(steerPos.x, val);
            }

            if (steerPos.x == float.NaN || steerPos.y == float.NaN)
                steerPos = float2.zero;

            steerInput = new SteeringInput
            {
                pitch = -pitch * Time.deltaTime,
                roll = roll * 50 * Time.deltaTime,
                yaw = 0
            };

            var speed = 0;
            if (Input.GetKey(KeyCode.Space))
            {
                speed = 6;
            }
            moveSpeed = new MoveSpeed
            {
                Value = new float3(0, 0, speed)
            };
        });
    }
}
