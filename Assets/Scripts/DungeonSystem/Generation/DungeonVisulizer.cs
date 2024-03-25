using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum DungeonStruct:int
{
    Ceiling=0,
    Wall=1,
    Floor=2
}

public class DungeonVisulizer : MonoBehaviour
{
    #region Parameters
    [SerializeField]
    private GameObject floorMap;
    [SerializeField]
    private GameObject[] floorTile;
    [SerializeField]
    private float floorTileSize = 4f;
    [SerializeField]
    private GameObject wallMap;
    [SerializeField]
    private GameObject[] wall;
    [SerializeField]
    private GameObject cornerWall;
    [SerializeField]
    private GameObject ceilingMap;
    [SerializeField]
    private GameObject[] ceilingTile;
    [SerializeField]
    private float ceilingHigh = 7f;
    [SerializeField]
    private GameObject[] startRoom;
    #endregion
    public float FloorTileSize => floorTileSize;

    public void GenerateDungeonFloor(IEnumerable<Vector3Int> floorPosition)
    {
        GenerateFloor(floorPosition, floorMap);
    }
    public void GenerateDungeonCeiling(IEnumerable<Vector3Int> ceilingPosition)
    {
        GenerateCeiling(ceilingPosition);
    }
    public void GenerateDungeonWall(IEnumerable<Vector3Int> wallPosition, Vector3Int direction)
    {
        GenerateWall(wallPosition, direction);
    }
    public void GenerateDungeonCorner(IEnumerable<Vector3Int> wallPosition, Vector3Int direction)
    {
        GenerateCorner(wallPosition, direction);
    }
    public void GenerateStartRoom(Vector3Int position,float faceyAngle,Vector3Int exitPosition)
    {
        GenerateSingleObject(position, ceilingMap, startRoom[(int)DungeonStruct.Ceiling], faceyAngle);
        GenerateSingleObject(position,wallMap, startRoom[(int)DungeonStruct.Wall], faceyAngle);
        GenerateSingleObject(position, floorMap, startRoom[(int)DungeonStruct.Floor], faceyAngle);
    }
    public void InitDungeonScale()
    {
        floorMap.transform.localScale = new Vector3(1, 1, 1);
        wallMap.transform.localScale = new Vector3(1, 1, 1);
        ceilingMap.transform.localScale = new Vector3(1, 1, 1);
        ceilingMap.transform.position = new Vector3(0, 0, 0);
    }
    public void SetDungeonScale(float scaleSize)
    {
        floorMap.transform.localScale = new Vector3(scaleSize, 1, scaleSize);
        wallMap.transform.localScale = new Vector3(scaleSize, scaleSize+0.5f, scaleSize);
        ceilingMap.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
        ceilingMap.transform.position = new Vector3(0, ceilingHigh * scaleSize, 0);
    }
    private void GenerateFloor(IEnumerable<Vector3Int> positions, GameObject floor)
    {
        foreach (var position in positions)
        {
            GenerateSingleTile(position, floor, floorTile[Random.Range(0, floorTile.Length)]);
        }
    }
    private void GenerateCeiling(IEnumerable<Vector3Int> positions)
    {
        foreach (var position in positions)
        {
            GenerateSingleTile(position, ceilingMap, ceilingTile[Random.Range(0, ceilingTile.Length)]);
        }
    }
    private void GenerateWall(IEnumerable<Vector3Int> positions,Vector3Int direction)
    {
        if (direction == Vector3Int.forward || direction == Vector3Int.back)
        {
            foreach (var position in positions)
            {
                GenerateSingelHorizontalWall(position, direction);
            }
        }
        else if (direction == Vector3Int.right || direction == Vector3Int.left)
        {
            foreach (var position in positions)
            {
                GenerateSingelVerticalWall(position, direction);
            }
        }
    }
    private void GenerateCorner(IEnumerable<Vector3Int> positions,Vector3Int direction)
    {
        foreach (var position in positions)
        {
            GenerateSingelCornerWall(position, direction);
        }
    }
    private void GenerateSingleTile(Vector3Int position, GameObject floor, GameObject tile)
    {
        Instantiate(tile, new Vector3(position.x, position.y,position.z) * floorTileSize, Quaternion.identity, floor.transform);
    }
    private void GenerateSingelCornerWall(Vector3Int position, Vector3Int dir)
    {
        Instantiate(cornerWall, new Vector3(position.x, position.y, position.z) * floorTileSize - (new Vector3(dir.x, dir.y, dir.z) * (floorTileSize / 2)), Quaternion.identity, wallMap.transform);
    }
    private void GenerateSingelVerticalWall(Vector3Int position, Vector3Int dir)
    {
        Instantiate(wall[Random.Range(0, wall.Length)], new Vector3(position.x, position.y, position.z) * floorTileSize - (new Vector3(dir.x, dir.y, dir.z) * (floorTileSize / 2)), Quaternion.identity, wallMap.transform);
    }
    private void GenerateSingelHorizontalWall(Vector3Int position, Vector3Int dir)
    {
        Instantiate(wall[Random.Range(0, wall.Length)], new Vector3(position.x, position.y, position.z) * floorTileSize - (new Vector3(dir.x, dir.y, dir.z) * (floorTileSize / 2)), Quaternion.Euler(0, 90, 0), wallMap.transform);
    }

    private void GenerateSingleObject(Vector3Int position, GameObject parent, GameObject obj, float faceyAngle)
    {
        Instantiate(obj, new Vector3(position.x, position.y, position.z) * floorTileSize, Quaternion.Euler(0,faceyAngle,0), parent.transform);
    }
    public void Clear()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            var tempFloorList = floorMap.transform.Cast<Transform>().ToList();
            foreach (Transform child in tempFloorList)
            {
                DestroyImmediate(child.gameObject);
            }
            var tempWallList = wallMap.transform.Cast<Transform>().ToList();
            foreach (Transform child in tempWallList)
            {
                DestroyImmediate(child.gameObject);
            }
            var tempCeilingList = ceilingMap.transform.Cast<Transform>().ToList();
            foreach (Transform child in tempCeilingList)
            {
                DestroyImmediate(child.gameObject);
            }
        }
        else
        {
            foreach (Transform child in floorMap.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in wallMap.transform)
            {
                DestroyImmediate(child.gameObject);
            }
            foreach (Transform child in ceilingMap.transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}
