using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Inst // 싱글톤 패턴을 구현하여 Inst라는 이름으로 인스턴스(접근) 할 수 있게끔 함.
	{
		get; private set; // 자동 프로퍼티 -> 
	}

	public PlayerCharacter playerCharacter; // playerCharacter 가져오기
	public GuardianUpgradeManager guardianUpgradeManager; // guardianUpgradeManager 가져오기
	public GuardianBuildManager guardianBuildManager; // guardianBuildManager 가져오기

	private void Awake()
	{
		if (Inst == null) // 만약 Inst가 null이라면 
		{
			Inst = this; // Inst값에 이걸 넣음
		}
		else // 아니면 
		{
			Destroy(Inst); // Inst를 없앰
		}
	}

	// 게임을 패배 했을 때
	public void GameDefeat()
	{

	}

	// 적이 죽었을 때
	public void EnemyDead(int coin)
	{
		playerCharacter.Coin += coin; // PlayerCharacter의 Coin을 coin 매개 변수만큼 더한다.
	}
}
