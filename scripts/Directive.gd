extends Node
class_name Directive

var Character = load("res://scripts/Character.gd")

var need

func init(_need):
	name = "Directive";
	need = _need

func handle(_character : Character, _delta):
	print("Handle not implemented")
