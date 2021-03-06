﻿using UnityEngine;
using UnityEngine.Assertions;
using DG.Tweening;
using HK.Framework;
using HK.AutoAnt.Extensions;
using System;
using UniRx;
using System.Linq;
using UnityEngine.Events;

namespace HK.AutoAnt
{
    /// <summary>
    /// <see cref="DOTweenAnimation"/>をアタッチするクラス
    /// </summary>
    public sealed class TweenAnimationAttacher : MonoBehaviour
    {
        private static readonly ObjectPoolBundle<TweenAnimationAttacher> pools = new ObjectPoolBundle<TweenAnimationAttacher>();

        private ObjectPool<TweenAnimationAttacher> pool = null;

        public DOTweenAnimation[] Animations { get; private set; }

        public TweenAnimationAttacher Rent(GameObject target)
        {
            var pool = pools.Get(this);
            var clone = pool.Rent();
            clone.pool = pool;
            if (clone.Animations == null)
            {
                clone.Animations = clone.GetComponents<DOTweenAnimation>();
            }

            foreach (var a in clone.Animations)
            {
                a.autoPlay = false;
                a.autoKill = false;
                a.targetIsSelf = false;
                a.targetGO = target;
                a.CreateTween();
            }

            return clone;
        }

        /// <summary>
        /// アニメーションが完了したらPoolされるよう仕込む
        /// </summary>
        public TweenAnimationAttacher ReturnToPoolOnComplete()
        {
            this.OnCompleteAsObservable()
                .SubscribeWithState(this, (_, _this) =>
                {
                    _this.pool.Return(_this);
                })
                .AddTo(this);

            return this;
        }

        /// <summary>
        /// アニメーションが完了したタイミングで通知が来るストリームを返す
        /// </summary>
        /// <remarks>
        /// 事前にInspectorでOnCompleteを有効化しないと機能しないので注意
        /// </remarks>
        public IObservable<Unit> OnCompleteAsObservable()
        {
            return Observable.Zip(this.Animations.Select(a => a.onComplete.AsObservable().Take(1)))
                .AsUnitObservable();
        }
    }
}
