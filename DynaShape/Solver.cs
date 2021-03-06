﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Dynamo.Wpf.ViewModels.Watch3D;
using DynaShape.Goals;
using DynaShape.GeometryBinders;


using Point = Autodesk.DesignScript.Geometry.Point;
using Vector = Autodesk.DesignScript.Geometry.Vector;

namespace DynaShape
{
    [IsVisibleInDynamoLibrary(false)]
    public class Solver : IDisposable
    {
        //==============================================================
        // static members
        //==============================================================

        internal static IWatch3DViewModel Viewport = null;
        internal static CameraData CameraData = null;

        //==============================================================

        public bool AllowMouseInteraction = true;

        public List<Node> Nodes = new List<Node>();
        public List<Goal> Goals = new List<Goal>();
        public List<GeometryBinder> GeometryBinders = new List<GeometryBinder>();

        public List<Triple> GetNodePositions() => Nodes.Select(node => node.Position).ToList();
        public List<Point> GetNodePositionsAsPoints() => Nodes.Select(node => node.Position.ToPoint()).ToList();
        public List<Triple> GetNodeVelocities() => Nodes.Select(node => node.Velocity).ToList();
        public List<Vector> GetNodeVelocitiesAsVectors() => Nodes.Select(node => node.Velocity.ToVector()).ToList();
        public List<List<Geometry>> GetGeometries() => GeometryBinders.Select(gb => gb.GetGeometries(Nodes)).ToList();
        public List<List<object>> GetGoalOutputs() => Goals.Select(goal => goal.GetOutput(Nodes)).ToList();


        internal int HandleNodeIndex = -1;
        internal int NearestNodeIndex = -1;
        internal IRay ClickRay = null;


        public Solver()
        {
            if (Viewport != null)
            {
                Viewport.ViewMouseDown += ViewportMouseDownHandler;
                Viewport.ViewMouseUp += ViewportMouseUpHandler;
                Viewport.ViewMouseMove += ViewportMouseMoveHandler;
                Viewport.ViewCameraChanged += ViewportCameraChangedHandler;
                Viewport.CanNavigateBackgroundPropertyChanged += CanNavigateBackgroundPropertyChangedHandler;
            }
        }


        public void Clear()
        {
            Nodes.Clear();
            Goals.Clear();
            GeometryBinders.Clear();

        }


        public void AddGoals(IEnumerable<Goal> goals, double nodeMergeThreshold = 0.001)
        {
            foreach (Goal goal in goals)
                AddGoal(goal, nodeMergeThreshold);
        }


        public void AddGeometryBinders(IEnumerable<GeometryBinder> geometryBinders, double nodeMergeThreshold = 0.001)
        {
            foreach (GeometryBinder geometryBinder in geometryBinders)
                AddGeometryBinder(geometryBinder, nodeMergeThreshold);
        }


        public void AddGoal(Goal goal, double nodeMergeThreshold = 0.001)
        {
            Goals.Add(goal);

            if (goal.StartingPositions == null && goal.NodeIndices != null) return;

            goal.NodeIndices = new int[goal.NodeCount];

            for (int i = 0; i < goal.NodeCount; i++)
            {
                bool nodeAlreadyExist = false;

                for (int j = 0; j < Nodes.Count; j++)
                    if ((goal.StartingPositions[i] - Nodes[j].Position).LengthSquared <
                        nodeMergeThreshold * nodeMergeThreshold)
                    {
                        goal.NodeIndices[i] = j;
                        nodeAlreadyExist = true;
                        break;
                    }

                if (!nodeAlreadyExist)
                {
                    Nodes.Add(new Node(goal.StartingPositions[i]));
                    goal.NodeIndices[i] = Nodes.Count - 1;
                }
            }
        }


        public void AddGeometryBinder(GeometryBinder geometryBinder, double nodeMergeThreshold = 0.001)
        {
            GeometryBinders.Add(geometryBinder);

            if (geometryBinder.StartingPositions == null && geometryBinder.NodeIndices != null) return;

            geometryBinder.NodeIndices = new int[geometryBinder.NodeCount];

            for (int i = 0; i < geometryBinder.NodeCount; i++)
            {
                bool nodeAlreadyExist = false;

                for (int j = 0; j < Nodes.Count; j++)
                    if ((geometryBinder.StartingPositions[i] - Nodes[j].Position).LengthSquared <
                        nodeMergeThreshold * nodeMergeThreshold)
                    {
                        geometryBinder.NodeIndices[i] = j;
                        nodeAlreadyExist = true;
                        break;
                    }

                if (!nodeAlreadyExist)
                {
                    Nodes.Add(new Node(geometryBinder.StartingPositions[i]));
                    geometryBinder.NodeIndices[i] = Nodes.Count - 1;
                }
            }
        }


        public void Reset()
        {
            foreach (Node node in Nodes) node.Reset();
        }


