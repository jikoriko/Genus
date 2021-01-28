using Genus2D.Core;
using Genus2D.Entities;
using Genus2D.GameData;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgEditor
{
    public class ParticlePreviewWindow : StateWindow
    {

        public static ParticlePreviewWindow PreviewInstance { get; private set; }

        private State _state;
        private Entity _emitterEntity = null;

        public ParticlePreviewWindow(ParticleEmitterData data) 
            : base(800, 800, "Particle Preview", GameWindowFlags.FixedWindow)
        {
            PreviewInstance = this;
            _state = new State();
            this.PushState(_state);
            SetParticleData(data);
        }

        public void SetParticleData(ParticleEmitterData data)
        {
            if (_emitterEntity != null)
                _emitterEntity.Destroy();
            _emitterEntity = Entity.CreateInstance(_state.EntityManager);
            _emitterEntity.GetTransform().Position = new Vector3(400, 400, 0);
            ParticleEmitterComponent component = new ParticleEmitterComponent(_emitterEntity, data);
        }

        public override void Dispose()
        {
            base.Dispose();
            Instance = null;

        }
    }
}
