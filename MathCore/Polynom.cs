﻿#nullable enable
using System.Collections;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

// ReSharper disable InconsistentNaming
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

// ReSharper disable UnusedMember.Global

namespace MathCore;

/// <summary>
/// Полином степени N-1
///  a[0]+a[1]*x+a[2]*x^2+...+a[N-1]*x^(N-1)+a[N-1]*x^(N-1)
/// где N - число элементов массива коэффициентов
/// Нулевой элемент массива при нулевой степени члена полинома 
/// </summary>
/// <remarks>Полином степени N, нулевой элемент массива a[0] при младшей степени x^0</remarks>
/// <param name="a">a[0]+a[1]*x+a[2]*x^2+...+a[N-1]*x^(N-1)+a[N-1]*x^(N-1)</param>
[Serializable]
[method: DST]//[DebuggerDisplay("GetPower = {GetPower}")]
public partial class Polynom(params double[] a) : ICloneable<Polynom>, IEquatable<Polynom>, IEnumerable<double>, IFormattable
{
    /// <inheritdoc />
    [DST]
    public Polynom(IEnumerable<double> a) : this(a.ToArray()) { }

    /// <inheritdoc />
    [DST]
    public Polynom(IEnumerable<int> a) : this(a.Select(v => (double)v)) { }

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Создание нового полинома из массива его коэффициентов</summary>
    /// <param name="a">Массив коэффициентов полинома</param>
    /// <returns>Полином, созданный на основе массива его коэффициентов</returns>
    public static Polynom FromCoefficients(params double[] a) => new(a);

    /// <summary>Создание нового полинома на основе его корней</summary>
    /// <param name="roots">Корни полинома</param>
    /// <returns>Полином, собранный из массива корней</returns>
    public static Polynom FromRoots(IEnumerable<double> roots) => FromRoots(roots.ToArray());

    /// <summary>Получить полином из корней полинома</summary>
    /// <param name="Root">Корни полинома</param>
    /// <returns>Полином с указанными корнями</returns>
    public static Polynom FromRoots(params double[] Root) => new(Array.GetCoefficients(Root));

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Коэффициенты при степенях</summary>
    /// <remarks>a[0]+a[1]*x+a[2]*x^2+...+a[N-1]*x^(N-1)+a[N-1]*x^(N-1)</remarks>
    private readonly double[] _a = a ?? throw new ArgumentNullException(nameof(a));

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>
    /// Коэффициенты при степенях
    ///   a[0]+a[1]*x+a[2]*x^2+...+a[N-1]*x^(N-1)+a[N-1]*x^(N-1)
    /// </summary>
    [XmlArray(ElementName = "a")]
    public double[] Coefficients => _a;

    /// <summary>Степень полинома = число коэффициентов - 1</summary>
    public int Power => _a.Length - 1;

    /// <summary>Длина полинома - число коэффициентов</summary>
    public int Length => _a.Length;

    ///<summary>
    /// Коэффициент при степени <paramref name="n"/>, где <paramref name="n"/> принадлежит [0; <see cref="Power"/>]
    /// <see cref="Power"/> = <see cref="Length"/> - 1
    /// </summary>
    ///<param name="n">Степень a[0]+a[1]*x+a[2]*x^2+...<b>+a[<paramref name="n"/>]*x^<paramref name="n"/>+</b>...+a[N-1]*x^(N-1)+a[N-1]*x^(N-1)</param>
    public ref double this[int n] { [DST] get => ref _a[n]; }

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Получить значение полинома</summary>
    /// <param name="x">Аргумент</param>
    /// <returns>Значение полинома в точке x</returns>
    [DST]
    public double Value(double x) => Array.GetValue(x, _a);

    /// <summary>Вычислить комплексное значение полинома</summary>
    /// <param name="z">Комплексный аргумент</param>
    /// <returns>Комплексное значение полинома в точке</returns>
    [DST]
    public Complex Value(Complex z) => Array.GetValue(z, _a);

    /// <summary>Получить функцию полинома</summary>
    /// <returns>Функция полинома</returns>
    public Func<double, double> GetFunction() => Value;

    /// <summary>Получить комплексную функцию полинома</summary>
    /// <returns>Комплексная функция полинома</returns>
    public Func<Complex, Complex> GetComplexFunction() => Value;

