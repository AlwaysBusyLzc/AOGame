using EGamePlay.Combat;
using System;
using GameUtils;
using ET;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 行动能力接口
    /// </summary>
    public interface IActionAbility
    {
        public CombatEntity OwnerEntity { get; set; }
        public bool Enable { get; set; }
    }
}