using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class BossSM : Node
{

	[Export] public State InitialState;

	public State CurrentState;
	private Boss _boss;
	
	public void Init()
	{	
		if (InitialState != null) {
			CurrentState = InitialState;
		}

		_boss = GetParent() as Boss;
		

		Godot.Collections.Array<State> states;
		GD.Print("Child nodes:");
		foreach (Node node in FindChildren("*", "State"))
		{
			GD.Print(node.Name);	
		}

		states = new Godot.Collections.Array<State>(FindChildren("*").OfType<State>().ToList<State>());

		foreach (State state in states) {
			GD.Print("Connecting "+state.Name+" to TransitionToNext()");
			state.Finished += TransitionToNext;
		}

		CurrentState.Enter("");

	}

	public override void _PhysicsProcess(double delta)
	{
		CurrentState.Update(delta);
	}

	public void TransitionToNext(string targetState)
	{
		GD.Print("TransitionToNext():   "+CurrentState.Name+" -> "+targetState);
	
		if (!HasNode(targetState))
		{
			GD.PrintErr(_boss.Name + ": trying to transition to state "+ targetState+ " but it does not exist");
			return;
		}
	
		if ((_boss.FacingRight() && _boss.GlobalPosition.X > _boss._player.GlobalPosition.X) || (!_boss.FacingRight() && _boss.GlobalPosition.X < _boss._player.GlobalPosition.X))
		{
			GD.Print("\n\nFLIPPPPPPPPP\n\n");
			_boss.Flip();
			GD.Print("setFlipH? "+_boss._animatedSprite2D.FlipH);
		}

	
		string prevState = CurrentState.Name;
		CurrentState.Exit();
		CurrentState = GetNode(targetState) as State;
		CurrentState.Enter(prevState);
		_boss.FacePlayer();
	}

}
