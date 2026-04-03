using Sandbox;

public sealed class Bullet : Component, Component.ITriggerListener
{
	public float Damage { get; set; } = 25f;

	private TimeSince _timer;

	protected override void OnStart()
	{
		_timer = 0;
	}

	protected override void OnUpdate()
	{
		if ( _timer > 30f )
		{
			Log.Info( "30 seconds passed!" );
			GameObject.Destroy();
		}
	}

	public void OnTriggerEnter( Collider other )
	{
		if ( !other.GameObject.Name.Contains( "Player" ) ) return;

		var allHealth = Scene.GetAll<Health>();
		foreach ( var h in allHealth )
		{
			if ( h.GameObject.Parent == other.GameObject )
			{
				h.health -= 25f;
				Log.Info( $"Hit! Health is now {h.health}" );
				GameObject.Destroy();
				return;
			}
		}
	}
}
