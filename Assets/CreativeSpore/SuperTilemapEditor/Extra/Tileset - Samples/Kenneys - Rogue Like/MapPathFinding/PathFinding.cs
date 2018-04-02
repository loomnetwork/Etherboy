using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace CreativeSpore.SuperTilemapEditor.PathFindingLib
{
    public class IPathNode
    {

        public virtual bool IsPassable() { return false; }        
        public virtual int GetNeighborCount() { return 0; }
        public virtual IPathNode GetNeighbor(int idx) { return null; }
        public virtual float GetNeigborMovingCost(int neigborIdx) { return 1f; }
        public virtual float GetHeuristic() { return 0f; } //H
        public virtual Vector3 Position { get; set; }

        internal IPathNode ParentNode = null;
        internal float Cost = 0f; //G
        internal float Score = 0f; //F

        internal int openTicks;
        internal int closeTicks;
    };

    //ref: http://www.policyalmanac.org/games/aStarTutorial.htm
    public class PathFinding
    {
        const int k_IterationsPerProcessChunk = 10; // how many iterations before returning the coroutine
        public const float k_InfiniteCostValue = float.MaxValue; // use this value to define an infinite cost

        public class FindingParams
        {
            public IPathNode startNode;
            public IPathNode endNode;
            //Last node will always be considered passable, used to reach the closest tile to a blocked tile, like a door, chest, etc
            public bool endNodeIsPassable = false; 
            public LinkedList<IPathNode> computedPath;
        }

        public int MaxIterations = 8000; // <= 0, for infinite iterations
        public bool IsComputing { get; private set; }

        LinkedList<IPathNode> m_openList = new LinkedList<IPathNode>();
        LinkedList<IPathNode> m_closeList = new LinkedList<IPathNode>();

        public LinkedList<IPathNode> ComputePath(IPathNode startNode, IPathNode endNode, bool endNodeIsPassable = false)
        {
            FindingParams findingParams = new FindingParams()
            {
                startNode = startNode,
                endNode = endNode,
                endNodeIsPassable = endNodeIsPassable,
                computedPath = new LinkedList<IPathNode>()
            };
            IEnumerator coroutine = ComputePathCoroutine(findingParams);
            while (coroutine.MoveNext());
            return findingParams.computedPath;
        }

        public IEnumerator ComputePathAsync(IPathNode startNode, IPathNode endNode, bool endNodeIsPassable = false)
        {
            FindingParams findingParams = new FindingParams()
            {
                startNode = startNode,
                endNode = endNode,
                endNodeIsPassable = endNodeIsPassable,
                computedPath = new LinkedList<IPathNode>()
            };
            
            yield return ComputePathCoroutine(findingParams);
            yield return findingParams.computedPath;
        }

        public IEnumerator ComputePathCoroutine(FindingParams findingParams)
        {
            //NOTE: curTicks will be different for each call.
            // if openTicks == curTicks, it means the node in in openList, same for closeList and closeTicks.
            // this is faster than reset a bool isInOpenList for all nodes before next call to this method
            int curTicks = (int)(Time.realtimeSinceStartup * 1000);

            IsComputing = true;
            if (findingParams.startNode == findingParams.endNode)
            {
                findingParams.computedPath.AddLast(findingParams.startNode);
            }
            else
            {

                //1) Add the starting square (or node) to the open list.
                m_closeList.Clear();
                m_openList.Clear();
                m_openList.AddLast(findingParams.startNode); findingParams.startNode.openTicks = curTicks;

                //2) Repeat the following:
                LinkedListNode<IPathNode> curNode;
                int iterations = 0;
                int iterChunkCounter = k_IterationsPerProcessChunk;
                do
                {
                    ++iterations;
                    --iterChunkCounter;
                    if (iterChunkCounter == 0)
                    {
                        iterChunkCounter = k_IterationsPerProcessChunk;
                        yield return null;
                    }

                    //a) Look for the lowest F cost square on the open list. We refer to this as the current square.
                    //curNode = m_vOpen.First(c => c.Score == m_vOpen.Min(c2 => c2.Score));
                    curNode = null;
                    for (LinkedListNode<IPathNode> pathNode = m_openList.First; pathNode != null; pathNode = pathNode.Next)
                    {
                        if (curNode == null || pathNode.Value.Score < curNode.Value.Score)
                        {
                            curNode = pathNode;
                        }
                    }

                    //b) Switch it to the closed list.
                    m_openList.Remove(curNode); curNode.Value.openTicks = 0;
                    m_closeList.AddLast(curNode); curNode.Value.closeTicks = curTicks;

                    //c) For each of the 8 squares adjacent to this current square …
                    for (int i = 0; i < curNode.Value.GetNeighborCount(); ++i)
                    {
                        //If it is not walkable or if it is on the closed list, ignore it. Otherwise do the following.           
                        IPathNode neigborNode = curNode.Value.GetNeighbor(i);
                        float movingCost = curNode.Value.GetNeigborMovingCost(i);
                        bool isNeighborNodePassable = neigborNode.IsPassable() || findingParams.endNodeIsPassable && neigborNode == findingParams.endNode;
                        if (
                            neigborNode.closeTicks != curTicks && // if closeList does not contains node
                            movingCost != k_InfiniteCostValue && isNeighborNodePassable
                           ) 
                        {
                            //If it isn’t on the open list, add it to the open list. Make the current square the parent of this square. Record the F, G, and H costs of the square. 
                            float newCost = curNode.Value.Cost + movingCost;
                            if (neigborNode.openTicks != curTicks) // if openList does not contains node
                            {
                                m_openList.AddLast(neigborNode); neigborNode.openTicks = curTicks;
                                neigborNode.ParentNode = curNode.Value;
                                neigborNode.Cost = newCost;
                                neigborNode.Score = neigborNode.Cost + neigborNode.GetHeuristic();
                                if (neigborNode == findingParams.endNode)
                                {
                                    curNode.Value = neigborNode;
                                    m_openList.Clear(); // force to exit while
                                    break;
                                }
                            }
                            //If it is on the open list already, check to see if this path to that square is better, using G cost as the measure. A lower G cost means that this is a better path. 
                            else if (newCost < neigborNode.Cost)
                            {
                                //If so, change the parent of the square to the current square, and recalculate the G and F scores of the square. 
                                neigborNode.ParentNode = curNode.Value;
                                neigborNode.Cost = newCost;
                                neigborNode.Score = neigborNode.Cost + neigborNode.GetHeuristic();
                            }
                        }
                    }
                }
                while (m_openList.Count > 0 && (MaxIterations <= 0 || iterations < MaxIterations));
                //Debug.Log("iterations: " + iterations);
                if (iterations >= MaxIterations)
                    Debug.LogWarning("Info: max iterations reached before finding path solution. MaxIterations is set to " + MaxIterations);
                //d) Stop when you:
                //Add the target square to the closed list, in which case the path has been found (see note below), or
                //Fail to find the target square, and the open list is empty. In this case, there is no path.   
                if (curNode.Value == findingParams.endNode)
                {
                    //3) Save the path. Working backwards from the target square, go from each square to its parent square until you reach the starting square. That is your path.             
                    findingParams.computedPath.AddLast(curNode.Value);
                    do
                    {
                        curNode.Value = curNode.Value.ParentNode;
                        //NOTE: I don't remember why the list was inverted here, but I decided to return a route from start to end points and not the inverted path
                        //findingParams.computedPath.AddLast(curNode.Value);
                        findingParams.computedPath.AddFirst(curNode.Value);
                    }
                    while (curNode.Value != findingParams.startNode);                    
                }
            }
            IsComputing = false;
            yield return findingParams;
        }
    }
}