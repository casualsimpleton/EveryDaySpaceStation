//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// SquareBounds - A simple square bounds with lower corner and width and height
// CenteredSquareBounds - A simple square bounds with center point and width and height
// Created: December 4 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 8 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using EveryDaySpaceStation.DataTypes;

namespace EveryDaySpaceStation.DataTypes
{
    /// <summary>
    /// Bounds used for squares, mostly tiles and stuff
    /// Bottom left is "anchor" and width,height is top right corner
    /// </summary>
    public struct SquareBounds
    {
        Vec2Int _anchor;
        Vec2Int _widthHeight;

        public Vec2Int AnchorPoint
        {
            get { return _anchor; }
        }

        public Vec2Int WidthHeight
        {
            get { return _widthHeight; }
        }

        public Vec2Int MinPoint
        {
            get { return _anchor * _widthHeight; }
        }

        public Vec2Int MaxPoint
        {
            get { return MinPoint + _widthHeight; }
        }

        public SquareBounds(int x, int y, int width, int height)
        {
            _anchor = new Vec2Int(x, y);
            _widthHeight = new Vec2Int(width, height);
        }

        public SquareBounds(Vec2Int anchorXY, Vec2Int widthHeight)
        {
            _anchor = anchorXY;
            _widthHeight = widthHeight;
        }

        public bool DoesContainPoint(Vector2 point)
        {
            if (point.x < _anchor.x)
                return false;

            if (point.x > _anchor.x + _widthHeight.x)
                return false;

            if (point.y < _anchor.y)
                return false;

            if (point.y > _anchor.y + _widthHeight.y)
                return false;

            return true;
        }

        public bool DoesContainPoint(Vec2Int point)
        {
            if (point.x < _anchor.x)
                return false;

            if (point.x > _anchor.x + _widthHeight.x)
                return false;

            if (point.y < _anchor.y)
                return false;

            if (point.y > _anchor.y + _widthHeight.y)
                return false;

            return true;
        }
    }

    //TODO - This isn't right. Haven't tested it, but I'm too tired ATM to figure out the dumb math
    ///// <summary>
    ///// Bounds used for squares, mostly tiles and stuff
    ///// Bottom left is "anchor" and width,height is top right corner
    ///// </summary>
    //public struct CenteredSquareBounds
    //{
    //    Vec2Int _centerPoint;
    //    Vec2Int _widthHeight;

    //    public Vec2Int AnchorPoint
    //    {
    //        get { return _centerPoint; }
    //    }

    //    public Vec2Int WidthHeight
    //    {
    //        get { return _widthHeight; }
    //    }

    //    public Vec2Int MinPoint
    //    {
    //        get { return _centerPoint * _widthHeight; }
    //    }

    //    public Vec2Int MaxPoint
    //    {
    //        get { return MinPoint + _widthHeight; }
    //    }

    //    public CenteredSquareBounds(int x, int y, int width, int height)
    //    {
    //        _centerPoint = new Vec2Int(x, y);
    //        _widthHeight = new Vec2Int(width, height);
    //    }

    //    public CenteredSquareBounds(Vec2Int centerPoint, Vec2Int widthHeight)
    //    {
    //        _centerPoint = centerPoint;
    //        _widthHeight = widthHeight;
    //    }

    //    public bool DoesContainPoint(Vector2 point)
    //    {
    //        if (point.x < _centerPoint.x)
    //            return false;

    //        if (point.x > _centerPoint.x + _widthHeight.x)
    //            return false;

    //        if (point.y < _centerPoint.y)
    //            return false;

    //        if (point.y > _centerPoint.y + _widthHeight.y)
    //            return false;

    //        return true;
    //    }

    //    public bool DoesContainPoint(Vec2Int point)
    //    {
    //        if (point.x < _centerPoint.x)
    //            return false;

    //        if (point.x > _centerPoint.x + _widthHeight.x)
    //            return false;

    //        if (point.y < _centerPoint.y)
    //            return false;

    //        if (point.y > _centerPoint.y + _widthHeight.y)
    //            return false;

    //        return true;
    //    }
    //}
}