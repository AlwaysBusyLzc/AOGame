using EGamePlay.Combat;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TComp = AO.Actor;

namespace AO
{
    public static class ActorSystem
    {
        [ObjectSystem]
        public class AwakeHandler : AwakeSystem<TComp, UnitInfo>
        {
            protected override void Awake(TComp self, UnitInfo unitInfo)
            {
                var combatEntity = CombatContext.Instance.AddChild<CombatEntity>();
                combatEntity.AddComponent<CombatUnitComponent>().Unit = self;
                combatEntity.Position = self.MapUnit().Position = unitInfo.Position;
                self.AddComponent<UnitCombatComponent>().CombatEntity = combatEntity;
            }
        }

        public static Scene GetScene(this Actor avatar)
        {
            return avatar.GetParent<Scene>();
        }
    }
}