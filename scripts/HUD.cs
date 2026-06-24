using Godot;
using System;

using Dizzy;

public partial class HUD : CanvasLayer
{
	
	private Player _player;
	private DizzyStats _stats;
	private RichTextLabel _label;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_label = GetNode<RichTextLabel>("RichTextLabel");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		_label.AddThemeColorOverride("default_color", Colors.Yellow);
		GD.Print("HUD Ready");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_player == null) {
			GD.PrintErr("ERR(HUD._Process()) - no player found");
			return;
		}
	
		
		_stats = _player.GetStats();
		
		_label.Text = string.Format("Guts: {0}\nSpins: {1}\nSuper: {2}\nCash: ${3}",
		_stats.Guts,
		_stats.Spins,
		_stats.Super,
		_stats.Cash
		);
	}
}
