using Godot;
using System;

using Dizzy;

public partial class HUD : CanvasLayer
{
	private int prevGuts = 0;
	
	private Player _player;
	private WaveManager _wm;
	private DizzyStats _stats;
	private RichTextLabel _label;
	private HealthBar _healthBar;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_healthBar = GetNode<HealthBar>("Healthbar");
		_label = GetNode<RichTextLabel>("RichTextLabel");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		_wm = GetTree().GetFirstNodeInGroup("WaveManager") as WaveManager;
		_label.AddThemeColorOverride("default_color", Colors.Yellow);
		_stats = _player.GetStats();
		GD.Print("HUD Ready");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		prevGuts = _stats.Guts;
		if (_player == null) {
			GD.PrintErr("ERR(HUD._Process()) - no player found");
			return;
		}
	
		
		_stats = _player.GetStats();
		
		if (_stats.Guts < prevGuts || _stats.Guts > prevGuts) {
			_healthBar.SetGuts(_stats.Guts);
		}
		
		_label.Text = string.Format("Guts: {0}\nSpins: {1}\nSuper: {2}\nCash: ${3}\nWave: {4}\nEnemies Left: {5}",
		_stats.Guts,
		_stats.Spins,
		_stats.Super,
		_stats.Cash,
		_wm.WaveCount,
		_wm.EnemiesLeft
		);
	}
	
	public void InitHud() {
		if (_player != null) {
			_stats = _player.GetStats();
			_healthBar.InitGuts(_stats.Guts);
		}
	}
}
