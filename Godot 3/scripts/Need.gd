extends Node
class_name Need

var Character = load("res://scripts/Character.gd")

export(float) var value = 0.0
export(float) var increment = 1.0
export(float) var minimum = 30.0

func handle(character : Character, delta):
	if (character.get_state() == character.States.INTERACTING):
		return
	value = min(value + increment * delta, 100)

func get_directive():
	pass
