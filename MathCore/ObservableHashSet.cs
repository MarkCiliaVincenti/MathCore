﻿#nullable enable
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using MathCore.Annotations;

namespace MathCore;

/// <summary>Хеш-таблица с уведомлением об изменениях</summary>
/// <typeparam name="T">Тип элементов коллекции</typeparam>
/// <remarks>Инициализация новой хеш-таблицы с уведомлениями об изменениях в содержимом</remarks>
/// <param name="Set">Внутренняя таблица</param>
public class ObservableHashSet<T>(HashSet<T> Set) : ICollection<T>, INotifyPropertyChanged, INotifyCollectionChanged
{
    #region INotifyPropertyChanged

    /// <summary>Событие происходит при изменении значения свойства</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Метод генерации события изменения значения свойства</summary>
    /// <param name="PropertyName">Имя изменившегося свойства</param>
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null!) => PropertyChanged?.Invoke(this, new(PropertyName));

    #endregion

    #region INotifyCollectionChanged

    /// <summary>Событие происходит при изменении коллекции</summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>Метод генерации события изменения коллекции</summary>
    /// <param name="e">Параметры события</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

    #endregion

    /// <summary>Внутренняя хеш-таблица</summary>
    private readonly HashSet<T> _HashSet = Set.NotNull();

    /// <summary>Инициализация новой хеш-таблицы с уведомлениями об изменениях в содержимом</summary>
    public ObservableHashSet() : this([]) { }

    /// <summary>Инициализация новой хеш-таблицы с уведомлениями об изменениях в содержимом</summary>
    /// <param name="Items">Исходный набор элементов</param>
    public ObservableHashSet(IEnumerable<T> Items) : this([..Items]) { }

    /// <inheritdoc />
    public int Count => _HashSet.Count;

    /// <inheritdoc />
    bool ICollection<T>.IsReadOnly => false;

    /// <inheritdoc />
    public void Add(T? item)
    {
        if (!_HashSet.Add(item)) return;
        OnPropertyChanged(nameof(Count));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item));
    }

    /// <inheritdoc />
    public void Clear()
    {
        if (_HashSet.Count == 0) return;
        _HashSet.Clear();
        OnPropertyChanged(nameof(Count));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }

    /// <inheritdoc />
    public bool Contains(T? item) => _HashSet.Contains(item);

    /// <inheritdoc />
    public void CopyTo(T[] array, int Index) => _HashSet.CopyTo(array, Index);

    /// <inheritdoc />
    public bool Remove(T? item)
    {
        if (!_HashSet.Remove(item)) return false;
        OnPropertyChanged(nameof(Count));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item));
        return true;
    }

    #region IEnumerable<T>

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _HashSet.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_HashSet).GetEnumerator();

    #endregion
}