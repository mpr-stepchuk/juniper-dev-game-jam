extends CanvasLayer

signal end_wheel

@onready var RouletteWheel = $RouletteWheel

@export var Player: Actor

func _on_wave_manager_wave_end() -> void:
	RouletteWheel.start_wheel()
