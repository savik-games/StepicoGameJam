using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : MonoBehaviour {
	[Header("Values"), Space]
	[SerializeField] bool isPlayerWeapon = false;
	[SerializeField] float startupTime = 0.5f;
	[SerializeField] float cooldownTime = 1.0f;
	[Space]
	[SerializeField] int maxBufferedAttacks = 2;
	[Space]
	[SerializeField] float startSize = 0.33f;
	[SerializeField] float endSize = 1.0f;

	[Header("Values - bullet")]
	[SerializeField] float damage = 25;
	[SerializeField] float flySpeed = 10.0f;

	[Header("Refs"), Space]
	[SerializeField] GameObject projectilePrefab;
	[SerializeField] Transform bulletSpawnPos;
	[SerializeField] SpriteRenderer sr;

#if UNITY_EDITOR
	private void OnValidate() {
		if (!sr)
			sr = GetComponent<SpriteRenderer>();
	}
#endif

	bool isAttacking = false;
	int currBufferedAttacks = 0;

	float currTimer = 0.0f;

	Projectile currProjectile;

	private void Update() {
		currTimer += Time.deltaTime;

		if (currProjectile != null) {
			currProjectile.transform.localScale = Vector3.one * LeanTween.easeOutBack(startSize, endSize, Mathf.Clamp01(currTimer / startupTime));

			if (currTimer >= startupTime) {
				currTimer -= startupTime;

				currProjectile.transform.parent = null;
				currProjectile.Launch();

				currProjectile = null;

				if (currBufferedAttacks != 0)
					--currBufferedAttacks;
			}
		}
		else if(isAttacking || currBufferedAttacks != 0) {
			if (currTimer >= cooldownTime) {
				currTimer = 0;
				currProjectile = Instantiate(projectilePrefab, bulletSpawnPos.position, transform.localRotation, bulletSpawnPos).GetComponent<Projectile>();
				currProjectile.transform.localEulerAngles = Vector3.zero;

				currProjectile.transform.localScale = Vector3.one * startSize;

				currProjectile.Init(isPlayerWeapon, damage, flySpeed);
			}
		}
	}

	public void AttackSingle() {
		currBufferedAttacks = Mathf.Clamp(currBufferedAttacks + 1, 0, maxBufferedAttacks);
	}

	public void StartAttackSequence() {
		isAttacking = true;
	}

	public void StopAttackSequence() {
		isAttacking = false;
	}

	public void ForceStopAttack() {
		if (currProjectile)
			Destroy(currProjectile);
		isAttacking = false;
		currBufferedAttacks = 0;
	}

	public void Enable() {
		enabled = true;
		sr.enabled = true;
	}

	public void Disable() {
		enabled = false;
		sr.enabled = false;
	
		ForceStopAttack();
	}

	private void OnDestroy() {
		ForceStopAttack();
	}
}