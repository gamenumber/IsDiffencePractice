	using System.Collections;
	using System.Collections.Generic;
	using TMPro;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	public class GuardianUpgradeManager : MonoBehaviour
	{
		public GuardianStatus[] GuardianStatuses; // ����� �������ͽ��� ��� �迭

		public Image AttackRangeImg; // ���� �����Ÿ� �̹���
		public Button UpgradeIconButton; // ���׷��̵� ��ư

		private Guardian _currentUpgradeGuardian; // ����� ��ũ��Ʈ�� ������

		public bool bIsUpgrading = false; // ���׷��̵带 �ϴ����� ���� ����
		private bool _isOnButtonHover = false; // ��ư�� ���ȴ��� ����

		public void Start()
		{
			ShowUpgradeIconAndRange(false); // ShowUpgradeIconAndRange���� false�� �ٲ�
			GameManager.Inst.guardianBuildManager.OnBuild.AddListener(() => ShowUpgradeIconAndRange(false)); // OnBuild �Լ��� ȣ��Ǹ� ShowUpgradeIconAndRange(false) -> �̷��� ȣ���
		}

	
		private void Update()
		{
			UpdateKeyInput(); // Update�� KeyInput�� ��� üũ 
		}

	
		public void UpgradeGuardian(Guardian guardian)
		{
			ShowUpgradeIconAndRange(true); // ShowUpgradeIconAndRange�� true�� �ٲ�
			_currentUpgradeGuardian = guardian; // guardian ���� _currentUpgradeGuardian���� �Ҵ�

			Vector3 guardianPos = _currentUpgradeGuardian.transform.position; // _currentUpgradeGuardian.transform.position�� guardianPos���� �Ҵ�
			Vector3 attackImgPos = Camera.main.WorldToScreenPoint(guardianPos); // attackImgPos�� Camera.main.WorldToScreenPoint(guardianPos) -> guardianPos�� 3D �������� ȭ�� ��ǥ�� ��ȯ

			float attackRadius = (_currentUpgradeGuardian.GuardianStatus.AttackRadius) + 1.5f; // attackRadius�� (_currentUpgradeGuardian.GuardianStatus.AttackRadius) + 1.5f ���� �Ҵ�
			AttackRangeImg.rectTransform.localScale = new Vector3(attackRadius, attackRadius, 1); // AttackRangeImg.rectTransform.localScale ���� new Vector3(attackRadius, attackRadius, 1)������ �����Ҵ�
			AttackRangeImg.rectTransform.position = attackImgPos; // attackImgPos ���� AttackRangeImg.rectTransform.position�� �Ҵ���

			UpgradeIconButton.transform.localScale = new Vector3(1 / attackRadius, 1 / attackRadius, 1); // UpgradeIconButton�� localScale��  new Vector3(1 / attackRadius, 1 / attackRadius, 1)�� �����Ҵ�
			UpgradeIconButton.onClick.AddListener(() => Upgrade(_currentUpgradeGuardian)); // UpgradeIconButton�� Ŭ���ϸ� Upgrade�� �ȴ�.
			bIsUpgrading = true;
		} 

		public void ShowUpgradeIconAndRange(bool active)
		{
			AttackRangeImg.gameObject.SetActive(active);
			UpgradeIconButton.gameObject.SetActive(active); // ->  �ش� �Լ��� �Ű� ������ ���� Ȱ��ȭ / ��Ȱ��ȭ
		}

		// ������� ���׷��̵� �ϴ� �κ�
		private void Upgrade(Guardian guardian)
		{
			if (guardian.Level < GuardianStatuses.Length - 1) // ���� guardian.Level�� GuardianStatuses.Length - 1���� ������ 
			{
				PlayerCharacter player = GameManager.Inst.playerCharacter; // GameManager.Inst.playerCharacter ���� PlayerCharacter player�� �Ҵ��Ŵ
				int cost = GuardianStatuses[guardian.Level + 1].UpgradeCost; // UpgradeCost�� �ø�

				if (player.CanUseCoin(cost)) // Coin�� �� �� ������
				{
					player.UseCoin(cost); // cost��ŭ Coin�� ���
					guardian.Upgrade(GuardianStatuses[guardian.Level + 1]); // guardian�� �����ؼ� Level�� �ø�
					bIsUpgrading = false;
					ShowUpgradeIconAndRange(false); // ShowUpgradeIconAndRange���� false�� �ٲ�
				}
			}
		}

		// ���� ���콺 �����Ͱ� ������
		public void OnPointerEnter()
		{
			_isOnButtonHover = true; // �����Ͱ� �پ��ִ�(true)�� ǥ�� 
		}

		// ���� ���콺 �����Ͱ� ��������
		public void OnPointerExit()
		{
			_isOnButtonHover = false; // �����Ͱ� ��������(false)�� ǥ��
		}

		// Ű �Է��� ������Ʈ�ϴ� �Լ�
		private void UpdateKeyInput()
		{
			if (Input.GetMouseButtonDown(0)) // ���� ���콺 ��Ŭ���� ���� ��
			{
				if (_isOnButtonHover) // ���� _isOnButtonHover�� true��� 
				{
					return; // �Լ��� �����Ŵ
				}

				bIsUpgrading = false; // ���׷��̵� ���θ� false�� �ٲ�
				ShowUpgradeIconAndRange(false); // ShowUpgradeIconAndRange���� false�� �ٲ�
			}
		}
	}
