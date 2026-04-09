using Sandbox;

public sealed class Bullet : Component, Component.ITriggerListener
{
	public GameObject Shooter { get; set; }
	public float Damage { get; set; } = 25f;
	private TimeSince _timer;

	protected override void OnStart()
	{
		_timer = 0;
	}

	protected override void OnUpdate()
	{
		if (_timer > 30f)
		{
			GameObject.Destroy();
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.GameObject == Shooter) return;

		if (other.GameObject.Tags.Has("player")) {

			Log.Info(other.GameObject.Name);
			var healths = other.GameObject.GetComponentsInChildren<Health>();

			foreach (var health in healths)
			{
				Log.Info("Found health");
				health.health -= Damage;
			}

			GameObject.Destroy();
		}
	}
}
