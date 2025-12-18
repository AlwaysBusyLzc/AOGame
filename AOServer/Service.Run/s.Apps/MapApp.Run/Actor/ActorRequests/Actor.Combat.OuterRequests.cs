namespace AO
{
    using AO;
    using ET;
    using EGamePlay;
    using EGamePlay.Combat;

    public static partial class ActorOuterRequests
    {
        public static async partial ETTask C2M_SpellRequest(Actor avatar, C2M_SpellRequest request, M2C_SpellResponse response)
        {
            var combatEntity = avatar.GetComponent<UnitCombatComponent>().CombatEntity;
            if (combatEntity.GetComponent<AbilityComponent>().IdSkills.TryGetValue(request.SkillId, out var skillAbility))
            {
                // if (skillAbility.SkillConfig.Id == 1002)
                if (skillAbility.ExecutionObject.TargetInputType == ExecutionTargetInputType.Point)
                {
                    combatEntity.GetComponent<SpellComponent>().SpellWithPoint(skillAbility, request.CastPoint);
                }
                else if  (skillAbility.ExecutionObject.TargetInputType == ExecutionTargetInputType.Target)
                {
                    // combatEntity.GetComponent<SpellComponent>().SpellWithTarget(skillAbility, skillAbility.OwnerEntity);

                    var curScene = avatar.GetParent<Scene>();
                    Actor targetActor = curScene.GetComponent<SceneUnitComponent>().Get(request.CastTargetId) as Actor;
                    CombatEntity targetCombat = targetActor.GetComponent<UnitCombatComponent>().CombatEntity;
                    combatEntity.GetComponent<SpellComponent>().SpellWithTarget(skillAbility, targetCombat);
                }
            }
            await ETTask.CompletedTask;
        }
    }
}