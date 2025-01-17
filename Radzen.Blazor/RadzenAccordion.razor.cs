﻿using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Radzen.Blazor
{
    /// <summary>
    /// Class RadzenAccordion.
    /// Implements the <see cref="Radzen.RadzenComponent" />
    /// </summary>
    /// <seealso cref="Radzen.RadzenComponent" />
    public partial class RadzenAccordion : RadzenComponent
    {
        /// <summary>
        /// Gets the component CSS class.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string GetComponentCssClass()
        {
            return "rz-accordion";
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenAccordion"/> is multiple.
        /// </summary>
        /// <value><c>true</c> if multiple; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool Multiple { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected.
        /// </summary>
        /// <value>The index of the selected.</value>
        [Parameter]
        public int SelectedIndex { get; set; } = 0;

        /// <summary>
        /// Gets or sets the expand.
        /// </summary>
        /// <value>The expand.</value>
        [Parameter]
        public EventCallback<int> Expand { get; set; }

        /// <summary>
        /// Gets or sets the collapse.
        /// </summary>
        /// <value>The collapse.</value>
        [Parameter]
        public EventCallback<int> Collapse { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        [Parameter]
        public RenderFragment Items { get; set; }

        /// <summary>
        /// The items
        /// </summary>
        List<RadzenAccordionItem> items = new List<RadzenAccordionItem>();

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddItem(RadzenAccordionItem item)
        {
            if (items.IndexOf(item) == -1)
            {
                if (item.Selected)
                {
                    SelectedIndex = items.Count;
                    if (!Multiple)
                    {
                        expandedIdexes.Clear();
                    }

                    if (!expandedIdexes.Contains(SelectedIndex))
                    {
                        expandedIdexes.Add(SelectedIndex);
                    }
                }

                items.Add(item);
                StateHasChanged();
            }
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void RemoveItem(RadzenAccordionItem item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
                try { InvokeAsync(StateHasChanged); } catch { }
            }
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        internal void Refresh()
        {
            StateHasChanged();
        }

        /// <summary>
        /// Determines whether the specified index is selected.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the specified index is selected; otherwise, <c>false</c>.</returns>
        protected bool IsSelected(int index, RadzenAccordionItem item)
        {
            return expandedIdexes.Contains(index);
        }

        /// <summary>
        /// The expanded idexes
        /// </summary>
        List<int> expandedIdexes = new List<int>();

        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="item">The item.</param>
        internal async System.Threading.Tasks.Task SelectItem(RadzenAccordionItem item)
        {
            await CollapseAll(item);

            var itemIndex = items.IndexOf(item);
            if (!expandedIdexes.Contains(itemIndex))
            {
                expandedIdexes.Add(itemIndex);
                await Expand.InvokeAsync(itemIndex);
            }
            else
            {
                expandedIdexes.Remove(itemIndex);
                await Collapse.InvokeAsync(itemIndex);
            }

            if (!Multiple)
            {
                SelectedIndex = itemIndex;
            }

            StateHasChanged();
        }

        /// <summary>
        /// Collapses all.
        /// </summary>
        /// <param name="item">The item.</param>
        async System.Threading.Tasks.Task CollapseAll(RadzenAccordionItem item)
        {
            if (!Multiple && items.Count > 1)
            {
                foreach (var i in items.Where(i => i != item))
                {
                    var itemIndex = items.IndexOf(i);
                    if (expandedIdexes.Contains(itemIndex))
                    {
                        expandedIdexes.Remove(itemIndex);
                        await Collapse.InvokeAsync(items.IndexOf(i));
                    }
                }
            }
        }
    }
}