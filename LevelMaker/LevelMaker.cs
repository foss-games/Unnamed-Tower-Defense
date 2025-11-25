using System;
using System.Linq;
using System.Security.Principal;
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
        debug = GetNode<Node2D>("Background").GetNode<Label>("DEBUG");
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
                            break;
                        case 1:
                            type++;
                            break;
                        case 2: //wall
                            break;
                        case 3: //start
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
                        case 4: //end
                            if (endSet)
                            {
                                type = 0;
                            }
                            endSet = true;
                            startSet = false;
                            break;
                        case 5:
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
        Level l = new Level();
        l.GUID = Guid.NewGuid();
        l.Obstacles = [];
        l.Waves = [
            new Wave(){
                Interval = 10,
                Enemies = []
            }
        ];
        l.AvailableTowers = ["da15da79-28cc-4b60-a768-91d7bb3fb475"];
        l.MaxHP = 1;

        foreach (Vector2I cell in map.GetUsedCells())
        {
            //Skip edge walls
            if (cell.X <= 0 || cell.X >= 15) continue;
            if (cell.Y <= 2 || cell.Y >= 22) continue;

            TileData t = map.GetCellTileData(cell);
            if ((bool)t.GetCustomData("startpos"))
            {
                l.StartLocation = cell;
                continue;
            }
            if ((bool)t.GetCustomData("endpos"))
            {
                l.EndLocation = cell;
                continue;
            }
            if ((bool)t.GetCustomData("solid"))
            {
                l.Obstacles.Add(cell);
            }

        }
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.Converters.Add(new Vector2Converter());
        options.Converters.Add(new Vector2IConverter());
        options.Converters.Add(new EnemyConverter());
        options.TypeInfoResolver = SourceGenerationContext.Default;
        DisplayServer.ClipboardSet(JsonSerializer.Serialize(l, options));
    }

    public void OnResetButtonPressed()
    {
        foreach (Vector2I cell in map.GetUsedCells())
        {
            if (!(cell.X >= 0 &&
                cell.Y >= 2 &&
                cell.X <= 15 &&
                cell.Y <= 22))
            {
                continue;
            }
            map.SetCell(cell, 0, new Vector2I(0, 0));
        }
        startSet = false;
        endSet = false;
    }
    private Label debug;
    public override void _PhysicsProcess(double delta)
    {
        Vector2 pos = GetGlobalMousePosition();
        debug.Text = $"{pos}\n{map.LocalToMap(pos)}";
        debug.GlobalPosition = pos;
    }
}
