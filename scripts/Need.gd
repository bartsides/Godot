extends Node

class_name Need

var Character = load("res://scripts/Character.gd")

export(float) var value = 0.0
export(float) var increment = 1.0

func handle(_character, delta):
	value = min(value + increment * delta, 100)
