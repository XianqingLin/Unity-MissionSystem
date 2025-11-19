
namespace Gameplay.MissionSystem
{
    /// <summary>require template base type</summary>
    public abstract class MissionRequireTemplate : MissionRequire<object>
    {
        public abstract class MissionRequireTemplateHandle : MissionRequireHandle<object>
        {
            protected MissionRequireTemplateHandle(MissionRequireTemplate require) : base(require) { }
        }
    }
}
