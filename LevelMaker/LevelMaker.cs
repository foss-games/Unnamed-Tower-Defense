using System.Text.Json;
using FOSSGames;
using Godot;
using Godot.Collections;

public partial class LevelMaker : Node2D
{
    private TileMapLayer map;
    private bool startSet = false;
    private bool endSet = false;
    public override void _Ready()
    {
        map = GetNode<Node2D>("Background").GetNode<TileMapLayer>("TileMapLayer");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton e)
        {
            if (@event.IsActionPressed("Click"))
            {
                Vector2I cell = map.LocalToMap(GetGlobalMousePosition());
                TileData data = map.GetCellTileData(cell);

                int type = 0;

                if (!data.HasCustomData("cellType"))
                {
                    data.SetCustomData("cellType", 0);
                }
                else
                {
                    type = (int)data.GetCustomData("cellType");
                    type++;
                    switch (type)
                    {
                        case 0:
                        case 1:
                            break;
                        case 2: //start
                            if (startSet && endSet)
                            {
                                type = 0;
                                break;
                            }
                            if (startSet)
                            {
                                type++;
                                endSet = true;
                                break;
                            }
                            startSet = true;
                            break;
                        case 3: //end
                            if (endSet)
                            {
                                type = 0;
                            }
                            endSet = true;
                            startSet = false;
                            break;
                        case 4:
                            endSet = false;
                            type = 0;
                            break;
                        default:
                            type = 0;
                            break;
                    }
                }

                map.SetCell(cell, 0, new Vector2I(type, 0));
                map.GetCellTileData(cell).SetCustomData("cellType", type);

            }
        }
    }

    public void OnButtonPressed()
    {
        string output = "\"Obstacles\":[";
        foreach (Vector2I cell in map.GetUsedCells())
        {
            //Skip edge walls
            if (cell.X <= -1 || cell.X >= 24) continue;
            if (cell.Y <= -1 || cell.Y >= 28) continue;

            TileData t = map.GetCellTileData(cell);

            if ((bool)t.GetCustomData("solid"))
            {
                output += $"{{\"X\": {cell.X}, \"Y\": {cell.Y}}},";
            }
            if ((bool)t.GetCustomData("startpos"))
            {
                output = $"\"StartLocation\":{{\"X\": {cell.X}, \"Y\": {cell.Y}}}," + output;
                //output += $"{{{cell.X}, {cell.Y}}},";
            }
            if ((bool)t.GetCustomData("endpos"))
            {
                output = $"\"EndLocation\":{{\"X\": {cell.X}, \"Y\": {cell.Y}}}," + output;
            }
        }

        DisplayServer.ClipboardSet(output[..^1] + "]");
    }
}
