﻿// 当前这个类，完全由Cursor进行编码及优化。

using System.Reflection;

namespace DynamicWallpaper
{
    public class CustomEventArgs : EventArgs
    {
        public object? Data { get; }

        public T? GetData<T>()
        {
            if (Data == null)
                return default;
            return (T)Data;
        }

        public CustomEventArgs()
        {
        }

        public CustomEventArgs(object? data)
        {
            Data = data;
        }
    }

    public static class EventName
    {
        public static string BoxReady => "Box.Ready";
        public static string BoxAutoOpen => "Box.AutoOpen";
        public static string BoxOpen => "Box.Open";
        public static string BoxLoad => "Box.Load";
        public static string BoxFail => "Box.Fail";
        public static string BoxLost => "Box.Lost";
        public static string BoxExists => "Box.Exists";
        public static string BoxSuccess => "Box.Success";
        public static string BoxFinish => "Box.Finish";
        public static string BoxRandom => "Box.Random";
        public static string SetLockScreenImageFailed => "SetLockScreenImageFailed";
        public static string AutoRefresh => "AutoRefresh";
        public static string DeleteOldFile => "DeleteOldFile";
        public static string SwitchLang => "SwitchLang";

        public static string WallPaperChanged => "WallPaperChanged";

        public static string DelWallpaper => "DelWallpaper";

        public static string SetWallpaper => "SetWallpaper";

        public static string SetLockScreen => "SetLockScreen";

        public static string DownFail  => "DownFail";

        public static string LovePaper => "LovePaper";
        public static string NoLovePaper => "NoLovePaper";
        public static string BrokenPaper => "BrokenPaper";
    }



    /// <summary>
    /// 事件总线，用于在应用程序中传递事件和处理程序。
    /// </summary>
    public static class EventBus
    {
        // 用字典存储事件名称和对应的事件处理程序列表
        private static readonly IDictionary<string, List<Action<CustomEventArgs>>> eventHandlers = new Dictionary<string, List<Action<CustomEventArgs>>>();

        // 最多允许3个任务并行
        private static readonly int maxConcurrentTasks = 3;

        static EventBus()
        {
            //  扫描当前程序集中，所有的类，找到RegisterEvent的静态无参数的方法，然后调用它
            //  用于确保所有的事件在订阅和发布前，都已被注册
            var methods = Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.Name == "RegisterEvent" && m.GetParameters().Length == 0));
            
            foreach (var m in methods)
            {
                m.Invoke(null, null);
            }
        }


        /// <summary>
        /// 注册事件。
        /// </summary>
        /// <param name="eventName">事件名称。</param>
        public static void Register(string eventName)
        {
            // 如果事件名称不存在，则创建一个新的事件处理程序列表
            if (!eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName] = new List<Action<CustomEventArgs>>();
            }
        }

        /// <summary>
        /// 订阅事件处理程序。
        /// </summary>
        /// <param name="eventName">事件名称。</param>
        /// <param name="handler">事件处理程序。</param>
        public static void Subscribe(string eventName, Action<CustomEventArgs> handler)
        {
            // 如果事件名称不存在，则抛出一个异常
            if (!eventHandlers.ContainsKey(eventName))
            {
                throw new ArgumentException($"Event name '{eventName}' does not exist.");
            }

            // 添加事件处理程序到列表中
            eventHandlers[eventName].Add(handler);
        }

        /// <summary>
        /// 发布事件。
        /// </summary>
        /// <param name="eventName">事件名称。</param>
        /// <param name="eventArgs">事件参数。</param>
        public static async Task PublishAsync(string eventName, CustomEventArgs eventArgs)
        {
            // 如果事件名称不存在，则抛出一个异常
            if (!eventHandlers.ContainsKey(eventName))
            {
                throw new ArgumentException($"Event name '{eventName}' does not exist.");
            }

            // 创建一个SemaphoreSlim对象，将其初始计数器设置为maxConcurrentTasks
            var semaphore = new SemaphoreSlim(maxConcurrentTasks);

            // 创建一个Task数组，用于存储每个事件处理程序的Task
            var tasks = new Task[eventHandlers[eventName].Count];

            // 对于每个事件处理程序，创建一个Task并将其存储在数组中
            for (int i = 0; i < tasks.Length; i++)
            {
                //  把i的值，过渡给变量，防止后续线程未执行，而i的值受循环影响变化，
                var index = i;
                // 等待信号量
                await semaphore.WaitAsync();

                // 创建一个Task并将其存储在数组中
                tasks[i] = Task.Run(() =>
                {
                    try
                    {
                        // 调用事件处理程序前，再次检查一下索引是否超过
                        if (index < eventHandlers[eventName].Count)
                        {
                            eventHandlers[eventName][index](eventArgs);
                        }
                    }
                    finally
                    {
                        // 释放信号量
                        semaphore.Release();
                    }
                });
            }

            // 等待所有任务完成
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 同步发布事件，当事件的订阅者有大于1个时，会用异步处理。
        /// </summary>
        /// <param name="eventName">事件名称。</param>
        /// <param name="eventArgs">事件参数。</param>
        public static void Publish(string eventName, CustomEventArgs eventArgs)
        {
            if (!eventHandlers.ContainsKey(eventName))
                throw new ArgumentException($"Event name '{eventName}' does not exist.");

            var handlerCount = eventHandlers[eventName].Count;
            switch (handlerCount)
            {
                case 0:
                    return;
                case 1:
                    eventHandlers[eventName][0](eventArgs);
                    break;
                default:
                    // 在后台线程中调用PublishAsync方法并等待其完成
                    Task.Run(async () => await PublishAsync(eventName, eventArgs)).Wait();
                    break;
            }
        }

        public static bool ContainsEvent(string eventName)
        {
            return eventHandlers.ContainsKey(eventName);
        }
    }
}