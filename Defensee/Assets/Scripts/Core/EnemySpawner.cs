using System.Collections;
using System.Collections.Generic; // �� ���ӽ����̽��� ���׸�(Generic) �÷��ǰ� ���õ� ��ɵ��� ������ -> ����Ʈ(List), ��ųʸ�(Dictionary), ť(Queue), ����(Stack) ��� ���� Ŭ������ �����޾Ƽ� ����� �� �ְԲ� ��.
using UnityEngine; // ����Ƽ �������� ����ϴ� ����� ������
using UnityEngine.UI;

namespace EnumTypes
{	
	public enum MonsterName
	{
		Ori, Spe
	}
}


[CreateAssetMenu(fileName = "WaveInfo", menuName = "Scriptable Object/WaveInfo")]
public class WaveInfo : ScriptableObject
{
	public int WaveNumber;
	public int MonsterSpawnCount = 15;
	public int Originalgoblin = 13;
	public int Specialgoblin = 2;

}

public class EnemySpawner : MonoBehaviour
{
	private int currentWaveIndex = 0;
	public WaveInfo[] WaveScriptableObject;
	// ���� ������ ��ġ
	public Transform SpawnPosition;

	// ���� ���󰡾� �� ��������Ʈ �迭
	public GameObject[] WayPoints;

	// ������ �� ������
	public GameObject OriginalEnemy;
	public GameObject SpecialEnemy;

	// �� ���� �ֱ�
	public float SpawnCycleTime = 1f;
	public Text WaveNumber;


	// �� ���� ���� ���θ� ��Ÿ���� �÷���
	private bool _bCanSpawn = true;

	private void Start()
	{
		// ������ Ȱ��ȭ
		Activate();
	}

	// �����ʸ� Ȱ��ȭ�ϴ� �޼���
	public void Activate()
	{
		// SpawnEnemy �ڷ�ƾ ����
		StartCoroutine(SpawnEnemy());
	}

	// �����ʸ� ��Ȱ��ȭ�ϴ� �޼���
	public void DeActivate()
	{
		// SpawnEnemy �ڷ�ƾ ����
		StopCoroutine(SpawnEnemy());
	}

	IEnumerator SpawnEnemy()
	{
		// ���̺갡 �����ִ� ���� �ݺ�
		while (currentWaveIndex < WaveScriptableObject.Length)
		{
			WaveInfo currentWave = WaveScriptableObject[currentWaveIndex];
			int totalSpawnCount = currentWave.Originalgoblin + currentWave.Specialgoblin;

			// ���� ���̺��� ���� ��� ������ ������ �ݺ�
			for (int i = 0; i < totalSpawnCount; i++)
			{
				// ���� �ð� ���� ���
				yield return new WaitForSeconds(SpawnCycleTime);

				// EnemyPrefab�� SpawnPosition�� ��ġ�� �����ϰ� ȸ���� ������� ����
				GameObject enemyPrefab = i < currentWave.Originalgoblin ? OriginalEnemy : SpecialEnemy;
				GameObject enemyInst = Instantiate(enemyPrefab, SpawnPosition.position, Quaternion.identity);

				// ������ ������ Enemy ��ũ��Ʈ�� ����
				Enemy enemyCom = enemyInst.GetComponent<Enemy>();

				// Enemy ��ũ��Ʈ�� �����ϴ� ���
				if (enemyCom)
				{
					// ������ ���� ���� ��������Ʈ �迭�� ����
					enemyCom.WayPoints = WayPoints;
				}
			}

			// ���� ���̺�� �Ѿ�� ���� ���� �ð� ���
			yield return new WaitForSeconds(5.0f);  // ���÷� 5�� ���

			WaveNumber.text = $"Wave {currentWaveIndex + 2}";
			WaveNumber.gameObject.SetActive(true);

			yield return new WaitForSeconds(2.0f);  // ���÷� 5�� ���

			WaveNumber.gameObject.SetActive(false);

			// ���� ���̺�� �Ѿ
			currentWaveIndex++;
		}
	}

}
