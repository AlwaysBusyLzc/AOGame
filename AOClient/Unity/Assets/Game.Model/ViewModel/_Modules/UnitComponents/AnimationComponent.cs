using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationComponent : MonoBehaviour
{
    public Animancer.AnimancerComponent AnimancerComponent;
    public AnimationClip IdleAnimation;
    public AnimationClip RunAnimation;
    public AnimationClip JumpAnimation;
    public AnimationClip AttackAnimation;
    public AnimationClip SkillAnimation;
    public AnimationClip StunAnimation; 
    public AnimationClip DamageAnimation;
    public AnimationClip DeadAnimation;
    public AnimationClip[] AnimationClips;
    public float Speed { get; set; } = 1f;
    
    
    private void Start()
    {
        AnimancerComponent.Animator.fireEvents = false;
        AnimancerComponent.States.CreateIfNew(IdleAnimation);
        AnimancerComponent.States.CreateIfNew(RunAnimation);
        AnimancerComponent.States.CreateIfNew(JumpAnimation);
        AnimancerComponent.States.CreateIfNew(AttackAnimation);
        AnimancerComponent.States.CreateIfNew(SkillAnimation);
        AnimancerComponent.States.CreateIfNew(StunAnimation);
        foreach (var item in AnimationClips)
        {
            AnimancerComponent.States.CreateIfNew(item);
        }
    }

    public void Play(AnimationClip clip)
    {
        var state = AnimancerComponent.States.GetOrCreate(clip);
        state.Speed = Speed;
        // 重置动画时间，确保每次都能重新播放
        state.Time = 0f;
        AnimancerComponent.Play(state);
    }
    
    public void PlayFade(AnimationClip clip)
    {
        var state = AnimancerComponent.States.GetOrCreate(clip);
        state.Speed = Speed;
        // 重置动画时间，确保每次都能重新播放
        state.Time = 0f;
        AnimancerComponent.Play(state, 0.25f);
    }

    public void TryPlayFade(AnimationClip clip)
    {
        var state = AnimancerComponent.States.GetOrCreate(clip);
        state.Speed = Speed;
        if (AnimancerComponent.IsPlaying(clip))
        {
            return;
        }
        // 重置动画时间，确保每次都能重新播放
        state.Time = 0f;
        AnimancerComponent.Play(state, 0.25f);
    }

    
    /// <summary>
    /// 播放动画，并在动画结束时执行回调
    /// </summary>
    /// <param name="clip">要播放的动画</param>
    /// <param name="onEnd">动画结束时的回调</param>
    /// <param name="fadeDuration">淡入淡出时间（0表示立即切换）</param>
    public void PlayWithCallback(AnimationClip clip, System.Action onEnd, float fadeDuration = 0.25f)
    {
        var state = AnimancerComponent.States.GetOrCreate(clip);
        state.Speed = Speed;
        
        // 清除之前的事件
        state.Events.OnEnd = null;
        
        // 绑定动画结束事件
        state.Events.OnEnd = onEnd;
        
        // 重置动画时间，确保每次都能重新播放
        state.Time = 0f;
        
        if (fadeDuration > 0)
        {
            AnimancerComponent.Play(state, fadeDuration);
        }
        else
        {
            AnimancerComponent.Play(state);
        }
    }
}
