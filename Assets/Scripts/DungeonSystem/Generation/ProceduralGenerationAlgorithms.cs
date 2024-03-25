

using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    #region RandomWalk
    public static HashSet<Vector3Int> SimpleRandomWalk(Vector3Int startPosition,int walkLength)
    {
        HashSet<Vector3Int> path = new HashSet<Vector3Int>();

        path.Add(startPosition);
        Vector3Int previousPosition = startPosition;

        for (int i = 0;i<walkLength;i++)
        {
            Vector3Int newPosition = previousPosition + Direction2D.GetRandomCarinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }
        return path;
    }
    public static List<Vector3Int> RandomWalkCorridor(Vector3Int startPosition, int corridorLength)
    {
        List<Vector3Int> corridor = new List<Vector3Int>();
        Vector3Int direction = Direction2D.GetRandomCarinalDirection();
        Vector3Int currentPosition = startPosition;
        corridor.Add(currentPosition);

        for (int i = 0; i<corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }
        return corridor;
    }
    #endregion

    #region Binary Space Partition Algorithm

    /// <summary>
    /// split space into min width and height
    /// </summary>
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit,int minWidth,int minHeight)
    {
        Queue<BoundsInt> roomQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomList = new List<BoundsInt>();

        roomQueue.Enqueue(spaceToSplit);
        while (roomQueue.Count>0)
        {
            BoundsInt room = roomQueue.Dequeue();
            if (room.size.z >= minHeight && room.size.x>=minWidth)
            {
                if (Random.value < 0.5f)
                {
                    if (room.size.z>=minHeight*2)
                    {
                        SplitHorizontally(minHeight,roomQueue,room);
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomQueue, room);
                    }
                    else if(room.size.z >= minHeight && room.size.x >= minWidth)
                    {
                        roomList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomQueue, room);
                    }
                    else if (room.size.z >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomQueue, room);
                    }
                    else if (room.size.z >= minHeight && room.size.x >= minWidth)
                    {
                        roomList.Add(room);
                    }
                }
            }
        }
        return roomList;
    }

    /// <summary>
    /// split a room into two vertically
    /// </summary>
    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomQueue, BoundsInt room)
    {
        int xSplit = Random.Range(1,room.size.x);
        BoundsInt roomA = new BoundsInt(room.min,new Vector3Int(xSplit,room.size.y,room.size.z));
        BoundsInt roomB = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomQueue.Enqueue(roomA);
        roomQueue.Enqueue(roomB);
    }

    /// <summary>
    /// split a room into two horizontally
    /// </summary>
    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomQueue, BoundsInt room)
    {
        int zSplit = Random.Range(1, room.size.z);
        BoundsInt roomA = new BoundsInt(room.min, new Vector3Int(room.size.x, room.size.y ,zSplit));
        BoundsInt roomB = new BoundsInt(new Vector3Int(room.min.x, room.min.y, room.min.z + zSplit),
            new Vector3Int(room.size.x, room.size.y, room.size.z - zSplit));
        roomQueue.Enqueue(roomA);
        roomQueue.Enqueue(roomB);
    }

    #endregion

    #region Dijkastra
    
    #endregion
}

public static class Direction2D
{
    public static List<Vector3Int> cardinalDirectionList = new List<Vector3Int>
    {
        new Vector3Int(0,0,1),//forward (up)
        new Vector3Int(0,0,-1),//back   (down)
        new Vector3Int(1,0,0),//right
        new Vector3Int(-1,0,0),//left
    };
    public static Vector3Int GetRandomCarinalDirection()
    {
        return cardinalDirectionList[Random.Range(0, cardinalDirectionList.Count)];
    }
}
