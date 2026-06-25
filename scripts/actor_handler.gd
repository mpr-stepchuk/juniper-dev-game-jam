extends Node2D

signal wave_start()
signal wave_end()

# Called when the node enters the scene tree for the first time.
func _ready():
	pass
	


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	if Input.is_action_just_pressed("debug_wave_start"):
		wave_start.emit()
	if Input.is_action_just_pressed("debug_wave_end"):
		wave_end.emit()
	pass
