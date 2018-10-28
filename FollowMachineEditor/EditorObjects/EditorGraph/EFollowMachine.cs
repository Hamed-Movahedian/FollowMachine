using FMachine;

namespace FollowMachineEditor.EditorObjects.EditorGraph
{
    public class EFollowMachine : EGraph
    {
        private readonly FollowMachine _followMachine;

        public bool IsRunning
        {
            get => _followMachine.IsRunning;
            set => _followMachine.IsRunning = value;
        }

        public EFollowMachine(FollowMachine followMachine) : base(followMachine)
        {
            _followMachine = followMachine;
        }
    }
}
