﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Radzen.Blazor
{
    /// <summary>
    /// Class RadzenSplitter.
    /// Implements the <see cref="Radzen.RadzenComponent" />
    /// </summary>
    /// <seealso cref="Radzen.RadzenComponent" />
    public partial class RadzenSplitter : RadzenComponent
    {
        /// <summary>
        /// The sizeautopanes
        /// </summary>
        private int _sizeautopanes = 0;

        /// <summary>
        /// Gets or sets the content of the child.
        /// </summary>
        /// <value>The content of the child.</value>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        [Parameter]
        public Orientation Orientation { get; set; } = Orientation.Horizontal;

        /// <summary>
        /// The panes
        /// </summary>
        internal List<RadzenSplitterPane> Panes = new List<RadzenSplitterPane>();

        /// <summary>
        /// Adds the pane.
        /// </summary>
        /// <param name="pane">The pane.</param>
        public void AddPane(RadzenSplitterPane pane)
        {
            if (Panes.IndexOf(pane) != -1 || !pane.Visible)
                return;

            if (string.IsNullOrWhiteSpace(pane.Size))
            {
                //no size defined
                pane.SizeAuto = true;
                _sizeautopanes++;
            }

            pane.Index = Panes.Count;
            Panes.Add(pane);

            foreach (var iPane in Panes)
            {
                if (!iPane.SizeAuto)
                    continue;

                iPane.SizeRuntine = (100 / _sizeautopanes) + "%";
            }
        }

        /// <summary>
        /// Removes the pane.
        /// </summary>
        /// <param name="pane">The pane.</param>
        public void RemovePane(RadzenSplitterPane pane)
        {
            if (Panes.Contains(pane))
            {
                Panes.Remove(pane);
                try
                {
                    InvokeAsync(StateHasChanged);
                }
                catch
                {
                }
            }
        }


        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh()
        {
            try
            {
                InvokeAsync(StateHasChanged);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Resizes the execute.
        /// </summary>
        /// <param name="args">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="paneIndex">Index of the pane.</param>
        /// <returns>Task.</returns>
        internal Task ResizeExec(MouseEventArgs args, int paneIndex)
        {
            var pane = Panes[paneIndex];
            if (!pane.Resizable)
                return Task.CompletedTask;

            var paneNextResizable = Panes.Skip(paneIndex + 1).FirstOrDefault(o => o.Resizable && !o.Collapsed);


            return JSRuntime.InvokeVoidAsync("Radzen.startSplitterResize",
                UniqueID,
                Reference,
                pane.UniqueID,
                paneNextResizable?.UniqueID,
                Orientation.ToString(), Orientation == Orientation.Horizontal ? args.ClientX : args.ClientY,
                pane.Min,
                pane.Max,
                paneNextResizable?.Min,
                paneNextResizable?.Max).AsTask();
        }

        /// <summary>
        /// Called when [pane resized].
        /// </summary>
        /// <param name="paneIndex">Index of the pane.</param>
        /// <param name="sizeNew">The size new.</param>
        /// <param name="paneNextIndex">Index of the pane next.</param>
        /// <param name="sizeNextNew">The size next new.</param>
        [JSInvokable("RadzenSplitter.OnPaneResized")]
        public async Task OnPaneResized(int paneIndex, double sizeNew, int? paneNextIndex, double? sizeNextNew)
        {
            var pane = Panes[paneIndex];

            if (Resize.HasDelegate)
            {
                var arg = new RadzenSplitterResizeEventArgs { PaneIndex = pane.Index, Pane = pane, NewSize = sizeNew };
                await Resize.InvokeAsync(arg);
                if (arg.Cancel)
                {
                    var oldSize = pane.SizeRuntine;
                    pane.SizeRuntine = "0";
                    await InvokeAsync(StateHasChanged);
                    pane.SizeRuntine = oldSize;
                    await InvokeAsync(StateHasChanged);
                    return;
                }
            }

            pane.SizeRuntine = sizeNew.ToString("0.##", CultureInfo.InvariantCulture) + "%";

            if (paneNextIndex.HasValue)
            {
                var paneNext = Panes[paneNextIndex.Value];

                if (Expand.HasDelegate)
                {
                    var arg = new RadzenSplitterResizeEventArgs { PaneIndex = paneNext.Index, Pane = paneNext, NewSize = sizeNextNew.Value };
                    await Resize.InvokeAsync(arg);
                    //cancel omitted because it is managed by the parent panel
                }

                paneNext.SizeRuntine = sizeNextNew.Value.ToString("0.##", CultureInfo.InvariantCulture) + "%";
            }
        }

        /// <summary>
        /// Collapses the execute.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="paneIndex">Index of the pane.</param>
        /// <param name="paneId">The pane identifier.</param>
        internal async Task CollapseExec(object args, int paneIndex, string paneId)
        {
            var pane = Panes[paneIndex];
            var paneNext = pane.Next();

            if (paneNext != null && paneNext.Collapsible && paneNext.IsLast && paneNext.Collapsed)
            {
                if (Expand.HasDelegate)
                {
                    var arg = new RadzenSplitterEventArgs { PaneIndex = paneNext.Index, Pane = paneNext };
                    await Expand.InvokeAsync(arg);
                    if (arg.Cancel)
                        return;
                }

                paneNext.Collapsed = false;
            }
            else
            {
                if (Collapse.HasDelegate)
                {
                    var arg = new RadzenSplitterEventArgs { PaneIndex = pane.Index, Pane = pane };
                    await Collapse.InvokeAsync(arg);
                    if (arg.Cancel)
                        return;
                }

                pane.Collapsed = true;
            }

            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Expands the execute.
        /// </summary>
        /// <param name="args">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="paneIndex">Index of the pane.</param>
        /// <param name="paneId">The pane identifier.</param>
        internal async Task ExpandExec(MouseEventArgs args, int paneIndex, string paneId)
        {
            var pane = Panes[paneIndex];
            var paneNext = pane.Next();

            if (paneNext != null && paneNext.Collapsible && paneNext.IsLast && !pane.Collapsed)
            {
                if (Collapse.HasDelegate)
                {
                    var arg = new RadzenSplitterEventArgs { PaneIndex = paneNext.Index, Pane = paneNext };
                    await Collapse.InvokeAsync(arg);
                    if (arg.Cancel)
                        return;
                }

                paneNext.Collapsed = true;
            }
            else
            {
                if (Expand.HasDelegate)
                {
                    var arg = new RadzenSplitterEventArgs { PaneIndex = pane.Index, Pane = pane };
                    await Expand.InvokeAsync(arg);
                    if (arg.Cancel)
                        return;
                }

                pane.Collapsed = false;
            }

            await InvokeAsync(StateHasChanged);
        }


        /// <summary>
        /// Gets the component CSS class.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string GetComponentCssClass()
        {
            return $"rz-splitter rz-splitter-{Enum.GetName(typeof(Orientation), Orientation).ToLower()}";
        }

        /// <summary>
        /// Gets or sets the collapse.
        /// </summary>
        /// <value>The collapse.</value>
        [Parameter]
        public EventCallback<RadzenSplitterEventArgs> Collapse { get; set; }

        /// <summary>
        /// Gets or sets the expand.
        /// </summary>
        /// <value>The expand.</value>
        [Parameter]
        public EventCallback<RadzenSplitterEventArgs> Expand { get; set; }

        /// <summary>
        /// Gets or sets the resize.
        /// </summary>
        /// <value>The resize.</value>
        [Parameter]
        public EventCallback<RadzenSplitterResizeEventArgs> Resize { get; set; }
    }
}