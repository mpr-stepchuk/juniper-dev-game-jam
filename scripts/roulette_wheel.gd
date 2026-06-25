extends Node2D

signal update_button

@onready var CanvasLayer0 = $CanvasLayer
@onready var CanvasLayer1 = $"CanvasLayer 2"
@onready var CanvasLayer2 = $"CanvasLayer 3"
@onready var SpinMenu = $"Spin Menu"
@onready var PrizeMenu = $"Prize Menu"
@onready var Display = $Display
@onready var CashBetDisplay = $Display/CashBetDisplay
@onready var BettingOnDisplay = $Display/BettingOnDisplay
@onready var PrizePicker = $"Prize Picker"
@onready var PrizeButton1 = $"Prize Picker/Prize1"
@onready var PrizeButton2 = $"Prize Picker/Prize2"
@onready var PrizeButton3 = $"Prize Picker/Prize3"
@onready var RouletteWheelLayer = $".."

@onready var Player = RouletteWheelLayer.Player

var win: bool = false
var even_odd: int = 0
var prize_mult: float = 0.0
var prize_text: Array = ["","",""]
var prize: Array = [0.0,0.0,0.0]
var colour: int
var number: int
var spinning: bool
var time_elapsed: float
var money_bet: float = 0.0

# Called when the node enters the scene tree for the first time.
func _ready():
	CanvasLayer1.transform = global_transform
	CanvasLayer2.transform = global_transform
	
	SpinMenu.rotation = 0.0
	PrizeMenu.rotation = 0.0
	Display.rotation = 0.0
	PrizeMenu.visible = false
	
	spinning = false
	number = randi_range(1, 36)
	if(number != 1):
		CanvasLayer1.rotation += deg_to_rad(-10*(number-1))


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if visible:
		time_elapsed += delta*3.0
		if(!spinning):
			if(int(time_elapsed) == 1):
				time_elapsed = 0.0
				number += 1
				CanvasLayer1.rotation += deg_to_rad(-10)
				
				if number == 37:
					number = 1

func spin_wheel() -> void:
	spinning = true
	for i in range (randi_range(20,70)):
		await get_tree().create_timer(0.1*(float(i)/30.0+1.0)).timeout
		number += 1
		CanvasLayer1.rotation += deg_to_rad(-10)
		
		if number == 37:
			number = 1
		
	spinning = false
	time_elapsed = 0.0
	return

func start_wheel() -> void:
	time_elapsed = 0
	money_bet = 0
	even_odd = 0
	CashBetDisplay.clear()
	BettingOnDisplay.clear()
	spinning = false
	Player.Teleport(Vector2(0, 55))
	CanvasLayer0.visible = true
	await get_tree().create_timer(1.0).timeout
	get_tree().paused = true
	RouletteWheelLayer.visible = true
	CanvasLayer1.visible = true
	CanvasLayer2.visible = true
	SpinMenu.visible = true
	PrizeMenu.visible = false
	PrizePicker.visible = false
	Display.visible = true
	return

func end_wheel() -> void:
	RouletteWheelLayer.visible = false
	CanvasLayer0.visible = false
	CanvasLayer1.visible = false
	CanvasLayer2.visible = false
	SpinMenu.visible = false
	PrizeMenu.visible = false
	PrizePicker.visible = false
	Display.visible = false
	get_tree().paused = false
	return

func prize_menu() -> void:
	Display.visible = false
	CanvasLayer1.visible = false
	CanvasLayer2.visible = false
	make_prize(PrizeButton1, 0)
	make_prize(PrizeButton2, 1)
	make_prize(PrizeButton3, 2)
	PrizePicker.visible = true

func make_prize(PrizeButton, prize_number: int) -> void:
	var stat_mult: float = 0
	print(prize_number)
	
	if even_odd == 1:
		match prize_number:
			0:
				prize_text[prize_number] = "Hustle"
				stat_mult = 0.01
			1:
				prize_text[prize_number] = "Crit Chance"
				stat_mult = 0.02
			2:
				prize_text[prize_number] = "Crit Mod"
				stat_mult = 0.03
	else:
		match prize_number:
			0:
				prize_text[prize_number] = "Hustle"
				stat_mult = 0.01
			1:
				prize_text[prize_number] = "Steadiness"
				stat_mult = 0.025
			2:
				prize_text[prize_number] = "Coordination"
				stat_mult = 0.10
	prize[prize_number] = ((money_bet/100)*stat_mult)*prize_mult
	PrizeButton.rich_text = prize_text[prize_number]
	update_button.emit(prize_number)
	return

func give_prize(prize_number: int) -> void:
	match prize_text[prize_number]:
		"Hustle":
			Player.AddHustle(prize[prize_number])
		"Crit Chance":
			Player.AddCritChance(prize[prize_number])
		"Crit Mod":
			Player.AddCritMod(prize[prize_number])
		"Steadiness":
			Player.AddSteadiness(prize[prize_number])
		"Coordination":
			Player.AddCoordination(prize[prize_number])
	Player.Teleport(Vector2(0, -22))
	await get_tree().create_timer(0.1).timeout
	end_wheel()
	return

func _on_let_it_ride_pressed() -> void:
	PrizeMenu.visible = false
	spinning = true
	await spin_wheel()
	spinning = true
	print("Wheel landed on:", number)
	if even_odd == 1 and number % 2 != 0:
		win = true
		prize_mult = 2.0
		print("WINNER!")
	elif even_odd == 2 and number % 2 == 0:
		win = true
		prize_mult = 2.0
		print("WINNER!")
	else:
		prize_mult = 0.0
		win = false
		print("LOSER!")
	await get_tree().create_timer(2.0).timeout
	prize_menu()


func _on_cashout_pressed() -> void:
	Display.visible = false
	PrizeMenu.visible = false
	prize_menu()


func _on_odd_pressed() -> void:
	even_odd = 1
	BettingOnDisplay.clear()
	BettingOnDisplay.append_text("ODD")
	return


func _on_even_pressed() -> void:
	even_odd = 2
	BettingOnDisplay.clear()
	BettingOnDisplay.append_text("EVEN")
	return


func _on_spin_pressed() -> void:
	if even_odd != 0 and !spinning:
		spinning = true
		await spin_wheel()
		spinning = true
		print("Wheel landed on:", number)
		if even_odd == 1 and number % 2 != 0:
			win = true
			prize_mult = 1.0
			print("WINNER!")
		elif even_odd == 2 and number % 2 == 0:
			win = true
			prize_mult = 1.0
			print("WINNER!")
		else:
			win = false
			prize_mult = 0.5
			print("LOSER!")
		SpinMenu.visible = false
		PrizeMenu.visible = true
		await get_tree().create_timer(2.0).timeout
	return


func _on_bet_100_pressed():
	money_bet += 1000.0
	CashBetDisplay.clear()
	CashBetDisplay.append_text(str(money_bet))


func _on_bet_1000_pressed():
	money_bet += 100.0
	CashBetDisplay.clear()
	CashBetDisplay.append_text(str(money_bet))


func _on_prize_1_pressed():
	give_prize(0)


func _on_prize_2_pressed():
	give_prize(1)


func _on_prize_3_pressed():
	give_prize(2)
