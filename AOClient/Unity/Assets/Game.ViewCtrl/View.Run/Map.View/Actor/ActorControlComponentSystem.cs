using ET;
using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AO
{
    public static class ActorControlComponentSystem
    {
        [ObjectSystem]
        public class AwakeHandler : AwakeSystem<ActorControlComponent>
        {
            protected override void Awake(ActorControlComponent self)
            {
                
            }
        }

        [ObjectSystem]
        public class UpdateHandler : UpdateSystem<ActorControlComponent>
        {
            protected override void Update(ActorControlComponent self)
            {
                // if (GRoot.inst.touchTarget != null)
                // {
                //     return;
                // }

                if (Input.GetMouseButtonDown(((int)MouseButton.RightMouse)))
                {
                    //Log.Debug($"GetMouseButtonDown {MouseButton.RightMouse}");
                    if (RaycastUtils.CastMapPoint(out var hitPoint))
                    {
                        //Log.Debug($"Raycast {hitPoint}");
                        AvatarCall.C2M_PathfindingResult(new C2M_PathfindingResult() { Position = hitPoint });
                        //Avatar.Main.MoveToAsync(hitPoint).Coroutine();
                        //GameObject.Find("Cube").transform.position = hitPoint;
                    }
                }

                if (Input.GetMouseButtonUp(((int)MouseButton.LeftMouse)))
                {
                    //Log.Debug($"GetMouseButtonUp {MouseButton.LeftMouse}");
                    if (RaycastUtils.CastMapPoint(out var hitPoint))
                    {
                        // AvatarCall.C2M_SpellRequest(new C2M_SpellRequest() { CastPoint = hitPoint, SkillId = 1002 }).Coroutine();
                    }               
                }

                // // 如果按下组合按键 alt + s
                // if (Input.GetKeyDown(KeyCode.S))
                // {
                //     Log.Debug("切换到技能编辑场景");
                //     // 切换到技能编辑场景
                //     //AOGame.ClientApp.GetComponent<MapModeComponent>().ChangeMapScene("ExecutionLinkScene");
                //     // 向服务端请求切换到技能编辑场景
                // }
                
                //var h = Input.GetAxis("Horizontal");
                //var v = Input.GetAxis("Vertical");
            }
        }
    }
}