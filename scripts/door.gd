extends Area2D

class_name Door

@export var destination_room_tag: String
@export var destination_door_tag: String

@onready var spawn: Marker2D = $Spawn

var _player: Actor

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	_player = get_tree().get_first_node_in_group("Player")
	StupidManager.GivePlayer.connect(set_player)

func _on_body_entered(body) -> void:
	if body == _player:
		NavigationManager.GoToRoom(destination_room_tag, destination_door_tag)

func set_player(Player: Actor) -> void:
	_player = Player
