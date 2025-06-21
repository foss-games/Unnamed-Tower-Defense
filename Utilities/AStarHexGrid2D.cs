using System.Diagnostics;
using Godot;
using Godot.Collections;

/**
    Ported from https://github.com/OmarQurashi868/a-star-hex-grid-2d/blob/main/a_star_hex_grid_2d.gd
**/

public partial class AStarHexGrid2D : AStar2D
{
    public Array<Vector2I> PointsArray = [];
    private TileMapLayer map;
    private string solidDataName = "solid";

    public void SetupHexGrid(TileMapLayer passedMap)
    {
        map = passedMap;
        Array<Vector2I> usedCells = passedMap.GetUsedCells();

        foreach (Vector2I cell in usedCells)
        {
            TileData tileData = map.GetCellTileData(cell);
            bool isTileSolid = (bool)tileData.GetCustomData(solidDataName);

            if (!isTileSolid)
            {
                AddHexPoint(cell);
                ConnectHexPont(cell);
            }
        }
    }

    public int AddHexPoint(Vector2I point)
    {
        int currentID = PointsArray.Count;
        AddPoint(currentID, map.MapToLocal(point));
        PointsArray.Add(point);
        return currentID;
    }

    public void ConnectHexPont(Vector2I point)
    {
        int x = point.X;
        int y = point.Y;

        int center = PointsArray.IndexOf(point);

        int top = PointsArray.IndexOf(new Vector2I(x, y - 1));
        int right = PointsArray.IndexOf(new Vector2I(x + 1, y));
        int bottom = PointsArray.IndexOf(new Vector2I(x, y + 1));
        int left = PointsArray.IndexOf(new Vector2I(x - 1, y));

        int topRight = PointsArray.IndexOf(new Vector2I(x + 1, y - 1));
        int topLeft = PointsArray.IndexOf(new Vector2I(x - 1, y - 1));
        int bottomRight = PointsArray.IndexOf(new Vector2I(x + 1, y + 1));
        int bottomLeft = PointsArray.IndexOf(new Vector2I(x - 1, y + 1));

        if (HasPoint(top))
            ConnectPoints(center, top);
        if (HasPoint(right))
            ConnectPoints(center, right);
        if (HasPoint(bottom))
            ConnectPoints(center, bottom);
        if (HasPoint(left))
            ConnectPoints(center, left);

        if (map.TileSet.TileOffsetAxis == TileSet.TileOffsetAxisEnum.Horizontal)
        {
            if (y % 2 == 0)
            {
                if (HasPoint(topLeft))
                    ConnectPoints(center, topLeft);
                if (HasPoint(bottomLeft))
                    ConnectPoints(center, bottomRight);
            }
            else
            {
                if (HasPoint(topRight))
                    ConnectPoints(center, topRight);
                if (HasPoint(bottomRight))
                    ConnectPoints(center, bottomRight);
            }
        }
        else
        {
            if (x % 2 == 0)
            {
                if (HasPoint(topRight))
                    ConnectPoints(center, topRight);
                if (HasPoint(topLeft))
                    ConnectPoints(center, topLeft);
            }
            else
            {
                if (HasPoint(bottomRight))
                    ConnectPoints(center, bottomRight);
                if (HasPoint(bottomLeft))
                    ConnectPoints(center, bottomLeft);
            }
        }

    }

    public int CoordsToID(Vector2I coord)
    {
        return PointsArray.IndexOf(coord);
    }

    public Vector2[] GetPath(Vector2I fromPoint, Vector2I toPoint)
    {
        int from = CoordsToID(fromPoint);
        int to = CoordsToID(toPoint);

        return GetPointPath(from, to);
    }
}