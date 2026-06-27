using Godot;
using System;

using Dizzy;

public partial class HealthBar : ProgressBar
{
	private Timer _barTimer;
	private ProgressBar _dmgBar;
	private int _guts;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_barTimer = GetNode<Timer>("Timer");
		_dmgBar = GetNode<ProgressBar>("DamageBar");
		_guts = 0;
	}
	
	public void SetGuts(int newGuts) {
		int prevGuts = _guts;
		_guts = (int) Mathf.Min(MaxValue, newGuts);
		Value = _guts;
		
		if (_guts < prevGuts) {
			_barTimer.Start();
		} else {
			_dmgBar.Value = _guts;
		}
	}
	
	public void InitGuts(int guts) {
		_guts = guts;
		MaxValue = _guts;
		Value = _guts;
		_dmgBar.MaxValue = _guts;
		_dmgBar.Value = _guts;
	}
	
	public void _on_timer_timeout(){
		_dmgBar.Value = _guts;
	}
}
