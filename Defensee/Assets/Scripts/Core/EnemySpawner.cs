using System.Collections;
using System.Collections.Generic; // 이 네임스페이스는 제네릭(Generic) 컬렉션과 관련된 기능들을 제공함 -> 리스트(List), 딕셔너리(Dictionary), 큐(Queue), 스택(Stack) 등과 같은 클래스를 제공받아서 사용할 수 있게끔 함.
using UnityEngine; // 유니티 엔진에서 사용하는 기능을 제공함
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
	// 적을 생성할 위치
	public Transform SpawnPosition;

	// 적이 따라가야 할 웨이포인트 배열
	public GameObject[] WayPoints;

	// 생성할 적 프리팹
	public GameObject OriginalEnemy;
	public GameObject SpecialEnemy;

	// 적 생성 주기
	public float SpawnCycleTime = 1f;
	public Text WaveNumber;


	// 적 생성 가능 여부를 나타내는 플래그
	private bool _bCanSpawn = true;

	private void Start()
	{
		// 스포너 활성화
		Activate();
	}

	// 스포너를 활성화하는 메서드
	public void Activate()
	{
		// SpawnEnemy 코루틴 시작
		StartCoroutine(SpawnEnemy());
	}

	// 스포너를 비활성화하는 메서드
	public void DeActivate()
	{
		// SpawnEnemy 코루틴 정지
		StopCoroutine(SpawnEnemy());
	}

	IEnumerator SpawnEnemy()
	{
		// 웨이브가 남아있는 동안 반복
		while (currentWaveIndex < WaveScriptableObject.Length)
		{
			WaveInfo currentWave = WaveScriptableObject[currentWaveIndex];
			int totalSpawnCount = currentWave.Originalgoblin + currentWave.Specialgoblin;

			// 현재 웨이브의 적을 모두 생성할 때까지 반복
			for (int i = 0; i < totalSpawnCount; i++)
			{
				// 일정 시간 동안 대기
				yield return new WaitForSeconds(SpawnCycleTime);

				// EnemyPrefab을 SpawnPosition의 위치에 생성하고 회전은 사용하지 않음
				GameObject enemyPrefab = i < currentWave.Originalgoblin ? OriginalEnemy : SpecialEnemy;
				GameObject enemyInst = Instantiate(enemyPrefab, SpawnPosition.position, Quaternion.identity);

				// 생성된 적에게 Enemy 스크립트에 접근
				Enemy enemyCom = enemyInst.GetComponent<Enemy>();

				// Enemy 스크립트가 존재하는 경우
				if (enemyCom)
				{
					// 생성된 적이 따라갈 웨이포인트 배열을 설정
					enemyCom.WayPoints = WayPoints;
				}
			}

			// 다음 웨이브로 넘어가기 전에 일정 시간 대기
			yield return new WaitForSeconds(5.0f);  // 예시로 5초 대기

			WaveNumber.text = $"Wave {currentWaveIndex + 2}";
			WaveNumber.gameObject.SetActive(true);

			yield return new WaitForSeconds(2.0f);  // 예시로 5초 대기

			WaveNumber.gameObject.SetActive(false);

			// 다음 웨이브로 넘어감
			currentWaveIndex++;
		}
	}

}
