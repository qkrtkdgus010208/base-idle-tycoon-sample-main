namespace Project.BackndServer
{
    public static class BackndInitializer
    {
        private static bool isInitialize;
        public static bool IsInitialize => isInitialize;

        /// <summary>
        /// Initialize backnd
        /// </summary>
        public static void Initialize()
        {
            if (isInitialize)
                return;

            LoadingUIController.Instance.Loading();
            
            var bro = BackEnd.Backend.Initialize(); // Initialize BACKND

            // Response value for BACKND initialization
            if (!bro.IsSuccess())
            {
                LoadingUIController.Instance.FinishLoading();
                NoticeUIController.Instance.ShowNotice("Initialization failed : " + bro.Message, Initialize); // If failed, a 4xx statusCode error occurs
                return;
            }

            isInitialize = bro.IsSuccess();
            LoadingUIController.Instance.FinishLoading();
        }
    }
}