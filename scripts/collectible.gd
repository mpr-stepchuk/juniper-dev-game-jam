extends Area2D

class_name Collectible

@onready var Collectible_Sprite: Sprite2D = $Sprite2D
@export var type: String
const SAUCE_SPRITE = preload("res://assets/placeholder_collectibles/sauce.png")
const SLIVER_SPRITE = preload("res://assets/placeholder_collectibles/sliver.png")
var _player: Actor

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	_player = get_tree().get_first_node_in_group("Player")
	set_data()
	connect("body_entered", collect)
	StupidManager.GivePlayer.connect(set_player)

func set_data() -> void:
	if type == "sauce":
		Collectible_Sprite.texture = SAUCE_SPRITE
	elif type == "sliver":
		Collectible_Sprite.texture = SLIVER_SPRITE

func collect(body: Node2D) -> void:
	if body == _player:
		if type == "sauce":
			body.AddGuts(50)
		elif type == "sliver":
			pass
		queue_free()

func set_player(Player: Actor) -> void:
	_player = Player
