﻿using Microsoft.AspNetCore.Components;

namespace Radzen.Blazor
{
    /// <summary>
    /// Class RadzenRadioButtonListItem.
    /// Implements the <see cref="Radzen.RadzenComponent" />
    /// </summary>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <seealso cref="Radzen.RadzenComponent" />
    public class RadzenRadioButtonListItem<TValue> : RadzenComponent
    {
        /// <summary>
        /// The text
        /// </summary>
        private string _text;
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [Parameter]
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value != _text)
                {
                    _text = value;

                    if (List != null)
                        List.Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Parameter]
        public TValue Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenRadioButtonListItem{TValue}"/> is disabled.
        /// </summary>
        /// <value><c>true</c> if disabled; otherwise, <c>false</c>.</value>
        [Parameter]
        public virtual bool Disabled { get; set; }

        /// <summary>
        /// The list
        /// </summary>
        RadzenRadioButtonList<TValue> _list;

        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>The list.</value>
        [CascadingParameter]
        public RadzenRadioButtonList<TValue> List
        {
            get
            {
                return _list;
            }
            set
            {
                if (_list != value)
                {
                    _list = value;
                    _list.AddItem(this);
                }
            }
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            List?.RemoveItem(this);
        }

        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="value">The value.</param>
        internal void SetText(string value)
        {
            Text = value;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        internal void SetValue(TValue value)
        {
            Value = value;
        }

        /// <summary>
        /// Sets the disabled.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal void SetDisabled(bool value)
        {
            Disabled = value;
        }

        /// <summary>
        /// Sets the visible.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal void SetVisible(bool value)
        {
            Visible = value;
        }
    }
}