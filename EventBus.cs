// 当前这个类，完全由Cursor进行编码及优化。

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

        public CustomEventArgs(object data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// 事件总线，用于在应用程序中传递事件和处理程序。
    /// </summary>
    public static class EventBus
    {
        // 用字典存储事件名称和对应的事件处理程序列表
        private static readonly IDictionary<string, List<Action<CustomEventArgs>>> eventHandlers = new Dictionary<string, List<Action<CustomEventArgs>>>();

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
        public static void Publish(string eventName, CustomEventArgs eventArgs)
        {
            // 如果事件名称不存在，则抛出一个异常
            if (!eventHandlers.ContainsKey(eventName))
            {
                throw new ArgumentException($"Event name '{eventName}' does not exist.");
            }

            // 对于每个事件处理程序，调用其处理方法
            foreach (var handler in eventHandlers[eventName])
            {
                handler(eventArgs);
            }
        }

        public static bool ContainsEvent(string eventName)
        {
            return eventHandlers.ContainsKey(eventName);
        }
    }
}