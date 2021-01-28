using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.Utililities;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.Entities
{
    public class ParticleEmitterComponent : EntityComponent
    {

        private ParticleEmitter _emitter;
        private Texture _particleTexture;

        public ParticleEmitterComponent(Entity entity, ParticleEmitterData data) 
            : base(entity)
        {
            _emitter = new ParticleEmitter(data);
            if (data.ParticleTexture != "")
            {
                _particleTexture = Assets.GetTexture("Particles/" + data.ParticleTexture);
            }
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            //Console.WriteLine(_emitter.Particles.Count);
        }

        public override void Render(FrameEventArgs e)
        {
            base.Render(e);
            Vector3 pos;

            for (int i = 0; i < _emitter.Particles.Count; i++)
            {
                ParticleEmitter.Particle particle = _emitter.Particles[i];
                if (particle != null)
                {
                    pos = this.Parent.GetTransform().Position + new Vector3(particle.Position.X, particle.Position.Y, 0);
                    Vector3 scale = new Vector3(particle.Scale);
                    Vector3 rotation = new Vector3(0, 0, particle.Rotation);

                    Shape shape = ShapeFactory.Rectangle;
                    switch (_emitter.EmitterData.EmitterShape)
                    {
                        case PaticleEmitterShape.Circle:
                            shape = ShapeFactory.Circle;
                            break;
                        case PaticleEmitterShape.Triangle:
                            shape = ShapeFactory.Triangle;
                            break;
                        case PaticleEmitterShape.Star:
                            shape = ShapeFactory.Star;
                            break;
                    }

                    if (_particleTexture != null)
                    {
                        Renderer.FillTexture(_particleTexture, shape, ref pos, ref scale, ref rotation, ref particle.Colour);
                    }
                    else
                    {
                        Renderer.FillShape(shape, ref pos, ref scale, ref rotation, ref particle.Colour);
                    }
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            _emitter.Destroy();
        }
    }
}
