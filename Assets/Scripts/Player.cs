using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public LiveGlobals globalsPrefab = null;
	private Vector3 startPos = Vector3.zero;
	public Rigidbody body;

	private bool godMode = false;
	public bool ignoreDamage = false;
	public float maxHealthSeconds = 100;
	private float health = 0;
	public int maxLivers = 1;
	public List<Liver> EatenLivers = new List<Liver>();
	public Transform liverSack = null;

	public float sceneChangeY = 100;

	public bool LiverFull => EatenLivers.Count >= maxLivers;

	public UnityEvent OnDie = null;

	void Awake() {
		if (globalsPrefab != null) {
			globalsPrefab.InitIntoScene();
		}

		LiveGlobals.Instance.player = this;

		health = maxHealthSeconds;

		startPos = transform.position;
	}

	void Reset(bool hard) {
		StartCoroutine(reset(hard, 0));
	}

	public void HardResetOnDelay(float delay) {
		StartCoroutine(reset(true, delay));
	}

	private IEnumerator reset (bool hard, float delay) {
		yield return new WaitForSeconds(delay);
		if (hard) {
			LiveGlobals.Instance.Respawn();
			SceneManager.LoadScene(LiveGlobals.DeliveryScene);
		}
		else {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	public void StashLiver(Liver liver) {
		liver.gameObject.SetActive(false);
		liver.transform.SetParent(liverSack);
		EatenLivers.Add(liver);

		Hud.Instance.AddLiver(liver);

		IgnoreDamage(false);
	}

	public void SoakDamage(float damage) {
		var respectDamage = !ignoreDamage;
#if UNITY_EDITOR
		respectDamage &= !godMode;
#endif

		if (respectDamage) {
			if (damage <= health) {
				health -= damage;
			} else {
				damage -= health;
				health = 0;
				for (int i = 0; i < EatenLivers.Count && damage > 0; i++) {
					if (EatenLivers[i] != null) {
						damage = EatenLivers[i].SoakDamage(damage);
					}
				}

				if (damage > 0) {
					Die();
				}
			}
		}
	}

	public void Die() {
		var body = GetComponent<Rigidbody>();
		if (body != null) { body.isKinematic = true; }
		OnDie.Invoke();
	}

	public void IgnoreDamage(bool ignore) {
		ignoreDamage = ignore;
	}

	void Update()
	{
		if (sceneChangeY <= 0) {
			if (transform.position.y < sceneChangeY) {
				SceneManager.LoadScene(LiveGlobals.DeliveryScene);
			}
		} else {
			if (transform.position.y > sceneChangeY) {
				SceneManager.LoadScene(LiveGlobals.CollectionScene);
			}
		}

		//TODO shouldn't all the Fixed updates use fixed delta time
		if (!ignoreDamage) {
			SoakDamage(Time.deltaTime);
		
			if (Hud.Instance != null) {
				Hud.Instance.UpdateHealth(health / maxHealthSeconds, EatenLivers);
			}
		}

#if UNITY_EDITOR
		if (Input.GetKeyDown("r")) {
			Reset(false);
		}
		if (Input.GetKeyDown("g")) {
			godMode = !godMode;
			Debug.Log("God Mode " + (godMode ? "Enabled" : "Disabled"));
		}
#endif
	}

#if UNITY_EDITOR
	void OnDrawGizmos() {
		if (!Application.isPlaying && LiveGlobals.Instance == null) {
			globalsPrefab.InitWithoutScene();
		}
	}
#endif
}
