using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	private GameObject _currentWayPoint;
	private int _wayPointCount = 0;
	private Vector3 _moveDirection = Vector3.zero;
	private int _hp = 5;

	[HideInInspector]
	public GameObject[] WayPoints;
	public int MaxHp = 5;
	public float MoveSpeed = 10;
	public int StealCoin = 100;
	public int Damage = 1;

	private void Start()
	{
		_hp = MaxHp;
		_currentWayPoint = WayPoints[0];
		SetRotationByDirection();
	}

	private void Update()
	{
		transform.position += _moveDirection * MoveSpeed * Time.deltaTime;

		Vector3 TargetPosition = _currentWayPoint.transform.position;
		TargetPosition.y = transform.position.y;

		if (Vector3.Distance(transform.position, TargetPosition) <= 0.02f)
		{
			if (_wayPointCount >= WayPoints.Length - 1)
			{
				//GameManager.Inst.playerCharacter.Damaged(Damage);
				Destroy(gameObject);
				return;
			}

			_wayPointCount = Mathf.Clamp(_wayPointCount + 1, 0, WayPoints.Length);
			_currentWayPoint = WayPoints[_wayPointCount];

			SetRotationByDirection();
		}
	}

	private void SetRotationByDirection()
	{
		_moveDirection = _currentWayPoint.transform.position - transform.position;
		_moveDirection.y = 0;
		_moveDirection.Normalize();

		transform.rotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Projectile"))
		{
			_hp = Mathf.Clamp(_hp - 1, 0, MaxHp);

			if (_hp <= 0)
			{
				gameObject.SetActive(false);
				//GameManager.Inst.EnemyDead(StealCoin);
				Destroy(gameObject);
			}
		}
	}
}
