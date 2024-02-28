using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static System.Net.WebRequestMethods;

[CreateAssetMenu(fileName = "GuardianStatus", menuName = "Scriptable Object/GuardianStatus")]
public class GuardianStatus : ScriptableObject
{
	public float AttackCycleTime = 1f;
	public float AttackRadius = 5f;
	public int Damage = 1;
	public int MaxTargetCount = 1;
	public int UpgradeCost = 100;
	public Color Color = Color.white;
}

public class Guardian : MonoBehaviour
{
	private List<GameObject> _targetEnemys = new List<GameObject>();

	public GameObject Projectile;
	public GuardianStatus GuardianStatus;
	public MeshRenderer GuardianRenderer;

	public int Level = 0;

	void Start()
	{
		StartCoroutine(Attack());
		GetComponent<SphereCollider>().radius = GuardianStatus.AttackRadius;
	}

	#region Attack
	IEnumerator Attack()
	{
		if (_targetEnemys.Count > 0)
		{
			SearchEnemy();
			foreach (GameObject target in _targetEnemys)
			{
				SetRotationByDirection();

				GameObject projectileInst = Instantiate(Projectile, transform.position, Quaternion.identity);
				if (projectileInst != null)
				{
					projectileInst.GetComponent<Projectile>().Damage = GuardianStatus.Damage;
					projectileInst.GetComponent<Projectile>().Target = target;
				}
			}
		}

		yield return new WaitForSeconds(GuardianStatus.AttackCycleTime);

		StartCoroutine(Attack());
	}
	private void SetRotationByDirection()
	{
		Vector3 targetPos = _targetEnemys[0].transform.position;
		targetPos.y = transform.position.y;

		Vector3 dir = targetPos - transform.position;
		dir.Normalize();
		transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
	}
	void SearchEnemy()
	{
		int count = 0;

		List<GameObject> tempList = new List<GameObject>();
		foreach (GameObject target in _targetEnemys)
		{
			if (target != null)
			{
				tempList.Add(target);
				count++;
			}

			if (count >= GuardianStatus.MaxTargetCount)
			{
				break;
			}
		}

		_targetEnemys = tempList;
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			if (false == _targetEnemys.Contains(other.gameObject))
			{
				_targetEnemys.Add(other.gameObject);
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			if (true == _targetEnemys.Contains(other.gameObject))
			{
				_targetEnemys.Remove(other.gameObject);
			}
		}
	}
	#endregion

	#region Upgrade
	public void Upgrade(GuardianStatus status)
	{
		Level += 1;
		GuardianStatus = status;

		GetComponent<SphereCollider>().radius = GuardianStatus.AttackRadius;
		GuardianRenderer.materials[0].color = GuardianStatus.Color;
	}

	#endregion
}