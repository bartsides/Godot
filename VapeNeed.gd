extends Node

class_name VapeNeed

export(float) var value = 0.0
export(float) var increment = 1.0

func handle(character, delta):
	value += increment * delta;
	if value > 100:
		value = 100;
