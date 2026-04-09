using Sandbox;

public sealed class Health : Component
{
	[Property] public float MaxHealth { get; set; } = 100f;
	[Property] public float RespawnTime { get; set; } = 3f;
	public float health { get; set; }

	private TimeUntil _respawnTimer;
	private bool _isDead = false;

	[Property, Sync] public GameObject Ragdoll { get; set; }

	protected override void OnStart()
	{
		health = MaxHealth;

		if ( !IsProxy )
		{
			var playerRenderer = GameObject.Parent.Components.GetInDescendantsOrSelf<SkinnedModelRenderer>();
			if ( playerRenderer != null )
			{
				Ragdoll = new GameObject( false, "Ragdoll" );
				Ragdoll.WorldPosition = new Vector3( 0, 0, -10000f );
				
				var renderer = Ragdoll.Components.Create<SkinnedModelRenderer>();
				renderer.Model = playerRenderer.Model;

				var physics = Ragdoll.Components.Create<ModelPhysics>();
				physics.Renderer = renderer;
				physics.Enabled = false;

				Ragdoll.NetworkSpawn();
				Ragdoll.Enabled = true;
			}
		}
	}

	protected override void OnUpdate()
	{
		if (health <= 0 && !_isDead)
			Die();

		if (_isDead && _respawnTimer)
			Respawn();
	}

	private Vector3 GetRandomSpawnPoint()
	{
		var spawnPoints = Scene.GetAll<SpawnPoint>().ToList();
		if (spawnPoints.Count == 0) return Vector3.Zero;

		var random = spawnPoints[Game.Random.Next(spawnPoints.Count)];
		return random.GameObject.WorldPosition;
	}

	private void Die()
	{
		_isDead = true;
		_respawnTimer = RespawnTime;
		Log.Info("Player died!");

		var root = GameObject.Parent;
		var cc = root.Components.Get<CharacterController>();
		var playerRenderer = root.Components.GetInDescendantsOrSelf<SkinnedModelRenderer>();
		var velocity = cc?.Velocity ?? Vector3.Zero;

		if ( cc != null ) cc.Enabled = false;
		if ( playerRenderer != null ) playerRenderer.Enabled = false;

		if ( Ragdoll != null )
		{
			Ragdoll.WorldPosition = root.WorldPosition;
			Ragdoll.WorldRotation = root.WorldRotation;

			var physics = Ragdoll.Components.Get<ModelPhysics>();
			if ( physics != null )
			{
				physics.Enabled = true;
				physics.CopyBonesFrom( playerRenderer, true );
				foreach ( var b in physics.Bodies )
				{
					b.Component.ApplyImpulse( velocity * b.Component.Mass );
				}
			}
		}

		if ( !IsProxy ) TeleportToPosition( new Vector3( 0, 0, -10000f ) );
	}

	private void Respawn()
	{
		_isDead = false;
		health = MaxHealth;
		Log.Info("Player respawned!");

		var root = GameObject.Parent;
		var cc = root.Components.Get<CharacterController>();

		if ( cc != null ) cc.Enabled = true;

		if ( Ragdoll != null )
		{
			Ragdoll.WorldPosition = new Vector3( 0, 0, -10000f );
			
			var physics = Ragdoll.Components.Get<ModelPhysics>();
			if ( physics != null ) physics.Enabled = false;
		}

		TeleportToPosition( GetRandomSpawnPoint() );
	}

	[Rpc.Owner]
	private void TeleportToPosition( Vector3 position )
	{
		GameObject.Parent.WorldPosition = position;
		Log.Info("Position set to: " + GameObject.Parent.WorldPosition);
	}
}
