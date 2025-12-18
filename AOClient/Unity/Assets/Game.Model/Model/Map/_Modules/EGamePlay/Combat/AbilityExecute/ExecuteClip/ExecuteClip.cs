using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class ExecuteEffectEvent
    {
        public ExecuteClip ExecutionClip;
    }

    /// <summary>
    /// 执行片段，针对于技能执行体的效果，如播放动作、生成碰撞体、位移等这些和技能表现相关的效果
    /// </summary>
    public partial class ExecuteClip : Entity
    {
        public ExecuteClipData ExecutionEffectConfig { get; set; }
        public SkillExecution ParentExecution => GetParent<SkillExecution>();


        public override void Awake(object initData)
        {
            ExecutionEffectConfig = initData as ExecuteClipData;
            Name = ExecutionEffectConfig.GetType().Name;

            var clipType = ExecutionEffectConfig.ExecuteClipType;
            if (clipType == ExecuteClipType.ActionEvent)
            {
                var actionEventData = ExecutionEffectConfig.ActionEventData;
                /// 应用效果给目标效果
                if (actionEventData.ActionEventType == FireEventType.AssignEffect)
                {
                    AddComponent<ExecuteAssignEffectToTargetComponent>().EffectApplyType = actionEventData.EffectApply;
                }
                /// 触发新的执行体效果
                if (actionEventData.ActionEventType == FireEventType.TriggerNewExecution)
                {
                    AddComponent<ExecuteTriggerNewExecutionComponent>().ActionEventData = actionEventData;
                }
            }
            /// 生成碰撞体效果，碰撞体再触发应用能力效果
            else if (clipType == ExecuteClipType.CollisionExecute)
            {
                var spawnItemEffect = ExecutionEffectConfig.CollisionExecuteData;
                AddComponent<ExecuteCollisionItemComponent>().CollisionExecuteData = spawnItemEffect;
            }
#if UNITY
            /// 播放动作效果
            if (clipType == ExecuteClipType.Animation)
            {
                var animationEffect = ExecutionEffectConfig.AnimationData;
                AddComponent<ExecuteAnimationComponent>().AnimationClip = animationEffect.AnimationClip;
            }
            /// 播放特效效果
            else if (clipType == ExecuteClipType.ParticleEffect)
            {
                var animationEffect = ExecutionEffectConfig.ParticleEffectData;
                AddComponent<ExecuteParticleEffectComponent>().ParticleEffectPrefab = animationEffect.ParticleEffect;
            }
            /// 播放音效
            else if (clipType == ExecuteClipType.Audio)
            {
                var audioEffect = ExecutionEffectConfig.AudioData;
                // AddComponent<ExecuteAudioComponent>().AudioClip = audioEffect.AudioClip;
            }
#endif

            /// 时间到触发执行效果
            if (clipType == ExecuteClipType.ActionEvent)
            {
                AddComponent<ExecuteTimeTriggerComponent>().StartTime = (float)ExecutionEffectConfig.StartTime;
            }
            else if (ExecutionEffectConfig.Duration > 0)
            {
                AddComponent<ExecuteTimeTriggerComponent>().StartTime = (float)ExecutionEffectConfig.StartTime;
                GetComponent<ExecuteTimeTriggerComponent>().EndTime = (float)ExecutionEffectConfig.EndTime;
            }
        }

        public void BeginExecute()
        {
            // 如果没找到时间触发组件，直接触发
            if (!TryGet(out ExecuteTimeTriggerComponent timeTriggerComponent))
            {
                TriggerEffect();
            }
            foreach (var item in Components.Values)
            {
                item.Enable = true;
            }
        }

        public void TriggerEffect()
        {
            //Log.Debug($"ExecutionEffect ApplyEffect");
            this.Publish(new ExecuteEffectEvent() { ExecutionClip = this });
            this.FireEvent(nameof(TriggerEffect));
        }

        public void EndEffect()
        {
            //Log.Debug($"ExecutionEffect ApplyEffect");
            //this.Publish(new ExecuteEffectEvent() { ExecutionEffect = this });
            this.FireEvent(nameof(EndEffect));
        }
    }
}
