using AwesomeAssertions;
using Sdk.Connections.Contracts;

namespace Sdk.Tests.Connections.Contracts;

public sealed class TagTests
{
    public sealed class EqualsTag
    {
        [Fact]
        public void Returns_false_if_parameter_is_null()
        {
            var firstTag = new Tag();
            Tag? secondTag = null;

            firstTag.Should().NotBe(secondTag);
        }

        [Fact]
        public void Returns_true_if_reference_or_id_is_equal()
        {
            var firstTag = new Tag();
            var equalReference = firstTag;
            var equalId = new Tag("", firstTag.Id);

            firstTag.Equals(equalReference).Should().BeTrue();
            firstTag.Equals(equalId).Should().BeTrue();
        }

        [Fact]
        public void Returns_false_if_reference_and_id_are_unequal()
        {
            var firstTag = new Tag();
            var secondTag = new Tag();

            firstTag.Equals(secondTag).Should().BeFalse();
        }
    }

    public sealed class EqualsObject
    {
        [Fact]
        public void Returns_false_if_parameter_is_null()
        {
            var firstTag = new Tag();
            object? secondTag = null;

            firstTag.Should().NotBe(secondTag);
        }

        [Fact]
        public void Returns_true_if_reference_or_id_is_equal()
        {
            var firstTag = new Tag();
            object equalReference = firstTag;
            object equalId = new Tag("", firstTag.Id);

            firstTag.Equals(equalReference).Should().BeTrue();
            firstTag.Equals(equalId).Should().BeTrue();
        }

        [Fact]
        public void Returns_false_if_reference_or_id_is_unequal()
        {
            var firstTag = new Tag();
            object secondTag = new Tag();

            firstTag.Equals(secondTag).Should().BeFalse();
        }
    }

    public new sealed class ToString
    {
        [Fact]
        public void Returns_correct_text()
        {
            const string Text = "Test";
            var tag = new Tag(Text, Guid.NewGuid());

            tag.ToString().Should().Be(Text);
        }

        [Fact]
        public void Returns_correct_text_for_protected_tag()
        {
            const string Text = "Test";
            var tag = new Tag(Text, Guid.NewGuid())
            {
                Protected = true
            };

            tag.ToString().Should().Be(Text + " [Protected]");
        }
    }
}
