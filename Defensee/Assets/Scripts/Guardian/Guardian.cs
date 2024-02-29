using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 2-0. 스크립터블 오브젝트는 무엇일까?, 어떻게 만들고? 어떻게 사용할까? 공부하기 ->

스크립터블 오브젝트 (ScriptableObject): Unity에서 데이터를 효율적으로 저장하고 관리하기 위한 클래스로, 프로젝트 설정, 리소스 데이터, 게임 설정 등을 저장할 때 사용됩니다.
만들기: 프로젝트 창에서 우클릭 -> Create -> Scriptable Object 선택하여 생성하며, C# 클래스를 정의하여 데이터 구조를 생성합니다.
사용하기: Inspector에서 해당 오브젝트를 생성하고, 코드에서 해당 스크립터블 오브젝트에 접근하여 데이터를 읽거나 쓸 수 있습니다. 주로 데이터 관리와 조직화에 활용됩니다.
2-1. Guardian의 SearchEnemy는 어떻게 작동하고 있을까? ->

SearchEnemy 함수는 Guardian이 현재 감지한 적들 중에서 최대 타겟 수(MaxTargetCount)에 따라 타겟을 선택합니다.
최대 타겟 수에 도달하면 더 이상 새로운 타겟을 추가하지 않고, 이미 감지된 적 중에서 타겟 수만큼만을 유지합니다.
2-2. OnTriggerStay, Exit는 왜 만들었을까? ->

OnTriggerStay는 Guardian이 적에게 접촉 중일 때 호출되며, 감지된 적을 _targetEnemys 리스트에 추가합니다.
OnTriggerExit는 Guardian이 적과의 접촉이 끝날 때 호출되어, _targetEnemys 리스트에서 해당 적을 제거합니다.
두 함수를 통해 Guardian은 주변의 적을 지속적으로 감지하고, 이 정보를 업데이트하여 유지합니다.
2-3. Guardian들의 공격 범위는 어떻게 설정하고 있을까? ->

Guardian의 공격 범위는 GetComponent<SphereCollider>().radius = GuardianStatus.AttackRadius; 코드를 사용하여 설정됩니다.
SphereCollider 컴포넌트의 반지름이 GuardianStatus.AttackRadius 값으로 설정되어, 해당 범위 내에 있는 적을 감지할 수 있게 됩니다.
 */

// Guardian의 속성을 담은 ScriptableObject 클래스
[CreateAssetMenu(fileName = "GuardianStatus", menuName = "Scriptable Object/GuardianStatus")]
public class GuardianStatus : ScriptableObject
{
	// Guardian의 공격 주기
	public float AttackCycleTime = 1f;

	// Guardian의 공격 범위
	public float AttackRadius = 5f;

	// Guardian의 공격 데미지
	public int Damage = 1;

	// Guardian이 동시에 타겟팅할 수 있는 최대 적의 수
	public int MaxTargetCount = 1;

	// Guardian 업그레이드 비용
	public int UpgradeCost = 100;

	// Guardian의 색상
	public Color Color = Color.white;
}


public class Guardian : MonoBehaviour
{
	private List<GameObject> _targetEnemys = new List<GameObject>(); // 감지된 적을 저장하는 리스트

	public GameObject Projectile; // Guardian이 발사하는 포탄의 프리팹
	public GuardianStatus GuardianStatus; // Guardian의 속성을 담은 ScriptableObject
	public MeshRenderer GuardianRenderer; // Guardian의 렌더러 컴포넌트

	public int Level = 0; // Guardian의 레벨

	void Start()
	{
		StartCoroutine(Attack()); // 일정 주기로 적을 공격하는 코루틴 시작
		GetComponent<SphereCollider>().radius = GuardianStatus.AttackRadius; // SphereCollider의 반지름 설정
	}


