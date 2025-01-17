﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Radzen.Blazor
{
    /// <summary>
    /// Class RadzenSlider.
    /// Implements the <see cref="Radzen.FormComponent{TValue}" />
    /// </summary>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <seealso cref="Radzen.FormComponent{TValue}" />
    public partial class RadzenSlider<TValue> : FormComponent<TValue>
    {
        /// <summary>
        /// The handle
        /// </summary>
        ElementReference handle;
        /// <summary>
        /// The minimum handle
        /// </summary>
        ElementReference minHandle;
        /// <summary>
        /// The maximum handle
        /// </summary>
        ElementReference maxHandle;

        /// <summary>
        /// The visible changed
        /// </summary>
        private bool visibleChanged = false;
        /// <summary>
        /// The disabled changed
        /// </summary>
        private bool disabledChanged = false;
        /// <summary>
        /// The maximum changed
        /// </summary>
        private bool maxChanged = false;
        /// <summary>
        /// The minimum changed
        /// </summary>
        private bool minChanged = false;
        /// <summary>
        /// The range changed
        /// </summary>
        private bool rangeChanged = false;
        /// <summary>
        /// The step changed
        /// </summary>
        private bool stepChanged = false;
        /// <summary>
        /// The first render
        /// </summary>
        private bool firstRender = true;

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>The left.</value>
        decimal Left => ((MinValue() - Min) * 100) / (Max - Min);
        /// <summary>
        /// Gets the second left.
        /// </summary>
        /// <value>The second left.</value>
        decimal SecondLeft => ((MaxValue() - Min) * 100) / (Max - Min);

        /// <summary>
        /// Set parameters as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            visibleChanged = parameters.DidParameterChange(nameof(Visible), Visible);
            disabledChanged = parameters.DidParameterChange(nameof(Disabled), Disabled);
            maxChanged = parameters.DidParameterChange(nameof(Max), Max);
            minChanged = parameters.DidParameterChange(nameof(Min), Min);
            rangeChanged = parameters.DidParameterChange(nameof(Range), Range);
            stepChanged = parameters.DidParameterChange(nameof(Step), Step);

            await base.SetParametersAsync(parameters);

            if ((visibleChanged || disabledChanged) && !firstRender)
            {
                if (Visible == false || Disabled == true)
                {
                    Dispose();
                }
            }
        }

        /// <summary>
        /// On after render as an asynchronous operation.
        /// </summary>
        /// <param name="firstRender">if set to <c>true</c> [first render].</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            this.firstRender = firstRender;

            if (firstRender || visibleChanged || disabledChanged || maxChanged || minChanged || rangeChanged || stepChanged)
            {
                visibleChanged = false;
                disabledChanged = false;

                if (maxChanged)
                {
                    maxChanged = false;
                }

                if (minChanged)
                {
                    minChanged = false;
                }

                if (rangeChanged)
                {
                    rangeChanged = false;
                }

                if (stepChanged)
                {
                    stepChanged = false;
                }

                if (Visible && !Disabled)
                {
                    await JSRuntime.InvokeVoidAsync("Radzen.createSlider", UniqueID, Reference, Element, Range, Range ? minHandle : handle, maxHandle, Min, Max, Value, Step);

                    StateHasChanged();
                }
            }
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (IsJSRuntimeAvailable)
            {
                JSRuntime.InvokeVoidAsync("Radzen.destroySlider", UniqueID, Element);
            }
        }

        /// <summary>
        /// Called when [value change].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="isMin">if set to <c>true</c> [is minimum].</param>
        [JSInvokable("RadzenSlider.OnValueChange")]
        public async System.Threading.Tasks.Task OnValueChange(decimal value, bool isMin)
        {
            var step = string.IsNullOrEmpty(Step) || Step == "any" ? 1 : decimal.Parse(Step.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

            var newValue = Math.Round(value / step) * step;

            if (Range)
            {
                var oldMinValue = ((IEnumerable)Value).OfType<object>().FirstOrDefault();
                var oldMaxValue = ((IEnumerable)Value).OfType<object>().LastOrDefault();

                var type = typeof(TValue).IsGenericType ? typeof(TValue).GetGenericArguments()[0] : typeof(TValue);
                var convertedNewValue = ConvertType.ChangeType(newValue, type);

                var newValueAsDecimal = (decimal)ConvertType.ChangeType(newValue, typeof(decimal));
                var oldMaxValueAsDecimal = (decimal)ConvertType.ChangeType(oldMaxValue, typeof(decimal));
                var oldMinValueAsDecimal = (decimal)ConvertType.ChangeType(oldMinValue, typeof(decimal));

                var values = Enumerable.Range(0, 2).Select(i =>
                {
                    if (i == 0)
                    {
                        return isMin &&
                            !object.Equals(oldMinValue, convertedNewValue) &&
                            newValueAsDecimal >= Min && newValueAsDecimal <= Max &&
                            newValueAsDecimal < oldMaxValueAsDecimal
                                ? convertedNewValue : oldMinValue;
                    }
                    else
                    {
                        return !isMin &&
                            !object.Equals(oldMaxValue, convertedNewValue) &&
                            newValueAsDecimal >= Min && newValueAsDecimal <= Max &&
                            newValueAsDecimal > oldMinValueAsDecimal
                                ? convertedNewValue : oldMaxValue;
                    }
                }).AsQueryable().Cast(type);

                if (!object.Equals(Value, values))
                {
                    Value = (TValue)values;

                    await ValueChanged.InvokeAsync(Value);

                    if (FieldIdentifier.FieldName != null)
                    {
                        EditContext?.NotifyFieldChanged(FieldIdentifier);
                    }

                    await Change.InvokeAsync(Value);

                    StateHasChanged();
                }
            }
            else
            {
                var valueAsDecimal = Value == null ? 0 : (decimal)ConvertType.ChangeType(Value, typeof(decimal));

                if (!object.Equals(valueAsDecimal, newValue) && newValue >= Min && newValue <= Max)
                {
                    Value = (TValue)ConvertType.ChangeType(newValue, typeof(TValue));

                    await ValueChanged.InvokeAsync(Value);

                    if (FieldIdentifier.FieldName != null)
                    {
                        EditContext?.NotifyFieldChanged(FieldIdentifier);
                    }

                    await Change.InvokeAsync(Value);

                    StateHasChanged();
                }
            }
        }

        /// <summary>
        /// Gets the component CSS class.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string GetComponentCssClass()
        {
            return $"rz-slider {(Disabled ? "rz-state-disabled " : "")}rz-slider-horizontal";
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Parameter]
        public override TValue Value
        {
            get
            {
                if (_value == null)
                {
                    if (Range)
                    {
                        var type = typeof(TValue).IsGenericType ? typeof(TValue).GetGenericArguments()[0] : typeof(TValue);

                        _value = (TValue)Enumerable.Range(0, 2).Select(i =>
                        {
                            if (i == 0)
                            {
                                return ConvertType.ChangeType(Min, type);
                            }
                            else
                            {
                                return ConvertType.ChangeType(Max, type);
                            }
                        }).AsQueryable().Cast(type);
                    }
                    else
                    {
                        _value = (TValue)ConvertType.ChangeType(Min, typeof(TValue));
                    }
                }

                return _value;
            }
            set
            {
                if (!EqualityComparer<TValue>.Default.Equals(value, _value))
                {
                    _value = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has value.
        /// </summary>
        /// <value><c>true</c> if this instance has value; otherwise, <c>false</c>.</value>
        public override bool HasValue
        {
            get
            {
                return Value != null;
            }
        }

        /// <summary>
        /// Minimums the value.
        /// </summary>
        /// <returns>System.Decimal.</returns>
        decimal MinValue()
        {
            if (Range)
            {
                var values = Value as IEnumerable;
                if (values != null && values.OfType<object>().Any())
                {
                    var v = values.OfType<object>().FirstOrDefault();
                    return (decimal)Convert.ChangeType(v != null ? v : Min, typeof(decimal));
                }
            }

            return HasValue ? (decimal)Convert.ChangeType(Value, typeof(decimal)) : Min;
        }

        /// <summary>
        /// Maximums the value.
        /// </summary>
        /// <returns>System.Decimal.</returns>
        decimal MaxValue()
        {
            if (Range)
            {
                var values = Value as IEnumerable;
                if (values != null && values.OfType<object>().Any())
                {
                    var v = values.OfType<object>().LastOrDefault();
                    return (decimal)Convert.ChangeType(v != null ? v : Max, typeof(decimal));
                }
            }

            return HasValue ? (decimal)Convert.ChangeType(Value, typeof(decimal)) : Min;
        }

        /// <summary>
        /// Gets or sets the step.
        /// </summary>
        /// <value>The step.</value>
        [Parameter]
        public string Step { get; set; } = "1";

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenSlider{TValue}"/> is range.
        /// </summary>
        /// <value><c>true</c> if range; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool Range { get; set; } = false;

        /// <summary>
        /// Determines the minimum of the parameters.
        /// </summary>
        /// <value>The minimum.</value>
        [Parameter]
        public decimal Min { get; set; } = 0;

        /// <summary>
        /// Determines the maximum of the parameters.
        /// </summary>
        /// <value>The maximum.</value>
        [Parameter]
        public decimal Max { get; set; } = 100;
    }
}