using UnityEngine;
using System.Collections;
using Jolly;

public class ChannelVisual : MonoBehaviour
{
	public float ChannelTime;
	public Hero Hero;
	public float TimeRemaining;
	private GameObject soundObject;

	void Start ()
	{
		this.TimeRemaining = this.ChannelTime;
		if(Hero) this.soundObject = SoundFX.Instance.OnHeroGrowChannel(this.Hero);
	}

	public void Stop()
	{
        /*
		AudioSourceExt.StopClipOnObject(this.soundObject);
		Destroy(this.soundObject);
		Destroy(this.gameObject);
        */
	}

	void Update ()
	{

		this.TimeRemaining -= Time.deltaTime;
		float yaw = 360.0f * (this.TimeRemaining / this.ChannelTime);

	    if (yaw <= 0f) return;

		this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, yaw));

		if (this.TimeRemaining < 0.0f)
		{
			this.Stop();
		}
	}
}
