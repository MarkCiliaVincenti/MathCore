﻿#nullable enable
namespace MathCore;

/// <summary>Построитель хеш-суммы</summary>
public readonly ref struct HashBuilder
{
    public const int BaseMultiplier = 379;

    /* ------------------------------------------------------------------------------------- */

    public static HashBuilder New(object? Obj) => Obj is null ? new(0) : new(Obj.GetHashCode());
    public static HashBuilder New(object? Obj, int HashBase) => Obj is null ? new(0) : new(Obj.GetHashCode(), HashBase);
    public static HashBuilder New<T>(T Struct) where T : struct => new(Struct.GetHashCode());
    public static HashBuilder New<T>(T Struct, int HashBase) where T : struct => new(Struct.GetHashCode(), HashBase);

    /* ------------------------------------------------------------------------------------- */

    /// <summary>Текущая хеш-сумма</summary>
    private readonly int _Hash;

    /// <summary>Базовый множитель суммы</summary>
    private readonly int _HashBase;

    /// <summary>Текущая хеш-сумма</summary>
    public int Hash => _Hash;

    /// <summary>Инициализация нового построителя хеш-суммы</summary>
    /// <param name="Hash">Базовое значение хеш-суммы</param>
    /// <param name="HashBase">Множитель хеш-суммы</param>
    public HashBuilder(int Hash, int HashBase = BaseMultiplier)
    {
        _Hash = Hash;
        _HashBase = HashBase;
    }

    /// <summary>Изменение множителя</summary>
    /// <param name="Base">Новое значение множителя хеш-суммы</param>
    /// <returns>Новый построитель хеш=суммы с изменённым значением множителя</returns>
    public HashBuilder HashBase(int Base) => new(_Hash, Base);

    /// <summary>Добавление компонента хеш-суммы</summary>
    /// <param name="hash">Добавляемое значение хеш-суммы</param>
    /// <returns>Новый построитель хеш-суммы с изменённым значением и тем же самым множителем</returns>
    public HashBuilder Append(int hash) => Append(hash, _HashBase == 0 ? BaseMultiplier : _HashBase);

    /// <summary>Добавление компонента хеш-суммы</summary>
    /// <param name="hash">Добавляемое значение хеш-суммы</param>
    /// <param name="Base">Новый множитель</param>
    /// <returns>Новый построитель хеш-суммы с изменённым значением и тем же самым множителем</returns>
    public HashBuilder Append(int hash, int Base) => new(unchecked((_Hash * Base) ^ hash), Base);

    /// <summary>Добавление хеш-суммы объекта к сумме</summary>
    /// <param name="Obj">Добавляемый объект</param>
    /// <returns>Новый построитель хеш-суммы с изменённым значением и тем же самым множителем</returns>
    public HashBuilder Append(object? Obj) => Obj is null ? this : Append(Obj.GetHashCode());

    /// <summary>Добавление хеш-суммы объекта к сумме</summary>
    /// <param name="Obj">Добавляемый объект</param>
    /// <returns>Новый построитель хеш-суммы с изменённым значением и тем же самым множителем</returns>
    public HashBuilder Append<T>(T? Obj) => Obj is null ? this : Append(Obj.GetHashCode());
    public HashBuilder Append<T>(T? Struct) where T : struct => Append(Struct.GetHashCode());

    public HashBuilder Append(double Value) => Append(Value.GetHashCode());
    public HashBuilder Append(float Value) => Append(Value.GetHashCode());
    public HashBuilder Append(byte Value) => Append(Value.GetHashCode());
    public HashBuilder Append(sbyte Value) => Append(Value.GetHashCode());
    public HashBuilder Append(uint Value) => Append(Value.GetHashCode());
    public HashBuilder Append(long Value) => Append(Value.GetHashCode());
    public HashBuilder Append(ulong Value) => Append(Value.GetHashCode());
    public HashBuilder Append(char Value) => Append(Value.GetHashCode());
    public HashBuilder Append(bool Value) => Append(Value.GetHashCode());

    public static HashBuilder operator +(HashBuilder Builder, int Hash) => Builder.Append(Hash);
    public static HashBuilder operator +(HashBuilder Builder, object Obj) => Builder.Append(Obj);

    public static implicit operator int(HashBuilder Builder) => Builder.Hash;

    public static explicit operator HashBuilder(int Hash) => new(Hash);
}