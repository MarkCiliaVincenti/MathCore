﻿using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Класс методов расширений для обработчиков событий</summary>
    public static class EventHandlerExtension
    {
        public static async Task InvokeAsync([CanBeNull] this EventHandler handler, object sender, EventArgs e)
        {
            if (handler is null) return;
            await Task.Factory.FromAsync(handler.BeginInvoke, handler.EndInvoke, sender, e, null);
        }

        public static async Task InvokeAsync<TEventArgs>([CanBeNull] this EventHandler<TEventArgs> handler, object sender, TEventArgs e)
            where TEventArgs : EventArgs
        {
            if (handler is null) return;
            await Task.Factory.FromAsync(handler.BeginInvoke, handler.EndInvoke, sender, e, null);
        }

        /// <summary>Потоко-безопасная генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргумент события</param>
        [DST]
        public static void Start([CanBeNull] this NotifyCollectionChangedEventHandler Handler, object Sender, NotifyCollectionChangedEventArgs e)
        {
            if (Handler is null) return;
            var invocations = Handler.GetInvocationList();
            foreach (var d in invocations)
                switch (d.Target)
                {
                    case ISynchronizeInvoke synchronize_invoke when synchronize_invoke.InvokeRequired:
                        synchronize_invoke.Invoke(d, new[] { Sender, e });
                        break;
                    default:
                        d.DynamicInvoke(Sender, e);
                        break;
                }
        }

        /// <summary>Потоко-безопасная генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="PropertyName">Имя изменившегося свойства</param>
        public static void Start(this PropertyChangedEventHandler Handler, object Sender,
            [CallerMemberName, CanBeNull]  string PropertyName = null)
            => Handler.Start(Sender, new PropertyChangedEventArgs(PropertyName));

        /// <summary>Потоко-безопасная генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="PropertyName">Имена изменившихся свойств</param>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static void Start([CanBeNull] this PropertyChangedEventHandler Handler, object Sender, [NotNull] params string[] PropertyName)
        {
            if (PropertyName is null) throw new ArgumentNullException(nameof(PropertyName));
            if (Handler is null || PropertyName.Length == 0) return;
            var args = PropertyName.ToArray(name => new PropertyChangedEventArgs(name));
            foreach (var d in Handler.GetInvocationList())
                switch (d.Target)
                {
                    case ISynchronizeInvoke synchronize_invoke when synchronize_invoke.InvokeRequired:
                        foreach (var arg in args)
                            synchronize_invoke.Invoke(d, new[] { Sender, arg });
                        break;
                    default:
                        foreach (var arg in args)
                            d.DynamicInvoke(Sender, arg);
                        break;
                }
        }

        /// <summary>Потоко-безопасная генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргумент события</param>
        [DST]
        public static void Start([CanBeNull] this PropertyChangedEventHandler Handler, object Sender, PropertyChangedEventArgs e)
        {
            if (Handler is null) return;
            foreach (var d in Handler.GetInvocationList())
                switch (d.Target)
                {
                    case ISynchronizeInvoke synchronize_invoke when synchronize_invoke.InvokeRequired:
                        synchronize_invoke.Invoke(d, new[] { Sender, e });
                        break;
                    default:
                        d.DynamicInvoke(Sender, e);
                        break;
                }
        }

        /// <summary>Потоко-безопасная генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргумент события</param>
        [DST]
        public static void Start([CanBeNull] this EventHandler Handler, object Sender, EventArgs e)
        {
            if (Handler is null) return;
            var invocations = Handler.GetInvocationList();
            foreach (var d in invocations)
                switch (d.Target)
                {
                    case ISynchronizeInvoke synchronize_invoke when synchronize_invoke.InvokeRequired:
                        synchronize_invoke.Invoke(d, new[] { Sender, e });
                        break;
                    default:
                        d.DynamicInvoke(Sender, e);
                        break;
                }
        }

        /// <summary>Потоко-безопасная асинхронная генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргумент события</param>
        /// <param name="CallBack">Метод завершения генерации события</param>
        /// <param name="State">Объект-состояние, передаваемый в метод завершения генерации события</param>
        [DST, CanBeNull]
        public static IAsyncResult StartAsync([CanBeNull] this EventHandler Handler, object Sender, EventArgs e, [CanBeNull] AsyncCallback CallBack = null, [CanBeNull] object State = null) =>
            Handler is null ? null : ((Action)(() => Handler.Invoke(Sender, e))).BeginInvoke(CallBack, State);

        /// <summary>Быстрая генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        [DST]
        public static void FastStart(this EventHandler Handler, object Sender) => Handler.FastStart(Sender, EventArgs.Empty);

        /// <summary>Быстрая генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        [DST]
        public static void FastStart([CanBeNull] this EventHandler Handler, object Sender, EventArgs e) => Handler?.Invoke(Sender, e);

        /// <summary>Быстрая генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <typeparam name="TEventArgs">Тип аргумента события</typeparam>
        /// <param name="e">Аргументы события</param>
        [DST]
        public static void FastStart<TEventArgs>([CanBeNull] this EventHandler<TEventArgs> Handler, object Sender, TEventArgs e) where TEventArgs : EventArgs
            => Handler?.Invoke(Sender, e);

        /// <summary>Потоко-безопасная генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <typeparam name="TEventArgs">Тип аргумента события</typeparam>
        /// <param name="e">Аргументы события</param>
        [DST]
        public static void Start<TEventArgs>([CanBeNull] this EventHandler<TEventArgs> Handler, object Sender, TEventArgs e)
            where TEventArgs : EventArgs
        {
            if (Handler is null) return;
            var invocations = Handler.GetInvocationList();
            foreach (var d in invocations)
                switch (d.Target)
                {
                    case ISynchronizeInvoke synchronize_invoke when synchronize_invoke.InvokeRequired:
                        synchronize_invoke.Invoke(d, new[] { Sender, e });
                        break;
                    default:
                        d.DynamicInvoke(Sender, e);
                        break;
                }
        }

        /// <summary>Потоко-безопасная асинхронная генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <typeparam name="TEventArgs">Тип аргумента события</typeparam>
        /// <param name="e">Аргументы события</param>
        /// <param name="CallBack">Метод завершения генерации события</param>
        /// <param name="State">Объект-состояние, передаваемый в метод завершения генерации события</param>
        [DST, CanBeNull]
        public static IAsyncResult StartAsync<TEventArgs>(
            [CanBeNull] this EventHandler<TEventArgs> Handler,
            object Sender,
            TEventArgs e,
            [CanBeNull] AsyncCallback CallBack = null,
            [CanBeNull] object State = null)
            where TEventArgs : EventArgs =>
            Handler is null
                ? null
                : ((Action)(() =>
               {
                   var invocation_list = Handler.GetInvocationList();
                   foreach (var action in invocation_list)
                       action.DynamicInvoke(Sender, e);
               })).BeginInvoke(CallBack, State);

        /// <summary>Потоко-безопасная генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <typeparam name="TArgs">Тип аргумента события</typeparam>
        /// <param name="Args">Аргументы события</param>
        /// <typeparam name="TResult">Тип результата обработки события</typeparam>
        /// <typeparam name="TSender">Тип источника события</typeparam>
        /// <returns>Массив результатов обработки события</returns>
        [DST, NotNull]
        public static TResult[] Start<TResult, TSender, TArgs>([CanBeNull] this EventHandler<TResult, TSender, TArgs> Handler, TSender Sender, TArgs Args) =>
            Handler is null
                ? Array.Empty<TResult>()
                : Handler
                   .GetInvocationList()
                   .Select(d => (TResult)(d.Target is ISynchronizeInvoke invoke && invoke.InvokeRequired
                        ? invoke.Invoke(d, new object[] { Sender, Args })
                        : d.DynamicInvoke(Sender, Args))).ToArray();
    }
}