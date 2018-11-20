using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	[Header("UnitsReffered")]
	[SerializeField] private AudioClip JumpSound;
	[SerializeField] private AudioClip UseEnhancementCard;
	[SerializeField] private AudioClip UseGnomePotion;
	[SerializeField] private AudioClip DeadSound;
	[SerializeField] private AudioClip[] Splash;
	[SerializeField] private AudioClip[] HurtSound;
	[SerializeField] private AudioClip AttackMissSound;
	[SerializeField] private AudioClip FoundPlayer;
	[SerializeField] private AudioClip SpellSound;
	[Header("UI")]
	[SerializeField] private AudioClip TapTileSound;
	[SerializeField] private AudioClip TapCardSound;
	[Header("BGM")] 
	[SerializeField] private AudioClip bgm;

	public static  SoundManager Instance { get; private set; }

	private AudioSource soundPlayer;
	void Awake () {
		if (!Instance)
			Instance = this;
		else
			Destroy(gameObject);
		soundPlayer = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Jump()
	{
		soundPlayer.PlayOneShot(JumpSound);
	}
	
	public void Enhancement()
	{
		soundPlayer.PlayOneShot(UseEnhancementCard);
	}
	
	public void GnomePotion()
	{
		soundPlayer.PlayOneShot(UseGnomePotion);
	}
	public void Dead()
	{
		soundPlayer.PlayOneShot(DeadSound);
	}
	public void TapTile()
	{
		soundPlayer.PlayOneShot(TapTileSound);
	}
	public void TapCard()
	{
		soundPlayer.PlayOneShot(TapCardSound);
	}

	public void Attack()
	{
		var temp = Random.Range(0, Splash.Length);
		soundPlayer.PlayOneShot(Splash[temp]);
	}
	
	public void AttackMiss()
	{
		soundPlayer.PlayOneShot(AttackMissSound);
	}

	public void Found()
	{
		soundPlayer.PlayOneShot(FoundPlayer);
	}
	
	public void Hurt()
	{
		var temp = Random.Range(0, HurtSound.Length);
		soundPlayer.PlayOneShot(HurtSound[temp]);
	}
	public void Spell()
	{
		soundPlayer.PlayOneShot(SpellSound);
	}
}
