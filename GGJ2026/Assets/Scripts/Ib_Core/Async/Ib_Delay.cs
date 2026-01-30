using UnityEngine;

namespace Ib_Core
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using System;

    #region 值类型Action封装

    internal interface IActionInvoker
    {
        void Invoke();
    }

    internal readonly struct ActionInvoker : IActionInvoker
    {
        private readonly Action _action;

        public ActionInvoker(Action action)
        {
            _action = action;
        }

        public void Invoke() => _action?.Invoke();
    }

    internal readonly struct ActionInvoker<T1> : IActionInvoker
    {
        private readonly Action<T1> _action;
        private readonly T1 _arg1;

        public ActionInvoker(Action<T1> action, T1 arg1)
        {
            _action = action;
            _arg1 = arg1;
        }

        public void Invoke() => _action?.Invoke(_arg1);
    }

    internal readonly struct ActionInvoker<T1, T2> : IActionInvoker
    {
        private readonly Action<T1, T2> _action;
        private readonly T1 _arg1;
        private readonly T2 _arg2;

        public ActionInvoker(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        public void Invoke() => _action?.Invoke(_arg1, _arg2);
    }

    internal readonly struct ActionInvoker<T1, T2, T3> : IActionInvoker
    {
        private readonly Action<T1, T2, T3> _action;
        private readonly T1 _arg1;
        private readonly T2 _arg2;
        private readonly T3 _arg3;

        public ActionInvoker(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        public void Invoke() => _action?.Invoke(_arg1, _arg2, _arg3);
    }

    internal readonly struct ActionInvoker<T1, T2, T3, T4> : IActionInvoker
    {
        private readonly Action<T1, T2, T3, T4> _action;
        private readonly T1 _arg1;
        private readonly T2 _arg2;
        private readonly T3 _arg3;
        private readonly T4 _arg4;

        public ActionInvoker(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
        }

        public void Invoke() => _action?.Invoke(_arg1, _arg2, _arg3, _arg4);
    }

    internal readonly struct ActionInvoker<T1, T2, T3, T4, T5> : IActionInvoker
    {
        private readonly Action<T1, T2, T3, T4, T5> _action;
        private readonly T1 _arg1;
        private readonly T2 _arg2;
        private readonly T3 _arg3;
        private readonly T4 _arg4;
        private readonly T5 _arg5;

        public ActionInvoker(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
        }

        public void Invoke() => _action?.Invoke(_arg1, _arg2, _arg3, _arg4, _arg5);
    }

    #endregion

    /// <summary>
    /// 负责异步调用，支持延迟行为
    /// </summary>
    public static partial class Ib_Async
    {
        #region 延迟方法与重载

        /// <summary>
        /// 无参数的延迟方法，若action带参数，请使用多参数的延迟方法，避免使用闭包()=>{};
        /// </summary>
        /// <param name="delay">延迟时间</param>
        /// <param name="action">延迟后执行的方法</param>
        /// <param name="cts">Unitask取消令牌</param>
        /// <param name="cancelCallBack">取消后的回调</param>
        public static void DelayDoSomething(float delay, Action action, CancellationToken cts, Action cancelCallBack = null)
        {
            if (action != null) DealDelayDoSomethingUniTaskCore(delay, new ActionInvoker(action), cts, cancelCallBack).Forget();
            else Ib_Log.Warning("[Action] to delay is Null");
        }

        /// <summary>
        /// 无参数的延迟方法，延迟1帧，若action带参数，请使用多参数的延迟方法，避免使用闭包()=>{};
        /// </summary>
        /// <param name="action">延迟后执行的方法</param>
        /// <param name="cts">Unitask取消令牌</param>
        /// <param name="cancelCallBack">取消后的回调</param>
        public static void DelayDoSomething(Action action, CancellationToken cts, Action cancelCallBack = null)
        {
            if (action != null) DealDelayDoSomethingUniTaskCore(0, new ActionInvoker(action), cts, cancelCallBack).Forget();
            else Ib_Log.Warning("[Action] to delay is Null");
        }

        public static void DelayDoSomething<T1>(float delay, Action<T1> action, T1 arg1, CancellationToken cts, Action cancelCallBack = null)
        {
            if (action != null) DealDelayDoSomethingUniTaskCore(delay, new ActionInvoker<T1>(action, arg1), cts, cancelCallBack).Forget();
            else Ib_Log.Warning("[Action] to delay is Null");
        }

        public static void DelayDoSomething<T1, T2>(float delay, Action<T1, T2> action, T1 arg1, T2 arg2, CancellationToken cts, Action cancelCallBack = null)
        {
            if (action != null) DealDelayDoSomethingUniTaskCore(delay, new ActionInvoker<T1, T2>(action, arg1, arg2), cts, cancelCallBack).Forget();
            else Ib_Log.Warning("[Action] to delay is Null");
        }

        public static void DelayDoSomething<T1, T2, T3>(float delay, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, CancellationToken cts, Action cancelCallBack = null)
        {
            if (action != null) DealDelayDoSomethingUniTaskCore(delay, new ActionInvoker<T1, T2, T3>(action, arg1, arg2, arg3), cts, cancelCallBack).Forget();
            else Ib_Log.Warning("[Action] to delay is Null");
        }

        public static void DelayDoSomething<T1, T2, T3, T4>(float delay, Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken cts, Action cancelCallBack = null)
        {
            if (action != null) DealDelayDoSomethingUniTaskCore(delay, new ActionInvoker<T1, T2, T3, T4>(action, arg1, arg2, arg3, arg4), cts, cancelCallBack).Forget();
            else Ib_Log.Warning("[Action] to delay is Null");
        }

        public static void DelayDoSomething<T1, T2, T3, T4, T5>(float delay, Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken cts, Action cancelCallBack = null)
        {
            if (action != null) DealDelayDoSomethingUniTaskCore(delay, new ActionInvoker<T1, T2, T3, T4, T5>(action, arg1, arg2, arg3, arg4, arg5), cts, cancelCallBack).Forget();
            else Ib_Log.Warning("[Action] to delay is Null");
        }

        #endregion

        #region 延迟核心方法

        private static async UniTaskVoid DealDelayDoSomethingUniTaskCore<TInvoker>(float delay, TInvoker invoker, CancellationToken cts, Action cancelCallBack)
            where TInvoker : struct, IActionInvoker
        {
            try
            {
                if (delay > 0)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cts);
                }
                else
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, cts);
                }

                invoker.Invoke();
            }
            catch (OperationCanceledException)
            {
                cancelCallBack?.Invoke();
                Ib_Log.Warning("DelayDoSomethingActionUniTask was canceled.");
            }
        }

        #endregion
    }
}