﻿using System;

using NN = MathCore.Annotations.NotNullAttribute;
using CN = MathCore.Annotations.CanBeNullAttribute;
using INN = MathCore.Annotations.ItemNotNullAttribute;
using ICN = MathCore.Annotations.ItemCanBeNullAttribute;

namespace MathCore.Monades.WorkFlow
{
    /// <summary>Работа, выполняющая действие</summary>
    public class ActionWork : Work
    {
        /// <summary>Действие, выполняемое работой</summary>
        private readonly Action _WorkAction;

        /// <summary>Инициализация нового работы на основе действия</summary>
        /// <param name="WorkAction">Действие, выполняемое в рамках работы</param>
        /// <param name="BaseWork">Базовая работа</param>
        internal ActionWork([NN] Action WorkAction, Work BaseWork = null) : base(BaseWork) => _WorkAction = WorkAction;

        /// <inheritdoc />
        protected override IWorkResult Execute(IWorkResult BaseResult)
        {
            try
            {
                _WorkAction();
                return new WorkResult(BaseResult?.Error);
            }
            catch (Exception error)
            {
                return new WorkResult(error, BaseResult?.Error);
            }
        }
    }
}