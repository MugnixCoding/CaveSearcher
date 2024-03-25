
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected DungeonVisulizer visulizer = null;
    [SerializeField]
    protected Vector3Int dungeonBasePosition = Vector3Int.zero;

    public void GenerateDungeon()
    {
        visulizer.Clear();
        RunProceduralGeneration();

    }
    public void GenerateDungeon(DungeonData data)
    {
        visulizer.Clear();
        RunProceduralGeneration(data);

    }
    public void ClearDungeon()
    {
        visulizer.Clear();
    }
    protected abstract void RunProceduralGeneration();
    protected abstract void RunProceduralGeneration(DungeonData data);
}
