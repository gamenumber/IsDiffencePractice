using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	[HideInInspector]
	public Guardian OwnGuardian;

	public bool CheckIsOwned()
	{
		return OwnGuardian != null;
	}

	public void ClearOwned()
	{
		OwnGuardian = null;
	}

	public void RemoveOwned()
	{
		Destroy(OwnGuardian);
		OwnGuardian = null;
	}
}
