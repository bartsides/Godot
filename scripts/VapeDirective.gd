extends Directive
class_name VapeDirective

var cloud_shown = false

func _init(_need):
	need = _need
	name = "Vape"

func handle(character : Character, delta):
	var States = character.States
	match character.get_state():
		States.FOLLOW:
			return
		States.IDLE:
			character.go_to_closest(1)
		States.INTERACTING:
			if not cloud_shown:
				# TODO: display cloud
				cloud_shown = true
				pass
				
			need.value = max(need.value - 10 * delta, 0)
			if need.value < 1:
				character.clear_directive()
			
