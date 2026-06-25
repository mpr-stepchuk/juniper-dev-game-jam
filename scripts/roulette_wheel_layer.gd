extends CanvasLayer

@onready var RouletteWheel = $RouletteWheel

@export var Player: Actor

func _on_actor_handler_wave_end():
	RouletteWheel.start_wheel()
