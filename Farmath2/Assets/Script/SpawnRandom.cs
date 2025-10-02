using UnityEngine;

public class SpawnRandom : MonoBehaviour
{
    public GameObject[] prefab;
    public Vector2 spawnPos, spawnPos2;
    public int spawnCount;
    void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject cloud = Instantiate(prefab[Random.Range(0, prefab.Length)], new Vector3(Random.Range(spawnPos.x, spawnPos2.x), Random.Range(spawnPos.y, spawnPos2.y)), Quaternion.identity);
            cloud.transform.localScale = Vector2.one * Random.Range(0.25f, 1f);
        }
    }
}
