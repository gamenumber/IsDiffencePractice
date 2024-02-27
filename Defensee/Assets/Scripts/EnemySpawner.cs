using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public Transform SpawnPosition;
	public GameObject[] WayPoints;
	public GameObject EnemyPrefab;
	public float SpawnCycleTime = 1f;

	private bool _bCanSpawn = true;

	private void Start()
	{
		Activate();
	}

	public void Activate()
	{
		StartCoroutine(SpawnEnemy());
	}

	public void DeActivate()
	{
		StopCoroutine(SpawnEnemy());
	}

	IEnumerator SpawnEnemy()
	{
		while (_bCanSpawn)
		{
			yield return new WaitForSeconds(SpawnCycleTime);

			GameObject EnemyInst = Instantiate(EnemyPrefab, SpawnPosition.position, Quaternion.identity);
			Enemy EnemyCom = EnemyInst.GetComponent<Enemy>();
			if (EnemyCom)
			{
				EnemyCom.WayPoints = WayPoints;
			}
		}
	}

}
