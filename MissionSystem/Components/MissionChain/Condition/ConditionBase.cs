
namespace Gameplay.MissionSystem
{
    /// <summary>base class for all mission graph conditions</summary>
    public abstract class ConditionBase : MissionChainEditorObject
    {
        public abstract bool IsConditionMet { get; }
    }
}
