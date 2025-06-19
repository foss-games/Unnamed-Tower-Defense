using System.Text.Json;
using FOSSGames;
using Godot;
using Godot.Collections;

public partial class LevelMaker : Node2D
{
    private TileMapLayer map;
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
                    if (type > 3) type = 0;
                }

                map.SetCell(cell, 0, new Vector2I(type, 0));
                map.GetCellTileData(cell).SetCustomData("cellType", type);

            }
        }
    }

    public void OnButtonPressed()
    {
        string output = "obstacles=[";
        foreach (Vector2I cell in map.GetUsedCells())
        {
            //Skip edge walls
            if (cell.X <= -1 || cell.X >= 24) continue;
            if (cell.Y <= -1 || cell.Y >= 28) continue;

            TileData t = map.GetCellTileData(cell);

            if ((bool)t.GetCustomData("solid"))
            {
                output += $"{{{cell.X}, {cell.Y}}},";
            }
            if ((bool)t.GetCustomData("startpos"))
            {
                output = $"StartLocation={{{cell.X}, {cell.Y}}}," + output;
                //output += $"{{{cell.X}, {cell.Y}}},";
            }
            if ((bool)t.GetCustomData("endpos"))
            {
                output = $"EndLocation={{{cell.X}, {cell.Y}}}," + output;
            }
        }

        DisplayServer.ClipboardSet(output[..^1] + "]");
    }
}
