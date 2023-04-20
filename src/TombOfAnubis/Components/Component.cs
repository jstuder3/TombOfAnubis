namespace TombOfAnubis
{
    public class Component
    {
        public Entity Entity { get; set; }

        public BaseSystem<Component> System { get; set; }

        public Visibility Visibility { get; set; } = Visibility.Both;

        /// <summary>
        /// Deletes the component. Override and call System.Deregister(this) on all registered systems.
        /// </summary>
        public virtual void Delete()
        {
        }

        public bool Visible()
        {
            return Visibility == Session.GetInstance().Visibility || Session.GetInstance().Visibility == Visibility.Both || Visibility == Visibility.Both;
        }
    }



}
