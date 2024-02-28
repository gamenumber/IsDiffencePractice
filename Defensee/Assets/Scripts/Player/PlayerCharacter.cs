using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
	public int Coin = 100; // 코인 양
	private int _heart = 10; // 생명력

	public int Heart // 현재 생명력 -> _heart를 리턴받음
	{
		get { return _heart; }
	}
	public int MaxHeart = 10; // 최대 생명력

	// 게임을 시작하면
	void Start()
	{
		_heart = MaxHeart; // _heart 값에 MaxHeart값을 할당 시켜줌.
	}

	// Damaged -> (int damage)매개변수 만큼 데미지 입는 것을 구현한 함수
	public void Damaged(int damage)
	{
		_heart -= damage; // 매개 변수 damage만큼 _heart에서 감소시킴
		if (_heart <= 0) // 만약 _heart <= 0이라면 
		{
			GameManager.Inst.GameDefeat(); // 게임을 패배함
		}
		Debug.Log(_heart); // 디버그로 _heart를 표시
	}
	
	// 코인사용을 구현한 함수 
	public void UseCoin(int coin)
	{
		Coin = Mathf.Clamp(Coin - coin, 0, int.MaxValue); //  Coin - coin 값이 0과 int.MaxValue안에 값으로 고정함.
	}

	// 매개변수 coin을 이용해서 코인을 사용 할 수 있는지 여부를 결정함
	public bool CanUseCoin(int coin)
	{
		return Coin >= coin; //  Coin 값이 coin 값보다 크거나 같다면 true, 그렇지 않다면 false를 반환하는 코드 
	}

}
