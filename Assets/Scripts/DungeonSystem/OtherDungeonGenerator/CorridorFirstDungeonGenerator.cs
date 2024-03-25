using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CorridorFirstDungeonGenerator : BaseDungeonGenerator
{
    [SerializeField]
    private int corridorLength = 14;
    [SerializeField]
    private int corridorCount = 5;
    [SerializeField]
    [Range(0.1f,1)]
    private float roomPercent=0.8f;


    protected override void RunProceduralGeneration()
    {
        CorridorFirstGenerate();
    }
    private void CorridorFirstGenerate()
    {
        HashSet<Vector3Int> floorPosition = new HashSet<Vector3Int>();
        HashSet<Vector3Int> potentailRoomPosition = new HashSet<Vector3Int>();  //every corridor end connect with a room
        List<List<Vector3Int>> corridors = CreateCorridors(floorPosition, potentailRoomPosition);

        HashSet<Vector3Int> roomPosition = CreateRooms(potentailRoomPosition);

        List<Vector3Int> deadEnds = FindAllDeadEnds(floorPosition);

        CreateRoomAtDeadEnd(deadEnds,roomPosition);

        floorPosition.UnionWith(roomPosition);

        for (int i =0;i<corridors.Count;i++)
        {
            corridors[i] = IncreaseCorridorSizeByOne(corridors[i]);
            //corridors[i] = IncreaseCorridorBrush3by3(corridors[i]);
            floorPosition.UnionWith(corridors[i]);
        }

        visulizer.GenerateDungeonFloor(floorPosition);
        //WallGenerator.CreateWalls(floorPosition, floorVisulizer);
    }

    /// <summary>
    /// Create A Room at every position in parameter
    /// </summary>
    private HashSet<Vector3Int> CreateRooms(HashSet<Vector3Int> potentailRoomPosition)
    {
        HashSet<Vector3Int> roomPositions = new HashSet<Vector3Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentailRoomPosition.Count * roomPercent);

        List<Vector3Int> roomToCreate = potentailRoomPosition.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var roomPosition in roomToCreate)
        {
            var roomFloor = RunRandomWalk(dungeonParameters, roomPosition);
            roomPositions.UnionWith(roomFloor);
        }
        return roomPositions;
    }

    #region creat room at corridor dead end

    /// <summary>
    /// Find All Corridor dead end (a end without a room)
    /// </summary>
    private List<Vector3Int> FindAllDeadEnds(HashSet<Vector3Int> floorPosition)
    {
        List<Vector3Int> deadEnds = new List<Vector3Int>();
        foreach(var position in floorPosition)
        {
            int neighbourCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionList)
            {
                if (floorPosition.Contains(position+direction))
                {
                    neighbourCount++;
                }
            }
            if (neighbourCount == 1)
            {
                deadEnds.Add(position);
            }
        }
        return deadEnds;
    }

    /// <summary>
    /// create room at corridor dead end or it's already a room exist
    /// </summary>
    private void CreateRoomAtDeadEnd(List<Vector3Int> deadEnds, HashSet<Vector3Int> roomPosition)
    {
        foreach (var position in deadEnds)
        {
            if (!roomPosition.Contains(position))
            {
                var newRoom = RunRandomWalk(dungeonParameters, position);
                roomPosition.UnionWith(newRoom);
            }
        }
    }

    #endregion

    #region width of corridor

    /// <summary>
    /// Widen the corridor width by 1
    /// </summary>
    private List<Vector3Int> IncreaseCorridorSizeByOne(List<Vector3Int> corridor)
    {
        List<Vector3Int> newCorridor = new List<Vector3Int>();
        Vector3Int previewDirection = Vector3Int.zero;
        for (int i = 1;i<corridor.Count;i++)
        {
            Vector3Int directionFromCell = corridor[i] - corridor[i - 1];
            if (previewDirection!=Vector3Int.zero &&
                directionFromCell!=previewDirection)
            {
                //handle corner
                for (int x = -1;x<2;x++)
                {
                    for (int z=-1;z<2;z++)
                    {
                        newCorridor.Add(corridor[i - 1] + new Vector3Int(x, 0,z));
                    }
                }
                previewDirection = directionFromCell;
            }
            else
            {
                //add a single cell in the direction + 90 degree
                Vector3Int newCorridorTileOffset = GetDirection90From(directionFromCell);
                newCorridor.Add(corridor[i - 1]);
                newCorridor.Add(corridor[i - 1] + newCorridorTileOffset);
            }
        }
        return newCorridor;
    }
    public Vector3Int GetDirection90From(Vector3Int direction)
    {
        if (direction == Vector3Int.up)
        {
            return Vector3Int.right;
        }
        else if (direction == Vector3Int.down)
        {
            return Vector3Int.left;
        }
        else if (direction == Vector3Int.right)
        {
            return Vector3Int.down;
        }
        else if (direction == Vector3Int.left)
        {
            return Vector3Int.up;
        }
        return Vector3Int.zero;
    }

    /// <summary>
    /// Widen the corridor width 3 by 3
    /// </summary>
    private List<Vector3Int> IncreaseCorridorBrush3by3(List<Vector3Int> corridor)
    {
        List<Vector3Int> newCorridor = new List<Vector3Int>();
        for (int i =1;i<corridor.Count; i++)
        {
            for (int x =-1;x<2;x++)
            {
                for (int z = -1;z<2;z++)
                {
                    newCorridor.Add(corridor[i - 1] + new Vector3Int(x,0, z));
                }
            }
        }
        return newCorridor;
    }

    #endregion

    /// <summary>
    /// Create Corridors and Record every end of Corridors as Potential Room Position
    /// </summary>
    private List<List<Vector3Int>> CreateCorridors(HashSet<Vector3Int> corridorfloorPosition, HashSet<Vector3Int> potentailRoomPosition)
    {
        var currentPosition = dungeonBasePosition;
        potentailRoomPosition.Add(currentPosition);
        
        List<List<Vector3Int>> corridors = new List<List<Vector3Int>>();

        for (int i= 0;i < corridorCount;i++)
        {
            var path = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            corridors.Add(path);
            currentPosition = path[path.Count - 1];
            potentailRoomPosition.Add(currentPosition);
            corridorfloorPosition.UnionWith(path);
        }
        return corridors;
    }
}
