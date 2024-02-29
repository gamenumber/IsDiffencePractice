using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 2-0. ��ũ���ͺ� ������Ʈ�� �����ϱ�?, ��� �����? ��� ����ұ�? �����ϱ� ->

��ũ���ͺ� ������Ʈ (ScriptableObject): Unity���� �����͸� ȿ�������� �����ϰ� �����ϱ� ���� Ŭ������, ������Ʈ ����, ���ҽ� ������, ���� ���� ���� ������ �� ���˴ϴ�.
�����: ������Ʈ â���� ��Ŭ�� -> Create -> Scriptable Object �����Ͽ� �����ϸ�, C# Ŭ������ �����Ͽ� ������ ������ �����մϴ�.
����ϱ�: Inspector���� �ش� ������Ʈ�� �����ϰ�, �ڵ忡�� �ش� ��ũ���ͺ� ������Ʈ�� �����Ͽ� �����͸� �аų� �� �� �ֽ��ϴ�. �ַ� ������ ������ ����ȭ�� Ȱ��˴ϴ�.
2-1. Guardian�� SearchEnemy�� ��� �۵��ϰ� ������? ->

SearchEnemy �Լ��� Guardian�� ���� ������ ���� �߿��� �ִ� Ÿ�� ��(MaxTargetCount)�� ���� Ÿ���� �����մϴ�.
�ִ� Ÿ�� ���� �����ϸ� �� �̻� ���ο� Ÿ���� �߰����� �ʰ�, �̹� ������ �� �߿��� Ÿ�� ����ŭ���� �����մϴ�.
2-2. OnTriggerStay, Exit�� �� ���������? ->

OnTriggerStay�� Guardian�� ������ ���� ���� �� ȣ��Ǹ�, ������ ���� _targetEnemys ����Ʈ�� �߰��մϴ�.
OnTriggerExit�� Guardian�� ������ ������ ���� �� ȣ��Ǿ�, _targetEnemys ����Ʈ���� �ش� ���� �����մϴ�.
�� �Լ��� ���� Guardian�� �ֺ��� ���� ���������� �����ϰ�, �� ������ ������Ʈ�Ͽ� �����մϴ�.
2-3. Guardian���� ���� ������ ��� �����ϰ� ������? ->

Guardian�� ���� ������ GetComponent<SphereCollider>().radius = GuardianStatus.AttackRadius; �ڵ带 ����Ͽ� �����˴ϴ�.
SphereCollider ������Ʈ�� �������� GuardianStatus.AttackRadius ������ �����Ǿ�, �ش� ���� ���� �ִ� ���� ������ �� �ְ� �˴ϴ�.
 */

// Guardian�� �Ӽ��� ���� ScriptableObject Ŭ����
[CreateAssetMenu(fileName = "GuardianStatus", menuName = "Scriptable Object/GuardianStatus")]
public class GuardianStatus : ScriptableObject
{
	// Guardian�� ���� �ֱ�
	public float AttackCycleTime = 1f;

	// Guardian�� ���� ����
	public float AttackRadius = 5f;

	// Guardian�� ���� ������
	public int Damage = 1;

	// Guardian�� ���ÿ� Ÿ������ �� �ִ� �ִ� ���� ��
	public int MaxTargetCount = 1;

	// Guardian ���׷��̵� ���
	public int UpgradeCost = 100;

	// Guardian�� ����
	public Color Color = Color.white;
}


public class Guardian : MonoBehaviour
{
	private List<GameObject> _targetEnemys = new List<GameObject>(); // ������ ���� �����ϴ� ����Ʈ

	public GameObject Projectile; // Guardian�� �߻��ϴ� ��ź�� ������
	public GuardianStatus GuardianStatus; // Guardian�� �Ӽ��� ���� ScriptableObject
	public MeshRenderer GuardianRenderer; // Guardian�� ������ ������Ʈ

	public int Level = 0; // Guardian�� ����

	void Start()
	{
		StartCoroutine(Attack()); // ���� �ֱ�� ���� �����ϴ� �ڷ�ƾ ����
		GetComponent<SphereCollider>().radius = GuardianStatus.AttackRadius; // SphereCollider�� ������ ����
	}


