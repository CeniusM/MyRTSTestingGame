using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NodeIndex = System.Int32;

namespace AStarPathfindingMaze
{
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Coord other)
        {
            return x == other.x && y == other.y;
        }
    }

    public struct Node
    {
        public bool DONE;
        public int HeapIndex;

        /// <summary>
        /// Distance from start
        /// </summary>
        public int GCost;

        /// <summary>
        /// Distance to end
        /// </summary>
        public int HCost;

        /// <summary>
        /// Total cost
        /// </summary>
        public int FCost;

        /// <summary>
        /// Index to parrent node
        /// </summary>
        public NodeIndex Parent;

        public Node(int gCost, int hCost, NodeIndex parent)
        {
            DONE = false;
            HeapIndex = 0;

            GCost = gCost;
            HCost = hCost;
            FCost = gCost + hCost;
            Parent = parent;
        }

        public override string ToString()
        {
            return "N:" + FCost;
        }
    }

    public class NodeHeap
    {
        // Instead, we just index with int into 1d node array
        // That way we have less heap, no update, and can get both FCost, GCost and HCost
        PathFinder _parrent;
        private string[] DebugHeap => Heap.Select(x => _parrent._nodes[x].ToString()!).ToArray();
        public readonly NodeIndex[] Heap;
        public readonly int Size;
        public int Count;

        public NodeHeap(PathFinder parrent, int size)
        {
            _parrent = parrent;
            Heap = new NodeIndex[size];
            Size = size;
            Count = 0;
        }

        public void Add(NodeIndex element)
        {
            if (Count == Size)
                throw new Exception();

            int index = Count;
            _parrent._nodes[element].HeapIndex = index;
            Heap[Count++] = element;
            SortUp(index);
        }

        public NodeIndex Pop()
        {
            if (Count == 0)
                throw new Exception();

            NodeIndex first = Heap[0];
            Count--;

            _parrent._nodes[Heap[Count]].HeapIndex = 0;
            Heap[0] = Heap[Count];

            SortDown(0);

            return first;
        }

        private void SortDown(int index)
        {
            while (true)
            {
                int leftChildIndex = LeftChild(index);
                int rightChildIndex = RightChild(index);

                if (leftChildIndex < Count)
                {
                    int swapIndex = leftChildIndex;

                    if (rightChildIndex < Count)
                    {
                        if (_parrent._nodes[Heap[leftChildIndex]].FCost > _parrent._nodes[Heap[rightChildIndex]].FCost)
                        {
                            swapIndex = rightChildIndex;
                        }
                    }

                    if (_parrent._nodes[Heap[index]].FCost > _parrent._nodes[Heap[swapIndex]].FCost)
                    {
                        Swap(index, swapIndex);
                        index = swapIndex;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void SortUp(int index)
        {
            while (index != 0)
            {
                int parentIndex = Parent(index);

                int parrentCost = _parrent._nodes[Heap[parentIndex]].FCost;

                if (parrentCost > _parrent._nodes[Heap[index]].FCost)
                {
                    Swap(index, parentIndex);
                    index = parentIndex;
                }
                else
                {
                    return;
                }
            }
        }

        private void Swap(int first, int second)
        {
            _parrent._nodes[Heap[first]].HeapIndex = second;
            _parrent._nodes[Heap[second]].HeapIndex = first;

            NodeIndex temp = Heap[first];
            Heap[first] = Heap[second];
            Heap[second] = temp;
        }

        private int LeftChild(int index)
        {
            return index * 2 + 1;
        }

        private int RightChild(int index)
        {
            return index * 2 + 2;
        }

        private int Parent(int index)
        {
            return (index - 1) / 2;
        }

        internal void SortUpNode(NodeIndex node)
        {
            SortUp(_parrent._nodes[node].HeapIndex);
        }

        internal void Clear()
        {
            Count = 0;
        }
    }

    // Should be remade to handle float positions and negative coordinates, so map from -50,-50 to 50,50 is possible
    // Currently have a problem where the units cut through the cornors
    public class PathFinder
    {
        private bool allowMoveDiagonallyThroughCornors = false;

        public readonly int Width;
        public readonly int Height;

        // Maybe switch out with int array, then have something like int.maxvalue as a wall
        // then we can use the rest of the values a move penelty
        // Can also for fun test if it makes a diffrence if we parse multiple units,
        // and increase the map value for where they wanna walk so they walk a bit diffrent paths
        // For when a unit is 2 wide, we can make a temp map and make all 1 gaps solid for that search #_# => ### and #__# => #__#
        public readonly bool[,] Map;

        private NodeIndex _end;
        private int _endX;
        private int _endY;
        private int _startX;
        private int _startY;
        internal readonly Node[] _nodes;
        private readonly NodeHeap _heap;

        const int DiagonalCost = 14;
        const int StraightCost = 10;

        public PathFinder(int width, int height)
        {
            Width = width;
            Height = height;
            Map = new bool[height, width];
            _nodes = new Node[height * width];
            _heap = new NodeHeap(this, Width * Height);
        }

        public void DebugCreateHCosts(Coord end)
        {
            Array.Clear(_nodes, 0, _nodes.Length);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _nodes[y * Width + x].HCost = Distance(x, y, end.x, end.y);
                }
            }
        }

        public List<Coord> SearchPath(Coord start, Coord end, bool allowMoveDiagonallyThroughCornors = false)
        {
            this.allowMoveDiagonallyThroughCornors = allowMoveDiagonallyThroughCornors;

            // ThrowIf()
            if (IsOutside(start.x, start.y) || IsOutside(end.x, end.y))
            {
                Debug.LogError("Start or end is outside of map\n" +
                    $"start: {start.x}:{start.y}\n" +
                    $"end: {end.x}:{end.y}\n" +
                    $"size: {Width}:{Height}");
                return new List<Coord>();
            }

            if (start.x == end.x && start.y == end.y)
            {
                return new List<Coord>() { end };
            }

            _end = end.y * Width + end.x;
            _endX = end.x;
            _endY = end.y;
            _startX = start.x;
            _startY = start.y;

            Array.Clear(_nodes, 0, _nodes.Length);

            _heap.Clear();
            _heap.Add(start.y * Width + start.x);

            NodeIndex lastPoint = start.y * Width + start.x;
            NodeIndex squareLookedAt;
            while ((squareLookedAt = Step()) != _end && squareLookedAt != int.MaxValue)
            {
                lastPoint = squareLookedAt;
            }

            // Still return list to the node that got the clostest to end
            bool failed = squareLookedAt == int.MaxValue;
            if (!failed)
            {
                lastPoint = squareLookedAt;
            }

            return GetSearchedPath(start.y * Width + start.x, lastPoint);
        }

        private NodeIndex Step()
        {
            if (_heap.Count == 0)
                return int.MaxValue;

            NodeIndex nodeIndex = _heap.Pop();
            _nodes[nodeIndex].DONE = true;
            int gCost = _nodes[nodeIndex].GCost;

            int nodeX = nodeIndex % Width;
            int nodeY = nodeIndex / Width;

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    if (yOffset == 0 && xOffset == 0)
                        continue;

                    int x = xOffset + nodeX;
                    int y = yOffset + nodeY;

                    NodeIndex nextIndex = y * Width + x;

                    if (IsOutside(x, y))
                        continue;
                    if (Map[y, x])
                        continue;
                    if (_nodes[nextIndex].DONE)
                        continue;

                    bool isDiagonal = xOffset != 0 && yOffset != 0;

                    int moveCost = isDiagonal ? DiagonalCost : StraightCost;

                    if (isDiagonal && !allowMoveDiagonallyThroughCornors)
                    {
                        // Check if moving diagonally would cut through a corner
                        bool corner1Blocked = Map[nodeY, x];
                        bool corner2Blocked = Map[y, nodeX];

                        if (corner1Blocked || corner2Blocked)
                        {
                            continue;
                        }
                    }

                    Node nextNode = _nodes[nextIndex];

                    bool hasBeenLookedAt = nextNode.FCost != 0;

                    Node newNode = new Node(
                        gCost + moveCost,
                        hasBeenLookedAt ?
                        nextNode.HCost : Distance(x, y, _endX, _endY),
                        nodeIndex
                        );

                    if (!hasBeenLookedAt ||
                       nextNode.FCost > newNode.FCost)
                    {
                        int indexSave = _nodes[nextIndex].HeapIndex;
                        _nodes[nextIndex] = newNode;
                        _nodes[nextIndex].HeapIndex = indexSave;

                        if (hasBeenLookedAt)
                        {
                            _heap.SortUpNode(nextIndex);
                        }
                        else
                        {
                            _heap.Add(nextIndex);
                        }
                    }

                    if (nextIndex == _end)
                        return _end;
                }
            }

            return nodeIndex;
        }

        private int Distance(int x1, int y1, int x2, int y2)
        {
            int xDiff = Math.Abs(x2 - x1);
            int yDiff = Math.Abs(y2 - y1);

            int high = Math.Max(xDiff, yDiff);
            int low = Math.Min(xDiff, yDiff);

            return
                DiagonalCost * low +
                StraightCost * (high - low);
        }

        // Here we should remove the nodes that are part of straight lines
        private List<Coord> GetSearchedPath(NodeIndex mazeStart, NodeIndex from)
        {
            Coord toCoord(NodeIndex index) => new Coord(index % Width, index / Width);

            List<Coord> path = new List<Coord>(
                Distance(_startX, _startY, _endX, _endY) / 10
                ) { toCoord(from) };

            while ((from = _nodes[from].Parent) != mazeStart && path.Count <= Width * Height)
                path.Add(toCoord(from));

            path.Reverse();

            return path;
        }

        private bool IsOutside(int x, int y)
        {
            return
                x < 0 || x >= Width ||
                y < 0 || y >= Height;
        }

        public void SetCellWalkable(int x, int y, bool v)
        {
            Map[y, x] = !v;
        }
    }
}