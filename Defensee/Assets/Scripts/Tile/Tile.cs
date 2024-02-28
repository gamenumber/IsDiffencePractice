using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	[HideInInspector]
	public Guardian OwnGuardian; // OwnGuardian을 가져옴

	public bool CheckIsOwned()
	{
		return OwnGuardian != null; // 만약 OwnGuardian이 null이 아니라면 true를, 그렇지 않으면 false를 반환함.
	}

	public void ClearOwned()
	{
		OwnGuardian = null; // OwnGuardian를 null로 바꿈
	}

	// Owned 제거 
	public void RemoveOwned()
	{
		Destroy(OwnGuardian); // OwnGuardian을 제거 
		OwnGuardian = null; // OwnGuardian을 null로 바꿈
	}
}
