using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GuardianBuildManager : MonoBehaviour
{
	public GameObject[] Tiles;

	public GameObject CurrentFocusTile;
	public GameObject GuardianPrefab;
	public GameObject BuildIconPrefab;

	public Material BuildCanMat;
	public Material BuildCanNotMat;

	public float BuildDeltaY = 0f;
	public float FocusTileDistance = 0.05f;

	public int NormalGuaridanCost = 50;

	public UnityEvent OnBuild;

	void Start()
	{
		Tiles = GameObject.FindGameObjectsWithTag("Tile");
		BuildIconPrefab = Instantiate(BuildIconPrefab, transform.position, Quaternion.Euler(90, 0, 0));
		BuildIconPrefab.gameObject.SetActive(false);
	}

	void Update()
	{
		bool bisUpgrading = GameManager.Inst.guardianUpgradeManager.bIsUpgrading;

		UpdateFindFocusTile();
		if (!bisUpgrading)
		{
			UpdateBuildImage();
			UpdateKeyInput();
		}
	}

	private void UpdateFindFocusTile()
	{
		CurrentFocusTile = null;
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
		mousePosition.y = 0f;

		foreach (var tile in Tiles)
		{
			Vector3 tilePos = tile.transform.position;
			tilePos.y = 0f;

			if (Vector3.Distance(mousePosition, tilePos) <= FocusTileDistance)
			{
				CurrentFocusTile = tile;
				break;
			}
		}
	}

	private void UpdateBuildImage()
	{
		bool bFocusTile = false;

		if (CurrentFocusTile)
		{
			Tile tile = CurrentFocusTile.GetComponent<Tile>();
			if (!tile.CheckIsOwned())
			{
				Vector3 position = tile.transform.position;
				position.y += BuildDeltaY;
				BuildIconPrefab.transform.position = position;
				bFocusTile = true;

				bool bCanBuild = GameManager.Inst.playerCharacter.CanUseCoin(NormalGuaridanCost);
				Material mat = bCanBuild ? BuildCanMat : BuildCanNotMat;
				BuildIconPrefab.GetComponent<MeshRenderer>().material = mat;
			}
		}

		if (bFocusTile)
		{
			BuildIconPrefab.gameObject.SetActive(true);
		}
		else
		{
			DeActivateBuildImage();
		}
	}

	private void DeActivateBuildImage()
	{
		BuildIconPrefab.gameObject.SetActive(false);
	}

	// TODO : Click Interface? 

	void CheckToBuildGuardian()
	{
		if (CurrentFocusTile != null)
		{
			Tile tile = CurrentFocusTile.GetComponent<Tile>();
			PlayerCharacter player = GameManager.Inst.playerCharacter;
			if (!tile.CheckIsOwned() && player.CanUseCoin(NormalGuaridanCost))
			{
				player.UseCoin(NormalGuaridanCost);

				Vector3 position = BuildIconPrefab.transform.position;
				GameObject guardianInst = Instantiate(GuardianPrefab, position, Quaternion.identity);

				tile.OwnGuardian = guardianInst.GetComponent<Guardian>();

				OnBuild.Invoke();
				DeActivateBuildImage();

				return;
			}

			if (tile && tile.OwnGuardian)
			{
				//GameManager.Inst.guardianUpgradeManager.UpgradeGuardian(tile.OwnGuardian);
			}
		}
	}

	private void UpdateKeyInput()
	{
		if (Input.GetMouseButtonUp(0))
		{
			CheckToBuildGuardian();
		}
	}
}