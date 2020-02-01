﻿using System.ComponentModel;
using MathCore.Annotations;

// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    /// <summary>Объект, осуществляющий контроль метода объекта <typeparamref name="TObject"/></summary>
    /// <typeparam name="TObject">Тип объекта, метод которого требуется контролировать</typeparam>
    /// <typeparam name="TResult">Тип значения метода</typeparam>
    public class Method<TObject, TResult>
    {
        private MethodInfo _MethodInfo;
        private string _Name;
        private TObject _Object;
        private bool _Private;

        private Func<object[], TResult> _Method;

        /// <summary>Имя контролируемого метода</summary>
        public string Name { get => _Name; set => Initialize(_Object, _Name = value, _Private); }

        /// <summary>Объект, метод которого контролируется</summary>
        public TObject Object { get => _Object; set => Initialize(_Object = value, _Name, _Private); }

        /// <summary>Метод не является публичным?</summary>
        public bool Private { get => _Private; set => Initialize(_Object, _Name, _Private = value); }

        /// <summary>Инициализация нового экземпляра <see cref="Method{TObject,TResult}"/></summary>
        /// <param name="o">Объект, метод которого контролируется</param>
        /// <param name="Name">Имя контролируемого метода</param>
        /// <param name="Private">Метод не является публичным?</param>
        public Method(TObject o, string Name, bool Private = false) => Initialize(_Object = o, _Name = Name, _Private = Private);

        private void Initialize([CanBeNull] TObject obj, [NotNull] string MethodName, bool IsPrivate)
        {
            var IsPublic = IsPrivate ? BindingFlags.NonPublic : BindingFlags.Public;
            var IsStatic = obj is null ? BindingFlags.Static : BindingFlags.Instance;

            var type = typeof(TObject);
            if(type == typeof(object) && obj != null)
                type = obj.GetType();

            _MethodInfo = type.GetMethod(MethodName, IsStatic | IsPublic);

            _Method = obj != null && obj is ISynchronizeInvoke
                ? (Func<object[], TResult>)(Args => (TResult)((ISynchronizeInvoke)obj).Invoke((Func<object[], TResult>)PrivateInvoke, new object[] { Args }))
                : PrivateInvoke;
        }

        /// <summary>Вызвать метод</summary>
        /// <param name="Args">Набор параметров, передаваемый методу</param>
        /// <returns>Результат вызова метода</returns>
        public TResult Invoke(params object[] Args) => _Method(Args);

        private TResult PrivateInvoke(params object[] Args) => (TResult)_MethodInfo.Invoke(_Object, Args);
    }
}