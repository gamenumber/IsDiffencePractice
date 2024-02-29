using System.Collections;
using System.Collections.Generic; // 이 네임스페이스는 제네릭(Generic) 컬렉션과 관련된 기능들을 제공함 -> 리스트(List), 딕셔너리(Dictionary), 큐(Queue), 스택(Stack) 등과 같은 클래스를 제공받아서 사용할 수 있게끔 함.
using UnityEngine; // 유니티 엔진에서 사용하는 기능을 제공함

public class Enemy : MonoBehaviour
{
	// 현재 중간 지점(Waypoint)을 나타내는 게임 오브젝트
	private GameObject _currentWayPoint;

	// 현재 웨이포인트의 인덱스
	private int _wayPointCount = 0;

	// 이동 방향
	private Vector3 _moveDirection = Vector3.zero;

	// 적 체력
	private int _hp = 5;

	// 숨겨진(public으로 노출되지 않음) 변수: 웨이포인트 배열
	[HideInInspector]
	public GameObject[] WayPoints;

	// 적 최대 체력
	public int MaxHp = 5;

	// 이동 속도
	public float MoveSpeed = 10;

	// 플레이어에게 주는 피해
	public int Damage = 1;

	// 플레이어가 뺏어가는 코인의 양
	public int StealCoin = 100;

	private void Start()
	{
		// 초기화: 현재 체력을 최대 체력으로 설정
		_hp = MaxHp;

		// 초기화: 첫 번째 웨이포인트 설정
		_currentWayPoint = WayPoints[0];

		// 초기화: 이동 방향에 따른 회전 설정
		SetRotationByDirection();
	}

	private void Update()
	{
		// 적 이동 로직
		transform.position += _moveDirection * MoveSpeed * Time.deltaTime;

		// 목표 위치 설정
		Vector3 TargetPosition = _currentWayPoint.transform.position;
		TargetPosition.y = transform.position.y;

		// 만약 현재 위치가 TargetPosition에 도달했으면 
		if (Vector3.Distance(transform.position, TargetPosition) <= 0.02f)
		{
			// 마지막 웨이포인트에 도달한 경우
			if (_wayPointCount >= WayPoints.Length - 1)
			{
				// 플레이어에게 피해를 줌?
				GameManager.Inst.playerCharacter.Damaged(Damage);

				// 적 제거
				Destroy(gameObject);
				return;
			}

			// 다음 웨이포인트로 이동
			_wayPointCount = Mathf.Clamp(_wayPointCount + 1, 0, WayPoints.Length);
			_currentWayPoint = WayPoints[_wayPointCount];

			// 이동 방향에 따른 회전 설정
			SetRotationByDirection();
		}
	}

	private void SetRotationByDirection()
	{
	
		_moveDirection = _currentWayPoint.transform.position - transform.position; // 현재 웨이포인트(_currentWayPoint)의 위치에서 현재 위치(transform.position)를 빼서 _moveDirection을 구함
		_moveDirection.y = 0; // y는 0으로 제한
		_moveDirection.Normalize(); // 이동 방향 벡터(_moveDirection)를 정규화시켜서 방향만 나타내게 함.

		// 이동 방향에 따른 회전 설정
		transform.rotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
	}

	// 해당 코드는 3D에서 쓰임
	private void OnTriggerEnter(Collider other)
	{
		// Projectile과 충돌하면
		if (other.CompareTag("Projectile"))
		{
			// 현재 체력에서 1을 감소시킴, 그리고 체력이 0미만으로 내려가지 않도록하고, MaxHp를 초과하지 않도록 제한함.
			_hp = Mathf.Clamp(_hp - 1, 0, MaxHp);

			// 체력이 0 이하로 감소한 경우
			if (_hp <= 0)
			{
				// 적 비활성화 
				gameObject.SetActive(false);

				// 이건 잘 모르겠지만, StealCoin 이라는 것을 봐서 아마도 적이 죽었을 때 플레이어가 코인을 얻는 것 같음
				GameManager.Inst.EnemyDead(StealCoin);

				// 적 제거
				Destroy(gameObject);
			}
		}
	}
}
