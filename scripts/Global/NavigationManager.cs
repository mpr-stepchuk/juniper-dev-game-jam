using Godot;
using System;

using Dizzy;

public partial class NavigationManager : Node
{
	public static NavigationManager Instance { get; private set; }
	
	public PackedScene SceneMap; 
	public PackedScene SceneRoom1;
	public PackedScene player;

	[Signal] public delegate void OnPlayerSpawnEventHandler(Vector2 position);
	//[Signal] public delegate void OnPlayerUhhhhEventHandler(DizzyStats stats);
	private string _spawnDoorTag;

	public override void _Ready()
	{
		Instance = this;
		SceneMap = GD.Load<PackedScene>("res://scenes/rooms/map.tscn");
		SceneRoom1 = GD.Load<PackedScene>("res://scenes/rooms/room1.tscn");
		player = GD.Load<PackedScene>("res://scenes/player.tscn");
	}

	public void GoToRoom(string roomTag, string destTag) {
		PackedScene destScene = new PackedScene();
		
		switch (roomTag){
			case "map":
				destScene = SceneMap;
				break;
			case "room1":
				destScene = SceneRoom1;
				break;
		}
		
		if (destScene != null) {
			_spawnDoorTag = destTag;
			Player _player = GetTree().GetFirstNodeInGroup("Player") as Player;
			Node newScene = destScene.Instantiate();
			_player.Reparent(newScene);
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToNode, newScene);
		}
		
	}
	
	public void CreateRoom(string roomTag) {
		PackedScene destScene = SceneMap;
		
		switch (roomTag){
			case "map":
				destScene = SceneMap;
				break;
			case "room1":
				destScene = SceneRoom1;
				break;
		}
		
		if (destScene != null) {
			GD.Print("MAKING NEW SCENE");
			Node newScene = destScene.Instantiate();
			Node main = GetTree().GetFirstNodeInGroup("Main");
			main.AddChild(newScene);
			Node _player = player.Instantiate();
			newScene.AddChild(_player);
		} 
	}

	public void PlayerSpawn(Vector2 position) {
		Player _player = GetTree().GetFirstNodeInGroup("Player") as Player;
		if (_player != null) {
			_player._OnSpawn(position);
		}
	}
	
	
	public void PlayerDeath() {
		GD.Print("PLAYER DIED!!!");
		CanvasLayer deathMenu = GetTree().GetFirstNodeInGroup("DeathMenu") as CanvasLayer;
		deathMenu.Visible = true;
		GetTree().Paused = true;
	}

}
