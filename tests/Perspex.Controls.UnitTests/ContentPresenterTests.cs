// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Perspex.Controls.Presenters;
using Xunit;

namespace Perspex.Controls.UnitTests
{
    public class ContentPresenterTests
    {
        [Fact]
        public void Setting_Content_To_Control_Should_Set_Child()
        {
            var target = new ContentPresenter();
            var child = new Border();

            target.Content = child;

            // Child should not update until ApplyTemplate called.
            Assert.Null(target.Child);

            target.ApplyTemplate();

            Assert.Equal(child, target.Child);
        }
        [Fact]
        public void Setting_Content_To_String_Should_Create_TextBlock()
        {
            var target = new ContentPresenter();

            target.Content = "Foo";

            // Child should not update until ApplyTemplate called.
            Assert.Null(target.Child);

            target.ApplyTemplate();

            Assert.IsType<TextBlock>(target.Child);
            Assert.Equal("Foo", ((TextBlock)target.Child).Text);
        }
    }
}