	#region Attack
	// ���� �ֱ�� ���� �����ϴ� �ڷ�ƾ �Լ�
	IEnumerator Attack()
	{
		if (_targetEnemys.Count > 0) // ������ ���� �����ϴ� ���
		{
			SearchEnemy(); // ���� ������ �� ������Ʈ
			foreach (GameObject target in _targetEnemys) // ��� ������ ���� ���� �ݺ�
			{
				SetRotationByDirection(); // �־��� �������� ȸ�� ����

				// ��ź ����
				GameObject projectileInst = Instantiate(Projectile, transform.position, Quaternion.identity);
				if (projectileInst != null)
				{
					// ��ź�� �������� ��� ����
					projectileInst.GetComponent<Projectile>().Damage = GuardianStatus.Damage;
					projectileInst.GetComponent<Projectile>().Target = target;
				}
			}
		}

		yield return new WaitForSeconds(GuardianStatus.AttackCycleTime); // �־��� �ð� ���� ���

		StartCoroutine(Attack()); // �ڷ�ƾ�� ȣ���Ͽ� ���� �ֱ⸶�� ���� ����
	}


	// �־��� ���⿡ ���� Guardian�� ȸ���� �����ϴ� �Լ�
	private void SetRotationByDirection()
	{
		Vector3 targetPos = _targetEnemys[0].transform.position;
		targetPos.y = transform.position.y; // Guardian�� ���̸� ����Ͽ� y ��ǥ�� ��ġ��Ŵ

		Vector3 dir = targetPos - transform.position; // Guardian���� Ÿ�ٱ����� ���� ���� ���
		dir.Normalize(); // ���� ���͸� ����ȭ�Ͽ� ���̰� 1�� �ǵ��� ����

		// ���� ���Ϳ� ���� Guardian�� ȸ���� ����
		transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
	}


	// ���� ã�� ���� ����� ������Ʈ�ϴ� �Լ�
	void SearchEnemy()
	{
		int count = 0; // ã�� ���� ���� ����ϴ� ����

		List<GameObject> tempList = new List<GameObject>(); // ���ο� ���� ����� �ӽ÷� ������ ����Ʈ

		foreach (GameObject target in _targetEnemys)
		{
			if (target != null) // Ÿ���� �����ϴ� ���
			{
				tempList.Add(target); // �ӽ� ����Ʈ�� Ÿ�� �߰�
				count++; // ã�� ���� �� ����
			}

			if (count >= GuardianStatus.MaxTargetCount) // ������ �ִ� Ÿ�� ���� �����ϸ� �� �̻� ã�� ����
			{
				break;
			}
		}

		_targetEnemys = tempList; // ���ο� ���� ������� ���� ����� ������Ʈ
	}



	// Ư�� ���� ���� ���� �ӹ����� ���� ��Ͽ� �߰��ϴ� �Լ�
	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Enemy")) // �浹�� ����� "Enemy" �±׸� ���� ������Ʈ���� Ȯ��
		{
			if (!_targetEnemys.Contains(other.gameObject)) // ���� ��Ͽ� �ش� ������Ʈ�� ���ԵǾ� ���� ���� ���
			{
				_targetEnemys.Add(other.gameObject); // ���� ��Ͽ� �ش� ������Ʈ �߰�
			}
		}
	}


	// Ư�� ���� ������ ���� ������ ���� ��Ͽ��� �����ϴ� �Լ�
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Enemy")) // �浹�� ����� "Enemy" �±׸� ���� ������Ʈ���� Ȯ��
		{
			if (_targetEnemys.Contains(other.gameObject)) // ���� ��Ͽ� �ش� ������Ʈ�� ���ԵǾ� �ִ��� Ȯ��
			{
				_targetEnemys.Remove(other.gameObject); // ���� ��Ͽ��� �ش� ������Ʈ�� ����
			}
		}
	}
	#endregion

	#region Upgrade

	// Guardian�� ���׷��̵��ϴ� �Լ�
	public void Upgrade(GuardianStatus status)
	{
		Level += 1; // Guardian ���� ����
		GuardianStatus = status; // Guardian�� ���¸� ���ο� ���·� ������Ʈ

		GetComponent<SphereCollider>().radius = GuardianStatus.AttackRadius; // SphereCollider�� �ݰ��� ���ο� ������ ���� �ݰ����� ����
		GuardianRenderer.materials[0].color = GuardianStatus.Color; // Guardian�� �������� ������ ���ο� ������ �������� ����
	}

	#endregion
}