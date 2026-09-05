using UnityEngine;

public class EnemyWaveMember : MonoBehaviour
{
    private WaveSpawner waveSpawner;

    private bool counted = false;

    public void Init(WaveSpawner spawner)
    {
        waveSpawner = spawner;
    }

    private void OnDestroy()
    {
        if (waveSpawner != null && !counted)
        {
            counted = true;

            waveSpawner.EnemyDie();
        }
    }
}