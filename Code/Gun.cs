using Sandbox;

public sealed class Gun : Component
{
	[Property] public int AmmoCapacity { get; set; } = 10;
	[Property] public float FireRate { get; set; } = 0.1f;
	[Property] public GameObject MuzzlePoint { get; set; }
	[Property] public GameObject BulletPrefab { get; set; }

	public bool IsReloading { get; private set; } = false;
	public int Ammo { get; set; }

	private TimeSince _timeSinceLastShot;
	private TimeUntil _reloadTimer;

	protected override void OnStart()
	{
		Ammo = AmmoCapacity;
	}

	protected override void OnUpdate()
	{
		if (IsProxy) return;
		if (IsReloading && _reloadTimer)
		{
			Ammo = AmmoCapacity;
			IsReloading = false;
			Log.Info("Reload done!");
		}

		if (Input.Pressed("attack1") && _timeSinceLastShot > FireRate && Ammo > 0 && !IsReloading)
		{
			Shoot();
			_timeSinceLastShot = 0;
			Ammo--;
		}

		if (Input.Pressed("reload") && !IsReloading)
		{
			IsReloading = true;
			_reloadTimer = 3f;
			Log.Info("Reloading...");
		}
	}

	private void Shoot()
	{
		var camera = Scene.Camera;
		var spawnPos = camera.WorldPosition + camera.WorldRotation.Forward * 50f;
		var bullet = BulletPrefab.Clone( spawnPos );
		var bulletComponent = bullet.GetComponent<Bullet>();

		bulletComponent.Shooter = GameObject;
		bullet.NetworkSpawn();
		var rb = bullet.Components.Get<Rigidbody>();
		rb.Velocity = camera.WorldRotation.Forward * 2000f;
	}
}
