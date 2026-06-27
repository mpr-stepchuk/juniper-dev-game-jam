extends CanvasLayer


# Called when the node enters the scene tree for the first time.
func _ready():
	visible = false
	get_tree().paused = false

func _input(_event: InputEvent) -> void:
	if Input.is_action_just_pressed("pause"):
		if get_tree().paused:
			visible = false
			get_tree().paused = false
		else:
			visible = true
			get_tree().paused = true

func _on_button_pressed():
	visible = false
	get_tree().paused = false


func _on_main_menu_pressed():
	pass # Replace with function body.
