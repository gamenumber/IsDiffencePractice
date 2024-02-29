using System; // C# �⺻ ���̺귯�� ��� ����
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // Unity �̺�Ʈ ����� ���� ���ӽ����̽�
using UnityEngine.UI; // UI ��� ����� ���� ���ӽ����̽�

/*
1-1.  ���߿� �ش� Ÿ�ϵ鿡 ���� ���� �� ������ �����ϰ� �Ϸ��� Ÿ�ϵ��� ã�Ƽ� Tiles �� ������. �̷��� �ϸ�  �ڵ� �󿡼� ���콺 ��ġ�� ���� ����� Ÿ���� ã�ų�, �Ǽ� ���θ� Ȯ���ϰų� �� �� �迭�� ���� ���� ó���� �� �ִٴ� ������ ����

1-2. ���� ���� �ÿ��� ���ܳ���, ���Ŀ� Ư�� ������ ������ ���� Ȱ��ȭ�ǵ��� �ϴµ� ����ϱ� ���ؼ�

1-3. UpdateBuildImage���� ? �����ڴ� *���� ������*��, bCanBuild�� ���̸� BuildCanMat��, �����̸� BuildCanNotMat�� �����ϴ� ����.

1-4. if(bFocusTile) else �ڵ�� ���� ���콺 ��ġ�� Ÿ���� �����ϴ��� ���θ� Ȯ���ϴ� �۾��� ��ġ�� ���Դϴ�.

1-5. CheckToBuildGuardian���� DeActiveBuildImage�� �Ǽ��� �Ϸ�Ǹ� �Ǽ� �������� ����� ���� ȣ��˴ϴ�.
 */

public class GuardianBuildManager : MonoBehaviour
{
	public GameObject[] Tiles; // Ÿ���� �����ϴ� �迭

	public GameObject CurrentFocusTile; // ���� ������ ������ Ÿ��
	public GameObject GuardianPrefab; // Guardian ������
	public GameObject BuildIconPrefab; // BuildIcon ������

	public Material BuildCanMat; // �Ǽ� ������ ����� ���͸���
	public Material BuildCanNotMat; // �Ǽ� �Ұ����� ����� ���͸���

	public float BuildDeltaY = 0f; // �Ǽ� ������ ��ġ ������ ���� Y ��ǥ
	public float FocusTileDistance = 0.05f; // ������ �������� Ÿ�ϰ��� �Ÿ�

	public int NormalGuardianCost = 50; // �Ϲ� ������� ���

	public UnityEvent OnBuild; // �Ǽ� �� ȣ��Ǵ� Unity �̺�Ʈ

	// ������ ���۵� ��
	void Start()
	{
		Tiles = GameObject.FindGameObjectsWithTag("Tile"); // "Tile" �±׸� ���� ���� ������Ʈ�� Tiles �迭�� �Ҵ�
		BuildIconPrefab = Instantiate(BuildIconPrefab, transform.position, Quaternion.Euler(90, 0, 0)); // BuildIconPrefab ���� -> (X������ 90�� ȸ��)
		BuildIconPrefab.gameObject.SetActive(false); // BuildIconPrefab ��Ȱ��ȭ
	}

	void Update()
	{
		bool bisUpgrading = GameManager.Inst.guardianUpgradeManager.bIsUpgrading; // Guardian ���׷��̵� ������ Ȯ���ϴ� ����

		UpdateFindFocusTile(); // ������ ������ Ÿ�� ������Ʈ �Լ� ȣ��

		if (!bisUpgrading) // ���� Guardian ���׷��̵� ���� �ƴ϶��
		{
			UpdateBuildImage(); // ���� �̹��� ������Ʈ �Լ� ȣ��
			UpdateKeyInput(); // Ű �Է� ������Ʈ �Լ� ȣ��
		}
	}

