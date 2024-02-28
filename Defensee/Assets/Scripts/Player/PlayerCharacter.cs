using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
	public int Coin = 100; // ���� ��
	private int _heart = 10; // �����

	public int Heart // ���� ����� -> _heart�� ���Ϲ���
	{
		get { return _heart; }
	}
	public int MaxHeart = 10; // �ִ� �����

	// ������ �����ϸ�
	void Start()
	{
		_heart = MaxHeart; // _heart ���� MaxHeart���� �Ҵ� ������.
	}

	// Damaged -> (int damage)�Ű����� ��ŭ ������ �Դ� ���� ������ �Լ�
	public void Damaged(int damage)
	{
		_heart -= damage; // �Ű� ���� damage��ŭ _heart���� ���ҽ�Ŵ
		if (_heart <= 0) // ���� _heart <= 0�̶�� 
		{
			GameManager.Inst.GameDefeat(); // ������ �й���
		}
		Debug.Log(_heart); // ����׷� _heart�� ǥ��
	}
	
	// ���λ���� ������ �Լ� 
	public void UseCoin(int coin)
	{
		Coin = Mathf.Clamp(Coin - coin, 0, int.MaxValue); //  Coin - coin ���� 0�� int.MaxValue�ȿ� ������ ������.
	}

	// �Ű����� coin�� �̿��ؼ� ������ ��� �� �� �ִ��� ���θ� ������
	public bool CanUseCoin(int coin)
	{
		return Coin >= coin; //  Coin ���� coin ������ ũ�ų� ���ٸ� true, �׷��� �ʴٸ� false�� ��ȯ�ϴ� �ڵ� 
	}

}
