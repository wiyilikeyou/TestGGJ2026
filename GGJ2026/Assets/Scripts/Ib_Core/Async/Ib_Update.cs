using JetBrains.Annotations;

namespace Ib_Core
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public static partial class Ib_Async
    {
        /// <summary>
        /// 重置令牌
        /// </summary>
        /// <param name="cts">取消令牌</param>
        /// <returns></returns>
        public static CancellationTokenSource ResetCts(ref CancellationTokenSource cts)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            cts = new();
            return cts;
        }
       
        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="cts">取消令牌</param>
        /// <returns></returns>
        public static void ClearCts(ref CancellationTokenSource cts)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            cts = null;
        }
        /// <summary>
        /// 取消并创建新的令牌,必须接收返回值
        /// </summary>
        /// <param name="cts">取消令牌</param>
        /// <returns></returns>
        [MustUseReturnValue]
        public static CancellationTokenSource CreateNewCts(CancellationTokenSource cts)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            cts = new();
            return cts;
        }
        /// <summary>
        /// 清除,必须接收返回值
        /// </summary>
        /// <param name="cts">取消令牌</param>
        /// <returns></returns>
        [MustUseReturnValue]
        public static CancellationTokenSource CreateNullCts(CancellationTokenSource cts)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }
            cts = null;
            return cts;
        }
    }


    /// <summary>
    /// 基于Unitask的异步Update申请方法，支持自定义更新频率。通过令牌控制生命周期
    /// </summary>
    public static partial class Ib_Async
    {
        #region 循环任务创建调用

        /// <summary>
        /// 创建一个在 Update 循环中异步执行的任务
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="updateInterval">更新间隔（秒），0 表示每帧执行</param>
        /// <param name="cts">取消令牌</param>
        /// <param name="ignoreTimeScale">是否忽略时间缩放</param>
        /// <param name="onBegin">任务开始时的回调</param>
        /// <param name="onEnd">任务结束时的回调（无论正常结束还是取消都会调用）</param>
        public static void CreateUpdateTask(Action action, float updateInterval, CancellationToken cts, bool ignoreTimeScale = true, Action onBegin = null, Action onEnd = null)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (updateInterval < 0) throw new ArgumentOutOfRangeException(nameof(updateInterval), "更新间隔不能为负数");
            DealUpdateTask(action, Mathf.Max(0, updateInterval), ignoreTimeScale ? DelayType.UnscaledDeltaTime : DelayType.DeltaTime, PlayerLoopTiming.Update, cts, onBegin, onEnd)
                .Forget();
        }
        /// <summary>
        /// 创建一个在 FixedUpdate 循环中异步执行的任务
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="updateInterval">更新间隔（秒），0 表示每帧执行</param>
        /// <param name="cts">取消令牌</param>
        /// <param name="onBegin">任务开始时的回调</param>
        /// <param name="onEnd">任务结束时的回调（无论正常结束还是取消都会调用）</param>
        public static void CreateFixedUpdateTask(Action action, float updateInterval, CancellationToken cts, Action onBegin = null, Action onEnd = null)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (updateInterval < 0) throw new ArgumentOutOfRangeException(nameof(updateInterval), "更新间隔不能为负数");
            DealUpdateTask(action, Mathf.Max(0, updateInterval), DelayType.DeltaTime, PlayerLoopTiming.FixedUpdate, cts, onBegin, onEnd).Forget();
        }
        /// <summary>
        /// 创建一个在 Update 按系统时间（不受TimeScale及unity时间系统影响） 循环中异步执行的任务
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="updateInterval">更新间隔（秒），0 表示每帧执行</param>
        /// <param name="cts">取消令牌</param>
        /// <param name="onBegin">任务开始时的回调</param>
        /// <param name="onEnd">任务结束时的回调（无论正常结束还是取消都会调用）</param>
        public static void CreateRealTimeUpdateTask(Action action, float updateInterval, CancellationToken cts, Action onBegin = null, Action onEnd = null)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (updateInterval < 0) throw new ArgumentOutOfRangeException(nameof(updateInterval), "更新间隔不能为负数");
            DealUpdateTask(action, Mathf.Max(0, updateInterval), DelayType.Realtime, PlayerLoopTiming.LastUpdate, cts, onBegin, onEnd)
                .Forget();
        }
        #endregion

        #region 核心循环调用执行方法
        private static async UniTaskVoid DealUpdateTask(Action action, float updateInterval, DelayType delayType, PlayerLoopTiming loopTiming, CancellationToken cts,
            Action onBegin = null, Action onEnd = null)
        {
            try
            {
                onBegin?.Invoke();
                if (updateInterval > 0)
                {
                    var interval = TimeSpan.FromSeconds(updateInterval);
                    while (!cts.IsCancellationRequested)
                    {
                        SafeInvokeAction(action);
                        await UniTask.Delay(interval, delayType, loopTiming, cts);
                    }
                }
                else
                {
                    while (!cts.IsCancellationRequested)
                    {
                        SafeInvokeAction(action);
                        await UniTask.Yield(loopTiming, cts);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex) // 捕获其他所有异常
            {
                Ib_Log.Error(ex);
            }
            finally
            {
                onEnd?.Invoke(); // 确保 onEnd 总是被调用
            }
        }
        
        private static void SafeInvokeAction(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                Ib_Log.Error(ex);
            }
        }
        
        #endregion
    }
}

