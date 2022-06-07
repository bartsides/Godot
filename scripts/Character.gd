extends Node2D

class_name Character

var VapeNeed = load("res://scripts/VapeNeed.gd")
var VapeDirective = load("res://scripts/VapeDirective.gd")

###### Movement ######
enum States { IDLE, FOLLOW }
const MASS = 5.0
const ARRIVE_DISTANCE = 10.0
export(float) var speed = 200.0
var _state = States.IDLE
var _path = []
var _target_point_world = Vector2()
var _target_position = Vector2()
var _velocity = Vector2()

var needs = Array()
var directive = null

func _ready():
	needs.append(VapeNeed.new())
	directive = VapeDirective.new()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	increment_needs(delta)
	handle_directives()
	
	if _state != States.FOLLOW:
		return
	var _arrived_to_next_point = _move_to(_target_point_world)
	if _arrived_to_next_point:
		_path.remove(0)
		if len(_path) == 0:
			_change_state(States.IDLE)
			print("clear")
			get_parent().get_node("TileMap").clear_previous_path_drawing()
			return
		_target_point_world = _path[0]

func _unhandled_input(event):
	if event.is_action_pressed("click"):
		var global_mouse_pos = get_global_mouse_position()
		set_target_position(global_mouse_pos)
		
func _move_to(world_position):
	var desired_velocity = (world_position - position).normalized() * speed
	var steering = desired_velocity - _velocity
	_velocity += steering / MASS
	position += _velocity * get_process_delta_time()
	rotation = _velocity.angle()
	return position.distance_to(world_position) < ARRIVE_DISTANCE

func _change_state(new_state):
	if new_state == States.FOLLOW:
		_path = get_parent().get_node("TileMap").get_astar_path(position, _target_position)
		if not _path or len(_path) == 1:
			_change_state(States.IDLE)
			return
		# The index 0 is the starting cell.
		# We don't want the character to move back to it in this example.
		_target_point_world = _path[1]
	_state = new_state

func increment_needs(delta):
	for need in needs:
		need.handle(self, delta)

func get_need(type):
	for need in needs:
		if typeof(need) == type:
			return need;

func handle_directives():
	if directive != null:
		directive.handle(self)
	else:
		set_next_directive()

func set_directive(next_directive):
	directive = next_directive

func set_next_directive():
	# directive ranking logic here
	pass

func set_target_position(position : Vector2):
	_target_position = position
	_change_state(States.FOLLOW)

func get_state():
	return _state
	
