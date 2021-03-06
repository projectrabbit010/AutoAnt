﻿using HK.AutoAnt.CameraControllers;
using HK.AutoAnt.CellControllers;
using HK.AutoAnt.Events;
using HK.AutoAnt.GameControllers;
using HK.AutoAnt.Systems;
using HK.Framework.EventSystems;
using HK.AutoAnt.Constants;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;


namespace HK.AutoAnt.InputControllers
{
    /// <summary>
    /// ゲームの入力を制御する
    /// </summary>
    public sealed class GameInputController : MonoBehaviour
    {
        [SerializeField]
        private CellManager cellManager = null;

        [SerializeField]
        private GameCameraController gameCameraController = null;

        private InputActions inputActions;

        private ClickToClickableObjectActions cachedClickToClickableObjectActions;

        private GenerateCellEventActions cachedGenerateCellEventActions;

        private EraseCellEventActions cachedEraseCellEventActions;

        private DevelopCellActions cachedDevelopCellActions;

        void Awake()
        {
            Broker.Global.Receive<RequestBuildingMode>()
                .SubscribeWithState(this, (_, _this) =>
                {
                    _this.inputActions = _this.cachedGenerateCellEventActions;
                })
                .AddTo(this);

            Broker.Global.Receive<RequestClickMode>()
                .SubscribeWithState(this, (_, _this) =>
                {
                    _this.inputActions = _this.cachedClickToClickableObjectActions;
                })
                .AddTo(this);

            Broker.Global.Receive<RequestDevelopMode>()
                .SubscribeWithState(this, (_, _this) =>
                {
                    _this.inputActions = _this.cachedDevelopCellActions;
                })
                .AddTo(this);

            var inputModule = InputControllers.Input.Current;
            this.inputActions = new ClickToClickableObjectActions(this.gameCameraController);

            inputModule.ClickDownAsObservable()
                .Where(x => x.Data.ButtonId == inputModule.MainPointerId)
                .Where(_ => this.inputActions.ClickDownAction != null)
                .SubscribeWithState(this, (x, _this) =>
                {
                    _this.inputActions.ClickDownAction.Do(x.Data);
                })
                .AddTo(this);

            inputModule.ClickUpAsObservable()
                .Where(x => x.Data.ButtonId == inputModule.MainPointerId)
                .Where(_ => this.inputActions.ClickUpAction != null)
                .SubscribeWithState(this, (x, _this) =>
                {
                    _this.inputActions.ClickUpAction.Do(x.Data);
                })
                .AddTo(this);

            inputModule.DragAsObservable()
                .Where(_ => this.inputActions.DragAction != null)
                .SubscribeWithState(this, (x, _this) =>
                {
                    _this.inputActions.DragAction.Do(x.Data);
                })
                .AddTo(this);

            inputModule.ScrollAsObservable()
                .Where(_ => this.inputActions.ScrollAction != null)
                .SubscribeWithState(this, (x, _this) =>
                {
                    _this.inputActions.ScrollAction.Do(x.Data);
                })
                .AddTo(this);
        }

        void Start()
        {
            this.cachedClickToClickableObjectActions = new ClickToClickableObjectActions(this.gameCameraController);
            this.cachedGenerateCellEventActions = new GenerateCellEventActions(this.cellManager.EventGenerator, this.gameCameraController);
            this.cachedEraseCellEventActions = new EraseCellEventActions(this.cellManager.EventGenerator, this.cellManager.Mapper, this.gameCameraController);
            this.cachedDevelopCellActions = new DevelopCellActions(
                this.cellManager.CellGenerator,
                this.cellManager.Mapper,
                100100,
                100000,
                1,
                this.gameCameraController
                );
        }
    }
}
