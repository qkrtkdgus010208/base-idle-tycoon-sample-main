namespace Project.Utility
{
    using UnityEngine;
    using UnityEngine.UI;

    public static class StaticImageSpriteFitter
    {
        /// <summary>
        /// Re fit image with it current sprite
        /// </summary>
        /// <param name="minSize"> minimum image size </param>
        /// <param name="maxSize"> maximum image size </param>
        /// <param name="image"> image target </param>
        public static void FitImageSprite(Vector2 minSize, Vector2 maxSize, Graphic image)
        {
            image.SetNativeSize();
            image.rectTransform.sizeDelta = CheckingSize(minSize, maxSize, image.rectTransform.sizeDelta, false, false);
        }

        /// <summary>
        /// Checking image size
        /// </summary>
        /// <param name="minSize"> minimum image size </param>
        /// <param name="maxSize"> maximum image size </param>
        /// <param name="currentSize"> current image size </param>
        /// <param name="isCheckMaxSize"> current maximum size checked state </param>
        /// <param name="isCheckMinSize"> current minimum size checked state </param>
        /// <returns> image fit size </returns>
        private static Vector2 CheckingSize(Vector2 minSize, Vector2 maxSize, Vector2 currentSize, bool isCheckMaxSize, bool isCheckMinSize)
        {
            // Infinite Loop Handler (Make It Min Size)
            if (!isCheckMinSize && isCheckMaxSize)
                return currentSize;

            if (!isCheckMinSize) // Check for min size
            {
                while (currentSize.x < minSize.x || currentSize.y < minSize.y) // if size is smaller then minimum size
                {
                    currentSize += currentSize * 0.1f; // enlarge size of image
                    isCheckMaxSize = false; // recheck maximum size
                }

                isCheckMinSize = true; // set min size checked

                if (!isCheckMaxSize)
                    return CheckingSize(minSize, maxSize, currentSize, isCheckMaxSize, isCheckMinSize);
            }
            else if (!isCheckMaxSize) // Check for max size
            {
                while (currentSize.x > maxSize.x || currentSize.y > maxSize.y)  // if size is bigger then maximum size
                {
                    currentSize -= currentSize * 0.1f; // minimize size of image
                    isCheckMinSize = false; // recheck minimum size
                }

                isCheckMaxSize = true;

                if (!isCheckMinSize)
                    return CheckingSize(minSize, maxSize, currentSize, isCheckMaxSize, isCheckMinSize);
            }

            return currentSize;
        }
    }
}
