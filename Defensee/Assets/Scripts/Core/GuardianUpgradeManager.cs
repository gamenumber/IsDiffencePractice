	using System.Collections;
	using System.Collections.Generic;
	using TMPro;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	public class GuardianUpgradeManager : MonoBehaviour
	{
		public GuardianStatus[] GuardianStatuses; // 가디언 스테이터스를 담는 배열

		public Image AttackRangeImg; // 공격 사정거리 이미지
		public Button UpgradeIconButton; // 업그레이드 버튼

		private Guardian _currentUpgradeGuardian; // 가디언 스크립트를 가져옴

		public bool bIsUpgrading = false; // 업그레이드를 하는지에 따른 여부
		private bool _isOnButtonHover = false; // 버튼이 눌렸는지 여부

		public void Start()
		{
			ShowUpgradeIconAndRange(false); // ShowUpgradeIconAndRange값을 false로 바꿈
			GameManager.Inst.guardianBuildManager.OnBuild.AddListener(() => ShowUpgradeIconAndRange(false)); // OnBuild 함수가 호출되면 ShowUpgradeIconAndRange(false) -> 이렇게 호출됨
		}

	
		private void Update()
		{
			UpdateKeyInput(); // Update로 KeyInput을 계속 체크 
		}

	
		public void UpgradeGuardian(Guardian guardian)
		{
			ShowUpgradeIconAndRange(true); // ShowUpgradeIconAndRange를 true로 바꿈
			_currentUpgradeGuardian = guardian; // guardian 값을 _currentUpgradeGuardian값에 할당

			Vector3 guardianPos = _currentUpgradeGuardian.transform.position; // _currentUpgradeGuardian.transform.position을 guardianPos값에 할당
			Vector3 attackImgPos = Camera.main.WorldToScreenPoint(guardianPos); // attackImgPos을 Camera.main.WorldToScreenPoint(guardianPos) -> guardianPos을 3D 공간에서 화면 좌표로 변환

			float attackRadius = (_currentUpgradeGuardian.GuardianStatus.AttackRadius) + 1.5f; // attackRadius에 (_currentUpgradeGuardian.GuardianStatus.AttackRadius) + 1.5f 값을 할당
			AttackRangeImg.rectTransform.localScale = new Vector3(attackRadius, attackRadius, 1); // AttackRangeImg.rectTransform.localScale 값을 new Vector3(attackRadius, attackRadius, 1)값으로 동적할당
			AttackRangeImg.rectTransform.position = attackImgPos; // attackImgPos 값을 AttackRangeImg.rectTransform.position에 할당함

			UpgradeIconButton.transform.localScale = new Vector3(1 / attackRadius, 1 / attackRadius, 1); // UpgradeIconButton의 localScale을  new Vector3(1 / attackRadius, 1 / attackRadius, 1)로 동적할당
			UpgradeIconButton.onClick.AddListener(() => Upgrade(_currentUpgradeGuardian)); // UpgradeIconButton을 클릭하면 Upgrade가 된다.
			bIsUpgrading = true;
		} 

		public void ShowUpgradeIconAndRange(bool active)
		{
			AttackRangeImg.gameObject.SetActive(active);
			UpgradeIconButton.gameObject.SetActive(active); // ->  해당 함수의 매개 변수에 따라 활성화 / 비활성화
		}

		// 가디언을 업그레이드 하는 부분
		private void Upgrade(Guardian guardian)
		{
			if (guardian.Level < GuardianStatuses.Length - 1) // 만약 guardian.Level이 GuardianStatuses.Length - 1보다 작으면 
			{
				PlayerCharacter player = GameManager.Inst.playerCharacter; // GameManager.Inst.playerCharacter 값을 PlayerCharacter player에 할당시킴
				int cost = GuardianStatuses[guardian.Level + 1].UpgradeCost; // UpgradeCost를 올림

				if (player.CanUseCoin(cost)) // Coin을 쓸 수 있으면
				{
					player.UseCoin(cost); // cost만큼 Coin을 사용
					guardian.Upgrade(GuardianStatuses[guardian.Level + 1]); // guardian에 접근해서 Level을 올림
					bIsUpgrading = false;
					ShowUpgradeIconAndRange(false); // ShowUpgradeIconAndRange값을 false로 바꿈
				}
			}
		}

		// 만약 마우스 포인터가 눌리면
		public void OnPointerEnter()
		{
			_isOnButtonHover = true; // 포인터가 붙어있다(true)로 표시 
		}

		// 만약 마우스 포인터가 때어지면
		public void OnPointerExit()
		{
			_isOnButtonHover = false; // 포인터가 때어졌다(false)로 표시
		}

		// 키 입력을 업데이트하는 함수
		private void UpdateKeyInput()
		{
			if (Input.GetMouseButtonDown(0)) // 만약 마우스 좌클릭을 했을 때
			{
				if (_isOnButtonHover) // 만약 _isOnButtonHover가 true라면 
				{
					return; // 함수를 종료시킴
				}

				bIsUpgrading = false; // 업그레이드 여부를 false로 바꿈
				ShowUpgradeIconAndRange(false); // ShowUpgradeIconAndRange값을 false로 바꿈
			}
		}
	}
