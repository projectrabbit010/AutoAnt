﻿using System;
using HK.AutoAnt.CellControllers.Gimmicks;
using HK.AutoAnt.Systems;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using HK.AutoAnt.Extensions;
using HK.AutoAnt.EffectSystems;
using HK.Framework.EventSystems;
using HK.AutoAnt.Events;
using HK.Framework.Text;
using HK.AutoAnt.Database;
using HK.AutoAnt.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HK.AutoAnt.CellControllers.Events
{
    /// <summary>
    /// <see cref="Cell"/>のイベントを持つ抽象クラス
    /// </summary>
    public abstract class CellEvent : ScriptableObject, ICellEvent
    {
        [SerializeField]
        protected StringAsset.Finder eventName = null;
        public string EventName
        {
            get
            {
                if(this.cachedRecord == null || this.cachedRecord.EventData == null)
                {
                    return this.eventName.Get;
                }
                else
                {
                    return this.cachedRecord.EventData.eventName.Get;
                }
            }
        }

        [SerializeField]
        protected Constants.CellEventCategory category;
        public Constants.CellEventCategory Category => this.category;

        [SerializeField]
        protected CellEventGenerateCondition condition = null;

        [SerializeField]
        protected int size = 1;
        public int Size => this.size;

        [SerializeField]
        protected AudioClip constructionSE = null;

        [SerializeField]
        protected AudioClip destructionSE = null;

        [SerializeField]
        protected PoolableEffect constructionEffect = null;

        [SerializeField]
        protected PoolableEffect destructionEffect = null;

        [SerializeField]
        protected GameObject gimmickPrefab = null;
        public GameObject GimmickPrefab => this.gimmickPrefab;

        [SerializeField]
        protected TweenAnimationAttacher visibleAnimation = null;

        public int Id => int.Parse(this.name);

        public Vector2Int Origin { get; protected set; }

        private readonly IMessageBroker broker = new MessageBroker();
        public IMessageBroker Broker => this.broker;

        /// <summary>
        /// 実体が持つイベント
        /// </summary>
        protected readonly CompositeDisposable instanceEvents = new CompositeDisposable();

        public GameObject Gimmick { get; protected set; }

        private MasterDataCellEvent.Record cachedRecord;

        public virtual GameObject CreateGimmickController(Vector2Int origin)
        {
            var gimmick = Instantiate(this.gimmickPrefab);
            var constants = GameSystem.Instance.Constants.Cell;
            var position = new Vector3(origin.x * (constants.Scale.x + constants.Interval), 0.0f, origin.y * (constants.Scale.z + constants.Interval));
            var fixedSize = this.size - 1;
            position += new Vector3((constants.Scale.x / 2.0f) * fixedSize, 0.0f, (constants.Scale.z / 2.0f) * fixedSize);
            position += new Vector3(constants.Interval * fixedSize, constants.Scale.y, constants.Interval * fixedSize);
            gimmick.transform.position = position;
            gimmick.transform.localScale = constants.EffectScale * this.size + (Vector3.one * (constants.Interval * fixedSize));

            return gimmick;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            this.size = this.size <= 1 ? 1 : this.size;
        }
#endif

        public virtual void Initialize(Vector2Int position, bool isInitializingGame)
        {
            this.Origin = position;

            // 自分自身のマスターデータを取得してデータを参照している
            // セーブデータから読み込む時にアセットの参照はセーブしていないのでちょっとややこしい作りになっている
            this.cachedRecord = GameSystem.Instance.MasterData.CellEvent.Records.Get(this.Id);
            this.Gimmick = this.cachedRecord.EventData.CreateGimmickController(this.Origin);

            foreach(var g in this.Gimmick.GetComponentsInChildren<ICellEventGimmick>())
            {
                g.Attach(this);
            }

            if(!isInitializingGame)
            {
                Assert.IsNotNull(this.cachedRecord.EventData.constructionSE, $"Id = {this.Id}の建設時のSE再生に失敗しました");
                GameSystem.Instance.SEController.Play(this.cachedRecord.EventData.constructionSE);

                Assert.IsNotNull(this.cachedRecord.EventData.constructionEffect, $"Id = {this.Id}の建設時のエフェクト生成に失敗しました");
                var effect = this.cachedRecord.EventData.constructionEffect.Rent();
                effect.transform.position = this.Gimmick.transform.position;
                effect.transform.localScale = Vector3.one * this.cachedRecord.EventData.size;

                this.visibleAnimation
                    .Rent(this.Gimmick)
                    .ReturnToPoolOnComplete()
                    .Animations.ForEach(a => a.DOPlay());
            }
        }

        public virtual void Remove()
        {
            var gameSystem = GameSystem.Instance;
            this.instanceEvents.Clear();

            // 自分自身のマスターデータを取得してデータを参照している
            // セーブデータから読み込む時にアセットの参照はセーブしていないのでちょっとややこしい作りになっている
            var record = gameSystem.MasterData.CellEvent.Records.Get(this.Id);

            foreach (var g in this.Gimmick.GetComponentsInChildren<ICellEventGimmick>())
            {
                g.Detach(this);
            }

            Assert.IsNotNull(record.EventData.destructionSE, $"Id = {this.Id}の破壊時のSE再生に失敗しました");
            gameSystem.SEController.Play(record.EventData.destructionSE);

            Assert.IsNotNull(record.EventData.destructionEffect, $"Id = {this.Id}の破壊時のエフェクト生成に失敗しました");
            var effect = record.EventData.destructionEffect.Rent();
            effect.transform.position = this.Gimmick.transform.position;
            effect.transform.localScale = Vector3.one * record.EventData.size;

            Destroy(this.Gimmick.gameObject);
        }

        public Constants.CellEventGenerateEvalute CanGenerate(Cell origin, int cellEventRecordId, CellMapper cellMapper)
        {
            Assert.IsNotNull(this.condition);

            var cellPositions = Vector2IntUtility.GetRange(origin.Position, Vector2Int.one * this.size, p => cellMapper.Cell.Map.ContainsKey(p));

            // 配置したいところにセルがない場合は生成できない
            if(cellPositions.Count != this.size * this.size)
            {
                return Constants.CellEventGenerateEvalute.NotCell;
            }

            // 配置したいセルにイベントがあった場合は生成できない
            var cells = cellMapper.GetCells(cellPositions);
            if(Array.FindIndex(cells, c => cellMapper.HasEvent(c)) != -1)
            {
                return Constants.CellEventGenerateEvalute.AlreadyExistsCellEvent;
            }

            // コストが満たしていない場合は生成できない
            var gameSystem = GameSystem.Instance;
            var masterData = gameSystem.MasterData.LevelUpCost.Records.Get(cellEventRecordId, 0);
            Assert.IsNotNull(masterData, $"CellEventRecordId = {cellEventRecordId}の{typeof(MasterDataLevelUpCost.Record)}がありませんでした");
            if(!masterData.Cost.IsEnough(gameSystem.User, gameSystem.MasterData.Item))
            {
                return Constants.CellEventGenerateEvalute.NotEnoughCost;
            }

            // セルイベントごとの条件を満たしていない場合は生成できない
            if(!this.condition.Evalute(cells))
            {
                return Constants.CellEventGenerateEvalute.NotEnoughCondition;
            }

            return Constants.CellEventGenerateEvalute.Possible;
        }

        public virtual void OnClick(Cell owner)
        {
        }

#if UNITY_EDITOR
        public static CellEvent GetOrCreateAsset(Database.SpreadSheetData.CellEventData data)
        {
            switch(data.Classname)
            {
                case "Housing":
                    return CellEvent.InternalGetOrCreateAsset<Housing>(data);
                case "Facility":
                    return CellEvent.InternalGetOrCreateAsset<Facility>(data);
                case "Road":
                    return CellEvent.InternalGetOrCreateAsset<Road>(data);
                default:
                    Debug.LogError($"CellEventType = {data.Classname}は未対応の値です");
                    return null;
            }
        }

        protected static T InternalGetOrCreateAsset<T>(Database.SpreadSheetData.CellEventData data)
            where T : CellEvent
        {
            var path = $"Assets/HK/AutoAnt/DataSources/CellEvents/{data.Classname}/{data.Id}.asset";
            var result = AssetDatabase.LoadAssetAtPath<T>(path);
            if (result == null)
            {
                result = CreateInstance<T>();
                result.name = data.Id.ToString();
                AssetDatabase.CreateAsset(result, path);
            }

            result.ApplyProperty(data);
            EditorUtility.SetDirty(result);

            return result;
        } 

        protected virtual void ApplyProperty(Database.SpreadSheetData.CellEventData data)
        {
            var stringAsset = AssetDatabase.LoadAssetAtPath<StringAsset>("Assets/HK/AutoAnt/DataSources/StringAsset/CellEvent.asset");
            this.eventName = stringAsset.CreateFinderSafe(data.Name);
            this.category = (Constants.CellEventCategory)Enum.Parse(typeof(Constants.CellEventCategory), data.Category);
            this.condition = AssetDatabase.LoadAssetAtPath<CellEventGenerateCondition>($"Assets/HK/AutoAnt/DataSources/CellEvents/Conditions/{data.Condition}.asset");
            this.size = data.Size;
            this.constructionSE = AssetDatabase.LoadAssetAtPath<AudioClip>($"Assets/HK/AutoAnt/DataSources/SE/{data.Constructionse}.mp3");
            this.destructionSE = AssetDatabase.LoadAssetAtPath<AudioClip>($"Assets/HK/AutoAnt/DataSources/SE/{data.Destructionse}.mp3");
            this.constructionEffect = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/HK/AutoAnt/Prefabs/Effects/{data.Constructioneffect}.prefab").GetComponent<PoolableEffect>();
            this.destructionEffect = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/HK/AutoAnt/Prefabs/Effects/{data.Destructioneffect}.prefab").GetComponent<PoolableEffect>();
            this.gimmickPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/HK/AutoAnt/Prefabs/CellEvent/{data.Gimmickprefab}.prefab");
            this.visibleAnimation = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/HK/AutoAnt/Prefabs/Tween/{data.Visibleanimation}.prefab").GetComponent<TweenAnimationAttacher>();
        }
#endif
    }
}
