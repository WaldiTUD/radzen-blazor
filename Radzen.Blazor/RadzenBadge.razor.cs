﻿using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Radzen.Blazor
{
    /// <summary>
    /// Class RadzenBadge.
    /// Implements the <see cref="Radzen.RadzenComponent" />
    /// </summary>
    /// <seealso cref="Radzen.RadzenComponent" />
    public partial class RadzenBadge : RadzenComponent
    {
        /// <summary>
        /// Gets the component CSS class.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string GetComponentCssClass()
        {
            var classList = new List<string>();

            classList.Add("rz-badge");
            classList.Add($"rz-badge-{BadgeStyle.ToString().ToLower()}");

            if (IsPill)
            {
                classList.Add("rz-badge-pill");
            }

            return string.Join(" ", classList);
        }

        /// <summary>
        /// Gets or sets the content of the child.
        /// </summary>
        /// <value>The content of the child.</value>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [Parameter]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the badge style.
        /// </summary>
        /// <value>The badge style.</value>
        [Parameter]
        public BadgeStyle BadgeStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pill.
        /// </summary>
        /// <value><c>true</c> if this instance is pill; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool IsPill { get; set; }
    }
}
