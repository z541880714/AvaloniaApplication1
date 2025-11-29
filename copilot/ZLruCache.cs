namespace AvaloniaApplication1.copilot;

using System;
using System.Collections.Generic;

public delegate void LruRelease<in T>(T data);

public class ZLruCache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, LinkedListNode<(TKey Key, TValue Value)>> _cache;
    private readonly LinkedList<(TKey Key, TValue Value)> _lruList;
    private readonly LruRelease<TValue>? _releaseAction;

    // 🔒 优化1：独立的锁对象，避免外部死锁风险
    private readonly object _lock = new();

    public ZLruCache(int capacity, LruRelease<TValue>? releaseAction = null)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        _capacity = capacity;
        _releaseAction = releaseAction;
        _cache = new Dictionary<TKey, LinkedListNode<(TKey, TValue)>>(capacity);
        _lruList = new LinkedList<(TKey, TValue)>();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var node))
            {
                // 命中：移到头部
                value = node.Value.Value;
                _lruList.Remove(node);
                _lruList.AddFirst(node);
                return true;
            }
        }

        value = default!;
        return false;
    }

    public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory)
    {
        lock (_lock)
        {
            // ⚡ 优化2：使用 TryGetValue，一次查找即可完成判断和获取 Node
            if (_cache.TryGetValue(key, out var existingNode))
            {
                // 命中：只刷新热度（移动到头部），不更新 Value
                _lruList.Remove(existingNode);
                _lruList.AddFirst(existingNode);
                return existingNode.Value.Value;
            }

            // --- 下面是原本的新增逻辑 ---

            // 容量检查
            if (_cache.Count >= _capacity)
            {
                var lastNode = _lruList.Last;
                if (lastNode != null)
                {
                    // 淘汰最老的
                    _cache.Remove(lastNode.Value.Key);
                    _lruList.RemoveLast();
                    // 触发回调
                    _releaseAction?.Invoke(lastNode.Value.Value);
                }
            }

            // 添加新节点
            var newValue = factory(key);
            var newNode = new LinkedListNode<(TKey, TValue)>((key, newValue));
            _lruList.AddFirst(newNode);
            _cache.Add(key, newNode);
            return newValue;
        }
    }


    // ✨ 优化3：必须要有 Clear，否则页面关闭后缓存还在占用内存
    public void Clear()
    {
        lock (_lock)
        {
            if (_releaseAction != null)
            {
                foreach (var node in _lruList)
                {
                    _releaseAction(node.Value);
                }
            }

            _cache.Clear();
            _lruList.Clear();
        }
    }
}