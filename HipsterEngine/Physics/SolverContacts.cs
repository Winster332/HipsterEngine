using Box2DX.Dynamics;

namespace HipsterEngine.Physics
{
    public delegate void EventSolverPoint(ContactPoint contactPoint);
    public delegate void EventSolverResult(ContactResult contactResult);
    
    public class SolverContacts : ContactListener
    {
        public event EventSolverPoint onAdd;
        public event EventSolverPoint onPersist;
        public event EventSolverResult onResult;
        public event EventSolverPoint onRemove;

        public SolverContacts()
        {
        }

        public override void Add(ContactPoint point)
        {
            base.Add(point);

            onAdd?.Invoke(point);
        }

        public override void Persist(ContactPoint point)
        {
            base.Persist(point);

            onPersist?.Invoke(point);
        }

        public override void Result(ContactResult point)
        {
            base.Result(point);

            onResult?.Invoke(point);
        }

        public override void Remove(ContactPoint point)
        {
            base.Remove(point);

            onRemove?.Invoke(point);
        }
    }
}