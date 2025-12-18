using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    //public class ApplyEffectEvent { public AbilityEffect AbilityEffect; }
    //public enum EffectSourceType { Ability, Execution }

    /// <summary>
    /// 能力效果触发系统接口
    /// </summary>
    public interface IEffectTriggerSystem
    {
        void OnTriggerApplyEffect(Entity effectAssign);
    }
    
    /// <summary>
    /// 效果触发上下文
    /// </summary>
    public struct TriggerContext
    {
        public TriggerContext(ICombatObserver observer, Entity source)
        {
            Observer = observer;
            Source = source;
            AbilityItem = null;
            Target = null;
        }

        public ICombatObserver Observer;
        public Entity Source;
        public Entity Target;
        public AbilityItem AbilityItem;
    }

    /// <summary>
    /// 能力效果，如伤害、治疗、施加状态等这些和技能数值、状态相关的效果
    /// </summary>
    public partial class AbilityEffect : Entity
    {
        public bool Enable { get; set; }
        public Entity OwnerAbility => Parent;       // 能力实体
        public CombatEntity OwnerEntity => (OwnerAbility as IAbilityEntity).OwnerEntity;  // 能力实体的所有者
        public Entity ParentEntity => (OwnerAbility as IAbilityEntity).ParentEntity;      // 能力实体的挂载实体
        public Effect EffectConfig { get; set; }
        public string ConditionParamValue { get; set; }            // 条件参数
        //public EffectSourceType EffectSourceType { get; set; }
        public TriggerObserver TriggerObserver { get; set; }


        public override void Awake(object initData)
        {
            this.EffectConfig = initData as Effect;
            Name = EffectConfig.GetType().Name;
            //Log.Debug($"AbilityEffect Awake {OwnerAbility.Name} {EffectConfig}");

            /// 行动禁制
            if (this.EffectConfig is ActionControlEffect) AddComponent<EffectActionControlComponent>();
            /// 属性修饰
            if (this.EffectConfig is AttributeModifyEffect) AddComponent<EffectAttributeModifyComponent>();

            /// 伤害效果
            if (this.EffectConfig is DamageEffect) AddComponent<EffectDamageComponent>();
            /// 治疗效果
            if (this.EffectConfig is CureEffect) AddComponent<EffectCureComponent>();
            /// 光盾防御效果
            if (this.EffectConfig is ShieldDefenseEffect) AddComponent<EffectShieldDefenseComponent>();
            /// 施加状态效果
            if (this.EffectConfig is AddStatusEffect) AddComponent<EffectAddStatusComponent>();
            /// 自定义效果
            if (this.EffectConfig is CustomEffect) AddComponent<EffectCustomComponent>();
            /// 效果修饰
            var decorators = this.EffectConfig.Decorators;
            if (decorators != null && decorators.Count > 0) AddComponent<EffectDecoratosComponent>();
        }

        public override void OnDestroy()
        {
            DisableEffect();
        }

        public void EnableEffect()
        {
            //Log.Debug("EnableEffect");
            Enable = true;
            if (EffectConfig.EffectTriggerType != EffectTriggerType.None)
            {
                // 启用所有组件， 例如伤害触发组件， 治疗触发组件...
                foreach (var item in Components.Values)
                {
                    item.Enable = true;
                }
            }

            //var triggable = !(this.EffectConfig is ActionControlEffect) && !(this.EffectConfig is AttributeModifyEffect);
            //if (triggable)
            {
                if (EffectConfig.EffectTriggerType == EffectTriggerType.None || EffectConfig.EffectTriggerType == EffectTriggerType.Instant)
                {
                    // 增加主动触发观察者
                    TriggerObserver = AddChild<TriggerObserver>();
                }
                else if (EffectConfig.EffectTriggerType == EffectTriggerType.Action)
                {
                    //Log.Debug("EnableEffect ActionPointObserver");
                    // 增加行动点触发观察者
                    AddChild<ActionPointObserver>();
                }
                else if (EffectConfig.EffectTriggerType == EffectTriggerType.Condition)
                {
                    var conditionType = EffectConfig.ConditionType;
                    var paramObj = ConditionParamValue;
                    if (conditionType == TimeStateEventType.WhenInTimeNoDamage && float.TryParse((string)paramObj, out var time))
                    {
                        var condition = AddChild<TimeState_WhenInTimeNoDamageObserver>(time);
                        condition.StartListen(null);
                    }
                    if (conditionType == TimeStateEventType.WhenIntervalTime && float.TryParse((string)paramObj, out var intervalTime))
                    {
                        var condition = AddChild<TimeState_TimeIntervalObserver>(intervalTime);
                        condition.StartListen(null);
                    }
                }

                if (EffectConfig.StateCheckList != null && EffectConfig.StateCheckList.Count > 0)
                {
                    foreach (var item in Children)
                    {
                        // 观察触发实体增加条件检查组件
                        if (item is ICombatObserver)
                        {
                            item.AddComponent<EffectStateCheckComponent>();
                        }
                    }
                }

                /// 立即触发
                if (EffectConfig.EffectTriggerType == EffectTriggerType.Instant)
                {
                    //TriggerEffectToParent();
                    TriggerObserver.OnTrigger(ParentEntity);
                }
            }

            //TriggerEventBind?.EnableTriggerBind();
        }

        public void DisableEffect()
        {
            Enable = false;
            foreach (var item in Components.Values)
            {
                item.Enable = false;
            }
        }

        public void OnObserverTrigger(TriggerContext context)
        {
            //Log.Debug("AbilityEffect OnObserverTrigger");
            var observer = context.Observer;
            var source = context.Source;
            Entity target = context.Target;
            if (target == null)
            {
                target = source;
            }

            var stateCheckResult = true;
            if (!(observer is ActionPointObserver) && !(observer is TriggerObserver))
            {
                // 如果触发观察者不是行动点触发 也不是主动触发， 则目标设置为挂载能力实体的战斗实体
                target = ParentEntity;
            }

            /// 这里是状态判断，状态判断是判断目标的状态是否满足条件，满足才能触发效果
            if ((observer as Entity).TryGet(out EffectStateCheckComponent component))
            {
                stateCheckResult = component.CheckTargetState(target);
            }

            /// 条件满足则触发效果
            if (stateCheckResult)
            {
                if (OwnerEntity.EffectAssignAbility.TryMakeAction(out var effectAssign))
                {
                    effectAssign.AssignTarget = target;
                    effectAssign.SourceAbility = OwnerAbility;      // 能力实体
                    effectAssign.AbilityEffect = this;
                    effectAssign.TriggerContext = context;
                    effectAssign.AssignEffect();
                }
            }
        }
    }
}
