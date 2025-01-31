﻿using Paint.Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace Paint.Figures;

internal class StraightLineWrapper : Movable, IDrawable, IStartEndPoints, ITolerance {
    public FiguresEnum FigureType { get; set; } = FiguresEnum.StraightLine;

    public int PenSize { get; set; }
    public Color PenColor { get; set; }
    public Color BrushColor { get; set; }
    public bool IsFilling { get; set; }

    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }

    public Dictionary<ResizePointsEnum, Point> ResizePointsDict { get; set; } = [];

    public int Tolerance { get; set; } = 10;

    public override void Move(int dx, int dy) {
        base.Move(dx, dy);

        this.StartPoint = new Point(this.StartPoint.X + dx, this.StartPoint.Y + dy);
        this.EndPoint = new Point(this.EndPoint.X + dx, this.EndPoint.Y + dy);
    }

    public void CalculateStartEndPoints() {
        if (this.StartPoint.X < this.EndPoint.X && this.StartPoint.Y < this.EndPoint.Y) {
            this.StartPoint = new Point(this.TopPoint.X, this.TopPoint.Y);
            this.EndPoint = new Point(this.BotPoint.X, this.BotPoint.Y);
            return;
        }

        if (this.StartPoint.X < this.EndPoint.X && this.StartPoint.Y > this.EndPoint.Y) {
            this.StartPoint = new Point(this.TopPoint.X, this.BotPoint.Y);
            this.EndPoint = new Point(this.BotPoint.X, this.TopPoint.Y);
            return;
        }
    }

    public static EllipseWrapper GetCircleFromCenter(Point point, int radius) {
        var wrapper = new EllipseWrapper() {
            PenSize = 2,
            IsFilling = true,
            PenColor = Color.Black,
            BrushColor = Color.Black,
            FigureType = FiguresEnum.Ellipse,
            TopPoint = new Point(point.X - radius, point.Y - radius),
            BotPoint = new Point(point.X + radius, point.Y + radius),
        };

        return wrapper;
    }

    public Dictionary<ResizePointsEnum, EllipseWrapper> GetResizeCircles() {
        var circles = new Dictionary<ResizePointsEnum, EllipseWrapper>();

        foreach ((ResizePointsEnum key, Point value) in this.ResizePointsDict) {
            circles[key] = GetCircleFromCenter(value, 5);
        }

        return circles;
    }

    public void UpdateResizePointsDict() {
        Point topLeftPoint = this.TopPoint;
        Point botRightPoint = this.BotPoint;
        var topRightPoint = new Point(botRightPoint.X, topLeftPoint.Y);
        var botLeftPoint = new Point(topLeftPoint.X, botRightPoint.Y);

        int middleX = topLeftPoint.X + ((topRightPoint.X - topLeftPoint.X) / 2);
        int middleY = topLeftPoint.Y + ((botLeftPoint.Y - topLeftPoint.Y) / 2);

        var middleLeftPoint = new Point(topLeftPoint.X, middleY);
        var middleTopPoint = new Point(middleX, topLeftPoint.Y);
        var middleRightPoint = new Point(botRightPoint.X, middleY);
        var middleBotPoint = new Point(middleX, botRightPoint.Y);

        this.ResizePointsDict[ResizePointsEnum.TopLeft] = topLeftPoint;
        this.ResizePointsDict[ResizePointsEnum.BotRight] = botRightPoint;
        this.ResizePointsDict[ResizePointsEnum.TopRight] = topRightPoint;
        this.ResizePointsDict[ResizePointsEnum.BotLeft] = botLeftPoint;
        this.ResizePointsDict[ResizePointsEnum.MiddleLeft] = middleLeftPoint;
        this.ResizePointsDict[ResizePointsEnum.MiddleTop] = middleTopPoint;
        this.ResizePointsDict[ResizePointsEnum.MiddleRight] = middleRightPoint;
        this.ResizePointsDict[ResizePointsEnum.MiddleBot] = middleBotPoint;
    }

    public void Draw(Graphics graphics) {
        this.ValidateEdgePoint();

        var pen = new Pen(this.PenColor, this.PenSize);
        graphics.DrawLine(pen, this.StartPoint, this.EndPoint);
    }

    public void DrawDash(Graphics graphics) {
        var pen = new Pen(Color.Black, this.PenSize) {
            DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
        };

        this.ValidateEdgePoint();

        graphics.DrawLine(pen, this.StartPoint, this.EndPoint);
    }

    public void DrawSelection(Graphics graphics) {
        var bluePen = new Pen(Color.Blue, this.PenSize) {
            DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
        };

        var blackPen = new Pen(Color.Black, this.PenSize) {
            DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
        };

        this.ValidateEdgePoint();

        var rectangle = Rectangle.FromLTRB(
            this.TopPoint.X, this.TopPoint.Y, this.BotPoint.X, this.BotPoint.Y
        );

        graphics.DrawRectangle(blackPen, rectangle);
        graphics.DrawLine(bluePen, this.StartPoint, this.EndPoint);
    }

    public bool ContainsPoint(Point point) {
        //int coefficientA = this.EndPoint.Y - this.StartPoint.Y;
        //int coefficientB = -(this.EndPoint.X - this.StartPoint.X);
        //int coefficientC = (this.EndPoint.X * this.StartPoint.Y) - (this.EndPoint.Y * this.StartPoint.X);

        //int numerator = Math.Abs((coefficientA * point.X) + (coefficientB * point.Y) + coefficientC);

        //double denominator = Math.Sqrt(Math.Pow(this.EndPoint.Y - this.StartPoint.Y, 2) + Math.Pow(this.EndPoint.X - this.StartPoint.X, 2));

        //double distance = numerator / denominator;

        //return distance <= this.Tolerance;

        this.ValidateEdgePoint();

        var rectangle = Rectangle.FromLTRB(
            this.TopPoint.X, this.TopPoint.Y, this.BotPoint.X, this.BotPoint.Y
        );

        return rectangle.Contains(point);
    }

    public void DrawResizing(Graphics graphics) {
        this.ValidateEdgePoint();

        Dictionary<ResizePointsEnum, EllipseWrapper> resizePoints = this.GetResizeCircles();
        foreach ((ResizePointsEnum _, EllipseWrapper value) in resizePoints) {
            value.Draw(graphics);
        }
    }
}
