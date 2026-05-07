using System;
using System.Collections.Generic;
using System.Linq;

using Engine.Physics;

using Microsoft.Xna.Framework;

namespace Engine.Utils;

public class QuadTreeNode
{
    public RectangleF NodeBox { get; }
    public int Depth { get; }

    public QuadTreeNode(float x, float y, float width, float height, int depth)
    {
        NodeBox = new RectangleF(x, y, width, height);
        Depth = depth;
    }
}

public class QuadTreeBranchNode : QuadTreeNode
{
    public QuadTreeNode TopLeft { get; set; }
    public QuadTreeNode TopRight { get; set; }
    public QuadTreeNode BottomRight { get; set; }
    public QuadTreeNode BottomLeft { get; set; }

    public QuadTreeBranchNode(float x, float y, float width, float height, int depth) : base(x, y, width, height, depth)
    {
        var childWidth = width / 2f;
        var childHeight = height / 2f;

        TopLeft = new QuadTreeLeafNode(x, y, childWidth, childHeight, depth + 1);
        TopRight = new QuadTreeLeafNode(x + childWidth, y, childWidth, childHeight, depth + 1);
        BottomRight = new QuadTreeLeafNode(x + childWidth, y + childHeight, childWidth, childHeight, depth + 1);
        BottomLeft = new QuadTreeLeafNode(x, y + childHeight, childWidth, childHeight, depth + 1);
    }
}

public class QuadTreeLeafNode : QuadTreeNode
{
    public List<RectangleF> BoundingBoxes { get; }

    public QuadTreeLeafNode(float x, float y, float width, float height, int depth) : base(x, y, width, height, depth)
    {
        BoundingBoxes = [];
    }
}

public class QuadTree
{
    private readonly QuadTreeBranchNode _rootNode;

    // Maximum number of bounding boxes within a node before splitting it
    private const int MaxBoundingBoxesCount = 5;

    private const int MaximumTreeDepth = 5;

    public QuadTree(int worldWith, int worldHeight)
    {
        _rootNode = new QuadTreeBranchNode(0, 0, worldWith, worldHeight, 0);
    }

    private QuadTreeNode UpdateNodeIfRequired(QuadTreeNode node)
    {
        if (node is not QuadTreeLeafNode)
        {
            return node;
        }

        if (node is QuadTreeLeafNode leafNode)
        {
            if (leafNode.BoundingBoxes.Count < MaxBoundingBoxesCount || leafNode.Depth >= MaximumTreeDepth)
            {
                return node;
            }

            var newBranch = new QuadTreeBranchNode(node.NodeBox.X, node.NodeBox.Y, node.NodeBox.Width, node.NodeBox.Height, node.Depth);

            foreach (var bb in leafNode.BoundingBoxes)
            {
                if (newBranch.TopLeft.NodeBox.Intersects(bb) && newBranch.TopLeft is QuadTreeLeafNode topLeftLeaf)
                {
                    topLeftLeaf.BoundingBoxes.Add(bb);
                }

                if (newBranch.TopRight.NodeBox.Intersects(bb) && newBranch.TopRight is QuadTreeLeafNode topRightLeaf)
                {
                    topRightLeaf.BoundingBoxes.Add(bb);
                }

                if (newBranch.BottomRight.NodeBox.Intersects(bb) && newBranch.BottomRight is QuadTreeLeafNode bottomRightLeaf)
                {
                    bottomRightLeaf.BoundingBoxes.Add(bb);
                }

                if (newBranch.BottomLeft.NodeBox.Intersects(bb) && newBranch.BottomLeft is QuadTreeLeafNode bottomLeftLeaf)
                {
                    bottomLeftLeaf.BoundingBoxes.Add(bb);
                }
            }

            return newBranch;
        }

        return node;
    }

    public void AddIntersector(RectangleF boundingBox)
    {
        // find the leaf node(s) that contain this bounding box and add it to them.
        WalkTree(_rootNode, (leaf) =>
        {
            if (leaf.NodeBox.Intersects(boundingBox))
            {
                leaf.BoundingBoxes.Add(boundingBox);
            }
        });

        // go through and check whether we need to split a leaf into a branch
        WalkTree(_rootNode, branchCallback: (branch) =>
        {
            branch.TopLeft = UpdateNodeIfRequired(branch.TopLeft);
            branch.TopRight = UpdateNodeIfRequired(branch.TopRight);
            branch.BottomRight = UpdateNodeIfRequired(branch.BottomRight);
            branch.BottomLeft = UpdateNodeIfRequired(branch.BottomLeft);
        });
    }

    public List<RectangleF> GetIntersectors(RectangleF boundingBox)
    {
        var results = GetIntersectorsInternal(_rootNode, boundingBox);
        return results.Distinct().ToList();
    }

    public void WalkTree(Action<QuadTreeLeafNode> leafAction, Action<QuadTreeBranchNode> branchAction)
    {
        this.WalkTree(_rootNode, leafAction, branchAction);
    }

    private List<RectangleF> GetIntersectorsInternal(QuadTreeNode node, RectangleF boundingBox)
    {
        var intersectors = new List<RectangleF>();

        if (node is QuadTreeLeafNode leafNode)
        {
            intersectors.AddRange(leafNode.BoundingBoxes.Where((bb) => boundingBox.Intersects(bb)));
        }
        else if (node is QuadTreeBranchNode branchNode)
        {
            if (boundingBox.Intersects(branchNode.TopLeft.NodeBox))
            {
                intersectors.AddRange(GetIntersectorsInternal(branchNode.TopLeft, boundingBox));
            }
            if (boundingBox.Intersects(branchNode.TopRight.NodeBox))
            {
                intersectors.AddRange(GetIntersectorsInternal(branchNode.TopRight, boundingBox));
            }
            if (boundingBox.Intersects(branchNode.BottomRight.NodeBox))
            {
                intersectors.AddRange(GetIntersectorsInternal(branchNode.BottomRight, boundingBox));
            }
            if (boundingBox.Intersects(branchNode.BottomLeft.NodeBox))
            {
                intersectors.AddRange(GetIntersectorsInternal(branchNode.BottomLeft, boundingBox));
            }
        }

        return intersectors;
    }

    /**
     * Recursively walks the tree and visits every node, starting from the given node.
     * Invokes a specific callback depending on the type of node encountered, either a leaf or a branch.
     */
    private void WalkTree(QuadTreeNode node, Action<QuadTreeLeafNode>? leafCallback = null, Action<QuadTreeBranchNode>? branchCallback = null)
    {
        if (node is QuadTreeLeafNode leafNode)
        {
            leafCallback?.Invoke(leafNode);
        }
        else if (node is QuadTreeBranchNode branchNode)
        {
            branchCallback?.Invoke(branchNode);

            WalkTree(branchNode.TopLeft, leafCallback, branchCallback);
            WalkTree(branchNode.TopRight, leafCallback, branchCallback);
            WalkTree(branchNode.BottomRight, leafCallback, branchCallback);
            WalkTree(branchNode.BottomLeft, leafCallback, branchCallback);
        }
    }
}