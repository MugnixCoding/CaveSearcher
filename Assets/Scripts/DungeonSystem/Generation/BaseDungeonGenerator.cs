
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;// Random conflict in Linq and UnityEngine

public class BaseDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    protected DungeonGenerateParameter dungeonParameters;

    protected override void RunProceduralGeneration()
    {
        HashSet<Vector3Int> floorPosition = RunRandomWalk(dungeonParameters, dungeonBasePosition);
        visulizer.GenerateDungeonFloor(floorPosition);
        CreateWall(floorPosition);

        foreach (var position in floorPosition)
        {
            Debug.Log(position);
        }
    }

    protected override void RunProceduralGeneration(DungeonData data)
    {
        HashSet<Vector3Int> floorPosition = RunRandomWalk(dungeonParameters, dungeonBasePosition);
        visulizer.GenerateDungeonFloor(floorPosition);
        CreateWall(floorPosition);
        data.startRoomPosition = dungeonBasePosition;
        data.startRoomFaceDirction = Vector3Int.forward;
        data.roomPosition = new List<Vector3Int> { dungeonBasePosition };
    }

    /// <summary>
    /// Generate Room by RandomWalk Algorithm
    /// </summary>
    protected HashSet<Vector3Int> RunRandomWalk(DungeonGenerateParameter parameter, Vector3Int position)
    {
        Vector3Int currentPosition = position;
        HashSet<Vector3Int> floorPosition = new HashSet<Vector3Int>();
        for (int i = 0; i < parameter.iterations; i++)
        {
            HashSet<Vector3Int> path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameter.walkLength);
            floorPosition.UnionWith(path);
            if (parameter.startRandomlyEachIteration)
            {
                currentPosition = floorPosition.ElementAt(Random.Range(0, floorPosition.Count));
            }
        }
        return floorPosition;
    }
    protected void CreateWall(HashSet<Vector3Int> floorPosition)
    {
        foreach (Vector3Int direction in Direction2D.cardinalDirectionList)
        {
            HashSet<Vector3Int> wallPosition = FindDirectionWallPosition(floorPosition,direction);
            visulizer.GenerateDungeonWall(wallPosition, direction);
        }
        HashSet<Vector3Int> ForwadWallPosition = FindDirectionWallPosition(floorPosition, Vector3Int.forward);
        visulizer.GenerateDungeonWall(ForwadWallPosition, Vector3Int.forward);

        HashSet<Vector3Int> BackWallPosition = FindDirectionWallPosition(floorPosition, Vector3Int.back);
        visulizer.GenerateDungeonWall(BackWallPosition, Vector3Int.back);

        HashSet<Vector3Int> RightWallPosition = FindDirectionWallPosition(floorPosition, Vector3Int.right);
        visulizer.GenerateDungeonWall(RightWallPosition, Vector3Int.right);

        HashSet<Vector3Int> LeftWallPosition = FindDirectionWallPosition(floorPosition, Vector3Int.left);
        HashSet<Vector3Int> LeftUpWallPosition = new HashSet<Vector3Int>(LeftWallPosition);

        LeftUpWallPosition.IntersectWith(BackWallPosition);
        LeftWallPosition.ExceptWith(LeftUpWallPosition);

        visulizer.GenerateDungeonWall(LeftWallPosition, Vector3Int.left);
        visulizer.GenerateDungeonCorner(LeftUpWallPosition, Vector3Int.left);



    }
    protected  HashSet<Vector3Int> FindDirectionWallPosition(HashSet<Vector3Int> floorPosition, Vector3Int direction)
    {
        HashSet<Vector3Int> wallPosition = new HashSet<Vector3Int>();
        foreach (Vector3Int position in floorPosition)
        {
            Vector3Int neighbourPosition = position + direction;
            if (!floorPosition.Contains(neighbourPosition))
            {
                wallPosition.Add(neighbourPosition);
            }
        }
        return wallPosition;
    }
}
