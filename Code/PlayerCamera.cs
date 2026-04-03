using Sandbox;

public sealed class PlayerCamera : Component
{
    protected override void OnStart()
    {
        if ( IsProxy )
        {
            Components.Get<CameraComponent>().Enabled = false;
        }
    }
}
