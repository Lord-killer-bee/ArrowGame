using UnityEngine;

public class Bullet : MonoBehaviour 
{
	[SerializeField]
	private float _BulletVelocity = 16f;

	[SerializeField]
	//private ParticleSystem _Splash;
	private int collisioncounter = 0;

	public int BulletNumber;

	private void OnEnable()
	{
		GetComponent<Rigidbody>().velocity = transform.forward * _BulletVelocity;
	}

	private void Update()
	{
		if(gameObject.GetComponent<Rigidbody>().velocity == new Vector3(0,0,0))
		{
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter(Collision other) 
	{
		//_Splash.Play();
		if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Platform"))
		{
			if(other.gameObject.CompareTag("Platform"))
			{
				//AkSoundEngine.SetSwitch("Character", "Body", gameObject);
				//AkSoundEngine.SetSwitch("Damage", "Object", gameObject);
				//AkSoundEngine.PostEvent("Play_SFX_Damage", gameObject);

				collisioncounter++;
			}

			ContactPoint contact = other.contacts[0];
			Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
			rot *= Quaternion.Euler(0,90,0);
			Vector3 pos = contact.point;

			//Vector3 pos = other.gameObject.transform.position + new Vector3(0,0.5f,0);
			//ParticleSystem SplashEffect = Instantiate(_Splash, pos, rot);

			

			//ParticleSystem SplashEffect = Instantiate(_Splash, pos, other.gameObject.transform.rotation);
			//ParticleSystem[] SubSplashes = SplashEffect.GetComponentsInChildren<ParticleSystem>();
			/*
			foreach (var item in SubSplashes)
			{
				item.startColor = gameObject.GetComponent<Renderer>().material.color;
				if(other.gameObject.GetComponent<MainPlatform>() == null)
				{
					if(item.GetComponent<Decal>() != null)
					{
						item.Stop();
					}
				}
			}
			*/
			//if(other.gameObject.CompareTag("Player"))
			//SplashEffect.transform.SetParent(other.gameObject.transform);

			if(other.gameObject.CompareTag("Player") || collisioncounter >= 2)
			Destroy(gameObject);	
		}
	}

	
	
}
