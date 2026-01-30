//支持至多五个参数的事件中心。
//使用方法：
//1、声明一个事件类，继承Ib_Event
//  public class OnGameLog : Ib_Event<OnGameLog,string> { };
//2、在Mono类中进行注册与注销
//  Action<string> testAction = (string content)=>Ib_Log.Info(content);
//  OnGameLog.Register(testAction);
//  OnGameLog.Deregister(testAction);
//3、调用
//OnGameLog.Invoke("成功调用");

namespace Ib_Core
{
    using System;
    using System.Collections.Generic;
    internal interface IIb_Event
    {
    }

    #region 无参数事件基类

    public abstract partial class Ib_Event<T> : IIb_Event where T : Ib_Event<T>
    {
        public static void Invoke() => Ib_EventHandler.Invoke<T>();
        public static void Register(Action action) => Ib_EventHandler.Register<T>(action);
        public static void Deregister(Action action) => Ib_EventHandler.Deregister<T>(action);
    }

    #endregion

    #region 单参数事件基类

    public abstract partial class Ib_Event<T, T1> : IIb_Event where T : Ib_Event<T, T1>
    {
        public static void Invoke(T1 arg1) => Ib_EventHandler.Invoke<T, T1>(arg1);
        public static void Register(Action<T1> action) => Ib_EventHandler.Register<T, T1>(action);
        public static void Deregister(Action<T1> action) => Ib_EventHandler.Deregister<T, T1>(action);
    }

    #endregion

    #region 双参数事件基类

    public abstract partial class Ib_Event<T, T1, T2> : IIb_Event where T : Ib_Event<T, T1, T2>
    {
        public static void Invoke(T1 arg1, T2 arg2) => Ib_EventHandler.Invoke<T, T1, T2>(arg1, arg2);
        public static void Register(Action<T1, T2> action) => Ib_EventHandler.Register<T, T1, T2>(action);
        public static void Deregister(Action<T1, T2> action) => Ib_EventHandler.Deregister<T, T1, T2>(action);
    }

    #endregion

    #region 三参数事件基类

    public abstract partial class Ib_Event<T, T1, T2, T3> : IIb_Event where T : Ib_Event<T, T1, T2, T3>
    {
        public static void Invoke(T1 arg1, T2 arg2, T3 arg3) => Ib_EventHandler.Invoke<T, T1, T2, T3>(arg1, arg2, arg3);
        public static void Register(Action<T1, T2, T3> action) => Ib_EventHandler.Register<T, T1, T2, T3>(action);
        public static void Deregister(Action<T1, T2, T3> action) => Ib_EventHandler.Deregister<T, T1, T2, T3>(action);
    }

    #endregion

    #region 四参数事件基类

    public abstract partial class Ib_Event<T, T1, T2, T3, T4> : IIb_Event where T : Ib_Event<T, T1, T2, T3, T4>
    {
        public static void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4) =>
            Ib_EventHandler.Invoke<T, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        public static void Register(Action<T1, T2, T3, T4> action) =>
            Ib_EventHandler.Register<T, T1, T2, T3, T4>(action);

