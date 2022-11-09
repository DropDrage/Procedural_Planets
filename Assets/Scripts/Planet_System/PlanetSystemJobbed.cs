using System.Linq;
using System.Runtime.CompilerServices;
using Planet;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Utils;

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
                _positions = new NativeArray<float3>(value.Length, Allocator.Persistent);
                _attractions = new NativeArray<float3>(value.Length, Allocator.Persistent);
            }
        }

        private NativeArray<float> _masses;
        private NativeArray<float3> _positions;
        private NativeArray<float3> _attractions;


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
            private readonly NativeArray<float3> _positions;
            [ReadOnly]
            private readonly NativeArray<float> _masses;

            [WriteOnly]
            public NativeArray<float3> attractions;


            public AttractionJob(NativeArray<float3> positions, NativeArray<float> masses,
                NativeArray<float3> attractions)
            {
                _positions = positions;
                _masses = masses;
                this.attractions = attractions;
            }


            public void Execute(int index)
            {
                var attraction = float3.zero;
                var attractedBodyPosition = _positions[index];
                var attractedBodyMass = _masses[index];

                for (var i = 0; i < _positions.Length; i++)
                {
                    if (i != index)
                    {
                        var position = _positions[i];
                        var normalizedDistance = math.normalize(position - attractedBodyPosition);
                        var sqrDistance = math.distancesq(position, attractedBodyPosition);
                        var attractionMagnitude = Universe.GravitationConstant
                            * attractedBodyMass * _masses[i]
                            / sqrDistance;
                        var attractionForce = normalizedDistance * attractionMagnitude;

                        attraction += attractionForce;
                    }
                }

                attractions[index] = attraction;
            }
        }
    }
}
