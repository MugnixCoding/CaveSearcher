using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : BaseDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 4;
    [SerializeField]
    private int minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20;
    [SerializeField]
    private int dungeonHeight = 20;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRoom = false;
    [SerializeField]
    private int dungeonScaleSize = 2;
    [SerializeField]
    private int startRoomDistant = 10;

    private int startRoomOffset = 3;

    protected override void RunProceduralGeneration()
    {
        CreateRoom();
    }
    protected override void RunProceduralGeneration(DungeonData data)
    {
        CreateRoom(data);
    }
    private void CreateRoom()
    {
        visulizer.InitDungeonScale();
        var roomList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt(dungeonBasePosition, new Vector3Int(dungeonWidth, 0, dungeonHeight)),
            minRoomWidth,
            minRoomHeight);

        HashSet<Vector3Int> floorPosition = new HashSet<Vector3Int>();
        if (randomWalkRoom)
        {
            floorPosition = CreateRandomRoom(roomList);
        }
        else
        {
            floorPosition = CreateSimpleRoom(roomList);
        }

        List<Vector3Int> roomCenterPosition = new List<Vector3Int>();
        foreach (var room in roomList)
        {
            roomCenterPosition.Add(new Vector3Int(Mathf.RoundToInt(room.center.x), 0, Mathf.RoundToInt(room.center.z)));
        }

        Vector3Int startRoomConnectTilePosition = CreateStartRoomConnectTile(roomCenterPosition);
        roomCenterPosition.Add(startRoomConnectTilePosition);
        floorPosition.Add(startRoomConnectTilePosition);
        HashSet<Vector3Int> corridorPosition = ConnectRooms(floorPosition,roomCenterPosition);
        floorPosition.UnionWith(corridorPosition);

        Vector3Int startRoomPosition = Vector3Int.zero;
        Vector3Int startRoomConnectDirection = Vector3Int.zero;
        Vector3Int exitPosition = Vector3Int.zero;
        float startRoomYAngle = StartRoomFaceAngle(corridorPosition, startRoomConnectTilePosition, out startRoomPosition, out startRoomConnectDirection,
            out exitPosition);

        visulizer.GenerateDungeonFloor(floorPosition);
        visulizer.GenerateDungeonCeiling(floorPosition);
        //start room position wall need special treatment or it will block the way
        CreateWall(floorPosition, startRoomConnectTilePosition,startRoomConnectDirection);
        visulizer.GenerateStartRoom(startRoomPosition, startRoomYAngle,exitPosition);


        visulizer.SetDungeonScale(dungeonScaleSize);
    }
    private void CreateRoom(DungeonData data)
    {
        visulizer.InitDungeonScale();
        var roomList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt(dungeonBasePosition, new Vector3Int(dungeonWidth, 0, dungeonHeight)),
            minRoomWidth,
            minRoomHeight);

        HashSet<Vector3Int> floorPosition = new HashSet<Vector3Int>();
        if (randomWalkRoom)
        {
            floorPosition = CreateRandomRoom(roomList);
        }
        else
        {
            floorPosition = CreateSimpleRoom(roomList);
        }

        List<Vector3Int> roomCenterPosition = new List<Vector3Int>();
        foreach (var room in roomList)
        {
            roomCenterPosition.Add(new Vector3Int(Mathf.RoundToInt(room.center.x), 0, Mathf.RoundToInt(room.center.z)));
        }

        Vector3Int startRoomConnectTilePosition = CreateStartRoomConnectTile(roomCenterPosition);
        roomCenterPosition.Add(startRoomConnectTilePosition);
        floorPosition.Add(startRoomConnectTilePosition);
        HashSet<Vector3Int> corridorPosition = ConnectRooms(floorPosition, roomCenterPosition);
        floorPosition.UnionWith(corridorPosition);

        Vector3Int startRoomPosition = Vector3Int.zero;
        Vector3Int startRoomConnectDirection = Vector3Int.zero;
        Vector3Int exitPosition = Vector3Int.zero;
        float startRoomYAngle = StartRoomFaceAngle(corridorPosition, startRoomConnectTilePosition, out startRoomPosition, out startRoomConnectDirection,
            out exitPosition);

        visulizer.GenerateDungeonFloor(floorPosition);
        visulizer.GenerateDungeonCeiling(floorPosition);
        //start room position wall need special treatment or it will block the way
        CreateWall(floorPosition, startRoomConnectTilePosition, startRoomConnectDirection);
        visulizer.GenerateStartRoom(startRoomPosition, startRoomYAngle, exitPosition);


        visulizer.SetDungeonScale(dungeonScaleSize);

        roomCenterPosition.Remove(startRoomConnectTilePosition);
        #region Integrate  data
        data.startRoomPosition = startRoomPosition;
        data.startRoomFaceDirction = startRoomConnectDirection;
        data.scaleSize = dungeonScaleSize;
        data.roomPosition = new List<Vector3Int>(roomCenterPosition);
        data.exitPosition = exitPosition;
        data.exitFaceDirction = startRoomConnectDirection;
        data.DungeonScaleSize = dungeonScaleSize;
        #endregion
    }

    private HashSet<Vector3Int> CreateRandomRoom(List<BoundsInt> roomList)
    {
        HashSet<Vector3Int> floor = new HashSet<Vector3Int>();
        foreach (var room in roomList)
        {
            Vector3Int roomCenter = new Vector3Int(Mathf.RoundToInt(room.center.x), 0, Mathf.RoundToInt(room.center.z));
            HashSet<Vector3Int> roomFloor = RunRandomWalk(dungeonParameters, roomCenter);
            //check if random walk generate floor is out of limit of room
            foreach (Vector3Int position in roomFloor)
            {
                if (position.x >= (room.xMin + offset) && position.x <= (room.xMax - offset) &&
                    position.z >= (room.zMin - offset) && position.z <= (room.zMax + offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector3Int> CreateSimpleRoom(List<BoundsInt> roomList)
    {
        HashSet<Vector3Int> floor = new HashSet<Vector3Int>();
        foreach (var room in roomList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.z - offset; row++)
                {
                    Vector3Int position = room.min + new Vector3Int(col, 0, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector3Int> ConnectRooms(HashSet<Vector3Int> floorPosition, List<Vector3Int> roomCenterPosition)
    {
        HashSet<Vector3Int> corridors = new HashSet<Vector3Int>();
        List<Vector3Int> copyRoomPosition = new List<Vector3Int>(roomCenterPosition);
        var currentRoomCenter = copyRoomPosition[Random.Range(0, copyRoomPosition.Count)];
        copyRoomPosition.Remove(currentRoomCenter);
        while (copyRoomPosition.Count > 0)
        {
            Vector3Int closest = FindClosestPointTo(currentRoomCenter, copyRoomPosition);
            copyRoomPosition.Remove(closest);
            HashSet<Vector3Int> newCorridor = CreateCorridor(currentRoomCenter, closest, floorPosition);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private Vector3Int FindClosestPointTo(Vector3Int currentRoomCenter, List<Vector3Int> roomCenterPosition)
    {
        Vector3Int closest = Vector3Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenterPosition)
        {
            float currentDistance = Vector3Int.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }
    private HashSet<Vector3Int> CreateCorridor(Vector3Int currentRoomCenter, Vector3Int destination, HashSet<Vector3Int> floorPosition)
    {
        HashSet<Vector3Int> corridors = new HashSet<Vector3Int>();
        Vector3Int position = currentRoomCenter;
        while (position.z != destination.z)
        {
            if (position.z < destination.z)
            {
                position += Vector3Int.forward;
            }
            else if (position.z > destination.z)
            {
                position += Vector3Int.back;
            }
            if (!floorPosition.Contains(position))
            {
                corridors.Add(position);
            }
        }

        while (position.x != destination.x)
        {
            if (position.x < destination.x)
            {
                position += Vector3Int.right;
            }
            else if (position.x > destination.x)
            {
                position += Vector3Int.left;
            }
            if (!floorPosition.Contains(position))
            {
                corridors.Add(position);
            }
        }
        return corridors;
    }
    protected void CreateWall(HashSet<Vector3Int> floorPosition, Vector3Int startRoomConnectTilePosition, Vector3Int startRoomConnectDirection)
    {
        HashSet<Vector3Int> ForwadWallPosition = FindDirectionWallPosition(floorPosition, Vector3Int.forward, startRoomConnectTilePosition, startRoomConnectDirection);
        visulizer.GenerateDungeonWall(ForwadWallPosition, Vector3Int.forward);

        HashSet<Vector3Int> BackWallPosition = FindDirectionWallPosition(floorPosition, Vector3Int.back, startRoomConnectTilePosition, startRoomConnectDirection);
        visulizer.GenerateDungeonWall(BackWallPosition, Vector3Int.back);

        HashSet<Vector3Int> RightWallPosition = FindDirectionWallPosition(floorPosition, Vector3Int.right, startRoomConnectTilePosition, startRoomConnectDirection);
        visulizer.GenerateDungeonWall(RightWallPosition, Vector3Int.right);

        HashSet<Vector3Int> LeftWallPosition = FindDirectionWallPosition(floorPosition, Vector3Int.left, startRoomConnectTilePosition, startRoomConnectDirection);
        HashSet<Vector3Int> LeftUpWallPosition = new HashSet<Vector3Int>(LeftWallPosition);

        LeftUpWallPosition.IntersectWith(BackWallPosition);
        LeftWallPosition.ExceptWith(LeftUpWallPosition);

        visulizer.GenerateDungeonWall(LeftWallPosition, Vector3Int.left);
        visulizer.GenerateDungeonCorner(LeftUpWallPosition, Vector3Int.left);



    }
    protected HashSet<Vector3Int> FindDirectionWallPosition(HashSet<Vector3Int> floorPosition, Vector3Int direction, Vector3Int startRoomConnectTilePosition,Vector3Int startRoomConnectDirection)
    {
        HashSet<Vector3Int> wallPosition = new HashSet<Vector3Int>();
        foreach (Vector3Int position in floorPosition)
        {
            if (position == startRoomConnectTilePosition)
            {
                if (direction==startRoomConnectDirection)
                {
                    continue;
                }
                Vector3Int neighbourPosition = position + direction;
                if (!floorPosition.Contains(neighbourPosition))
                {
                    wallPosition.Add(neighbourPosition);
                }
            }
            else
            {
                Vector3Int neighbourPosition = position + direction;
                if (!floorPosition.Contains(neighbourPosition))
                {
                    wallPosition.Add(neighbourPosition);
                }
            }

        }
        return wallPosition;
    }
    private Vector3Int CreateStartRoomConnectTile(List<Vector3Int> roomCenterPosition)
    {
        Vector3Int outermost = Vector3Int.zero;
        float minDistance = float.MaxValue;
        foreach (var position in roomCenterPosition)
        {
            float distanceX = Mathf.Min(Mathf.Abs(dungeonWidth+dungeonBasePosition.x-position.x), Mathf.Abs(position.x - dungeonBasePosition.x ));
            float distanceZ = Mathf.Min(Mathf.Abs(dungeonHeight + dungeonBasePosition.z - position.z), Mathf.Abs(position.z - dungeonBasePosition.z));
            float distance = Mathf.Min(distanceX, distanceZ);

            if (distance < minDistance)
            {
                minDistance = distance;
                if (distance == distanceX)
                {
                    outermost = position;
                    if (distance - startRoomDistant > dungeonBasePosition.x)
                    {
                        outermost.x = dungeonBasePosition.x + dungeonWidth + startRoomDistant;
                    }
                    else
                    {
                        outermost.x = dungeonBasePosition.x - startRoomDistant;
                    }
                }
                else
                {
                    outermost = position;
                    if (distance - startRoomDistant > dungeonBasePosition.z)
                    {
                        outermost.z = dungeonBasePosition.z +dungeonHeight + startRoomDistant;
                    }
                    else
                    {
                        outermost.z = dungeonBasePosition.z - startRoomDistant;
                    }
                }
            }
        }
        return outermost;
    }
    private float StartRoomFaceAngle(HashSet<Vector3Int> corridor,Vector3Int startRoomPosition,out Vector3Int fixPosition,out Vector3Int connectDirection,out Vector3Int exitPosition)
    {
        fixPosition = startRoomPosition;
        connectDirection = Vector3Int.forward;
        if (corridor.Contains(startRoomPosition + Vector3Int.right))
        {
            connectDirection = Vector3Int.left;
            fixPosition.x  -= startRoomOffset;
            exitPosition = fixPosition;
            exitPosition.x -= startRoomOffset;
            return 90;
        }
        else if (corridor.Contains(startRoomPosition + Vector3Int.left))
        {
            connectDirection = Vector3Int.right;
            fixPosition.x += startRoomOffset;
            exitPosition = fixPosition;
            exitPosition.x += startRoomOffset;
            return -90;
        }
        else if (corridor.Contains(startRoomPosition + Vector3Int.forward))
        {
            connectDirection = Vector3Int.back;
            fixPosition.z -= startRoomOffset;
            exitPosition = fixPosition;
            exitPosition.z -= startRoomOffset;
            return 0;
        }
        else if (corridor.Contains(startRoomPosition + Vector3Int.back))
        {
            connectDirection = Vector3Int.forward;
            fixPosition.z += startRoomOffset;
            exitPosition = fixPosition;
            exitPosition.z += startRoomOffset;
            return 180;
        }
        else
        {
            connectDirection = Vector3Int.left;
            fixPosition.x -= startRoomOffset;
            exitPosition = fixPosition;
            exitPosition.x -= startRoomOffset;
            return 0;
        }
    }
}