        public static void Deregister(Action<T1, T2, T3, T4> action) =>
            Ib_EventHandler.Deregister<T, T1, T2, T3, T4>(action);
    }

    #endregion

    #region 五参数事件基类

    public abstract partial class Ib_Event<T, T1, T2, T3, T4, T5> : IIb_Event where T : Ib_Event<T, T1, T2, T3, T4, T5>
    {
        public static void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) =>
            Ib_EventHandler.Invoke<T, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
        public static void Register(Action<T1, T2, T3, T4, T5> action) =>
            Ib_EventHandler.Register<T, T1, T2, T3, T4, T5>(action);
        public static void Deregister(Action<T1, T2, T3, T4, T5> action) =>
            Ib_EventHandler.Deregister<T, T1, T2, T3, T4, T5>(action);
    }

    #endregion

    #region 事件容器

    /// <summary>
    /// 事件容器，使用写时复制(Copy-on-Write)策略来实现 0 GC 的 Invoke
    /// </summary>
    internal class Ib_EventContainer
    {
        // 直接持有数组，Invoke时直接遍历这个数组
        public Delegate[] Actions = Array.Empty<Delegate>();
        public bool IsEmpty => Actions.Length == 0;

        public void Add(Delegate callback)
        {
            if (callback == null) return;
            // 注册时：创建新数组，长度+1，复制旧数据，添加新数据
            // 虽然这里产生了GC，但注册是低频操作，为了换取 Invoke 的 0 GC
            var oldArray = Actions;
            var newArray = new Delegate[oldArray.Length + 1];
            Array.Copy(oldArray, newArray, oldArray.Length);
            newArray[oldArray.Length] = callback;

            // 原子操作替换引用
            Actions = newArray;
        }

        public void Remove(Delegate callback)
        {
            if (callback == null) return;

            var oldArray = Actions;
            if (oldArray.Length == 0) return; // 空数组直接跳过

            int index = Array.IndexOf(oldArray, callback);
            if (index < 0) return;

            // 注销时：创建新数组，长度-1
            var newArray = new Delegate[oldArray.Length - 1];
            if (index > 0)
            {
                Array.Copy(oldArray, 0, newArray, 0, index);
            }

            if (index < oldArray.Length - 1)
            {
                Array.Copy(oldArray, index + 1, newArray, index, oldArray.Length - index - 1);
            }

            Actions = newArray;
        }
    }

    #endregion

    internal static partial class Ib_EventHandler
    {
        private static readonly Dictionary<Type, Ib_EventContainer> EventsDict = new();
        private static readonly object _lock = new object();

        // 获取或创建容器的辅助方法
        private static Ib_EventContainer GetContainer(Type type, bool createIfNull)
        {
            if (!EventsDict.TryGetValue(type, out var container))
            {
                if (createIfNull)
                {
                    container = new Ib_EventContainer();
                    EventsDict[type] = container;
                }
            }

            return container;
        }

        #region 无参数的事件处理

        public static void Invoke<T>() where T : IIb_Event
        {
            Delegate[] actions = null;
            lock (_lock)
            {
                if (EventsDict.TryGetValue(typeof(T), out var container))
                {
                    actions = container.Actions;
                }
            }

            if (actions == null || actions.Length == 0) return;

            for (int i = 0; i < actions.Length; i++)
            {
                var action = (Action)actions[i];
                try
                {
                    action?.Invoke();
                }
                catch (Exception ex)
                {
                    Ib_Log.Error($"Error while invoking event {typeof(T).Name}: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        public static void Register<T>(Action action) where T : IIb_Event
        {
            lock (_lock)
            {
                var container = GetContainer(typeof(T), true);
                container.Add(action);
            }
        }

        public static void Deregister<T>(Action action) where T : IIb_Event
        {
            lock (_lock)
            {
                var container = GetContainer(typeof(T), false);
                if (container != null)
                {
                    container.Remove(action);
                    if (container.IsEmpty)
                    {
                        EventsDict.Remove(typeof(T));
                    }
                }
            }
        }

        #endregion

        #region 单参数的事件处理

        public static void Invoke<T, T1>(T1 arg1) where T : IIb_Event
        {
            Delegate[] actions = null;
            lock (_lock)
            {
                if (EventsDict.TryGetValue(typeof(T), out var container))
                {
                    actions = container.Actions;
                }
            }

            if (actions == null || actions.Length == 0) return;

            for (int i = 0; i < actions.Length; i++)
            {
                var action = (Action<T1>)actions[i];
                try
                {
                    action?.Invoke(arg1);
                }
                catch (Exception ex)
                {
                    Ib_Log.Error($"Error while invoking event {typeof(T).Name}: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        public static void Register<T, T1>(Action<T1> action) where T : IIb_Event
        {
            lock (_lock)
            {
                var container = GetContainer(typeof(T), true);
                container.Add(action);
            }
        }

        public static void Deregister<T, T1>(Action<T1> action) where T : IIb_Event
        {
            lock (_lock)
            {
                var container = GetContainer(typeof(T), false);
                if (container != null)
                {
                    container.Remove(action);
                    if (container.IsEmpty)
                    {
                        EventsDict.Remove(typeof(T));
                    }
                }
            }
        }

        #endregion

        #region 双参数的事件处理

        public static void Invoke<T, T1, T2>(T1 arg1, T2 arg2) where T : IIb_Event
        {
            Delegate[] actions = null;
            lock (_lock)
            {
                if (EventsDict.TryGetValue(typeof(T), out var container))
                {
                    actions = container.Actions;
                }
            }

            if (actions == null || actions.Length == 0) return;

            for (int i = 0; i < actions.Length; i++)
            {
                var action = (Action<T1, T2>)actions[i];
                try
                {
                    action?.Invoke(arg1, arg2);
                }
                catch (Exception ex)
                {
                    Ib_Log.Error($"Error while invoking event {typeof(T).Name}: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        public static void Register<T, T1, T2>(Action<T1, T2> action) where T : IIb_Event
        {
            lock (_lock)
            {
                var container = GetContainer(typeof(T), true);
                container.Add(action);
            }
        }

        public static void Deregister<T, T1, T2>(Action<T1, T2> action) where T : IIb_Event
        {
            lock (_lock)
            {
                var container = GetContainer(typeof(T), false);
                if (container != null)
                {
                    container.Remove(action);
                    if (container.IsEmpty)
                    {
                        EventsDict.Remove(typeof(T));
                    }
                }
            }
        }

        #endregion

        #region 三参数的事件处理

        public static void Invoke<T, T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3) where T : IIb_Event
        {
            Delegate[] actions = null;
            lock (_lock)
            {
                if (EventsDict.TryGetValue(typeof(T), out var container))
                {
                    actions = container.Actions;
                }
            }

            if (actions == null || actions.Length == 0) return;

            for (int i = 0; i < actions.Length; i++)
            {
                var action = (Action<T1, T2, T3>)actions[i];
                try
                {
                    action?.Invoke(arg1, arg2, arg3);
                }
                catch (Exception ex)
                {
                    Ib_Log.Error($"Error while invoking event {typeof(T).Name}: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        public static void Register<T, T1, T2, T3>(Action<T1, T2, T3> action) where T : IIb_Event
        {
            lock (_lock)
            {
                var container = GetContainer(typeof(T), true);
                container.Add(action);
            }
        }

        public static void Deregister<T, T1, T2, T3>(Action<T1, T2, T3> action) where T : IIb_Event
        {
            lock (_lock)
            {
                var container = GetContainer(typeof(T), false);
                if (container != null)
                {
                    container.Remove(action);
                    if (container.IsEmpty)
                    {
                        EventsDict.Remove(typeof(T));
                    }
                }
            }
        }

        #endregion

        #region 四参数的事件处理

        public static void Invoke<T, T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : IIb_Event
        {
            Delegate[] actions = null;
            lock (_lock)
            {
                if (EventsDict.TryGetValue(typeof(T), out var container))
                {
                    actions = container.Actions;
                }
            }

            if (actions == null || actions.Length == 0) return;

            for (int i = 0; i < actions.Length; i++)
            {
                var action = (Action<T1, T2, T3, T4>)actions[i];
                try
                {
                    action?.Invoke(arg1, arg2, arg3, arg4);
                }
                catch (Exception ex)
                {
                    Ib_Log.Error($"Error while invoking event {typeof(T).Name}: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        public static void Register<T, T1, T2, T3, T4>(Action<T1, T2, T3, T4> action) where T : IIb_Event
        {
            lock (_lock)
            {
                var container = GetContainer(typeof(T), true);
                container.Add(action);
            }
        }

        public static void Deregister<T, T1, T2, T3, T4>(Action<T1, T2, T3, T4> action) where T : IIb_Event
        {
            lock (_lock)
            {
                var container = GetContainer(typeof(T), false);
                if (container != null)
                {
                    container.Remove(action);
                    if (container.IsEmpty)
                    {
                        EventsDict.Remove(typeof(T));
                    }
                }
            }
        }

        #endregion

        #region 五参数的事件处理

        public static void Invoke<T, T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where T : IIb_Event
        {
            Delegate[] actions = null;
            lock (_lock)
            {
                if (EventsDict.TryGetValue(typeof(T), out var container))
                {
                    actions = container.Actions;
                }
            }

            if (actions == null || actions.Length == 0) return;

            for (int i = 0; i < actions.Length; i++)
            {
                var action = (Action<T1, T2, T3, T4, T5>)actions[i];
                try
                {
                    action?.Invoke(arg1, arg2, arg3, arg4, arg5);
                }
                catch (Exception ex)
                {
                    Ib_Log.Error($"Error while invoking event {typeof(T).Name}: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        public static void Register<T, T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action) where T : IIb_Event
        {
            lock (_lock)
            {
                var container = GetContainer(typeof(T), true);
                container.Add(action);
            }
        }

        public static void Deregister<T, T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action) where T : IIb_Event
        {
            lock (_lock)
            {
                var container = GetContainer(typeof(T), false);
                if (container != null)
                {
                    container.Remove(action);
                    if (container.IsEmpty)
                    {
                        EventsDict.Remove(typeof(T));
                    }
                }
            }
        }

        #endregion
    }
}