    /// <summary>Выполнить операцию деления полинома на полином</summary>
    /// <param name="Divisor">Полином - делимое</param>
    /// <param name="Remainder">Полином - делитель</param>
    /// <returns>Полином - частное</returns>
    public Polynom DivideTo(Polynom Divisor, out Polynom Remainder)
    {
        if (Divisor is null) throw new ArgumentNullException(nameof(Divisor));

        Array.Divide(_a, Divisor._a, out var result, out var remainder);
        var n = remainder.Length;
        while (n >= 1 && remainder[n - 1].Equals(0)) n--;
        System.Array.Resize(ref remainder, n);
        Remainder = new(remainder);
        return new(result);
    }

    /// <summary>Представить полином в виде математической записи в степенной форме</summary>
    /// <returns>Строковое представление полинома в степенной форме</returns>
    public string ToMathString()
    {
        var result = new StringBuilder();
        var length = _a.Length;
        for (var n = _a.Length - 1; n > 0; n--)
        {
            var a = _a[n];
            if (a == 0) continue;

            if (result.Length > 0 && a > 0)
                result.Append("+");

            if (a != 1)
                result.Append(a == -1 ? "-" : a.ToString(CultureInfo.CurrentCulture));

            result.Append("x");
            if (n > 1)
                result.AppendFormat("^{0}", n.ToString());
        }
        if (length > 0 && _a[0] != 0)
            result.AppendFormat("{0}{1}",
                _a[0] < 0 ? string.Empty : "+",
                _a[0]);

        return result.Length == 0 ? "0" : result.ToString();
    }

    /// <summary>Дифференцирование полинома</summary>
    /// <param name="Order">Порядок дифференциала</param>
    /// <returns>Полином - результат дифференцирования</returns>
    public Polynom GetDifferential(int Order = 1)
    {
        var coefficients = Array.GetDifferential(_a, Order);
        var zeros = 0;
        while (zeros < coefficients.Length)
        {
            if (coefficients[coefficients.Length - 1 - zeros] != 0)
                break;
            zeros++;
        }

        if (zeros > 0)
            System.Array.Resize(ref coefficients, coefficients.Length - zeros);

        return new(coefficients);
    }

    /// <summary>Интегрирование полинома</summary>
    /// <param name="C">Константа интегрирования</param>
    /// <param name="Order">Кратность интеграла</param>
    /// <returns>Полином - результат интегрирования полинома</returns>
    public Polynom GetIntegral(double C = 0, int Order = 1) => new(Array.GetIntegral(_a, C, Order));

    /// <summary>Вычислить обратный полином</summary>
    /// <returns>Полином, являющийся обратным к текущим</returns>
    public Polynom GetInversed()
    {
        Array.Divide([1d], _a, out var result, out _);
        return new(result);
    }

    /// <summary>Масштабирование полинома Q(x) = P(x * c)</summary>
    /// <param name="c">Коэффициент масштабирования полинома</param>
    /// <returns>Отмасштабированный полином</returns>
    public Polynom ScalePolynom(double c)
    {
        var result = Clone();
        var a = result._a;
        var cc = 1d;
        for (int i = 1, length = a.Length; i < length; i++)
            a[i] *= cc *= c;
        return result;
    }

    /// <summary>Подстановка полинома x' = P(x) в полином Q(x')=Q(P(x))</summary>
    /// <param name="P">Полином - подстановка</param>
    /// <returns>Полином - результат подстановки</returns>
    public Polynom Substitute(Polynom P)
    {
        var p = P.Clone();
        var result = new Polynom(_a[0]);
        var i = 1;
        for (; i < _a.Length - 1; i++)
        {
            result += _a[i] * p;
            p *= P;
        }
        if (i < _a.Length) result += _a[i] * p;
        return result;
    }

    /* -------------------------------------------------------------------------------------------- */

    #region Implementation of IEquatable<Polynom>

