﻿using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using DynaShape.Goals;


namespace DynaShape.ZeroTouch
{
    public static class Goals
    {
        //==================================================================
        // Change the weight value of a goal (of any type)
        //==================================================================

        /// <summary>
        /// Change the weight value of a goal (of any kind)
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static Goal Goal_ChangeWeight(Goal goal, double weight)
        {
            goal.Weight = (float) weight;
            return goal;
        }


        //==================================================================
        // Anchor
        //==================================================================

        /// <summary>
        /// Keep a node an the specified anchor point.
        /// By default the weight for this goal set very high to ensure the node really "sticks" to the anchor
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="anchor"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static AnchorGoal AnchorGoal_Create(
            Point startPosition,
            [DefaultArgument("null")] Point anchor,
            [DefaultArgument("1000.0")] double weight)
        {
            return new AnchorGoal(
                startPosition.ToTriple(),
                anchor?.ToTriple() ?? startPosition.ToTriple(),
                (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="anchorGoal"></param>
        /// <param name="anchor"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static AnchorGoal AnchorGoal_Change(
            AnchorGoal anchorGoal,
            [DefaultArgument("null")] Point anchor,
            [DefaultArgument("-1.0")] double weight)
        {
            if (anchor != null) anchorGoal.Anchor = anchor.ToTriple();
            if (weight >= 0.0) anchorGoal.Weight = (float) weight;
            return anchorGoal;
        }


        //==================================================================
        // CoCircular
        //==================================================================

        /// <summary>
        /// Force a set of nodes to lie on a common circular arc
        /// </summary>
        /// <param name="startPositions"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static CoCircularGoal CoCircularGoal_Create(
            List<Point> startPositions,
            [DefaultArgument("1.0")] double weight)
        {
            return new CoCircularGoal(startPositions.ToTriples(), (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="cyclicPolygonGoal"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static CoCircularGoal CoCircularGoal_Change(
            CoCircularGoal cyclicPolygonGoal,
            [DefaultArgument("-1.0")] double weight)
        {
            if (weight >= 0.0) cyclicPolygonGoal.Weight = (float) weight;
            return cyclicPolygonGoal;
        }


        //==================================================================
        // CoLinear
        //==================================================================

        /// <summary>
        /// Force a set of nodes to lie on a common line.
        /// The line position and orientation are computed based on the current node positions.
        /// This is different from the OnLine goal, where the target line is fixed and defined in advance.
        /// </summary>
        /// <param name="startPositions"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static CoLinearGoal CoLinearGoal_Create(
            List<Point> startPositions,
            [DefaultArgument("1000.0")] double weight)
        {
            return new CoLinearGoal(startPositions.ToTriples(), (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="coLinearGoal"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static CoLinearGoal CoLinearGoal_Change(
            CoLinearGoal coLinearGoal,
            [DefaultArgument("-1.0")] double weight)
        {
            if (weight >= 0.0) coLinearGoal.Weight = (float) weight;
            return coLinearGoal;
        }


        //==================================================================
        // Constant
        //==================================================================

        /// <summary>
        /// Apply a constant directional offset to the specified nodes. 
        /// For example, this is useful to simulate gravity
        /// </summary>
        /// <param name="startPositions"></param>
        /// <param name="constant"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static ConstantGoal ConstantGoal_Create(
            List<Point> startPositions,
            [DefaultArgument("Vector.ByCoordinates(0, 0, -0.1)")] Vector constant,
            [DefaultArgument("1.0")] double weight)
        {
            return new ConstantGoal(startPositions.ToTriples(), constant.ToTriple(), (float) weight);
        }

        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="constantGoal"></param>
        /// <param name="constant"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static ConstantGoal ConstantGoal_Change(
            ConstantGoal constantGoal,
            [DefaultArgument("null")] Vector constant,
            [DefaultArgument("-1.0")] double weight)
        {
            if (constant != null) constantGoal.Move = constant.ToTriple();
            if (weight >= 0.0) constantGoal.Weight = (float) weight;
            return constantGoal;
        }


        //==================================================================
        // CoPlanar
        //==================================================================

        /// <summary>
        /// Force a set of nodes to lie on a common plane.
        /// The plane position and orientation are computed based on the current node positions.
        /// This is different from the OnPlane goal, where the target plane is fixed and defined in advance.
        /// </summary>
        /// <param name="startPositions"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static CoPlanarGoal CoPlanarGoal_Create(
            List<Point> startPositions,
            [DefaultArgument("1.0")] double weight)
        {
            return new CoPlanarGoal(startPositions.ToTriples(), (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="coPlanarGoal"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static CoPlanarGoal CoPlanarGoal_Change(
            CoPlanarGoal coPlanarGoal,
            [DefaultArgument("-1.0")] double weight)
        {
            if (weight >= 0.0) coPlanarGoal.Weight = (float) weight;
            return coPlanarGoal;
        }


        //==================================================================
        // CoSpherical
        //==================================================================

        /// <summary>
        /// Force a set of nodes to lie on a common spherical surface.
        /// The sphere position and radius are computed based the current node positions
        /// </summary>
        /// <param name="startPositions"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static CoSphericalGoal CoSphericalGoal_Create(
            List<Point> startPositions,
            [DefaultArgument("1.0")] double weight)
        {
            return new CoSphericalGoal(startPositions.ToTriples(), (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="coSphericalGoal"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static CoSphericalGoal CoSphericalGoal_Change(
            CoSphericalGoal coSphericalGoal,
            [DefaultArgument("-1.0")] double weight)
        {
            if (weight >= 0.0) coSphericalGoal.Weight = (float) weight;
            return coSphericalGoal;
        }


        //==================================================================
        // Direction
        //==================================================================

        /// <summary>
        /// Force the line connecting two nodes to be parallel to the specified direction.
        /// </summary>
        /// <param name="startPosition1"></param>
        /// <param name="startPosition2"></param>
        /// <param name="targetDirection"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static DirectionGoal DirectionGoal_Create(
            Point startPosition1,
            Point startPosition2,
            [DefaultArgument("null")] Vector targetDirection,
            [DefaultArgument("1.0")] double weight)
        {
            return new DirectionGoal(
                startPosition1.ToTriple(),
                startPosition2.ToTriple(),
                targetDirection?.ToTriple() ?? (startPosition2.ToTriple() - startPosition1.ToTriple()).Normalise(),
                (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="directionGoal"></param>
        /// <param name="targetDirection"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static DirectionGoal DirectionGoal_Change(
            DirectionGoal directionGoal,
            [DefaultArgument("null")] Vector targetDirection,
            [DefaultArgument("-1.0")] double weight)
        {
            if (targetDirection != null) directionGoal.TargetDirection = targetDirection.ToTriple();
            if (weight >= 0.0) directionGoal.Weight = (float) weight;
            return directionGoal;
        }


        //==================================================================
        // EqualLength
        //==================================================================

        /// <summary>
        /// Force a set of lines (as defined by pairs of nodes) to maintain equal lengths.
        /// </summary>
        /// <param name="startPositions">The nodes at the linie start points and end points, in the following order: start1, end1, start2, end2, etc...</param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static EqualLengthsGoal EqualLengthsGoal_Create(
            List<Point> startPositions,
            [DefaultArgument("1.0")] double weight)
        {
            return new EqualLengthsGoal(startPositions.ToTriples(), (float) weight);
        }


        /// <summary>
        /// Force a set of lines (as defined by pairs of nodes) to maintain equal lengths.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static EqualLengthsGoal EqualLengthsGoal_Create(
            List<Line> lines,
            [DefaultArgument("1.0")] double weight)
        {
            List<Triple> startPositions = new List<Triple>();
            foreach (Line line in lines)
            {
                startPositions.Add(line.StartPoint.ToTriple());
                startPositions.Add(line.EndPoint.ToTriple());
            }
            return new EqualLengthsGoal(startPositions, (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="equalLengthsGoal"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static EqualLengthsGoal EqualLengthsGoal_Change(
            EqualLengthsGoal equalLengthsGoal,
            [DefaultArgument("-1.0")] double weight)
        {
            if (weight >= 1.0) equalLengthsGoal.Weight = (float) weight;
            return equalLengthsGoal;
        }


        //==================================================================
        // Floor
        //==================================================================

        /// <summary>
        /// Force a set of nodes to stay above a horizontal floor plane.
        /// </summary>
        /// <param name="startPositions"></param>
        /// <param name="floorHeight"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static FloorGoal FloorGoal_Create(
            List<Point> startPositions,
            [DefaultArgument("0.0")] double floorHeight,
            [DefaultArgument("1000.0")] double weight)
        {
            return new FloorGoal(startPositions.ToTriples(), (float) floorHeight, (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="floorGoal"></param>
        /// <param name="floorHeight"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static FloorGoal FloorGoal_Change(
            FloorGoal floorGoal,
            [DefaultArgument("0.0")] double floorHeight,
            [DefaultArgument("-1.0")] double weight)
        {
            floorGoal.FloorHeight = (float)floorHeight;
            if (weight > 0.0) floorGoal.Weight = (float)weight;
            return floorGoal;
        }


        //==================================================================
        // Length
        //==================================================================

        /// <summary>
        /// Force a pair of nodes to maintain the specified distance/length
        /// </summary>
        /// <param name="startPosition1"></param>
        /// <param name="startPosition2"></param>
        /// <param name="targetLength">If unspecified, the target length will be the distance between the starting node positions</param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static LengthGoal LengthGoal_Create(
            Point startPosition1,
            Point startPosition2,
            [DefaultArgument("-1.0")] double targetLength,
            [DefaultArgument("1.0")] double weight)
        {
            return new LengthGoal(
                startPosition1.ToTriple(),
                startPosition2.ToTriple(),
                targetLength >= 0.0
                    ? (float) targetLength
                    : (startPosition2.ToTriple() - startPosition1.ToTriple()).Length,
                (float) weight);
        }


        /// <summary>
        /// Maintain the specified distance between two nodes located at the start and end of the given line
        /// </summary>
        /// <param name="line">This line will be used to create two nodes located at is start and end point</param>
        /// <param name="targetLength">If unspecified, the target length will be the distance between the starting node positions</param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static LengthGoal LengthGoal_Create(
            Line line,
            [DefaultArgument("-1.0")] double targetLength,
            [DefaultArgument("1.0")] double weight)
        {
            return new LengthGoal(
                line.StartPoint.ToTriple(),
                line.EndPoint.ToTriple(),
                targetLength >= 0.0 ? (float) targetLength : (float) line.Length,
                (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="lengthGoal"></param>
        /// <param name="targetLength"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static LengthGoal LengthGoal_Change(
            LengthGoal lengthGoal,
            [DefaultArgument("-1.0")] double targetLength,
            [DefaultArgument("-1.0")] double weight)
        {
            if (targetLength >= 0.0) lengthGoal.TargetLength = (float) targetLength;
            if (weight >= 0.0) lengthGoal.Weight = (float) weight;
            return lengthGoal;
        }


        //==================================================================
        // Merge
        //==================================================================

        /// <summary>
        /// Pull a set of nodes to their center of mass. 
        /// This is useful to force the nodes to have coincident positions.
        /// By default the weight value is set very high to ensure that the nodes really merge together at one position.
        /// </summary>
        /// <param name="startPositions"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static MergeGoal MergeGoal_Create(
            List<Point> startPositions,
            [DefaultArgument("1000.0")] double weight)
        {
            return new MergeGoal(startPositions.ToTriples(), (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="mergeGoal"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static MergeGoal MergeGoal_Change(
            MergeGoal mergeGoal,
            [DefaultArgument("-1.0")] double weight)
        {
            if (weight >= 0.0) mergeGoal.Weight = (float) weight;
            return mergeGoal;
        }


        //==================================================================
        // OnCurve
        //==================================================================

        /// <summary>
        /// Force a set of nodes to lie on the specified curve.
        /// </summary>
        /// <param name="startPositions"></param>
        /// <param name="targetCurve"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static OnCurveGoal OnCurveGoal_Create(
            List<Point> startPositions,
            Curve targetCurve,
            [DefaultArgument("1.0")] double weight)
        {
            return new OnCurveGoal(startPositions.ToTriples(), targetCurve, (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="onCurveGoal"></param>
        /// <param name="targetCurve"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static OnCurveGoal OnCurveGoal_Change(
            OnCurveGoal onCurveGoal,
            [DefaultArgument("null")] Curve targetCurve,
            double weight)
        {
            if (targetCurve != null) onCurveGoal.TargetCurve = targetCurve;
            if (weight >= 0.0) onCurveGoal.Weight = (float) weight;
            return onCurveGoal;
        }


        //==================================================================
        // OnLine
        //==================================================================

        /// <summary>
        /// Force a set of nodes to lie on the specified line.
        /// This is different from othe CoLinear goal, where the target line is computed based on the current node positions rather than being defined and fixed in advance.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="targetLineOrigin"></param>
        /// <param name="targetLineDirection"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static OnLineGoal OnLineGoal_Create(
            List<Point> startPosition,
            [DefaultArgument("Point.Origin()")] Point targetLineOrigin,
            [DefaultArgument("Vector.XAxis()")] Vector targetLineDirection,
            [DefaultArgument("1.0")] double weight)
        {
            return new OnLineGoal(
                startPosition.ToTriples(),
                targetLineOrigin.ToTriple(),
                targetLineDirection.ToTriple().Normalise(),
                (float) weight);
        }


        /// <summary>
        /// Force a set of nodes to lie on the specified line.
        /// This is different from othe CoLinear goal, where the target line is computed based on the current node positions rather than being defined and fixed in advance.
        /// </summary>
        /// <param name="startPositions"></param>
        /// <param name="targetLine"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static OnLineGoal OnLineGoal_Create(
            List<Point> startPositions,
            [DefaultArgument("Line.ByStartPointEndPoint(Point.Origin(), Point.ByCoordinates(1.0, 0.0, 0.0))")] Line
                targetLine,
            [DefaultArgument("1.0")] double weight)
        {
            return new OnLineGoal(startPositions.ToTriples(), targetLine, (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="onLineGoal"></param>
        /// <param name="targetLineOrigin"></param>
        /// <param name="targetLineDirection"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static OnLineGoal OnLineGoal_Change(
            OnLineGoal onLineGoal,
            [DefaultArgument("null")] Point targetLineOrigin,
            [DefaultArgument("null")] Vector targetLineDirection,
            [DefaultArgument("-1.0")] double weight)
        {
            if (targetLineOrigin != null) onLineGoal.TargetLineOrigin = targetLineOrigin.ToTriple();
            if (targetLineDirection != null) onLineGoal.TargetLineDirection = targetLineDirection.ToTriple();
            if (weight >= 0.0) onLineGoal.Weight = (float) weight;
            return onLineGoal;
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="onLineGoal"></param>
        /// <param name="targetLine"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static OnLineGoal OnLineGoal_Change(
            OnLineGoal onLineGoal,
            [DefaultArgument("null")] Line targetLine,
            [DefaultArgument("-1.0")] double weight)
        {
            if (targetLine != null)
            {
                onLineGoal.TargetLineOrigin = targetLine.StartPoint.ToTriple();
                onLineGoal.TargetLineDirection =
                    (targetLine.EndPoint.ToTriple() - targetLine.StartPoint.ToTriple()).Normalise();
            }

            if (weight >= 0.0) onLineGoal.Weight = (float) weight;
            return onLineGoal;
        }


        //==================================================================
        // OnPlane
        //==================================================================

        /// <summary>
        /// Force a set of nodes to lie on the specified plane.
        /// This is different from othe CoPlanar goal, where the target plane is computed based on the current node positions rather than being defined and fixed in advance.
        /// </summary>
        /// <param name="startPositions"></param>
        /// <param name="targetPlaneOrigin"></param>
        /// <param name="targetPlaneNormal"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static OnPlaneGoal OnPlaneGoal_Create(
            List<Point> startPositions,
            [DefaultArgument("Point.Origin()")] Point targetPlaneOrigin,
            [DefaultArgument("Vector.ZAxis()")] Vector targetPlaneNormal,
            [DefaultArgument("1.0")] double weight)
        {
            return new OnPlaneGoal(
                startPositions.ToTriples(),
                targetPlaneOrigin.ToTriple(),
                targetPlaneNormal.ToTriple(),
                (float) weight);
        }

        /// <summary>
        /// Force a set of nodes to lie on the specified plane.
        /// This is different from othe CoPlanar goal, where the target plane is computed based on the current node positions rather than being defined and fixed in advance.
        /// </summary>
        /// <param name="startPositions"></param>
        /// <param name="targetPlane"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static OnPlaneGoal OnPlaneGoal_Create(
            List<Point> startPositions,
            [DefaultArgument("Plane.ByOriginNormal(Point.Origin(), Vector.ZAxis())")] Plane targetPlane,
            [DefaultArgument("1.0")] double weight)
        {
            return new OnPlaneGoal(startPositions.ToTriples(), targetPlane.Origin.ToTriple(),
                targetPlane.Normal.ToTriple(), (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="onPlaneGoal"></param>
        /// <param name="targetPlaneOrigin"></param>
        /// <param name="targetPlaneNormal"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static OnPlaneGoal OnPlaneGoal_Change(
            OnPlaneGoal onPlaneGoal,
            [DefaultArgument("null")] Point targetPlaneOrigin,
            [DefaultArgument("null")] Vector targetPlaneNormal,
            [DefaultArgument("-1.0")] double weight)
        {
            if (targetPlaneOrigin != null) onPlaneGoal.TargetPlaneOrigin = targetPlaneOrigin.ToTriple();
            if (targetPlaneNormal != null) onPlaneGoal.TargetPlaneNormal = targetPlaneNormal.ToTriple();
            if (weight >= 0.0) onPlaneGoal.Weight = (float) weight;
            return onPlaneGoal;
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="onPlaneGoal"></param>
        /// <param name="targetPlane"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static OnPlaneGoal OnPlaneGoal_Change(
            OnPlaneGoal onPlaneGoal,
            [DefaultArgument("null")] Plane targetPlane,
            [DefaultArgument("-1.0")] double weight)
        {
            if (targetPlane != null)
            {
                onPlaneGoal.TargetPlaneOrigin = targetPlane.Origin.ToTriple();
                onPlaneGoal.TargetPlaneNormal = targetPlane.Normal.ToTriple();
            }
            if (weight >= 0.0) onPlaneGoal.Weight = (float) weight;
            return onPlaneGoal;
        }


        //==================================================================
        // ParallelLines
        //==================================================================

        /// <summary>
        /// Force a set of lines to be parallel.
        /// </summary>
        /// <param name="startPositions">The nodes at the linie start points and end points, in the following order: start1, end1, start2, end2, etc...</param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static ParallelLinesGoal ParallelLinesGoal_Create(
            List<Point> startPositions,
            [DefaultArgument("1.0")] double weight)
        {
            return new ParallelLinesGoal(startPositions.ToTriples(), (float) weight);
        }


        /// <summary>
        /// Force a set of lines to be parallel.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static ParallelLinesGoal ParallelLinesGoal_Create(
            List<Line> lines,
            [DefaultArgument("1.0")] double weight)
        {
            List<Triple> startPositions = new List<Triple>();
            foreach (Line line in lines)
            {
                startPositions.Add(line.StartPoint.ToTriple());
                startPositions.Add(line.EndPoint.ToTriple());
            }
            return new ParallelLinesGoal(startPositions, (float)weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="parallelLinesGoal"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static ParallelLinesGoal ParallelLinesGoal_Change(
            ParallelLinesGoal parallelLinesGoal,
            [DefaultArgument("-1.0")] double weight)
        {
            if (weight >= 0.0) parallelLinesGoal.Weight = (float) weight;
            return parallelLinesGoal;
        }


        //==================================================================
        // ShapeMatching
        //==================================================================

        /// <summary>
        /// Move a set of nodes to positions that resemble a target shape.
        /// The target shape is defined by a sequence of points, in the same order as how the nodes are specified.
        /// </summary>
        /// <param name="startPositions"></param>
        /// <param name="targetShapePoints"></param>
        /// <param name="allowScaling"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static ShapeMatchingGoal ShapeMatchingGoal_Create(
            List<Point> startPositions,
            [DefaultArgument("null")] List<Point> targetShapePoints,
            [DefaultArgument("false")] bool allowScaling,
            [DefaultArgument("1.0")] double weight)
        {
            return targetShapePoints == null
                ? new ShapeMatchingGoal(startPositions.ToTriples(), allowScaling, (float) weight)
                : new ShapeMatchingGoal(startPositions.ToTriples(), targetShapePoints.ToTriples(), allowScaling,
                    (float) weight);
        }


        /// <summary>
        /// Adjust the goal's parameters while the solver is running.
        /// </summary>
        /// <param name="shapeMatchingGoal"></param>
        /// <param name="targetShapePoints"></param>
        /// <param name="allowScaling"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static ShapeMatchingGoal ShapeMatchingGoal_Change(
            ShapeMatchingGoal shapeMatchingGoal,
            [DefaultArgument("null")] List<Point> targetShapePoints,
            [DefaultArgument("false")] bool? allowScaling,
            [DefaultArgument("-1.0")] double weight)
        {
            if (targetShapePoints != null) shapeMatchingGoal.SetTargetShapePoints(targetShapePoints.ToTriples());
            if (weight >= 0.0) shapeMatchingGoal.Weight = (float) weight;
            if (allowScaling.HasValue) shapeMatchingGoal.AllowScaling = allowScaling.Value;
            return shapeMatchingGoal;
        }
    }
}