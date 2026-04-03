using Sandbox;

public sealed class Health : Component
{
	[Property] public float MaxHealth { get; set; } = 100f;
	[Property] public float RespawnTime { get; set; } = 3f;
	public float health { get; set; }

	private TimeUntil _respawnTimer;
	private bool _isDead = false;

	protected override void OnStart()
	{
		health = MaxHealth;
	}

	protected override void OnUpdate()
	{
		if ( _isDead && _respawnTimer )
			Respawn();

		if ( health <= 0 && !_isDead )
			Die();
	}

	private Vector3 GetRandomSpawnPoint()
	{
		var spawnPoints = Scene.GetAll<SpawnPoint>().ToList();
		if ( spawnPoints.Count == 0 ) return Vector3.Zero;

		var random = spawnPoints[Game.Random.Next( spawnPoints.Count )];
		return random.GameObject.WorldPosition;
	}

	private void Die()
	{
		_isDead = true;
		_respawnTimer = RespawnTime;
		Log.Info( "Player died!" );

		var cc = GameObject.Parent.Components.Get<CharacterController>();
		if ( cc != null ) cc.Enabled = false;

		TeleportToPosition( new Vector3( 0, 0, -10000f ) );
	}

	private void Respawn()
	{
		_isDead = false;
		health = MaxHealth;
		Log.Info( "Player respawned!" );

		var cc = GameObject.Parent.Components.Get<CharacterController>();
		if ( cc != null ) cc.Enabled = true;

		TeleportToPosition( GetRandomSpawnPoint() );
	}

	[Rpc.Owner]
	private void TeleportToPosition( Vector3 position )
	{
		GameObject.Parent.WorldPosition = position;
		Log.Info( "Position set to: " + GameObject.Parent.WorldPosition );
	}
}
