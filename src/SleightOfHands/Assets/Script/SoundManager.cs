using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	[Header("UnitsReffered")]
	[SerializeField] private AudioClip Jump;
	[SerializeField] private AudioClip UseEnhancementCard;
	[SerializeField] private AudioClip UseGnomePotion;
	
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

	public void PlaySoundJump()
	{
		soundPlayer.PlayOneShot(Jump);
	}
	
	public void PlaySoundEnhancement()
	{
		soundPlayer.PlayOneShot(UseEnhancementCard);
	}
	
	public void PlaySoundGnomePotion()
	{
		soundPlayer.PlayOneShot(UseGnomePotion);
	}
}
