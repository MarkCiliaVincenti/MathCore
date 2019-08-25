﻿using System;
using System.Collections.Generic;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using System.Diagnostics.Contracts;
using System.Linq;
using MathCore.Annotations;
using static MathCore.Matrix.Array.Operator;
// ReSharper disable ExceptionNotThrown
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable LocalizableElement
// ReSharper disable UnusedMember.Global

namespace MathCore
{
    /// <summary>Матрица NxM</summary>
    /// <remarks>
    /// i (первый индекс) - номер строки, 
    /// j (второй индекс) - номер столбца
    /// ------------ j ---------->
    /// | a11 a12 a13 a14 a15 a16 a1M
    /// | a21........................
    /// | a31........................
    /// | a41.......aij..............
    /// i a51........................
    /// | a61........................
    /// | aN1.....................aNM
    /// \/
    /// </remarks>
    [Serializable]
    public partial class Matrix : ICloneable<Matrix>, ICloneable<double[,]>, IFormattable,
        IEquatable<Matrix>, IEquatable<double[,]>, IIndexable<int, int, double>
    {
        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Создать матрицу-столбец</summary><param name="data">Элементы столбца</param><returns>Матрица-столбец</returns>
        /// <exception cref="ArgumentNullException">Если массив <paramref name="data"/> не определён</exception>
        /// <exception cref="ArgumentException">Если массив <paramref name="data"/> имеет длину 0</exception>
        [NotNull] public static Matrix CreateCol([NotNull] params double[] data) => new Matrix(Array.CreateColArray(data));

        /// <summary>Создать матрицу-строку</summary><param name="data">Элементы строки</param><returns>Матрица-строка</returns>
        /// <exception cref="ArgumentNullException">Если массив <paramref name="data"/> не определён</exception>
        /// <exception cref="ArgumentException">Если массив <paramref name="data"/> имеет длину 0</exception>
        [NotNull] public static Matrix CreateRow([NotNull] params double[] data) => new Matrix(Array.CreateRowArray(data));

        /// <summary>Создать диагональную матрицу</summary><param name="elements">Элементы диагональной матрицы</param>
        /// <returns>Диагональная матрица</returns>
        [NotNull] public static Matrix CreateDiagonalMatrix([NotNull] params double[] elements) => new Matrix(Array.CreateDiagonal(elements));

        /// <summary>Операции над двумерными массивами</summary>
        public static partial class Array
        {
            /// <summary>Операторы над двумерными массивами</summary>
            public static partial class Operator { }
        }

        /// <summary>Получить единичную матрицу размерности NxN</summary>
        /// <param name="N">Размерность матрицы</param><returns>Единичная матрица размерности NxN с 1 на главной диагонали</returns>
        [DST]
        public static Matrix GetUnitaryMatryx(int N) => new Matrix(Array.GetUnitaryArrayMatrix(N));

        /// <summary>Трансвекция матрицы</summary><param name="A">Трансвецируемая матрица</param><param name="j">Оборный столбец</param>
        /// <returns>Трансвекция матрицы А</returns>                    
        public static Matrix GetTransvection(Matrix A, int j) => new Matrix(Array.GetTransvection(A._Data, j));

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Число строк матрицы</summary>
        private readonly int _N;

        /// <summary>Число столбцов матрицы</summary>
        private readonly int _M;

        /// <summary>Элементы матрицы</summary>
        [NotNull] private readonly double[,] _Data;

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Число строк матрицы</summary>
        public int N => _N;

        /// <summary>Число столбцов матрицы</summary>
        public int M => _M;

        /// <summary>Элемент матрицы</summary>
        /// <param name="i">Номер строки (элемента в столбце)</param>
        /// <param name="j">Номер столбца (элемента в строке)</param>
        /// <returns>Элемент матрицы</returns>
        public double this[int i, int j] { [DST] get => _Data[i, j]; [DST] set => _Data[i, j] = value; }

        /// <summary>Вектор-стольбец</summary><param name="j">Номер столбца</param><returns>Столбец матрицы</returns>
        [NotNull] public Matrix this[int j] => GetCol(j);

        /// <summary>Матрица является квадратной матрицей</summary>
        public bool IsSquare => _M == _N;

        /// <summary>Матрица является столбцом</summary>
        public bool IsCol => _M == 1;

        /// <summary>Матрица является строкой</summary>
        public bool IsRow => _N == 1;

        /// <summary>Матрица является числом</summary>
        public bool IsScalar => _N == 1 && _M == 1;

        /// <summary>Транспонированная матрица</summary>
        public Matrix T => GetTransponse();

        /// <summary>Максимум среди абсолютных сумм элементов строк</summary>
        public double Norm_m => Array.GetMaxRowAbsSumm(_Data);

        /// <summary>Максимум среди абсолютных сумм элементов столбцов</summary>
        public double Norm_l => Array.GetMaxColAbsSumm(_Data);

        /// <summary>Среднеквадратическое значение элементов матрицы</summary>
        public double Norm_k => Array.GetRMS(_Data);

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Матрица</summary><param name="N">Число строк</param><param name="M">Число столбцов</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="N"/> &lt; 0 || <paramref name="M"/> &lt; 0</exception>
        [DST]
        public Matrix(int N, int M)
        {
            if (N <= 0) throw new ArgumentOutOfRangeException(nameof(N), N, "N должна быть больше 0");
            if (M <= 0) throw new ArgumentOutOfRangeException(nameof(M), M, "M должна быть больше 0");
            Contract.EndContractBlock();

            _Data = new double[_N = N, _M = M];
        }

        /// <summary>Квадратная матрица</summary><param name="N">Размерность</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="N" /> &lt; 0</exception>
        [DST] public Matrix(int N) : this(N, N) { }

        /// <summary>Метод определения значения элемента матрицы</summary>
        /// <param name="i">Номер строки</param><param name="j">Номер столбца</param>
        /// <returns>Значение элемента матрицы M[<paramref name="i"/>, <paramref name="j"/>]</returns>
        public delegate double MatrixItemCreator(int i, int j);

        /// <summary>Квадратная матрица</summary>
        /// <param name="N">Размерность</param>
        /// <param name="CreateFunction">Порождающая функция</param>
        [DST] public Matrix(int N, [NotNull] MatrixItemCreator CreateFunction) : this(N, N, CreateFunction) { }

        /// <summary>Матрица</summary><param name="N">Число строк</param><param name="M">Число столбцов</param>
        /// <param name="CreateFunction">Порождающая функция</param>
        [DST]
        public Matrix(int N, int M, [NotNull] MatrixItemCreator CreateFunction) : this(N, M)
        {
            Contract.Requires(N > 0);
            Contract.Requires(M > 0);
            Contract.Requires(CreateFunction != null);
            for (var i = 0; i < N; i++) for (var j = 0; j < M; j++) _Data[i, j] = CreateFunction(i, j);
        }

        /// <summary>Инициализация новой матрицы по двумерному массиву её элементов</summary>
        /// <param name="Data">Двумерный массив элементов матрицы</param>
        /// <param name="clone">Создать копию данных</param>
        [DST]
        public Matrix([NotNull] double[,] Data, bool clone = false)
        {
            Contract.Requires(Data != null);
            _N = Data.GetLength(0);
            _M = Data.GetLength(1);
            _Data = clone ? Data.CloneObject() : Data;
        }

        /// <summary>Инициализация новой матрицы - столбца/строки</summary>
        /// <param name="DataCol">Элементы столбца матрицы</param>
        /// <param name="IsColumn">Создаётся матрица-столбец</param>
        [DST]
        public Matrix([NotNull] IList<double> DataCol, bool IsColumn = true) : this(IsColumn ? DataCol.Count : 1, IsColumn ? 1 : DataCol.Count)
        {
            Contract.Requires(DataCol != null);
            if (IsColumn) for (var i = 0; i < _N; i++) _Data[i, 0] = DataCol[i];
            else for (var j = 0; j < _M; j++) _Data[0, j] = DataCol[j];
        }

        /// <summary>Инициализация новой матрицы на основе перечисления строк (перечисления элементов строк) </summary>
        /// <param name="Items">Перечисление строк, состоящих из перечисления эламентов строк</param>
        public Matrix([NotNull] IEnumerable<IEnumerable<double>> Items) : this(GetElements(Items)) { }

        /// <summary>Получить двумерный массив элементов матрицы</summary>
        /// <param name="ColsItems">Перечисление элементов (по столбцам)</param>
        /// <returns>Двумерный массив элементов матрицы</returns>
        [DST, NotNull]
        private static double[,] GetElements([NotNull] IEnumerable<IEnumerable<double>> ColsItems)
        {
            Contract.Requires(ColsItems != null);
            var cols = ColsItems.Select(col => col.ToListFast()).ToList();
            var cols_count = cols.Count;
            var rows_count = cols.Max(col => col.Count);
            var data = new double[rows_count, cols_count];
            for (var j = 0; j < cols_count; j++)
            {
                var col = cols[j];
                for (var i = 0; i < col.Count && i < rows_count; i++) data[i, j] = col[i];
            }
            return data;
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Получить столбец матрицы</summary>
        /// <param name="j">Номер столбца</param>
        /// <returns>Столбец матрицы номер j</returns>
        [DST, NotNull] public Matrix GetCol(int j) => new Matrix(Array.GetCol(_Data, j));

        /// <summary>Получить строку матрицы</summary>
        /// <param name="i">Номер строки</param>
        /// <returns>Строка матрицы номер i</returns>
        [DST, NotNull] public Matrix GetRow(int i) => new Matrix(Array.GetRow(_Data, i));

        /// <summary>Приведение матрицы к ступенчатому виду методом гауса</summary>
        /// <param name="P">Матрица перестановок</param>
        /// <param name="rank">Ранг матрицы</param>
        /// <param name="D">Определитель</param>
        /// <returns>Триугольная матрица</returns>
        [NotNull]
        public Matrix GetTriangle([NotNull] out Matrix P, out int rank, out double D)
        {
            var result = new Matrix(Array.GetTriangle(_Data, out var p, out rank, out D));
            P = new Matrix(p);
            return result;
        }

        /// <summary>Приведение матрицы к ступенчатому виду методом гауса</summary>
        /// <param name="B">Присоединённая матрица правой части СЛАУ</param>
        /// <param name="CloneB">Работать с клоном матрицы <paramref name="B"/></param>
        /// <returns>Триугольная матрица</returns>
        /// <exception cref="ArgumentNullException">Если <paramref name="B"/> <see langword="null"/></exception>
        [NotNull]
        public Matrix GetTriangle([NotNull] ref Matrix B, bool CloneB = true)
        {
            var b = CloneB ? B._Data.CloneObject() : B._Data;
            var result = new Matrix(Array.GetTriangle(_Data, b, out var _, out var _));
            if (CloneB) B = new Matrix(b);
            return result;
        }

        /// <summary>Приведение матрицы к ступенчатому виду методом гауса</summary>
        /// <param name="B">Матрица правой части СЛАУ</param>
        /// <param name="P">Матрица перестановок</param>
        /// <param name="rank">Ранг матрицы</param>
        /// <param name="d">Определитель матрицы</param>
        /// <param name="CloneB">Клонировать матрицу правой части</param>
        /// <returns>Треугольная матрица</returns>
        [NotNull]
        public Matrix GetTriangle([NotNull] ref Matrix B, [NotNull] out Matrix P, out int rank, out double d, bool CloneB = true)
        {
            var b = B._Data;
            var result = new Matrix(Array.GetTriangle(_Data, ref b, out var p, out rank, out d, CloneB));
            P = new Matrix(p);
            if (CloneB) B = new Matrix(b);
            return result;
        }

        /// <summary>Получить обратную матрицу</summary>                                                     
        /// <param name="P">Матрица перестановок</param>
        /// <returns>Обратная матрица</returns>
        [NotNull]
        public Matrix GetInverse(out Matrix P)
        {
            var inverse = new Matrix(Array.Inverse(_Data, out var p));
            P = new Matrix(p);
            return inverse;
        }

        /// <summary>Транспонирование матрицы</summary>
        /// <returns>Транспонированная матрица</returns>
        [DST, NotNull] public Matrix GetTransponse() => new Matrix(Array.Transponse(_Data));

        /// <summary>Алгебраическое дополнение к элементу [n,m]</summary>
        /// <param name="n">Номер столбца</param>
        /// <param name="m">Номер строки</param>
        /// <returns>Алгебраическое дополнение к элементу [n,m]</returns>
        public double GetAdjunct(int n, int m) => Array.GetAdjunct(_Data, n, m);

        /// <summary>Минор матрицы по определённому элементу</summary>
        /// <param name="n">Номер столбца</param>
        /// <param name="m">Номер строки</param>
        /// <returns>Минор элемента матрицы [n,m]</returns>
        [NotNull] public Matrix GetMinor(int n, int m) => new Matrix(Array.GetMinor(_Data, n, m));

        /// <summary>Определитель матрицы</summary>
        public double GetDeterminant() => Array.GetDeterminant(_Data);

        /// <summary>Разложение матрицы на верхне-треугольную и нижне-треугольную</summary>
        /// <param name="L">Нижне-треугольная матрица</param>
        /// <param name="U">Верхнетреугольная матрица</param>
        /// <param name="P">Матрица преобразований P*X = L*U</param>
        /// <param name="D">Знак определителя</param>
        /// <returns>Истина, если разложение выполнено успешно, ложь - если матрица вырожденная</returns>
        public bool GetLUDecomposition([CanBeNull] out Matrix L, [CanBeNull] out Matrix U, [CanBeNull] out Matrix P, out double D)
        {
            if (!IsSquare) throw new InvalidOperationException("Невозможно осуществить LU-разложение неквадратной метрицы");

            var decomposition_success = Array.GetLUPDecomposition(_Data, out var l, out var u, out var p, out var d);
            L = decomposition_success ? new Matrix(l) : null;
            U = decomposition_success ? new Matrix(u) : null;
            P = decomposition_success ? new Matrix(p) : null;
            D = decomposition_success ? d : 0;
            return decomposition_success;
        }

        /// <summary>Получить внутренний массив элементов матрицы</summary>
        /// <returns></returns>
        [DST, NotNull] public double[,] GetData() => _Data;

        /* -------------------------------------------------------------------------------------------- */

        /// <inheritdoc/>
        [DST] public override string ToString() => $"Matrix[{_N}x{_M}]";

        /// <summary>Преобразование матрицы в строку с форматированием</summary>
        /// <param name="Format">Строка формата вывода чисел</param>
        /// <param name="Splitter">Разделитель элементов матрицы</param>
        /// <param name="provider">Механизм форматирования чисел матрицы</param>
        /// <returns>Строковое представление матрицы</returns>
        [DST, NotNull]
        public string ToStringFormat
        (
            [NotNull] string Format = "r",
            [CanBeNull] string Splitter = "\t",
            [CanBeNull] IFormatProvider provider = null
        ) => _Data.ToStringFormatView(Format, Splitter, provider) ?? throw new InvalidOperationException();

        /// <inheritdoc/>
        [DST] public string ToString([NotNull] string format, [CanBeNull] IFormatProvider provider) => _Data.ToStringFormatView(format, "\t", provider) ?? throw new InvalidOperationException();

        /* -------------------------------------------------------------------------------------------- */

        #region ICloneable Members

        /// <inheritdoc/>
        [DST] object ICloneable.Clone() => Clone();

        /// <inheritdoc/>
        [DST, NotNull] double[,] ICloneable<double[,]>.Clone() => _Data.CloneObject();

        /// <inheritdoc/>
        [DST, NotNull] public Matrix Clone() => new Matrix(_Data, true);

        #endregion

        /* -------------------------------------------------------------------------------------------- */

        [DST] public static bool operator ==([CanBeNull] Matrix A, [CanBeNull] Matrix B) => A is null && B is null || A is { } && B is { } && A.Equals(B);

        [DST] public static bool operator !=([CanBeNull] Matrix A, [CanBeNull] Matrix B) => !(A == B);

        [DST] public static bool operator ==([CanBeNull] double[,] A, [CanBeNull] Matrix B) => B == A;

        [DST] public static bool operator ==([CanBeNull] Matrix A, [CanBeNull] double[,] B) => A is null && B is null || A is { } && B is { } && A.Equals(B);

        [DST] public static bool operator !=([CanBeNull] double[,] A, [CanBeNull] Matrix B) => !(A == B);

        [DST] public static bool operator !=([CanBeNull] Matrix A, [CanBeNull] double[,] B) => !(A == B);

        [DST, NotNull] public static Matrix operator +([NotNull] Matrix M, double x) => new Matrix(Add(M._Data, x));

        [DST, NotNull] public static Matrix operator +(double x, [NotNull] Matrix M) => new Matrix(Add(M._Data, x));

        [DST, NotNull] public static Matrix operator -([NotNull] Matrix M, double x) => new Matrix(Substract(M._Data, x));

        [DST, NotNull] public static Matrix operator -([NotNull] Matrix M) => new Matrix(new double[M._N, M._M].Initialize(M._Data, (i, j, data) => -data[i, j]));

        [DST, NotNull] public static Matrix operator -(double x, [NotNull] Matrix M) => new Matrix(Substract(x, M._Data));

        [DST, NotNull] public static Matrix operator *([NotNull] Matrix M, double x) => new Matrix(Multiply(M._Data, x));

        [DST, NotNull] public static Matrix operator *(double x, [NotNull] Matrix M) => new Matrix(Multiply(M._Data, x));

        [DST, NotNull] public static Matrix operator *([NotNull] double[,] A, [NotNull] Matrix B) => new Matrix(Multiply(A, B._Data));

        [DST, NotNull] public static Matrix operator *([NotNull] double[] A, [NotNull] Matrix B) => new Matrix(Multiply(Array.CreateColArray(A), B._Data));

        [DST, NotNull] public static Matrix operator *([NotNull] Matrix A, [NotNull] double[] B) => new Matrix(Multiply(A._Data, Array.CreateColArray(B)));

        [DST, NotNull] public static Matrix operator *([NotNull] Matrix A, [NotNull] double[,] B) => new Matrix(Multiply(A._Data, B));

        [DST, NotNull] public static Matrix operator /([NotNull] Matrix M, double x) => new Matrix(Divade(M._Data, x));

        [DST, NotNull] public static Matrix operator /(double x, [NotNull] Matrix M) => new Matrix(Divade(x, M._Data));

        [DST, NotNull]
        public static Matrix operator ^([NotNull] Matrix M, int n)
        {
            if (!M.IsSquare) throw new ArgumentException("Матрица не квадратная", nameof(M));
            switch (n)
            {
                case 1: return M.Clone();
                case -1: return M.GetInverse(out _);
                default:
                    var m = M._Data;
                    if (n < 0)
                    {
                        m = Array.Inverse(m, out _);
                        n = -n;
                    }
                    var result = Array.GetUnitaryArrayMatrix(M._N);
                    for (var i = 0; i < n; i++) result = Multiply(result, m);
                    return new Matrix(result);
            }
        }

        /// <summary>Оператор сложения двух матриц</summary>
        /// <param name="A">Первое слогаемое</param><param name="B">Второе слогаемое</param><returns>Сумма двух матриц</returns>
        [DST, NotNull] public static Matrix operator +([NotNull] Matrix A, [NotNull] Matrix B) => new Matrix(Add(A._Data, B._Data));

        /// <summary>Оператор разности двух матриц</summary>
        /// <param name="A">Уменьшаемое</param><param name="B">Вычитаемое</param><returns>Разность двух матриц</returns>
        [DST, NotNull] public static Matrix operator -([NotNull] Matrix A, [NotNull] Matrix B) => new Matrix(Substract(A._Data, B._Data));

        /// <summary>Оператор произведения двух матриц</summary>
        /// <param name="A">Первый сомножитель</param><param name="B">Второй сомножитель</param><returns>Произведение двух матриц</returns>
        [DST, NotNull] public static Matrix operator *([NotNull] Matrix A, [NotNull] Matrix B) => new Matrix(Multiply(A._Data, B._Data));

        /// <summary>Оператор деления двух матриц</summary>
        /// <param name="A">Делимое</param><param name="B">Делитель</param><returns>Частное двух матриц</returns>
        [DST, NotNull] public static Matrix operator /([NotNull] Matrix A, [NotNull] Matrix B) => new Matrix(Divade(A._Data, B._Data));

        /// <summary>Конкатинация двух матриц (либо по строкам, либо по столбцам)</summary>
        /// <param name="A">Первое слогаемое</param><param name="B">Второе слогаемое</param><returns>Объединённая матрица</returns>
        [DST, NotNull] public static Matrix operator |([NotNull] Matrix A, [NotNull] Matrix B) => new Matrix(Concatinate(A._Data, B._Data));

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Оператор неявного преведения типа вещественного числа двойной точнойсти к типу Матрица порядка 1х1</summary>
        /// <param name="X">Приводимое число</param><returns>Матрица порадка 1х1</returns>
        [DST, NotNull] public static implicit operator Matrix(double X) => new Matrix(1, 1) { [0, 0] = X };

        [DST, NotNull] public static explicit operator double[,] ([NotNull] Matrix M) => M._Data;

        [DST, NotNull] public static explicit operator Matrix([NotNull] double[,] Data) => new Matrix(Data);

        [DST, NotNull] public static explicit operator Matrix([NotNull] double[] Data) => new Matrix(Data);

        /* -------------------------------------------------------------------------------------------- */

        #region IEquatable Members

        /// <inheritdoc/>
        [DST] public bool Equals(double[,] other) => other is { } && Array.AreEquals(_Data, other);

        /// <inheritdoc/>
        [DST] public bool Equals(Matrix other) => other is { } && (ReferenceEquals(this, other) || Array.AreEquals(_Data, other._Data));

        #endregion

        /// <inheritdoc/>
        [DST] public override bool Equals(object obj) => obj is { } && (ReferenceEquals(this, obj) || Equals(obj as Matrix) || Equals(obj as double[,]));

        /// <inheritdoc/>
        [DST]
        public override int GetHashCode()
        {
            unchecked
            {
                var result = (_N * 397) ^ _M;
                for (var i = 0; i < _N; i++)
                    for (var j = 0; j < _M; j++)
                        result = (result * 397) ^ i ^ j ^ _Data[i, j].GetHashCode();
                return result;
            }
        }

        /* -------------------------------------------------------------------------------------------- */
    }
}