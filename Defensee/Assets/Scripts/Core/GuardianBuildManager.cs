using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // Events를 사용할 때 필요한 네임스페이스
using UnityEngine.UI; // UI를 사용할 때 필요한 네임스페이스 

public class GuardianBuildManager : MonoBehaviour
{
	public GameObject[] Tiles; // Tiles를 저장해놓을 배열 공간

	public GameObject CurrentFocusTile; // 현재 Focus를 두고 있는 Tile
	public GameObject GuardianPrefab; // Guardian 프리팹
	public GameObject BuildIconPrefab; // BuildIcon 프리팹

	public Material BuildCanMat; // BuildCanMat 머터리얼
	public Material BuildCanNotMat; // BuildCanNotMat 머터리얼

	public float BuildDeltaY = 0f;
	public float FocusTileDistance = 0.05f;

	public int NormalGuaridanCost = 50;

	public UnityEvent OnBuild;

	// 게임을 시작 했을 때
	void Start()
	{
		Tiles = GameObject.FindGameObjectsWithTag("Tile"); // Tile 태그를 가진 여러 오브젝트를 배열 Tile에 할당함
		BuildIconPrefab = Instantiate(BuildIconPrefab, transform.position, Quaternion.Euler(90, 0, 0)); // BuildIconPrefab생성 -> (X축으로 90도 가량 회전해서)
		BuildIconPrefab.gameObject.SetActive(false); // BuildIconPrefab을 비활성화 시킴
	}

	void Update()
	{
		bool bisUpgrading = GameManager.Inst.guardianUpgradeManager.bIsUpgrading; // bisUpgrading 논리값을 GameManager.Inst.guardianUpgradeManager.bIsUpgrading값으로 할당시킴 

		UpdateFindFocusTile(); // UpdateFindFocusTile 함수 호출
							  
		if (!bisUpgrading) // 만약 bisUpgrading가 false라면 
		{
			UpdateBuildImage(); // UpdateBuildImage 함수 호출 
			UpdateKeyInput(); // UpdateKeyInput 함수 호출 
		}
	}

	// UpdateFindFocusTile을 찾는 함수 
	private void UpdateFindFocusTile()
	{
		CurrentFocusTile = null; // CurrentFocusTile를 null로 바꿈
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)); // mousePosition을 마우스 포지션 x, y 가져와서 가까운 타일을 찾음
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