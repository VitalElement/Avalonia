﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Perspex.Animation;
using Perspex.Controls.Generators;
using Perspex.Controls.Primitives;
using Perspex.Controls.Templates;

namespace Perspex.Controls
{
    /// <summary>
    /// A tab control that displays a tab strip along with the content of the selected tab.
    /// </summary>
    public class TabControl : SelectingItemsControl
    {
        /// <summary>
        /// Defines the <see cref="Transition"/> property.
        /// </summary>
        public static readonly PerspexProperty<IPageTransition> TransitionProperty =
            Carousel.TransitionProperty.AddOwner<TabControl>();

        /// <summary>
        /// Defines an <see cref="IMemberSelector"/> that selects the content of a <see cref="TabItem"/>.
        /// </summary>
        public static readonly IMemberSelector ContentSelector =
            new FuncMemberSelector<object, object>(SelectContent);

        /// <summary>
        /// Defines an <see cref="IMemberSelector"/> that selects the header of a <see cref="TabItem"/>.
        /// </summary>
        public static readonly IMemberSelector HeaderSelector =
            new FuncMemberSelector<object, object>(SelectHeader);

        /// <summary>
        /// Defines the <see cref="TabStripPlacement"/> property.
        /// </summary>
        public static readonly PerspexProperty<Dock> TabStripPlacementProperty =
            PerspexProperty.Register<TabControl, Dock>(nameof(TabStripPlacement), defaultValue: Dock.Top);

        /// <summary>
        /// Initializes static members of the <see cref="TabControl"/> class.
        /// </summary>
        static TabControl()
        {
            SelectionModeProperty.OverrideDefaultValue<TabControl>(SelectionMode.AlwaysSelected);
            FocusableProperty.OverrideDefaultValue<TabControl>(false);
            AffectsMeasure(TabStripPlacementProperty);
        }

        /// <summary>
        /// Gets the content portion of the <see cref="TabControl"/>.
        /// </summary>
        public IControl Content
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tab strip portion of the <see cref="TabControl"/>.
        /// </summary>
        public IControl TabStrip
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the transition to use when switching tabs.
        /// </summary>
        public IPageTransition Transition
        {
            get { return GetValue(TransitionProperty); }
            set { SetValue(TransitionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tabstrip placement of the tabcontrol.
        /// </summary>
        public Dock TabStripPlacement
        {
            get { return GetValue(TabStripPlacementProperty); }
            set { SetValue(TabStripPlacementProperty, value); }
        }

        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            // TabControl doesn't actually create items - instead its TabStrip and Carousel
            // children create the items. However we want it to be a SelectingItemsControl
            // so that it has the Items/SelectedItem etc properties. In this case, we can
            // return a null ItemContainerGenerator to disable the creation of item containers.
            return null;
        }

        protected override void OnTemplateApplied(INameScope nameScope)
        {
            base.OnTemplateApplied(nameScope);

            TabStrip = nameScope.Find<IControl>("PART_TabStrip");
            Content = nameScope.Find<IControl>("PART_Content");

            this.LogicalChildren.Clear();

            if (TabStrip != null)
            {
                this.LogicalChildren.Add(TabStrip);
            }

            if (Content != null)
            {
                this.LogicalChildren.Add(Content);
            }
        }

        /// <summary>
        /// Selects the content of a tab item.
        /// </summary>
        /// <param name="o">The tab item.</param>
        /// <returns>The content.</returns>
        private static object SelectContent(object o)
        {
            var content = o as IContentControl;

            if (content != null)
            {
                return content.Content;
            }
            else
            {
                return o;
            }       
        }

        /// <summary>
        /// Selects the header of a tab item.
        /// </summary>
        /// <param name="o">The tab item.</param>
        /// <returns>The content.</returns>
        private static object SelectHeader(object o)
        {
            var headered = o as IHeadered;
            var control = o as IControl;

            if (headered != null)
            {
                return headered.Header;
            }
            else if (control != null)
            {
                // Non-headered control items should result in TabStripItems with empty content.
                // If a TabStrip is created with non IHeadered controls as its items, don't try to
                // display the control in the TabStripItem: the content portion will also try to 
                // display this control, resulting in dual-parentage breakage.
                return string.Empty;
            }
            else
            {
                return o;
            }
        }
    }
}
