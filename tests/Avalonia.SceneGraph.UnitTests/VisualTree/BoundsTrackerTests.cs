// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.VisualTree;
using Avalonia.Rendering;
using Xunit;
using Avalonia.Media;
using Moq;
using Avalonia.UnitTests;

namespace Avalonia.SceneGraph.UnitTests.VisualTree
{
    public class BoundsTrackerTests
    {
        [Fact]
        public void Should_Track_Bounds()
        {
            using (UnitTestApplication.Start(TestServices.StyledWindow))
            {
                var target = new BoundsTracker();
                var control = default(Rectangle);
                var tree = new Decorator
                {
                    Padding = new Thickness(10),
                    Child = new Decorator
                    {
                        Padding = new Thickness(5),
                        Child = control = new Rectangle
                        {
                            Width = 15,
                            Height = 15,
                        },
                    }
                };

                var context = new DrawingContext(Mock.Of<IDrawingContextImpl>());

                tree.Measure(Size.Infinity);
                tree.Arrange(new Rect(0, 0, 100, 100));
                context.Render(tree);

                var track = target.Track(control);
                var results = new List<TransformedBounds>();
                track.Subscribe(results.Add);

                Assert.Equal(new Rect(0, 0, 15, 15), results[0].Bounds);
                Assert.Equal(Matrix.CreateTranslation(42, 42), results[0].Transform);
            }
        }
    }
}
