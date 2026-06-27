extends CanvasLayer

signal end_wheel

@onready var RouletteWheel = $RouletteWheel
@onready var Player = get_tree().get_first_node_in_group("Player")

func _ready() -> void:
	NavigationManager.OnPlayerSpawn.connect(_on_spawn)

func _on_wave_manager_wave_end() -> void:
	Player = get_tree().get_first_node_in_group("Player")
	RouletteWheel.start_wheel()

func _on_spawn(_position: Vector2) -> void:
	Player = get_tree().get_first_node_in_group("Player")
