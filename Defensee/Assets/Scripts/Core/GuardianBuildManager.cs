using System; // C# 기본 라이브러리 사용 선언
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // Unity 이벤트 사용을 위한 네임스페이스
using UnityEngine.UI; // UI 요소 사용을 위한 네임스페이스

/*
1-1.  나중에 해당 타일들에 대한 조작 및 접근을 용이하게 하려고 타일들을 찾아서 Tiles 에 저장함. 이렇게 하면  코드 상에서 마우스 위치에 가장 가까운 타일을 찾거나, 건설 여부를 확인하거나 할 때 배열을 통해 쉽게 처리할 수 있다는 장점이 있음

1-2. 게임 시작 시에는 숨겨놓고, 이후에 특정 조건이 충족될 때만 활성화되도록 하는데 사용하기 위해서

1-3. UpdateBuildImage에서 ? 연산자는 *삼항 연산자*로, bCanBuild가 참이면 BuildCanMat을, 거짓이면 BuildCanNotMat을 선택하는 구문.

1-4. if(bFocusTile) else 코드는 현재 마우스 위치에 타일이 존재하는지 여부를 확인하는 작업을 거치는 것입니다.

1-5. CheckToBuildGuardian에서 DeActiveBuildImage는 건설이 완료되면 건설 아이콘을 숨기기 위해 호출됩니다.
 */

public class GuardianBuildManager : MonoBehaviour
{
	public GameObject[] Tiles; // 타일을 저장하는 배열

	public GameObject CurrentFocusTile; // 현재 초점이 맞춰진 타일
	public GameObject GuardianPrefab; // Guardian 프리팹
	public GameObject BuildIconPrefab; // BuildIcon 프리팹

	public Material BuildCanMat; // 건설 가능한 경우의 머터리얼
	public Material BuildCanNotMat; // 건설 불가능한 경우의 머터리얼

	public float BuildDeltaY = 0f; // 건설 아이콘 위치 조절을 위한 Y 좌표
	public float FocusTileDistance = 0.05f; // 초점이 맞춰지는 타일과의 거리

	public int NormalGuardianCost = 50; // 일반 가디언의 비용

	public UnityEvent OnBuild; // 건설 시 호출되는 Unity 이벤트

	// 게임이 시작될 때
	void Start()
	{
		Tiles = GameObject.FindGameObjectsWithTag("Tile"); // "Tile" 태그를 가진 여러 오브젝트를 Tiles 배열에 할당
		BuildIconPrefab = Instantiate(BuildIconPrefab, transform.position, Quaternion.Euler(90, 0, 0)); // BuildIconPrefab 생성 -> (X축으로 90도 회전)
		BuildIconPrefab.gameObject.SetActive(false); // BuildIconPrefab 비활성화
	}

	void Update()
	{
		bool bisUpgrading = GameManager.Inst.guardianUpgradeManager.bIsUpgrading; // Guardian 업그레이드 중인지 확인하는 변수

		UpdateFindFocusTile(); // 초점이 맞춰진 타일 업데이트 함수 호출

		if (!bisUpgrading) // 만약 Guardian 업그레이드 중이 아니라면
		{
			UpdateBuildImage(); // 빌드 이미지 업데이트 함수 호출
			UpdateKeyInput(); // 키 입력 업데이트 함수 호출
		}
	}

	// 초점이 맞춰진 타일을 찾는 함수
	private void UpdateFindFocusTile()
	{
		CurrentFocusTile = null; // 현재 초점 타일을 초기화

		// 현재 마우스 위치를 화면에서 월드 좌표로 변환하여 가져옴
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
		mousePosition.y = 0f; // Y 좌표를 0으로 설정하여 3D 공간 상의 평면상으로 이룸

		// 모든 타일에 대해 반복문 실행 -> 순회
		foreach (var tile in Tiles)
		{
			// 현재 타일의 위치를 가져와서 Y 좌표를 0으로 설정하여 3D 공간 상의 평면상으로 이룸
			Vector3 tilePos = tile.transform.position;
			tilePos.y = 0f;

			// 마우스 위치와 타일의 위치 간의 거리가 설정한 최대 거리 이내라면
			if (Vector3.Distance(mousePosition, tilePos) <= FocusTileDistance)
			{
				CurrentFocusTile = tile; // 현재 초점 타일을 해당 타일로 설정하고 반복문 종료
				break;
			}
		}
	}


	// 빌드 이미지를 업데이트하는 함수
	private void UpdateBuildImage()
	{
		bool bFocusTile = false; // 초점이 맞춰진 타일 여부 확인 변수

		if (CurrentFocusTile) // 현재 초점이 있는 타일이 있는지 확인
		{
			Tile tile = CurrentFocusTile.GetComponent<Tile>(); // 현재 초점이 있는 타일에서 Tile 컴포넌트 가져오기
			if (!tile.CheckIsOwned()) // 타일이 소유되지 않았는지 확인
			{
				Vector3 position = tile.transform.position; // 타일의 위치를 가져옴
				position.y += BuildDeltaY; // Y축 위치 조정
				BuildIconPrefab.transform.position = position; // 빌드 아이콘의 위치 설정
				bFocusTile = true; // 초점이 맞춰진 타일이 발견됨

				bool bCanBuild = GameManager.Inst.playerCharacter.CanUseCoin(NormalGuardianCost); // 코인으로 가디언 건설 가능 여부 확인
				Material mat = bCanBuild ? BuildCanMat : BuildCanNotMat; // 건설 가능 여부에 따라 머터리얼 선택
				BuildIconPrefab.GetComponent<MeshRenderer>().material = mat; // 빌드 아이콘의 머터리얼 설정
			}
		}

		if (bFocusTile) // 초점이 맞춰진 타일이 있다면
		{
			BuildIconPrefab.gameObject.SetActive(true); // 빌드 아이콘 활성화
		}
		else
		{
			DeActivateBuildImage(); // 초점이 맞춰진 타일이 없으면 빌드 이미지를 비활성화
		}
	}

	// 빌드 이미지를 비활성화하는 함수
	private void DeActivateBuildImage()
	{
		BuildIconPrefab.gameObject.SetActive(false); // BuildIconPrefab을 비활성화
	}

	// 가디언을 건설하고 확인하는 함수
	void CheckToBuildGuardian()
	{
		if (CurrentFocusTile != null)
		{
			Tile tile = CurrentFocusTile.GetComponent<Tile>();
			PlayerCharacter player = GameManager.Inst.playerCharacter;
			if (!tile.CheckIsOwned() && player.CanUseCoin(NormalGuardianCost))
			{
				player.UseCoin(NormalGuardianCost);

				Vector3 position = BuildIconPrefab.transform.position;
				GameObject guardianInst = Instantiate(GuardianPrefab, position, Quaternion.identity);

				tile.OwnGuardian = guardianInst.GetComponent<Guardian>();

				OnBuild.Invoke(); // 건설 이벤트 호출
				DeActivateBuildImage(); // 건설 완료 후 빌드 이미지 비활성화

				return;
			}

			if (tile && tile.OwnGuardian)
			{
				GameManager.Inst.guardianUpgradeManager.UpgradeGuardian(tile.OwnGuardian);
			}
		}
	}

	// 키 입력을 업데이트하는 함수
	private void UpdateKeyInput()
	{
		if (Input.GetMouseButtonUp(0)) // 만약 마우스 왼쪽 클릭을 하면 
		{
			CheckToBuildGuardian(); // CheckToBuildGuardian 함수 호출
		}
	}
}
