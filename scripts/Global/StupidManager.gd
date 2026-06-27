extends Node

signal GivePlayer(Player: Actor)

func reset_room() -> void:
	var room = get_tree().get_first_node_in_group("Room")
	var main = get_tree().get_first_node_in_group("Main")
	room.queue_free()
	main.remove_child(room)

func give_player() -> void:
	var Player: Actor = get_tree().get_first_node_in_group("Player")
	GivePlayer.emit(Player)
