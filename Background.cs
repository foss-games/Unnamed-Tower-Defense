using Godot;
using System;
using System.Formats.Tar;

public partial class Background : Node2D
{
    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (@event is InputEventMouse mouseEvent)
        {
            if (@event.IsActionPressed("Click"))
            {
                Vector2 mp = mouseEvent.Position;
                Vector2 gp = mouseEvent.GlobalPosition;

                GD.Print(mp.X, ",", mp.Y);

                TileMapLayer map = GetNode<TileMapLayer>("TileMapLayer");
                Vector2I v = map.LocalToMap((Vector2I)gp);

                TileData tile = map.GetCellTileData(v);

                map.SetCell(v, 0, new Vector2I(1, 0));

                // int cID = map.GetCellSourceId(v);

                // GD.Print(v.X, v.Y);
            }
        }
    }
}
