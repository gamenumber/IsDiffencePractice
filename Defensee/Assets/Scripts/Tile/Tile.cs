using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	[HideInInspector]
	public Guardian OwnGuardian; // OwnGuardian�� ������

	public bool CheckIsOwned()
	{
		return OwnGuardian != null; // ���� OwnGuardian�� null�� �ƴ϶�� true��, �׷��� ������ false�� ��ȯ��.
	}

	public void ClearOwned()
	{
		OwnGuardian = null; // OwnGuardian�� null�� �ٲ�
	}

	// Owned ���� 
	public void RemoveOwned()
	{
		Destroy(OwnGuardian); // OwnGuardian�� ���� 
		OwnGuardian = null; // OwnGuardian�� null�� �ٲ�
	}
}
