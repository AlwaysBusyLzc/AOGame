using Sirenix.OdinInspector;
using System;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 观察触发接口
    /// </summary>
    public interface ICombatObserver
    {
        void OnTrigger(Entity source);
    }
}