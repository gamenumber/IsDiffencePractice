using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Inst // �̱��� ������ �����Ͽ� Inst��� �̸����� �ν��Ͻ�(����) �� �� �ְԲ� ��.
	{
		get; private set; // �ڵ� ������Ƽ -> 
	}

	public PlayerCharacter playerCharacter; // playerCharacter ��������
	public GuardianUpgradeManager guardianUpgradeManager; // guardianUpgradeManager ��������
	public GuardianBuildManager guardianBuildManager; // guardianBuildManager ��������

	private void Awake()
	{
		if (Inst == null) // ���� Inst�� null�̶�� 
		{
			Inst = this; // Inst���� �̰� ����
		}
		else // �ƴϸ� 
		{
			Destroy(Inst); // Inst�� ����
		}
	}

	// ������ �й� ���� ��
	public void GameDefeat()
	{

	}

	// ���� �׾��� ��
	public void EnemyDead(int coin)
	{
		playerCharacter.Coin += coin; // PlayerCharacter�� Coin�� coin �Ű� ������ŭ ���Ѵ�.
	}
}
