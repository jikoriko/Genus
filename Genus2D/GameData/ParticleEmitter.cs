using Genus2D.Utilities;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    public class ParticleEmitter
    {

        public class Particle
        {
            public Vector2 Direction;
            public Vector2 Position;
            public Vector2 Velocity;
            public float Scale;
            public float Rotation;
            public Color4 Colour;
            public float Life;
        }

        public ParticleEmitterData EmitterData { get; private set; }
        public List<Particle> Particles { get; private set; }
        private List<Particle> _deadParticles;

        private Random _random;
        private bool _destroyed;
        private Thread ParticleUpdateThread;

        public ParticleEmitter(ParticleEmitterData particleData)
        {
            EmitterData = particleData;
            Particles = new List<Particle>();
            _deadParticles = new List<Particle>();

            _random = new Random();
            _destroyed = false;

            ParticleUpdateThread = new Thread(new ThreadStart(Update));
            ParticleUpdateThread.Start();

        }

        public void Update()
        {
            long ticks = DateTime.Now.Ticks;
            long prevTicks = ticks;
            double deltaTime = 0.0;
            float updateTimer = 0.0f;

            while (!_destroyed)
            {
                prevTicks = ticks;
                ticks = DateTime.Now.Ticks;
                deltaTime = (ticks - prevTicks) / 10000000.0;
                updateTimer += (float)deltaTime;

                for (int i = 0; i < Particles.Count; i++)
                {
                    Particles[i].Life -= (float)deltaTime;
                    if (Particles[i].Life <= 0)
                    {
                        _deadParticles.Add(Particles[i]);
                        Particles.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        float deltaLife = 1.0f - (Particles[i].Life / EmitterData.MaxLife);
                        Particles[i].Velocity = Particles[i].Direction * MathTools.Lerp(EmitterData.StartVelocity, EmitterData.EndVelocity, deltaLife);
                        Particles[i].Position += Particles[i].Velocity * (float)deltaTime;
                        Particles[i].Scale = MathTools.Lerp(EmitterData.StartScale, EmitterData.EndScale, deltaLife);
                        Particles[i].Rotation += EmitterData.RotationSpeed * (float)deltaTime;
                        Particles[i].Colour = MathTools.Lerp(ref EmitterData.StartColour, ref EmitterData.EndColour, deltaLife);
                    }
                }

                while (updateTimer > 0)
                {
                    updateTimer -= 1.0f / EmitterData.EmissionRate;

                    Particle particle;
                    if (_deadParticles.Count > 0)
                    {
                        particle = _deadParticles[0];
                        _deadParticles.RemoveAt(0);
                    }
                    else
                    {
                        particle = new Particle();
                    }

                    float angle = MathHelper.DegreesToRadians(MathTools.Lerp(EmitterData.AngleMin, EmitterData.AngleMax, (float)_random.NextDouble()));
                    float offset = MathTools.Lerp(EmitterData.OffsetMin, EmitterData.OffsetMax, (float)_random.NextDouble());

                    particle.Direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                    particle.Direction.Normalize();

                    particle.Position = particle.Direction * offset;
                    particle.Velocity = particle.Direction * EmitterData.StartVelocity;
                    particle.Scale = EmitterData.StartScale;
                    particle.Rotation = 0;
                    particle.Colour = EmitterData.StartColour;
                    particle.Life = EmitterData.MaxLife;

                    Particles.Add(particle);
                }

            }
        }

        public void Destroy()
        {
            _destroyed = true;
        }

    }
}
