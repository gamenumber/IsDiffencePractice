using System.Collections;
using System.Collections.Generic; // �� ���ӽ����̽��� ���׸�(Generic) �÷��ǰ� ���õ� ��ɵ��� ������ -> ����Ʈ(List), ��ųʸ�(Dictionary), ť(Queue), ����(Stack) ��� ���� Ŭ������ �����޾Ƽ� ����� �� �ְԲ� ��.
using UnityEngine; // ����Ƽ �������� ����ϴ� ����� ������

public class EnemySpawner : MonoBehaviour
{
	// ���� ������ ��ġ
	public Transform SpawnPosition;

	// ���� ���󰡾� �� ��������Ʈ �迭
	public GameObject[] WayPoints;

	// ������ �� ������
	public GameObject EnemyPrefab;

	// �� ���� �ֱ�
	public float SpawnCycleTime = 1f;

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

	// ���� ���� �ֱ�� �����ϴ� �ڷ�ƾ
	IEnumerator SpawnEnemy()
	{
		// �� ���� ���� ������ ���� �ݺ�
		while (_bCanSpawn)
		{
			// ���� �ð� ���� ���
			yield return new WaitForSeconds(SpawnCycleTime);

			// EnemyPrefab�� SpawnPosition�� ��ġ�� �����ϰ� ȸ���� ������� ����
			GameObject EnemyInst = Instantiate(EnemyPrefab, SpawnPosition.position, Quaternion.identity);

			// ������ ������ Enemy ��ũ��Ʈ�� ����
			Enemy EnemyCom = EnemyInst.GetComponent<Enemy>();

			// Enemy ��ũ��Ʈ�� �����ϴ� ���
			if (EnemyCom)
			{
				// ������ ���� ���� ��������Ʈ �迭�� ����
				EnemyCom.WayPoints = WayPoints;
			}
		}
	}
}
