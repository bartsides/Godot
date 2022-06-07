extends Directive

class_name VapeDirective

func _init():
	name = "Vape";

func handle(character : Character):
	if (character.get_state() == character.States.FOLLOW):
		return
	var tilemap : TileMap = character.get_parent().get_node("TileMap")
	var cells : Array = tilemap.get_used_cells_by_id(1)
	if cells.size() < 1:
		return
	# TODO: find closest cell based on path length
	var cell = cells.front()
	
	var pointPath = tilemap.get_point_path(Vector2(0,0), cell)
	if pointPath && pointPath.size() > 1:
		var dest = pointPath[-2]
		character.set_target_position(tilemap.get_cell_pos(Vector2(dest.x, dest.y)))
