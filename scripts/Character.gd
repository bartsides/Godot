extends Node2D
class_name Character

var VapeNeed = load("res://scripts/VapeNeed.gd")
var VapeDirective = load("res://scripts/VapeDirective.gd")

###### Movement ######
enum States { IDLE, FOLLOW, INTERACTING }
const MASS = 5.0
const ARRIVE_DISTANCE = 10.0
export(float) var speed = 200.0
var _path = []
var _target_point_world = Vector2()
var _target_position = Vector2()
var _velocity = Vector2()

var _state = States.IDLE
func get_state(): return _state

var needs = [ VapeNeed.new() ]
var moving_to_directive = false

var directive = null

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	increment_needs(delta)
	handle_directives(delta)
	
	if _state != States.FOLLOW:
		return
	var _arrived_to_next_point = _move_to(_target_point_world)
	if _arrived_to_next_point:
		_path.remove(0)
		if len(_path) == 0:
			if moving_to_directive:
				_change_state(States.INTERACTING)
			else:
				_change_state(States.IDLE)
				moving_to_directive = false
			get_parent().get_node("TileMap").clear_previous_path_drawing()
			return
		_target_point_world = _path[0]

func _unhandled_input(event):
	if event.is_action_pressed("click"):
		clear_directive()
		# TODO: Find nearest valid neighbor if click is an obstacle
		set_target_position(get_global_mouse_position())
		
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

func handle_directives(delta):
	if directive != null:
		directive.handle(self, delta)
	else:
		set_next_directive()

func set_next_directive():
	# rank needs
	var next_need
	for need in needs:
		if need.value > need.minimum && (not next_need || need.value > next_need.value):
			next_need = need
	if next_need:
		directive = next_need.get_directive()

func clear_directive():
	directive = null
	moving_to_directive = false
	
func set_target_position(position : Vector2):
	_target_position = position
	_change_state(States.FOLLOW)

func go_to_closest(id):
	var tilemap : TileMap = get_parent().get_node("TileMap")
	var cells : Array = tilemap.get_used_cells_by_id(id)
	if cells.size() < 1:
		return
	
	var char_pos = tilemap.world_to_map(tilemap.to_local(global_position))
	var closest_cell
	var shortest_path = []
	
	for cell in cells:
		var point_path = tilemap.get_point_path(char_pos, cell)
		if point_path && (not shortest_path || shortest_path.size() > point_path.size()):
			closest_cell = cell
			shortest_path = point_path
	
	if shortest_path.size() > 0:
		var dest = shortest_path[-1]
		if Vector2(dest.x, dest.y) == closest_cell:
			dest = shortest_path[-2]
		else:
			dest = shortest_path[-1]
		
		set_target_position(tilemap.get_cell_pos(Vector2(dest.x, dest.y)))
		moving_to_directive = true
