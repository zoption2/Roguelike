using Gameplay;

namespace BehaviourTree
{
    public interface IBehaviourTree
    {
        public void TickTree();
        public void InitTree(ICharacterController characterController);
        public void SetCharacters(IRoomContext characterContext);

    }
    public abstract class BehaviourTree : IBehaviourTree
    {
        protected Node _root;
        protected Blackboard _blackboard;
        protected ICharacterController _characterController;
        protected IRoomContext _characterScenarioContext;

        public void SetCharacters(IRoomContext characterContext)
        {
            _characterScenarioContext = characterContext;
        }

        public void InitTree(ICharacterController characterController)
        {
            _characterController = characterController;
            _blackboard = new Blackboard();
            _root = SetupRootNode();
            SetupBlackboard(_root);
            SetupCharacter(_root);
        }
        protected abstract Node SetupRootNode();
        protected abstract void UpdateData();

        protected void SetupBlackboard(Node node)
        {
            if (node.GetType() != typeof(Selector) && node.GetType() != typeof(Sequence))
            {
                node.SetBlackboard(_blackboard);
            }

            if (node.HasChildren())
            {
                foreach (Node node1 in node.GetChildren())
                {
                    SetupBlackboard(node1);
                }
            }
        }

        protected void SetupCharacter(Node node)
        {
            if (node.GetType() != typeof(Selector) && node.GetType() != typeof(Sequence))
            {
                node.SetCharacter(_characterController);
            }

            if (node.HasChildren())
            {
                foreach (Node node1 in node.GetChildren())
                {
                    SetupCharacter(node1);
                }
            }
        }
        public void TickTree()
        {
            UpdateData();
            _root.Evaluate();
        }
    }
}
