﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Linq;
using Perspex.Controls.Presenters;
using Perspex.Controls.Primitives;
using Perspex.Controls.Templates;
using Perspex.LogicalTree;
using Xunit;

namespace Perspex.Controls.UnitTests
{
    public class TabControlTests
    {
        [Fact]
        public void First_Tab_Should_Be_Selected_By_Default()
        {
            TabItem selected;
            var target = new TabControl
            {
                Template = new FuncControlTemplate<TabControl>(CreateTabControlTemplate),
                Items = new[]
                {
                    (selected = new TabItem
                    {
                        Name = "first",
                        Content = "foo",
                    }),
                    new TabItem
                    {
                        Name = "second",
                        Content = "bar",
                    },
                }
            };

            target.ApplyTemplate();

            Assert.Equal(0, target.SelectedIndex);
            Assert.Equal(selected, target.SelectedItem);
        }

        [Fact]
        public void Logical_Children_Should_Be_TabItems()
        {
            var items = new[]
            {
                new TabItem
                {
                    Content = "foo"
                },
                new TabItem
                {
                    Content = "bar"
                },
            };

            var target = new TabControl
            {
                Template = new FuncControlTemplate<TabControl>(CreateTabControlTemplate),
                Items = items,
            };

            Assert.Equal(items, target.GetLogicalChildren());
            target.ApplyTemplate();
            Assert.Equal(items, target.GetLogicalChildren());
        }

        [Fact]
        public void Removal_Should_Set_Next_Tab()
        {
            var collection = new ObservableCollection<TabItem>()
            {
                new TabItem
                {
                    Name = "first",
                    Content = "foo",
                },
                new TabItem
                {
                    Name = "second",
                    Content = "bar",
                },
                new TabItem
                {
                    Name = "3rd",
                    Content = "barf",
                },
            };

            var target = new TabControl
            {
                Template = new FuncControlTemplate<TabControl>(CreateTabControlTemplate),
                Items = collection,
            };

            target.ApplyTemplate();
            target.SelectedItem = collection[1];
            collection.RemoveAt(1);

            // compare with former [2] now [1] == "3rd"
            Assert.Same(collection[1], target.SelectedItem);
        }

        [Fact]
        public void DataContexts_Should_Be_Correctly_Set()
        {
            var items = new object[]
            {
                "Foo",
                new Item("Bar"),
                new TextBlock { Text = "Baz" },
                new TabItem { Content = "Qux" },
                new TabItem { Content = new TextBlock { Text = "Bob" } }
            };

            var target = new TabControl
            {
                Template = new FuncControlTemplate<TabControl>(CreateTabControlTemplate),
                DataContext = "Base",
                DataTemplates = new DataTemplates
                {
                    new FuncDataTemplate<Item>(x => new Button { Content = x })
                },
                Items = items,
            };

            target.ApplyTemplate();

            var carousel = target.GetLogicalChildren().OfType<Carousel>().Single();

            var dataContext = ((TextBlock)carousel.Presenter.Panel.GetLogicalChildren().Single()).DataContext;
            Assert.Equal(items[0], dataContext);

            target.SelectedIndex = 1;
            dataContext = ((Button)carousel.Presenter.Panel.GetLogicalChildren().Single()).DataContext;
            Assert.Equal(items[1], dataContext);

            target.SelectedIndex = 2;
            dataContext = ((TextBlock)carousel.Presenter.Panel.GetLogicalChildren().Single()).DataContext;
            Assert.Equal("Base", dataContext);

            target.SelectedIndex = 3;
            dataContext = ((TextBlock)carousel.Presenter.Panel.GetLogicalChildren().Single()).DataContext;
            Assert.Equal("Qux", dataContext);

            target.SelectedIndex = 4;
            dataContext = ((TextBlock)carousel.Presenter.Panel.GetLogicalChildren().Single()).DataContext;
            Assert.Equal("Base", dataContext);
        }

        /// <summary>
        /// Non-headered control items should result in TabStripItems with empty content.
        /// </summary>
        /// <remarks>
        /// If a TabStrip is created with non IHeadered controls as its items, don't try to
        /// display the control in the TabStripItem: if the TabStrip is part of a TabControl
        /// then *that* will also try to display the control, resulting in dual-parentage 
        /// breakage.
        /// </remarks>
        [Fact]
        public void Non_IHeadered_Control_Items_Should_Be_Ignored()
        {
            var items = new[]
            {
                new TextBlock { Text = "foo" },
                new TextBlock { Text = "bar" },
            };

            var target = new TabControl
            {
                Template = new FuncControlTemplate<TabControl>(CreateTabControlTemplate),
                Items = items,
            };

            target.ApplyTemplate();

            var result = target.TabStrip.GetLogicalChildren()
                .OfType<TabStripItem>()
                .Select(x => x.Content)
                .ToList();

            Assert.Equal(new object[] { string.Empty, string.Empty }, result);
        }

        private Control CreateTabControlTemplate(TabControl parent)
        {
            return new StackPanel
            {
                Children = new Controls
                {
                    new TabStrip
                    {
                        Name = "PART_TabStrip",
                        Template = new FuncControlTemplate<TabStrip>(CreateTabStripTemplate),
                        MemberSelector = TabControl.HeaderSelector,
                        [!TabStrip.ItemsProperty] = parent[!TabControl.ItemsProperty],
                        [!!TabStrip.SelectedIndexProperty] = parent[!!TabControl.SelectedIndexProperty]
                    },
                    new Carousel
                    {
                        Name = "PART_Content",
                        Template = new FuncControlTemplate<Carousel>(CreateCarouselTemplate),
                        MemberSelector = TabControl.ContentSelector,
                        [!Carousel.ItemsProperty] = parent[!TabControl.ItemsProperty],
                        [!Carousel.SelectedItemProperty] = parent[!TabControl.SelectedItemProperty],
                    }
                }
            };
        }

        private Control CreateTabStripTemplate(TabStrip parent)
        {
            return new ItemsPresenter
            {
                Name = "PART_ItemsPresenter",
                [~ItemsPresenter.ItemsProperty] = parent[~ItemsControl.ItemsProperty],
                [!CarouselPresenter.MemberSelectorProperty] = parent[!ItemsControl.MemberSelectorProperty],
            };
        }

        private Control CreateCarouselTemplate(Carousel control)
        {
            return new CarouselPresenter
            {
                Name = "PART_ItemsPresenter",
                [!CarouselPresenter.ItemsProperty] = control[!ItemsControl.ItemsProperty],
                [!CarouselPresenter.ItemsPanelProperty] = control[!ItemsControl.ItemsPanelProperty],
                [!CarouselPresenter.MemberSelectorProperty] = control[!ItemsControl.MemberSelectorProperty],
                [!CarouselPresenter.SelectedIndexProperty] = control[!SelectingItemsControl.SelectedIndexProperty],
                [~CarouselPresenter.TransitionProperty] = control[~Carousel.TransitionProperty],
            };
        }

        private class Item
        {
            public Item(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }
    }
}
