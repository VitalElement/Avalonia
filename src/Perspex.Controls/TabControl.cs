// Copyright (c) The Perspex Project. All rights reserved.
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

        private static readonly IMemberSelector s_contentSelector =
            new FuncMemberSelector<object, object>(SelectContent);

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
        /// Gets an <see cref="IMemberSelector"/> that selects the content of a <see cref="TabItem"/>.
        /// </summary>
        public IMemberSelector ContentSelector => s_contentSelector;

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

            var carousel = nameScope.Find<ILogical>("PART_Carousel");
            var tabStrip = nameScope.Find<SelectingItemsControl>("PART_TabStrip");

            this.LogicalChildren.Clear();

            if (tabStrip != null)
            {
                this.LogicalChildren.Add(tabStrip);
            }

            if (carousel != null)
            {
                this.LogicalChildren.Add(carousel);
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
    }
}
