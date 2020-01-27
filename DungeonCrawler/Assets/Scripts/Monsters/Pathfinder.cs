using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Node
    {
        public Node parent;
        public Vector2Int position;

        public float g;
        public float h;
        public float f;

        public Node(Node inParent, Vector2Int inPosition)
        {
            parent = inParent;
            position = inPosition;
            g = 0.0f;
            h = 0.0f;
            f = 0.0f;
        }

        public bool ComaprePositions(Node other)
        {
            return other.position.Equals(position);
        }
    }

    public class Pathfinder
    {
        List<Node> path;

        Vector2Int[] nextPositions = { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };

        float xFraction;
        float zFraction;
        float y;
        int rangeOfChase;

        public Pathfinder(float xFraction, float zFraction, float y, int rangeOfChase)
        {
            path = new List<Node>();
            this.xFraction = xFraction;
            this.zFraction = zFraction;
            this.y = y;
            this.rangeOfChase = rangeOfChase;
        }

        public void FindPath(Vector2Int start, Vector2Int end)
        {
            path.Clear();
            Node startNode = new Node(null, start);
            Node endNode = new Node(null, end);

            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();

            openList.Add(startNode);

            while(openList.Count > 0)
            {
                Node currentNode = openList[0];
                int currentIndex = 0;

                for(int ind = 1; ind < openList.Count; ind++)
                {
                    if(currentNode.f > openList[ind].f)
                    {
                        currentNode = openList[ind];
                        currentIndex = ind;
                    }
                }

                openList.RemoveAt(currentIndex);
                closedList.Add(currentNode);

                // Reached goal
                if(currentNode.ComaprePositions(endNode))
                {
                    while(currentNode.parent != null)
                    {
                        path.Add(currentNode);
                        currentNode = currentNode.parent;
                    }
                    path.Reverse();
                    return;
                }

                List<Node> children = new List<Node>();

                foreach(Vector2Int nextPosition in nextPositions)
                {
                    Vector2Int nodePosition = currentNode.position + nextPosition;

                    if(CheckNewPosition(start, nodePosition))
                    {
                        continue;
                    }

                    Node newNode = new Node(currentNode, nodePosition);

                    children.Add(newNode);
                }

                foreach(Node child in children)
                {
                    bool isInClosedList = false;
                    foreach(Node closedNode in closedList)
                    {
                        if(child.ComaprePositions(closedNode))
                        {
                            isInClosedList = true;
                            break;
                        }
                    }

                    if(isInClosedList)
                    {
                        continue;
                    }

                    child.g = currentNode.g + 1;
                    child.h = (child.position.x - endNode.position.x)^2 + (child.position.y - endNode.position.y) ^2;
                    child.f = child.g + child.h;

                    bool isInOpenList = false;
                    foreach (Node openNode in openList)
                    {
                        if (child.ComaprePositions(openNode) && child.g > openNode.g)
                        {
                            isInOpenList = true;
                            break;
                        }
                        else if(child.ComaprePositions(openNode))
                        {
                            openNode.f = child.f;
                            openNode.g = child.g;
                            openNode.h = child.h;
                            openNode.parent = currentNode;
                            isInOpenList = true;
                            break;
                        }
                    }

                    if (isInOpenList)
                    {
                        continue;
                    }

                    openList.Add(child);
                }
            }
        }

        bool CheckNewPosition(Vector2Int start, Vector2Int nodePosition)
        {
            Vector3 pos = new Vector3(nodePosition.x + xFraction, y, nodePosition.y + zFraction);
            Collider[] hitColliders = Physics.OverlapSphere(pos, 0.01f);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                // TODO if no floor return true
                if (hitColliders[i].tag.Equals("Obstacle"))
                {
                    return true;
                }
            }

            if (Math.Abs(start.x - nodePosition.x) + Math.Abs(start.y - nodePosition.y) > rangeOfChase)
            {
                return true;
            }

            return false;
        }

        public List<Node> GetPath()
        {
            return path;
        }
    }
}
