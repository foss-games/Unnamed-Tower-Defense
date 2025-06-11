using Godot;

public partial class Particles : GpuParticles2D
{
    public override void _Ready()
    {
        base._Ready();
        Emitting = true;
        Finished += Die;
    }

    private void Die()
    {
        QueueFree();
    }
}
