using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	public TextMeshProUGUI HeartText;
	public TextMeshProUGUI CoinText;

	void Update()
	{
		HeartText.text = $"x {GameManager.Inst.playerCharacter.Heart}";
		CoinText.text = $"x {GameManager.Inst.playerCharacter.Coin}";
	}
}
