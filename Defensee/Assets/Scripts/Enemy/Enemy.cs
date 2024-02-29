using System.Collections;
using System.Collections.Generic; // �� ���ӽ����̽��� ���׸�(Generic) �÷��ǰ� ���õ� ��ɵ��� ������ -> ����Ʈ(List), ��ųʸ�(Dictionary), ť(Queue), ����(Stack) ��� ���� Ŭ������ �����޾Ƽ� ����� �� �ְԲ� ��.
using UnityEngine; // ����Ƽ �������� ����ϴ� ����� ������

public class Enemy : MonoBehaviour
{
	// ���� �߰� ����(Waypoint)�� ��Ÿ���� ���� ������Ʈ
	private GameObject _currentWayPoint;

	// ���� ��������Ʈ�� �ε���
	private int _wayPointCount = 0;

	// �̵� ����
	private Vector3 _moveDirection = Vector3.zero;

	// �� ü��
	private int _hp = 5;

	// ������(public���� ������� ����) ����: ��������Ʈ �迭
	[HideInInspector]
	public GameObject[] WayPoints;

	// �� �ִ� ü��
	public int MaxHp = 5;

	// �̵� �ӵ�
	public float MoveSpeed = 10;

	// �÷��̾�� �ִ� ����
	public int Damage = 1;

	// �÷��̾ ����� ������ ��
	public int StealCoin = 100;

	private void Start()
	{
		// �ʱ�ȭ: ���� ü���� �ִ� ü������ ����
		_hp = MaxHp;

		// �ʱ�ȭ: ù ��° ��������Ʈ ����
		_currentWayPoint = WayPoints[0];

		// �ʱ�ȭ: �̵� ���⿡ ���� ȸ�� ����
		SetRotationByDirection();
	}

	private void Update()
	{
		// �� �̵� ����
		transform.position += _moveDirection * MoveSpeed * Time.deltaTime;

		// ��ǥ ��ġ ����
		Vector3 TargetPosition = _currentWayPoint.transform.position;
		TargetPosition.y = transform.position.y;

		// ���� ���� ��ġ�� TargetPosition�� ���������� 
		if (Vector3.Distance(transform.position, TargetPosition) <= 0.02f)
		{
			// ������ ��������Ʈ�� ������ ���
			if (_wayPointCount >= WayPoints.Length - 1)
			{
				// �÷��̾�� ���ظ� ��?
				GameManager.Inst.playerCharacter.Damaged(Damage);

				// �� ����
				Destroy(gameObject);
				return;
			}

			// ���� ��������Ʈ�� �̵�
			_wayPointCount = Mathf.Clamp(_wayPointCount + 1, 0, WayPoints.Length);
			_currentWayPoint = WayPoints[_wayPointCount];

			// �̵� ���⿡ ���� ȸ�� ����
			SetRotationByDirection();
		}
	}

	private void SetRotationByDirection()
	{
	
		_moveDirection = _currentWayPoint.transform.position - transform.position; // ���� ��������Ʈ(_currentWayPoint)�� ��ġ���� ���� ��ġ(transform.position)�� ���� _moveDirection�� ����
		_moveDirection.y = 0; // y�� 0���� ����
		_moveDirection.Normalize(); // �̵� ���� ����(_moveDirection)�� ����ȭ���Ѽ� ���⸸ ��Ÿ���� ��.

		// �̵� ���⿡ ���� ȸ�� ����
		transform.rotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
	}

	// �ش� �ڵ�� 3D���� ����
	private void OnTriggerEnter(Collider other)
	{
		// Projectile�� �浹�ϸ�
		if (other.CompareTag("Projectile"))
		{
			// ���� ü�¿��� 1�� ���ҽ�Ŵ, �׸��� ü���� 0�̸����� �������� �ʵ����ϰ�, MaxHp�� �ʰ����� �ʵ��� ������.
			_hp = Mathf.Clamp(_hp - 1, 0, MaxHp);

			// ü���� 0 ���Ϸ� ������ ���
			if (_hp <= 0)
			{
				// �� ��Ȱ��ȭ 
				gameObject.SetActive(false);

				// �̰� �� �𸣰�����, StealCoin �̶�� ���� ���� �Ƹ��� ���� �׾��� �� �÷��̾ ������ ��� �� ����
				GameManager.Inst.EnemyDead(StealCoin);

				// �� ����
				Destroy(gameObject);
			}
		}
	}
}
