extends Node2D

class_name Room;

# Called when the node enters the scene tree for the first time.
func _ready():
	if NavigationManager._spawnDoorTag != null:
		_on_room_spawn(NavigationManager._spawnDoorTag)

func _on_room_spawn(destination_tag: String) -> void:
	var door_path = "Doors/Door" + destination_tag
	if has_node(door_path):
		var door = get_node(door_path) as Door
		NavigationManager.PlayerSpawn(door.spawn.global_position)
