using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookRainSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> prefabs;

    [Header("Spawn Area (centered on this object)")]
    public float areaSizeX = 10f;
    public float areaSizeZ = 10f;
    public float spawnHeightOffset = 0f;

    [Header("Timing")]
    public float minSpawnDelay = 0.2f;
    public float maxSpawnDelay = 1.0f;

    [Header("Book Force")]
    public float randomTorque = 5f;
    public float downwardForce = 10f; // kitapları aşağı doğru hızlandırır

    [Header("Cleanup")]
    public float destroyAfterSeconds = 10f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnOneBook();
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    void SpawnOneBook()
    {
        if (prefabs == null || prefabs.Count == 0) return;

        GameObject selectedPrefab = prefabs[Random.Range(0, prefabs.Count)];
        if (selectedPrefab == null) return;

        float randomX = Random.Range(-areaSizeX * 0.5f, areaSizeX * 0.5f);
        float randomZ = Random.Range(-areaSizeZ * 0.5f, areaSizeZ * 0.5f);

        Vector3 spawnPos = transform.position + new Vector3(randomX, spawnHeightOffset, randomZ);
        Quaternion randomRot = Random.rotation;

        GameObject spawned = Instantiate(selectedPrefab, spawnPos, randomRot);

        Rigidbody rb = spawned.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Kitabı aşağı doğru hızlandırır
            rb.AddForce(Vector3.down * downwardForce, ForceMode.Impulse);

            // Kitabın havada dönmesini sağlar
            Vector3 torque = Random.insideUnitSphere * randomTorque;
            rb.AddTorque(torque, ForceMode.Impulse);
        }

        Destroy(spawned, destroyAfterSeconds);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + new Vector3(0f, spawnHeightOffset, 0f);
        Gizmos.DrawWireCube(center, new Vector3(areaSizeX, 0.1f, areaSizeZ));
    }
}