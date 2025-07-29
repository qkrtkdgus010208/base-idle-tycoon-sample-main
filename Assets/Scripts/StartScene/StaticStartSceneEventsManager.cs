namespace Project.StartScene
{
    using System;


	public static class StaticStartSceneEventsManager
	{
        /// <summary>
        /// Set active login UI events
        /// </summary>
        public static Action SetActiveLoginUI;

        /// <summary>
        /// Set active tap to start UI events
        /// </summary>
        public static Action SetActiveEnterTheGameUI;

        /// <summary>
        /// Events when splash screen finish
        /// </summary>
        public static Action OnSplashScreenDone;

        /// <summary>
        /// Events when player enter the game (tap on tap to start)
        /// </summary>
        public static Action OnEnterTheGame;

        /// <summary>
        /// Events to show maintenance notice
        /// </summary>
        public static Action ShowMaintenanceNotice;
    }
}
