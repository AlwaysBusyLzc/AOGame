using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TComp = AO.UnitAnimationComponent;

namespace AO
{
    public static class UnitAnimationComponentSystem
    {
        [ObjectSystem]
        public class UnitAnimationComponentAwakeSystem : AwakeSystem<TComp>
        {
            protected override void Awake(TComp self)
            {
                self.AnimationType = AnimationType.Idle;
                var gameObject = self.Parent.GetComponent<UnitViewComponent>().UnitObj;
                self.animationComponent = gameObject.GetComponent<AnimationComponent>();
                
                var combatEntity = self.Parent.GetComponent<UnitCombatComponent>().CombatEntity;
                combatEntity.Subscribe<AnimationClip>(self.OnPlayAnimation);
            }
        }
        
        
        private static void OnPlayAnimation(this TComp self, AnimationClip animationClip)
        {
            ET.Log.Debug($"OnPlayAnimation {animationClip.name}");
            // self.animationComponent.PlayFade(animationClip);
            self.animationComponent.Play(animationClip);
        }
            
        
        public static async void Play(this TComp self, AnimationType animationType)
        {
            // var animator = self.Parent.GetComponent<UnitViewComponent>().UnitObj.GetComponentInChildren<Animator>();
            // if (animator == null)
            // {
            //     return;
            // }
            
            if (animationType == self.AnimationType)
            {
                return;
            }
            //Log.Debug($"Play {animationType}");
            if (animationType == AnimationType.Run)
            {
                //animator.SetFloat("Walk", 3);
                // animator.SetTrigger("Run");
                self.animationComponent.Play(self.animationComponent.RunAnimation);
            }
            if (animationType == AnimationType.Idle)
            {
                //animator.SetFloat("Walk", 0);
                // animator.SetTrigger("Idle");
                self.animationComponent.Play(self.animationComponent.IdleAnimation);
            }
            if (animationType == AnimationType.Attack)
            {
                // animator.SetTrigger("Attack");
                // 等待攻击动画播放结束 播放idel
                self.animationComponent.PlayWithCallback(self.animationComponent.AttackAnimation, 
                ()=>{
                    self.Play(AnimationType.Idle);
                },
                0);
            }
            self.AnimationType = animationType;
        
            // if (animationType == AnimationType.Attack)
            // {
            //     await TimeHelper.WaitAsync(1000);
            //     self.Play(AnimationType.Idle);
            // }
        }
    }
}