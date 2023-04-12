extends Need
class_name VapeNeed

const VapeDirective = preload("res://scripts/VapeDirective.gd")

func _init():
	value = 0.0
	increment = 7.0
	minimum = 30.0

func get_directive():
	return VapeDirective.new(self)
