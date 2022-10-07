using System.Linq;
using System.Runtime.CompilerServices;
using Planet;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Utils;
using Utils.Extensions;

namespace Planet_System
{
    public class PlanetSystemJobbed : BasePlanetSystem
    {
        public override GravityBody[] Bodies
        {
            set
            {
                bodies = value;

                ReleaseNativeArrays();

                _masses = new NativeArray<float>(value.Select(body => body.Mass).ToArray(), Allocator.Persistent);
                _positions = new NativeArray<Vector3>(value.Length, Allocator.Persistent);
                _attractions = new NativeArray<Vector3>(value.Length, Allocator.Persistent);
            }
        }

        private NativeArray<float> _masses;
        private NativeArray<Vector3> _positions;
        private NativeArray<Vector3> _attractions;


        private void FixedUpdate()
        {
            GetPositions();

            var attractionJob = new AttractionJob(_positions, _masses, _attractions);
            var attractionHandle = attractionJob.Schedule(bodies.Length, bodies.Length);
            attractionHandle.Complete();

            for (var i = 0; i < attractionJob.attractions.Length; i++)
            {
                bodies[i].AddAttraction(attractionJob.attractions[i]);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void GetPositions()
            {
                for (var i = 0; i < bodies.Length; i++)
                {
                    _positions[i] = bodies[i].Position;
                }
            }
        }



        private void OnDestroy()
        {
            ReleaseNativeArrays();
        }

        private void ReleaseNativeArrays()
        {
            if (_masses.IsCreated)
            {
                _masses.Dispose();
            }

            if (_positions.IsCreated)
            {
                _positions.Dispose();
            }

            if (_attractions.IsCreated)
            {
                _attractions.Dispose();
            }
        }


        [BurstCompile]
        private struct AttractionJob : IJobParallelFor
        {
            [ReadOnly]
            private readonly NativeArray<Vector3> _positions;
            [ReadOnly]
            private readonly NativeArray<float> _masses;

            [WriteOnly]
            public NativeArray<Vector3> attractions;


            public AttractionJob(NativeArray<Vector3> positions, NativeArray<float> masses,
                NativeArray<Vector3> attractions)
            {
                _positions = positions;
                _masses = masses;
                this.attractions = attractions;
            }


            public void Execute(int index)
            {
                var attraction = Vector3.zero;
                var attractedBodyPosition = _positions[index];
                var attractedBodyMass = _masses[index];

                for (var i = 0; i < _positions.Length; i++)
                {
                    if (i != index)
                    {
                        var distance = _positions[i] - attractedBodyPosition;
                        var sqrDistance = distance.sqrMagnitude;
                        var attractionMagnitude = Universe.GravitationConstant
                            * attractedBodyMass * _masses[i]
                            / sqrDistance;
                        var attractionForce = distance.normalized * attractionMagnitude;

                        attraction.Add(ref attractionForce);
                    }
                }
                attractions[index] = attraction;
            }
        }
    }
}
