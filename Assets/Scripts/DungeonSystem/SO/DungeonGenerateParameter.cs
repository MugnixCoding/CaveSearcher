using UnityEngine;

[CreateAssetMenu(fileName = "DungeonGenerateParameters", menuName = "PDG/DungeonGenerateData")]
public class DungeonGenerateParameter : ScriptableObject
{
    public int iterations = 10;
    public int walkLength = 10;
    public bool startRandomlyEachIteration = true;
}
