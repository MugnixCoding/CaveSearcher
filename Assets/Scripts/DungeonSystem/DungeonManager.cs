using System;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonManager : MonoBehaviour
{
    [SerializeField]
    private AbstractDungeonGenerator dungeonGenerator;
    [SerializeField]
    private DungeonVisulizer visulizer;
    [SerializeField]
    private NavMeshSurface navMeshSurface;
    [SerializeField]
    private DungeonAudioController audioController;

    [SerializeField]
    private CharacterController player;

    [SerializeField]
    private Monster monster;

    [SerializeField]
    private GameObject chest;

    [SerializeField]
    private GameObject exit;

    private DungeonData dungeonData;

    private float DungeonSizeOffeset = 0;

    private void Awake()
    {
        dungeonData = new DungeonData();
    }
    private void Start()
    {
        ClearDungeon();
        GenerateDungeon();
        player.GetComponent<PlayerHealthState>().OnDead += PlayerDead;
        exit.GetComponent<Exit>().OnSuccess += PlayerSuccess; ;
    }

    private void OnDestroy()
    {
        navMeshSurface.navMeshData = null;
    }
    #region Dungeon Generate
    public void GenerateDungeon()
    {
        monster.gameObject.SetActive(false);
        dungeonData = new DungeonData();
        dungeonGenerator.GenerateDungeon(dungeonData);
        DungeonSizeOffeset = dungeonData.scaleSize * visulizer.FloorTileSize;
        for (int i=0; i< dungeonData.roomPosition.Count;i++)
        {
            dungeonData.roomPosition[i] *= (int)DungeonSizeOffeset;
        }

        NavMeshBake();
        DeployPlayer();
        DeployChest();
        DeployMonster();
        DeployExit();
        monster.gameObject.SetActive(true);
        audioController.PlayAmbient();
    }
    private void NavMeshBake()
    {
        navMeshSurface.RemoveData();
        navMeshSurface.BuildNavMesh();
    }
    private void DeployPlayer()
    {
        player.enabled = false;
        player.transform.position = (Vector3)dungeonData.startRoomPosition * DungeonSizeOffeset;
        player.transform.LookAt(player.transform.position - dungeonData.startRoomFaceDirction);
        player.enabled = true;
    }
    private void DeployMonster()
    {
        monster.transform.position = (Vector3)dungeonData.roomPosition[dungeonData.roomPosition.Count-1];
        while (Vector3.Distance(monster.transform.position, chest.transform.position)<1)
        {
            monster.transform.position = (Vector3)dungeonData.roomPosition[Random.Range(0,dungeonData.roomPosition.Count - 1)];
        }
        monster.SetRoomPosition(dungeonData.roomPosition);
    }
    private void DeployChest()
    {
        Vector3 chestPosition = Vector3.zero;
        float farest = -1;
        for (int i = 0;i<dungeonData.roomPosition.Count;i++)
        {
            float currentDistance = Vector3.Distance(dungeonData.startRoomPosition, dungeonData.roomPosition[i]);
            if (currentDistance > farest)
            {
                chestPosition = dungeonData.roomPosition[i];
                farest = currentDistance;
            }
        }
        chest.transform.position = chestPosition;
    }
    private void DeployExit()
    {
        exit.transform.position = (Vector3)dungeonData.exitPosition * DungeonSizeOffeset;
        exit.transform.LookAt(exit.transform.position - dungeonData.exitFaceDirction);
        exit.transform.localScale = new Vector3(dungeonData.DungeonScaleSize, dungeonData.DungeonScaleSize+0.5f, dungeonData.DungeonScaleSize);
    }
    public void ClearDungeon()
    {
        dungeonGenerator.ClearDungeon();
        navMeshSurface.RemoveData();
        navMeshSurface.navMeshData = null;
    }
    #endregion
    private void PlayerDead(object sender,EventArgs e)
    {
        Loader.Instance.Load(Loader.Scene.FailScene,false);
    }

    private void PlayerSuccess(object sender, EventArgs e)
    {
        Loader.Instance.Load(Loader.Scene.SuccessScene, false);
    }
}
