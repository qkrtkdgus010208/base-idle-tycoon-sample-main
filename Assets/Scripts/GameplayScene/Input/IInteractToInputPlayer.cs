namespace Project.Gameplay
{
    /// <summary>
    /// Object that listen to player input
    /// </summary>
    public interface IInteractToInputPlayer
    {
        /// <summary>
        /// On player click in area of object
        /// </summary>
        void OnPlayerClick();

        /// <summary>
        /// On player stop to interact
        /// </summary>
        void OnStopInteract();
    }
}