﻿using UnityEngine;
using UnityEngine.Assertions;
using UniRx;
using System;

namespace HK.AutoAnt.AudioSystems
{
    /// <summary>
    /// BGMを制御するクラス
    /// </summary>
    public sealed class BGMController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource = null;
        public AudioSource AudioSource => this.audioSource;

        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();
        
        public void Play(ClipBundle clipBundle)
        {
            Assert.IsNotNull(clipBundle);
            Assert.IsNotNull(this.audioSource);
            Assert.IsNotNull(this.compositeDisposable);

            this.compositeDisposable.Clear();
            this.audioSource.clip = clipBundle.Intro;
            this.audioSource.Play();
            this.audioSource.loop = false;

            this.audioSource.ObserveEveryValueChanged(x => x.time)
                .Where(x => (x - clipBundle.OffsetIntroChangeSeconds) >= this.audioSource.clip.length)
                .Take(1)
                .SubscribeWithState2(this, clipBundle.Loop, (_, _this, _loop) =>
                {
                    _this.audioSource.clip = _loop;
                    _this.audioSource.loop = true;
                    _this.audioSource.Play();
                })
                .AddTo(this)
                .AddTo(this.compositeDisposable);
        }

        [Serializable]
        public class ClipBundle
        {
            [SerializeField]
            private AudioClip intro = null;
            public AudioClip Intro => this.intro;

            [SerializeField]
            private AudioClip loop = null;
            public AudioClip Loop => this.loop;

            /// <summary>
            /// イントロからループに切り替えるタイミングの補正値（秒）
            /// </summary>
            [SerializeField]
            private float offsetIntroChangeSeconds = 0.0f;
            public float OffsetIntroChangeSeconds => this.offsetIntroChangeSeconds;
        }
    }
}
