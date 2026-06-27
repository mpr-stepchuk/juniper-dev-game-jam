extends CanvasLayer


func _on_play_pressed():
	NavigationManager.CreateRoom("map")
	var PlayerSpawn: Marker2D = get_tree().get_first_node_in_group("PlayerSpawn")
	print(PlayerSpawn.global_position)
	NavigationManager.PlayerSpawn(PlayerSpawn.global_position)
	StupidManager.give_player()
	visible = false


func _on_quit_pressed():
	get_tree().quit()
