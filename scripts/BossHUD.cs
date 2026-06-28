using Godot;
using System;

using Enemies;

public partial class BossHUD : Node
{
	// Called when the node enters the scene tree for the first time.
	
	private RichTextLabel _label;
	private Boss _boss;
	private Timer _throwCD;
	private BossSM _sm;
	

	public override void _Ready()
	{
		_label = GetNode<RichTextLabel>("RichTextLabel"); 
		_boss = GetTree().GetFirstNodeInGroup("Boss") as Boss;
		_throwCD = _boss.GetNode<Timer>("ThrowTimer");
		_sm = _boss.GetNode<BossSM>("BossSM");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_boss == null) {
			GD.PrintErr("ERR(BossHUD._Process()) - no boss found");
			return;
		}
	
		
		
		_label.Text = string.Format("Boss Guts: {0}\nCurrent State: {1}\nThrow Timer : {2} s\nGlobalPosition: {3}\nFacing right? {4}\nFlipH: {5}",
		_boss._stats.Guts,
		_sm.CurrentState.Name,
		_throwCD.GetTimeLeft(),
		_boss.GlobalPosition,
		_boss.FacingRight(),
		_boss._animatedSprite2D.FlipH
		);
	}

}
