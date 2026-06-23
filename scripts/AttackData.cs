using Godot;

public partial class AttackData : Resource
{
	[ExportCategory("Name")]
	[Export] public string AttackName;
	[ExportCategory("Animation")]
	[Export] public string AnimationName;
	[Export] public float StartupTime; 
	[Export] public float ActiveTime;
	[Export] public float RecoveryTime;
	[ExportCategory("HitboxDetails")]
	[Export] public Vector2 HitboxOffset;
	[Export] public Vector2 HitboxScale;
	[ExportCategory("Attack Details")]
	[Export] public int Damage;
	[Export] public bool CanCrit;
	[Export] public bool Friendly;
	[Export] public float KnockbackForce = 0.0f;
	[Export] public float KnockbackDuration = 0.0f;
	[Export] public float KnockbackAngle = 0.0f;
}