	#region Attack
	// 일정 주기로 적을 공격하는 코루틴 함수
	IEnumerator Attack()
	{
		if (_targetEnemys.Count > 0) // 감지된 적이 존재하는 경우
		{
			SearchEnemy(); // 현재 감지된 적 업데이트
			foreach (GameObject target in _targetEnemys) // 모든 감지된 적에 대해 반복
			{
				SetRotationByDirection(); // 주어진 방향으로 회전 설정

				// 포탄 생성
				GameObject projectileInst = Instantiate(Projectile, transform.position, Quaternion.identity);
				if (projectileInst != null)
				{
					// 포탄에 데미지와 대상 설정
					projectileInst.GetComponent<Projectile>().Damage = GuardianStatus.Damage;
					projectileInst.GetComponent<Projectile>().Target = target;
				}
			}
		}

		yield return new WaitForSeconds(GuardianStatus.AttackCycleTime); // 주어진 시간 동안 대기

		StartCoroutine(Attack()); // 코루틴을 호출하여 일정 주기마다 공격 수행
	}


	// 주어진 방향에 따라 Guardian의 회전을 설정하는 함수
	private void SetRotationByDirection()
	{
		Vector3 targetPos = _targetEnemys[0].transform.position;
		targetPos.y = transform.position.y; // Guardian의 높이를 고려하여 y 좌표를 일치시킴

		Vector3 dir = targetPos - transform.position; // Guardian에서 타겟까지의 방향 벡터 계산
		dir.Normalize(); // 방향 벡터를 정규화하여 길이가 1이 되도록 조정

		// 방향 벡터에 따라 Guardian의 회전을 설정
		transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
	}


	// 적을 찾아 감지 목록을 업데이트하는 함수
	void SearchEnemy()
	{
		int count = 0; // 찾은 적의 수를 기록하는 변수

		List<GameObject> tempList = new List<GameObject>(); // 새로운 감지 목록을 임시로 저장할 리스트

		foreach (GameObject target in _targetEnemys)
		{
			if (target != null) // 타겟이 존재하는 경우
			{
				tempList.Add(target); // 임시 리스트에 타겟 추가
				count++; // 찾은 적의 수 증가
			}

			if (count >= GuardianStatus.MaxTargetCount) // 설정된 최대 타겟 수에 도달하면 더 이상 찾지 않음
			{
				break;
			}
		}

		_targetEnemys = tempList; // 새로운 감지 목록으로 기존 목록을 업데이트
	}



	// 특정 범위 내에 적이 머무르면 감지 목록에 추가하는 함수
	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Enemy")) // 충돌한 대상이 "Enemy" 태그를 가진 오브젝트인지 확인
		{
			if (!_targetEnemys.Contains(other.gameObject)) // 감지 목록에 해당 오브젝트가 포함되어 있지 않은 경우
			{
				_targetEnemys.Add(other.gameObject); // 감지 목록에 해당 오브젝트 추가
			}
		}
	}


	// 특정 범위 내에서 적이 나가면 감지 목록에서 제거하는 함수
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Enemy")) // 충돌한 대상이 "Enemy" 태그를 가진 오브젝트인지 확인
		{
			if (_targetEnemys.Contains(other.gameObject)) // 감지 목록에 해당 오브젝트가 포함되어 있는지 확인
			{
				_targetEnemys.Remove(other.gameObject); // 감지 목록에서 해당 오브젝트를 제거
			}
		}
	}
	#endregion

	#region Upgrade

	// Guardian을 업그레이드하는 함수
	public void Upgrade(GuardianStatus status)
	{
		Level += 1; // Guardian 레벨 증가
		GuardianStatus = status; // Guardian의 상태를 새로운 상태로 업데이트

		GetComponent<SphereCollider>().radius = GuardianStatus.AttackRadius; // SphereCollider의 반경을 새로운 상태의 공격 반경으로 설정
		GuardianRenderer.materials[0].color = GuardianStatus.Color; // Guardian의 렌더러의 색상을 새로운 상태의 색상으로 설정
	}

	#endregion
}