        public void Step(bool momentum = true)
        {
            if (momentum) foreach (Node node in Nodes) node.Position += node.Velocity;

            Parallel.ForEach(Goals, goal => goal.Compute(Nodes));

            Triple[] nodeMoveSums = new Triple[Nodes.Count];
            float[] nodeWeightSums = new float[Nodes.Count];

            foreach (Goal goal in Goals)
                for (int i = 0; i < goal.NodeCount; i++)
                {
                    nodeMoveSums[goal.NodeIndices[i]] += goal.Moves[i] * goal.Weight;
                    nodeWeightSums[goal.NodeIndices[i]] += goal.Weight;
                }


            //=================================================================================

            if (HandleNodeIndex != -1)
            {
                float mouseInteractionWeight = 30f;
                nodeWeightSums[HandleNodeIndex] += mouseInteractionWeight;

                Triple clickRayOrigin = new Triple(ClickRay.Origin.X, ClickRay.Origin.Y, ClickRay.Origin.Z);
                Triple clickRayDirection = new Triple(ClickRay.Direction.X, ClickRay.Direction.Y, ClickRay.Direction.Z);
                clickRayDirection = clickRayDirection.Normalise();
                Triple v = Nodes[HandleNodeIndex].Position - clickRayOrigin;
                Triple grabMove = v.Dot(clickRayDirection) * clickRayDirection - v;
                nodeMoveSums[HandleNodeIndex] += mouseInteractionWeight * grabMove;
            }

            //=================================================================================

            for (int i = 0; i < Nodes.Count; i++)
            {
                Triple move = nodeMoveSums[i] / nodeWeightSums[i];
                Nodes[i].Position += move;
                if (momentum) Nodes[i].Velocity += move;
                if (Nodes[i].Velocity.Dot(move) < 0.0) Nodes[i].Velocity *= 0.9f;
            }
        }


        public void Step(int iterationCount, bool momentum = true)
        {
            for (int i = 0; i < iterationCount; i++) Step(momentum);
        }


        public void Step(double miliseconds, bool momentum = true)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed.TotalMilliseconds < miliseconds)
                Step(momentum);
        }


        private void ViewportCameraChangedHandler(object sender, RoutedEventArgs args)
        {
            NearestNodeIndex = -1;
        }


        private void ViewportMouseDownHandler(object sender, MouseButtonEventArgs args)
        {
            if (args.LeftButton == MouseButtonState.Pressed && AllowMouseInteraction)
                HandleNodeIndex = FindNearestNodeIndex(ClickRay);
        }


        private void ViewportMouseUpHandler(object sender, MouseButtonEventArgs args)
        {
            HandleNodeIndex = -1;
            NearestNodeIndex = -1;
        }


        private void ViewportMouseMoveHandler(object sender, MouseEventArgs args)
        {
            if (!AllowMouseInteraction) return;
            ClickRay = Viewport.GetClickRay(args);
            if (args.LeftButton == MouseButtonState.Released) HandleNodeIndex = -1;
            NearestNodeIndex = FindNearestNodeIndex(ClickRay);
        }


        internal int FindNearestNodeIndex(IRay clickRay, float range = 0.03f)
        {
            CameraData cameraData = Viewport.GetCameraInformation();
            Triple camZ = new Triple(cameraData.LookDirection.X, -cameraData.LookDirection.Z,
                cameraData.LookDirection.Y).Normalise();
            Triple camY = new Triple(cameraData.UpDirection.X, -cameraData.UpDirection.Z, cameraData.UpDirection.Y)
                .Normalise();
            Triple camX = camY.Cross(camZ).Normalise();

            Triple clickRayOrigin = new Triple(clickRay.Origin.X, clickRay.Origin.Y, clickRay.Origin.Z);
            Triple clickRayDirection = new Triple(clickRay.Direction.X, clickRay.Direction.Y, clickRay.Direction.Z);

            Triple mousePosition2D = new Triple(clickRayDirection.Dot(camX), clickRayDirection.Dot(camY),
                clickRayDirection.Dot(camZ));
            mousePosition2D /= mousePosition2D.Z;

            int nearestNodeIndex = -1;

            float minDistSquared = range * range;

            for (int i = 0; i < Nodes.Count; i++)
            {
                Triple v = Nodes[i].Position - clickRayOrigin;
                v = new Triple(v.Dot(camX), v.Dot(camY), v.Dot(camZ));
                Triple nodePosition2D = v / v.Z;

                float distSquared = (mousePosition2D - nodePosition2D).LengthSquared;

                if (distSquared < minDistSquared)
                {
                    minDistSquared = distSquared;
                    nearestNodeIndex = i;
                }
            }

            return nearestNodeIndex;
        }


        private void CanNavigateBackgroundPropertyChangedHandler(bool canNavigate)
        {
            HandleNodeIndex = -1;
            NearestNodeIndex = -1;
        }


        public void Dispose()
        {
            if (Viewport != null)
            {
                Viewport.ViewMouseDown -= ViewportMouseDownHandler;
                Viewport.ViewMouseUp -= ViewportMouseUpHandler;
                Viewport.ViewMouseMove -= ViewportMouseMoveHandler;
                Viewport.ViewCameraChanged -= ViewportCameraChangedHandler;
                Viewport.CanNavigateBackgroundPropertyChanged -= CanNavigateBackgroundPropertyChangedHandler;
            }
        }
    }
}
