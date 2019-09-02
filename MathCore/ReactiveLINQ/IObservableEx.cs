﻿using System.Diagnostics.Contracts;

namespace System.Linq.Reactive
{
    /// <summary>Обозреваемый объект</summary>
    /// <typeparam name="T">Тип объектов последовательности событий</typeparam>
    [ContractClass(typeof(ObservableExContract<>))]
    public interface IObservableEx<T> : IObservable<T>
    {
        /// <summary>Метод получения наблюдателя</summary>
        /// <param name="observer">Наблюдатель объекта</param>
        /// <returns>Объект, реализующий возможность разрушшения связи с наблюдаемым объектом</returns>
        IDisposable Subscribe(IObserverEx<T> observer);
    }
}