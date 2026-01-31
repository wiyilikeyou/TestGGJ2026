// =================================================================================
// Ib_EventAsync - 异步事件中心
// ---------------------------------------------------------------------------------
// 这是一个独立的、类型安全的异步事件系统，使用 UniTask 实现高性能的异步操作。
//
// 使用方法:
// 1. 声明一个事件类，继承 Ib_EventAsync:
//    public class OnSaveGameAsync : Ib_EventAsync<OnSaveGameAsync> { };
//    public class OnDataLoadedAsync : Ib_EventAsync<OnDataLoadedAsync, string, int> { };
//
// 2. 在需要监听事件的地方注册异步方法:
//    private async UniTask SaveGameHandler() { ... }
//    OnSaveGameAsync.RegisterAsync(SaveGameHandler);
//
// 3. 在需要触发事件的地方调用:
//    await OnSaveGameAsync.InvokeAsync();
//
// 4. 在不再需要监听时注销 (例如在 OnDestroy 中):
//    OnSaveGameAsync.DeregisterAsync(SaveGameHandler);
// =================================================================================

using System.Threading;

namespace Ib_Core
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;

    // 标记接口，用于泛型约束
    internal interface IIb_EventAsync { }
    #region 零参数事件基类延迟拓展
 
    public abstract partial class Ib_Event<T> : IIb_Event where T : Ib_Event<T>
    {
        public static void InvokeNextFrame(CancellationToken cts) =>
            Ib_Async.DelayDoSomething(0, Ib_EventHandler.Invoke<T>, cts);
    }

    #endregion

    #region 单参数事件基类延迟拓展

    public abstract partial class Ib_Event<T, T1> : IIb_Event where T : Ib_Event<T, T1>
    {
        public static void InvokeNextFrame(T1 arg1, CancellationToken cts) =>
            Ib_Async.DelayDoSomething(0, Ib_EventHandler.Invoke<T, T1>, arg1, cts);
    }

    #endregion

    #region 双参数事件基类延迟拓展

    public abstract partial class Ib_Event<T, T1, T2> : IIb_Event where T : Ib_Event<T, T1, T2>
    {
        public static void InvokeNextFrame(T1 arg1, T2 arg2, CancellationToken cts) =>
            Ib_Async.DelayDoSomething(0, Ib_EventHandler.Invoke<T, T1, T2>, arg1, arg2, cts);
    }

    #endregion

    #region 三参数事件基类延迟拓展

    public abstract partial class Ib_Event<T, T1, T2, T3> : IIb_Event where T : Ib_Event<T, T1, T2, T3>
    {
        public static void InvokeNextFrame(T1 arg1, T2 arg2, T3 arg3, CancellationToken cts) =>
            Ib_Async.DelayDoSomething(0, Ib_EventHandler.Invoke<T, T1, T2, T3>, arg1, arg2, arg3, cts);
    }

    #endregion

    #region 四参数事件基类延迟拓展

    public abstract partial class Ib_Event<T, T1, T2, T3, T4> : IIb_Event where T : Ib_Event<T, T1, T2, T3, T4>
    {
        public static void InvokeNextFrame(T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken cts) =>
            Ib_Async.DelayDoSomething(0, Ib_EventHandler.Invoke<T, T1, T2, T3, T4>, arg1, arg2, arg3, arg4, cts);
    }

    #endregion

    #region 五参数事件基类延迟拓展

    public abstract partial class Ib_Event<T, T1, T2, T3, T4, T5> : IIb_Event where T : Ib_Event<T, T1, T2, T3, T4, T5>
    {
        public static void InvokeNextFrame(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken cts) =>
            Ib_Async.DelayDoSomething(0, Ib_EventHandler.Invoke<T, T1, T2, T3, T4, T5>, arg1, arg2, arg3, arg4, arg5,
                cts);
    }

    #endregion
    
    #region 零参数异步事件
    public abstract class Ib_EventAsync<T> : IIb_EventAsync where T : Ib_EventAsync<T>
    {
        public static UniTask InvokeAsync() => Ib_EventHandler.InvokeAsync<T>();
        public static void RegisterAsync(Func<UniTask> action) => Ib_EventHandler.RegisterAsync<T>(action);
        public static void DeregisterAsync(Func<UniTask> action) => Ib_EventHandler.DeregisterAsync<T>(action);
    }
    #endregion

    #region 单参数异步事件
    public abstract class Ib_EventAsync<T, T1> : IIb_EventAsync where T : Ib_EventAsync<T, T1>
    {
        public static UniTask InvokeAsync(T1 arg1) => Ib_EventHandler.InvokeAsync<T, T1>(arg1);
        public static void RegisterAsync(Func<T1, UniTask> action) => Ib_EventHandler.RegisterAsync<T, T1>(action);
        public static void DeregisterAsync(Func<T1, UniTask> action) => Ib_EventHandler.DeregisterAsync<T, T1>(action);
    }
    #endregion

    #region 双参数异步事件
    public abstract class Ib_EventAsync<T, T1, T2> : IIb_EventAsync where T : Ib_EventAsync<T, T1, T2>
    {
        public static UniTask InvokeAsync(T1 arg1, T2 arg2) => Ib_EventHandler.InvokeAsync<T, T1, T2>(arg1, arg2);
        public static void RegisterAsync(Func<T1, T2, UniTask> action) => Ib_EventHandler.RegisterAsync<T, T1, T2>(action);
        public static void DeregisterAsync(Func<T1, T2, UniTask> action) => Ib_EventHandler.DeregisterAsync<T, T1, T2>(action);
    }
    #endregion

    #region 三参数异步事件
    public abstract class Ib_EventAsync<T, T1, T2, T3> : IIb_EventAsync where T : Ib_EventAsync<T, T1, T2, T3>
    {
        public static UniTask InvokeAsync(T1 arg1, T2 arg2, T3 arg3) => Ib_EventHandler.InvokeAsync<T, T1, T2, T3>(arg1, arg2, arg3);
        public static void RegisterAsync(Func<T1, T2, T3, UniTask> action) => Ib_EventHandler.RegisterAsync<T, T1, T2, T3>(action);
        public static void DeregisterAsync(Func<T1, T2, T3, UniTask> action) => Ib_EventHandler.DeregisterAsync<T, T1, T2, T3>(action);
    }
    #endregion

    #region 四参数异步事件
    public abstract class Ib_EventAsync<T, T1, T2, T3, T4> : IIb_EventAsync where T : Ib_EventAsync<T, T1, T2, T3, T4>
    {
        public static UniTask InvokeAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => Ib_EventHandler.InvokeAsync<T, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        public static void RegisterAsync(Func<T1, T2, T3, T4, UniTask> action) => Ib_EventHandler.RegisterAsync<T, T1, T2, T3, T4>(action);
        public static void DeregisterAsync(Func<T1, T2, T3, T4, UniTask> action) => Ib_EventHandler.DeregisterAsync<T, T1, T2, T3, T4>(action);
    }
    #endregion

    #region 五参数异步事件
    public abstract class Ib_EventAsync<T, T1, T2, T3, T4, T5> : IIb_EventAsync where T : Ib_EventAsync<T, T1, T2, T3, T4, T5>
    {
        public static UniTask InvokeAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => Ib_EventHandler.InvokeAsync<T, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
        public static void RegisterAsync(Func<T1, T2, T3, T4, T5, UniTask> action) => Ib_EventHandler.RegisterAsync<T, T1, T2, T3, T4, T5>(action);
        public static void DeregisterAsync(Func<T1, T2, T3, T4, T5, UniTask> action) => Ib_EventHandler.DeregisterAsync<T, T1, T2, T3, T4, T5>(action);
    }
    #endregion

    /// <summary>
    /// 异步事件处理核心，仅供内部使用。
    /// </summary>
    internal static partial class Ib_EventHandler
    {
        private static readonly Dictionary<Type, Delegate> AsyncEventsDict = new();
        private static readonly object _asyncLock = new object();

        #region 零参数异步处理
        public static UniTask InvokeAsync<T>() where T : IIb_EventAsync { return InvokeAsync_Internal<Func<UniTask>>(typeof(T), action => action()); }
        public static void RegisterAsync<T>(Func<UniTask> action) where T : IIb_EventAsync { RegisterAsyncDelegate(typeof(T), action); }
        public static void DeregisterAsync<T>(Func<UniTask> action) where T : IIb_EventAsync { DeregisterAsyncDelegate(typeof(T), action); }
        #endregion

        #region 单参数异步处理
        public static UniTask InvokeAsync<T, T1>(T1 arg1) where T : IIb_EventAsync { return InvokeAsync_Internal<Func<T1, UniTask>>(typeof(T), action => action(arg1)); }
        public static void RegisterAsync<T, T1>(Func<T1, UniTask> action) where T : IIb_EventAsync { RegisterAsyncDelegate(typeof(T), action); }
        public static void DeregisterAsync<T, T1>(Func<T1, UniTask> action) where T : IIb_EventAsync { DeregisterAsyncDelegate(typeof(T), action); }
        #endregion

        #region 双参数异步处理
        public static UniTask InvokeAsync<T, T1, T2>(T1 arg1, T2 arg2) where T : IIb_EventAsync { return InvokeAsync_Internal<Func<T1, T2, UniTask>>(typeof(T), action => action(arg1, arg2)); }
        public static void RegisterAsync<T, T1, T2>(Func<T1, T2, UniTask> action) where T : IIb_EventAsync { RegisterAsyncDelegate(typeof(T), action); }
        public static void DeregisterAsync<T, T1, T2>(Func<T1, T2, UniTask> action) where T : IIb_EventAsync { DeregisterAsyncDelegate(typeof(T), action); }
        #endregion

        #region 三参数异步处理
        public static UniTask InvokeAsync<T, T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3) where T : IIb_EventAsync { return InvokeAsync_Internal<Func<T1, T2, T3, UniTask>>(typeof(T), action => action(arg1, arg2, arg3)); }
        public static void RegisterAsync<T, T1, T2, T3>(Func<T1, T2, T3, UniTask> action) where T : IIb_EventAsync { RegisterAsyncDelegate(typeof(T), action); }
        public static void DeregisterAsync<T, T1, T2, T3>(Func<T1, T2, T3, UniTask> action) where T : IIb_EventAsync { DeregisterAsyncDelegate(typeof(T), action); }
        #endregion

        #region 四参数异步处理
        public static UniTask InvokeAsync<T, T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : IIb_EventAsync { return InvokeAsync_Internal<Func<T1, T2, T3, T4, UniTask>>(typeof(T), action => action(arg1, arg2, arg3, arg4)); }
        public static void RegisterAsync<T, T1, T2, T3, T4>(Func<T1, T2, T3, T4, UniTask> action) where T : IIb_EventAsync { RegisterAsyncDelegate(typeof(T), action); }
        public static void DeregisterAsync<T, T1, T2, T3, T4>(Func<T1, T2, T3, T4, UniTask> action) where T : IIb_EventAsync { DeregisterAsyncDelegate(typeof(T), action); }
        #endregion

        #region 五参数异步处理
        public static UniTask InvokeAsync<T, T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : IIb_EventAsync { return InvokeAsync_Internal<Func<T1, T2, T3, T4, T5, UniTask>>(typeof(T), action => action(arg1, arg2, arg3, arg4, arg5)); }
        public static void RegisterAsync<T, T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, UniTask> action) where T : IIb_EventAsync { RegisterAsyncDelegate(typeof(T), action); }
        public static void DeregisterAsync<T, T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, UniTask> action) where T : IIb_EventAsync { DeregisterAsyncDelegate(typeof(T), action); }
        #endregion
        
        #region 异步通用逻辑
        private static UniTask InvokeAsync_Internal<TFunc>(Type eventType, Func<TFunc, UniTask> invoke) where TFunc : class
        {
            Delegate d;
            lock (_asyncLock)
            {
                AsyncEventsDict.TryGetValue(eventType, out d);
            }
            if (d == null) return UniTask.CompletedTask;

            var invocationList = d.GetInvocationList();
            // 优化：如果只有一个监听者，直接调用，避免 List<T> 的开销
            if (invocationList.Length == 1)
            {
                try
                {
                    return invoke((TFunc)(object)invocationList[0]);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Error starting async event handler for {eventType.Name}: {ex.Message}\n{ex.StackTrace}");
                    return UniTask.CompletedTask;
                }
            }
            
            // 多个监听者，使用 UniTask.WhenAll
            var tasks = new List<UniTask>(invocationList.Length);
            foreach (var singleDelegate in invocationList)
            {
                try
                {
                    tasks.Add(invoke((TFunc)(object)singleDelegate));
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Error starting async event handler for {eventType.Name}: {ex.Message}\n{ex.StackTrace}");
                }
            }
            return UniTask.WhenAll(tasks);
        }

        private static void RegisterAsyncDelegate(Type eventType, Delegate action)
        {
            lock (_asyncLock)
            {
                AsyncEventsDict.TryGetValue(eventType, out Delegate d);
                AsyncEventsDict[eventType] = Delegate.Combine(d, action);
            }
        }

        private static void DeregisterAsyncDelegate(Type eventType, Delegate action)
        {
            lock (_asyncLock)
            {
                if (AsyncEventsDict.TryGetValue(eventType, out Delegate d))
                {
                    Delegate newDelegate = Delegate.Remove(d, action);
                    if (newDelegate == null)
                        AsyncEventsDict.Remove(eventType);
                    else
                        AsyncEventsDict[eventType] = newDelegate;
                }
            }
        }
        #endregion
    }
}