    /// <inheritdoc />
    public bool Equals(Polynom? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other) || ReferenceEquals(_a, other._a)) return true;

        var b = other._a;
        var length = _a.Length;

        if (length != b.Length) return false;

        for (var i = 0; i < length; i++)
            if (_a[i] != other._a[i])
                return false;

        return true;
    }

    /// <inheritdoc />
    public Polynom Clone() => new((double[])_a.Clone());

    /// <inheritdoc />
    object ICloneable.Clone() => Clone();

    /// <inheritdoc />
    [DST]
    public override bool Equals(object obj) => Equals(obj as Polynom);

    #endregion

    /// <inheritdoc />
    [DST]
    public override int GetHashCode() => _a.Select((i, a) => i.GetHashCode() ^ a.GetHashCode()).Aggregate(0x285da41, (S, s) => S ^ s);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<double>)this).GetEnumerator();

    /// <inheritdoc />
    IEnumerator<double> IEnumerable<double>.GetEnumerator() => ((IEnumerable<double>)_a).GetEnumerator();

    //public override string ToString() =>
    //    _a.Aggregate(new StringBuilder(), (S, a, i) => S.AppendFormat("{0}{1}{2}",
    //        a < 0 || i == 0 ? string.Empty : "+",
    //        a,
    //        i == 0 ? string.Empty : $"*x{(i == 1 ? string.Empty : "^" + i)}"))
    //      .ToString();

    //public override string ToString() => _a
    //   .Aggregate(new StringBuilder(), (S, a, i) =>
    //    {
    //        switch (a)
    //        {
    //            case 0:
    //                return S;
    //            case > 0 when i > 0:
    //                S.Append('+');
    //                break;
    //            case < 0: 
    //                S.Append('-');
    //                break;
    //        }

    //        return i switch
    //        {
    //            0 => S.Append(a),
    //            1 => S.Append(a).Append("*x"),
    //            _ => S.Append(a).Append("*x^").Append(i)
    //        };
    //    })
    //   .ToString();

    /// <inheritdoc />
    public override string ToString()
    {
        var result = new StringBuilder();
        var length = _a.Length;
        for (var i = 0; i <= length; i++)
        {
            var a = _a[i];
            switch (a)
            {
                case > 0 when i > 0:
                    result.Append('+');
                    break;
                case < 0:
                    result.Append('-');
                    break;
            }

            result.Append(a);
            switch (i)
            {
                case 1: result.Append("*x"); break;
                default: result.Append("*x^").Append(i); break;
            }
        }

        return result.ToString();
    }

    /// <summary>Строковое представление полинома с форматированием</summary>
    /// <param name="Format">Строка форматирования</param>
    /// <returns>Форматированное представление полинома</returns>
    public string ToString(string Format)
    {
        var result = new StringBuilder();
        var length = _a.Length;
        for (var i = 0; i <= length; i++)
        {
            var a = _a[i];
            switch (a)
            {
                case > 0 when i > 0:
                    result.Append('+');
                    break;
                case < 0:
                    result.Append('-');
                    break;
            }

            result.Append(a.ToString(Format));
            switch (i)
            {
                case 1: result.Append("*x"); break;
                default: result.Append("*x^").Append(i); break;
            }
        }

        return result.ToString();
    }

    /// <summary>Строковое представление полинома</summary>
    /// <param name="provider">Информация о формате</param>
    /// <returns>Строковое представление полинома</returns>
    public string ToString(IFormatProvider provider)
    {
        var result = new StringBuilder();
        var length = _a.Length;
        for (var i = 0; i <= length; i++)
        {
            var a = _a[i];
            switch (a)
            {
                case > 0 when i > 0:
                    result.Append('+');
                    break;
                case < 0:
                    result.Append('-');
                    break;
            }

            result.Append(a.ToString(provider));
            switch (i)
            {
                case 1: result.Append("*x"); break;
                default: result.Append("*x^").Append(i); break;
            }
        }

        return result.ToString();
    }

    /// <summary>Возвращает строковое представление полинома</summary>
    /// <param name="format">Строка формата.</param>
    /// <param name="provider">Информация о формате.</param>
    /// <returns>Строковое представление полинома.</returns>
    public string ToString(string Format, IFormatProvider provider)
    {
        var result = new StringBuilder();
        var length = _a.Length;
        for (var i = 0; i <= length; i++)
        {
            var a = _a[i];
            switch (a)
            {
                case > 0 when i > 0:
                    result.Append('+');
                    break;
                case < 0:
                    result.Append('-');
                    break;
            }

            result.Append(a.ToString(Format, provider));
            switch (i)
            {
                case 1: result.Append("*x"); break;
                default: result.Append("*x^").Append(i); break;
            }
        }

        return result.ToString();
    }
}