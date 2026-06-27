extends CanvasLayer


# Called when the node enters the scene tree for the first time.
func _ready():
	visible = false
	get_tree().paused = false


func _on_button_pressed():
	pass # Replace with function body.


func _on_retry_pressed():
	StupidManager.reset_room()
	NavigationManager.CreateRoom("map")
	var PlayerSpawn: Marker2D = get_tree().get_first_node_in_group("PlayerSpawn")
	NavigationManager.PlayerSpawn(PlayerSpawn.global_position)
	StupidManager.give_player()
	visible = false


func _on_main_menu_pressed():
	StupidManager.reset_room()
	var MainMenu: CanvasLayer = get_tree().get_first_node_in_group("MainMenu")
	MainMenu.visible = true
	visible = false
