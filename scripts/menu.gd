extends CanvasLayer

var death_menu: CanvasLayer

# Called when the node enters the scene tree for the first time.
func _ready():
	death_menu = get_tree().get_first_node_in_group("DeathMenu")
	visible = false
	get_tree().paused = false

func _input(_event: InputEvent) -> void:
	if Input.is_action_just_pressed("pause") and death_menu.visible == false:
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
