using System.Collections.Generic;
using UnityEngine;
public class DungeonData {
    public Vector3Int startRoomPosition { get; set; }
    public Vector3Int startRoomFaceDirction { get; set; }
    public int scaleSize { get; set; }
    public List<Vector3Int> roomPosition { get; set; }//not include start room
    public Vector3Int exitPosition { get; set; }
    public Vector3Int exitFaceDirction { get; set; }
    public float DungeonScaleSize { get; set; }
}
