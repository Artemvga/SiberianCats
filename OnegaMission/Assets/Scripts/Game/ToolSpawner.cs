using System.Collections.Generic;
using Items;
using Player;
using UnityEngine;

public class ToolSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct ToolSpawnInfo
    {
        public ToolType toolType;
        public GameObject toolPrefab;
        public Transform spawnPoint;
    }

    [SerializeField] private List<ToolSpawnInfo> _toolsToSpawn;

    private void Start()
    {
        foreach (var info in _toolsToSpawn)
        {
            if (!PlayerTools.Instance.HasTool(info.toolType))
            {
                Instantiate(info.toolPrefab, info.spawnPoint.position, info.spawnPoint.rotation);
                Debug.Log($"Спавн инструмента {info.toolType}");
            }
        }
    }
}