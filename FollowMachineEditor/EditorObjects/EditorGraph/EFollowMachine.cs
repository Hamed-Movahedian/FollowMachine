using FMachine;

namespace FollowMachineEditor.EditorObjects.EditorGraph
{
    public class EFollowMachine : EGraph
    {
        private readonly FollowMachine _followMachine;

        public EFollowMachine(FollowMachine followMachine) : base(followMachine)
        {
            _followMachine = followMachine;
        }
    }
}
