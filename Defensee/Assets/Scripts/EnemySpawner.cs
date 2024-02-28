using System.Collections;
using System.Collections.Generic; // 이 네임스페이스는 제네릭(Generic) 컬렉션과 관련된 기능들을 제공함 -> 리스트(List), 딕셔너리(Dictionary), 큐(Queue), 스택(Stack) 등과 같은 클래스를 제공받아서 사용할 수 있게끔 함.
using UnityEngine; // 유니티 엔진에서 사용하는 기능을 제공함

public class EnemySpawner : MonoBehaviour
{
	// 적을 생성할 위치
	public Transform SpawnPosition;

	// 적이 따라가야 할 웨이포인트 배열
	public GameObject[] WayPoints;

	// 생성할 적 프리팹
	public GameObject EnemyPrefab;

	// 적 생성 주기
	public float SpawnCycleTime = 1f;

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

	// 적을 일정 주기로 생성하는 코루틴
	IEnumerator SpawnEnemy()
	{
		// 적 생성 가능 상태인 동안 반복
		while (_bCanSpawn)
		{
			// 일정 시간 동안 대기
			yield return new WaitForSeconds(SpawnCycleTime);

			// EnemyPrefab을 SpawnPosition의 위치에 생성하고 회전은 사용하지 않음
			GameObject EnemyInst = Instantiate(EnemyPrefab, SpawnPosition.position, Quaternion.identity);

			// 생성된 적에게 Enemy 스크립트에 접근
			Enemy EnemyCom = EnemyInst.GetComponent<Enemy>();

			// Enemy 스크립트가 존재하는 경우
			if (EnemyCom)
			{
				// 생성된 적이 따라갈 웨이포인트 배열을 설정
				EnemyCom.WayPoints = WayPoints;
			}
		}
	}
}
