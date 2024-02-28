using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuardianUpgradeManager : MonoBehaviour
{
	public GuardianStatus[] GuardianStatuses;

	public Image AttackRangeImg;
	public Button UpgradeIconButton;

	private Guardian _currentUpgradeGuardian;

	public bool bIsUpgrading = false;
	private bool _isOnButtonHover = false;

	public void Start()
	{
		ShowUpgradeIconAndRange(false);
		GameManager.Inst.guardianBuildManager.OnBuild.AddListener(() => ShowUpgradeIconAndRange(false));
	}

	private void Update()
	{
		UpdateKeyInput();
	}

	
	public void UpgradeGuardian(Guardian guardian)
	{
		ShowUpgradeIconAndRange(true);
		_currentUpgradeGuardian = guardian;

		Vector3 guardianPos = _currentUpgradeGuardian.transform.position;
		Vector3 attackImgPos = Camera.main.WorldToScreenPoint(guardianPos);

		float attackRadius = (_currentUpgradeGuardian.GuardianStatus.AttackRadius) + 1.5f;
		AttackRangeImg.rectTransform.localScale = new Vector3(attackRadius, attackRadius, 1);
		AttackRangeImg.rectTransform.position = attackImgPos;

		UpgradeIconButton.transform.localScale = new Vector3(1 / attackRadius, 1 / attackRadius, 1);
		UpgradeIconButton.onClick.AddListener(() => Upgrade(_currentUpgradeGuardian));
		bIsUpgrading = true;
	} 

	public void ShowUpgradeIconAndRange(bool active)
	{
		AttackRangeImg.gameObject.SetActive(active);
		UpgradeIconButton.gameObject.SetActive(active);
	}

	private void Upgrade(Guardian guardian)
	{
		if (guardian.Level < GuardianStatuses.Length - 1)
		{
			PlayerCharacter player = GameManager.Inst.playerCharacter;
			int cost = GuardianStatuses[guardian.Level + 1].UpgradeCost;

			if (player.CanUseCoin(cost))
			{
				player.UseCoin(cost);
				guardian.Upgrade(GuardianStatuses[guardian.Level + 1]);
				bIsUpgrading = false;
				ShowUpgradeIconAndRange(false);
			}
		}
	}

	public void OnPointerEnter()
	{
		_isOnButtonHover = true;
	}
	public void OnPointerExit()
	{
		_isOnButtonHover = false;
	}

	private void UpdateKeyInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (_isOnButtonHover)
			{
				return;
			}

			bIsUpgrading = false;
			ShowUpgradeIconAndRange(false);
		}
	}
}
