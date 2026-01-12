using AwesomeAssertions;
using Sdk.Extensions;

namespace Sdk.Tests.Extensions;

public sealed class TreeExtensionsTests
{
    public sealed class ToTree
    {
        [Fact]
        public void Builds_tree_properly()
        {
            var items = CreateFlatItems();

            var tree = items.ToTree(i => i.Id, i => i.ParentId);
            //Level 0
            AssertTreeNode(tree, 3);
            //Level 1
            AssertTreeNode(tree!.Children!.Single(c => c.Item.Id == 1), 2);
            AssertTreeNode(tree.Children!.Single(c => c.Item.Id == 4), 2);
            AssertTreeNode(tree.Children!.Single(c => c.Item.Id == 10));
            //Level 2
            AssertTreeNode(tree
                .Children!.Single(c => c.Item.Id == 1)
                .Children!.Single(c => c.Item.Id == 2));
            AssertTreeNode(tree
                .Children!.Single(c => c.Item.Id == 1)
                .Children!.Single(c => c.Item.Id == 3));
            AssertTreeNode(tree
                .Children!.Single(c => c.Item.Id == 4)
                .Children!.Single(c => c.Item.Id == 5), 2);
            AssertTreeNode(tree
                .Children!.Single(c => c.Item.Id == 4)
                .Children!.Single(c => c.Item.Id == 8), 1);
            //Level 3
            AssertTreeNode(tree
                .Children!.Single(c => c.Item.Id == 4)
                .Children!.Single(c => c.Item.Id == 5)
                .Children!.Single(c => c.Item.Id == 6));
            AssertTreeNode(tree
                .Children!.Single(c => c.Item.Id == 4)
                .Children!.Single(c => c.Item.Id == 5)
                .Children!.Single(c => c.Item.Id == 7));
        }

        [Fact]
        public void Throws_on_multiple_roots()
        {
            var items = CreateFlatItems();
            items.Add(new FlatItem(42, null));

            var ex = Assert.Throws<InvalidOperationException>(() => items.ToTree(i => i.Id, i => i.ParentId));
            Assert.Contains("2", ex.Message, StringComparison.InvariantCulture);

        }
        [Fact]
        public void Throws_on_missing_root()
        {
            var items = CreateFlatItems();
            items.RemoveAt(0);

            var ex = Assert.Throws<InvalidOperationException>(() => items.ToTree(i => i.Id, i => i.ParentId));
            Assert.Contains("0", ex.Message, StringComparison.InvariantCulture);
        }

        private static void AssertTreeNode(TreeNode<FlatItem>? tree, int? expectedChildren = null)
        {
            Assert.NotNull(tree);
            if (expectedChildren is not null)
            {
                Assert.NotNull(tree.Children);
                Assert.Equal(expectedChildren, tree.Children.Count);
            }
            else
            {
                Assert.Null(tree.Children);
            }
        }
    }

    public sealed class TraverseBreadthFirstWithParent
    {
        [Fact]
        public void Acceptance()
        {
            var root = CreateTree();

            var items = root.TraverseBreadthFirstWithParent(t => t.Children);

            items.Should().BeEquivalentTo(new (Item, Item?)[]
            {
                (new(0), default),
                (new(1), new(0)),
                (new(4), new(0)),
                (new(10), new(0)),
                (new(2), new(1)),
                (new(3), new(1)),
                (new(5), new(4)),
                (new(8), new(4)),
                (new(6), new(5)),
                (new(7), new(5)),
                (new(9), new(8)),
            }, o => o.WithStrictOrdering());
        }
    }

    public sealed class TraverseDepthFirstWithPath
    {
        [Fact]
        public void Acceptance()
        {
            var root = CreateTree();

            var items = root.TraverseDepthFirstWithPath(t => t.Children);

            items.Should().BeEquivalentTo(new List<(Item, List<Item>)>()
            {
                (new(0), []),
                (new(1), [new(0),]),
                (new(2), [new(0), new(1),]),
                (new(3), [new(0), new(1),]),
                (new(4), [new(0),]),
                (new(5), [new(0), new(4),]),
                (new(6), [new(0), new(4), new(5),]),
                (new(7), [new(0), new(4), new(5),]),
                (new(8), [new(0), new(4),]),
                (new(9), [new(0), new(4), new(8),]),
                (new(10), [new(0),]),
            }, o => o.WithStrictOrdering());
        }
    }

    public sealed class Flatten_Single
    {
        [Fact]
        public void Acceptance()
        {
            var root = CreateTree();

            var items = root.Flatten(t => t.Children);

            items.Should().OnlyHaveUniqueItems().And.HaveCount(11);
        }
    }

    public sealed class Flatten_IEnumerable
    {
        [Fact]
        public void Acceptance()
        {
            var root = CreateTree();

            var items = root.Children.Flatten(t => t.Children);

            items.Should().OnlyHaveUniqueItems().And.HaveCount(10);
        }
    }

    private static TreeItem CreateTree()
        => new(0)
        {
            Children =
            [
                new(1)
                {
                    Children =
                    [
                        new(2),
                        new(3),
                    ]
                },
                new(4)
                {
                    Children =
                    [
                        new(5)
                        {
                            Children =
                            [
                                new(6),
                                new(7),
                            ]
                        },
                        new(8)
                        {
                            Children =
                            [
                                new(9),
                            ]
                        },
                    ]
                },
                new(10),
            ]
        };

    private static List<FlatItem> CreateFlatItems()
        =>
        [
            new FlatItem(0, null),
            new FlatItem(1, 0),
            new FlatItem(2, 1),
            new FlatItem(3, 1),
            new FlatItem(4, 0),
            new FlatItem(5, 4),
            new FlatItem(6, 5),
            new FlatItem(7, 5),
            new FlatItem(8, 4),
            new FlatItem(9, 8),
            new FlatItem(10, 0),
        ];

    internal record Item(int Id);
    internal sealed record TreeItem(int Id) : Item(Id)
    {
        internal List<TreeItem> Children { get; init; } = [];
    }
    internal sealed record FlatItem(int Id, int? ParentId) : Item(Id);
}
