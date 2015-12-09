// Copyright (c) The Perspex Project. All rights reserved.
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
        public void Logical_Children_Should_Be_TabStrip_And_Carousel()
        {
            var target = new TabControl
            {
                Template = new FuncControlTemplate<TabControl>(CreateTabControlTemplate),
                Items = new[]
                {
                    new TabItem
                    {
                        Content = "foo"
                    },
                    new TabItem
                    {
                        Content = "bar"
                    },
                },
            };

            target.ApplyTemplate();

            var logicalChildren = target.GetLogicalChildren().ToList();
            Assert.Equal(2, logicalChildren.Count);
            Assert.IsType<TabStrip>(logicalChildren[0]);
            Assert.IsType<Carousel>(logicalChildren[1]);
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

            var dataContext = ((TextBlock)carousel.GetLogicalChildren().Single()).DataContext;
            Assert.Equal(items[0], dataContext);

            target.SelectedIndex = 1;
            dataContext = ((Button)carousel.GetLogicalChildren().Single()).DataContext;
            Assert.Equal(items[1], dataContext);

            target.SelectedIndex = 2;
            dataContext = ((TextBlock)carousel.GetLogicalChildren().Single()).DataContext;
            Assert.Equal("Base", dataContext);

            target.SelectedIndex = 3;
            dataContext = ((TextBlock)carousel.GetLogicalChildren().Single()).DataContext;
            Assert.Equal("Qux", dataContext);

            target.SelectedIndex = 4;
            dataContext = ((TextBlock)carousel.GetLogicalChildren().Single()).DataContext;
            Assert.Equal("Base", dataContext);
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
                        [!TabStrip.ItemsProperty] = parent[!TabControl.ItemsProperty],
                        [!!TabStrip.SelectedIndexProperty] = parent[!!TabControl.SelectedIndexProperty]
                    },
                    new Carousel
                    {
                        Name = "PART_Carousel",
                        Template = new FuncControlTemplate<Carousel>(CreateCarouselTemplate),
                        MemberSelector = parent.ContentSelector,
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