	// ������ ������ Ÿ���� ã�� �Լ�
	private void UpdateFindFocusTile()
	{
		CurrentFocusTile = null; // ���� ���� Ÿ���� �ʱ�ȭ

		// ���� ���콺 ��ġ�� ȭ�鿡�� ���� ��ǥ�� ��ȯ�Ͽ� ������
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
		mousePosition.y = 0f; // Y ��ǥ�� 0���� �����Ͽ� 3D ���� ���� �������� �̷�

		// ��� Ÿ�Ͽ� ���� �ݺ��� ���� -> ��ȸ
		foreach (var tile in Tiles)
		{
			// ���� Ÿ���� ��ġ�� �����ͼ� Y ��ǥ�� 0���� �����Ͽ� 3D ���� ���� �������� �̷�
			Vector3 tilePos = tile.transform.position;
			tilePos.y = 0f;

			// ���콺 ��ġ�� Ÿ���� ��ġ ���� �Ÿ��� ������ �ִ� �Ÿ� �̳����
			if (Vector3.Distance(mousePosition, tilePos) <= FocusTileDistance)
			{
				CurrentFocusTile = tile; // ���� ���� Ÿ���� �ش� Ÿ�Ϸ� �����ϰ� �ݺ��� ����
				break;
			}
		}
	}


	// ���� �̹����� ������Ʈ�ϴ� �Լ�
	private void UpdateBuildImage()
	{
		bool bFocusTile = false; // ������ ������ Ÿ�� ���� Ȯ�� ����

		if (CurrentFocusTile) // ���� ������ �ִ� Ÿ���� �ִ��� Ȯ��
		{
			Tile tile = CurrentFocusTile.GetComponent<Tile>(); // ���� ������ �ִ� Ÿ�Ͽ��� Tile ������Ʈ ��������
			if (!tile.CheckIsOwned()) // Ÿ���� �������� �ʾҴ��� Ȯ��
			{
				Vector3 position = tile.transform.position; // Ÿ���� ��ġ�� ������
				position.y += BuildDeltaY; // Y�� ��ġ ����
				BuildIconPrefab.transform.position = position; // ���� �������� ��ġ ����
				bFocusTile = true; // ������ ������ Ÿ���� �߰ߵ�

				bool bCanBuild = GameManager.Inst.playerCharacter.CanUseCoin(NormalGuardianCost); // �������� ����� �Ǽ� ���� ���� Ȯ��
				Material mat = bCanBuild ? BuildCanMat : BuildCanNotMat; // �Ǽ� ���� ���ο� ���� ���͸��� ����
				BuildIconPrefab.GetComponent<MeshRenderer>().material = mat; // ���� �������� ���͸��� ����
			}
		}

		if (bFocusTile) // ������ ������ Ÿ���� �ִٸ�
		{
			BuildIconPrefab.gameObject.SetActive(true); // ���� ������ Ȱ��ȭ
		}
		else
		{
			DeActivateBuildImage(); // ������ ������ Ÿ���� ������ ���� �̹����� ��Ȱ��ȭ
		}
	}

	// ���� �̹����� ��Ȱ��ȭ�ϴ� �Լ�
	private void DeActivateBuildImage()
	{
		BuildIconPrefab.gameObject.SetActive(false); // BuildIconPrefab�� ��Ȱ��ȭ
	}

	// ������� �Ǽ��ϰ� Ȯ���ϴ� �Լ�
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

				OnBuild.Invoke(); // �Ǽ� �̺�Ʈ ȣ��
				DeActivateBuildImage(); // �Ǽ� �Ϸ� �� ���� �̹��� ��Ȱ��ȭ

				return;
			}

			if (tile && tile.OwnGuardian)
			{
				GameManager.Inst.guardianUpgradeManager.UpgradeGuardian(tile.OwnGuardian);
			}
		}
	}

	// Ű �Է��� ������Ʈ�ϴ� �Լ�
	private void UpdateKeyInput()
	{
		if (Input.GetMouseButtonUp(0)) // ���� ���콺 ���� Ŭ���� �ϸ� 
		{
			CheckToBuildGuardian(); // CheckToBuildGuardian �Լ� ȣ��
		}
	}
}
