namespace Project.Gameplay
{
    using Pathfinding;
    using System;
    using UnityEngine;


    public class MoveController : AIPath
    {
        /// <summary>
        /// is moving state
        /// true if move to current destination
        /// false if destination reached
        /// </summary>
        private bool isMoved;

        /// <summary>
        /// current calback after reaching current destination
        /// </summary>
        private Action OnReachDestination;

        /// <summary>
        /// Events to change character sprite flip state
        /// to facing character to right direction while moving
        /// </summary>
        private Action<bool> FlipSpriteAction;

        /// <summary>
        /// Character sprite offset from MoveController transform point
        /// to stop exactly at point on reach destination
        /// </summary>
        private Vector2 characterSpriteOffset;

        /// <summary>
        /// Set character sprite
        /// </summary>
        /// <param name="characterSpriteOffset"> Character sprite offset from move controller point </param>
        /// <param name="flipAction"> Character flip events </param>
        public void SetCharacterSprite(Vector2 characterSpriteOffset, Action<bool> flipAction)
        {
            this.characterSpriteOffset = characterSpriteOffset; // Assign characterSpriteOffset
            FlipSpriteAction = flipAction; // Assign FlipSpriteAction
        }

        // Set current target destination to go
        public void SetDestination(Vector2 destination, Action onReachDestination)
        {
            OnReachDestination = onReachDestination; // Assign current onReachDestination event
            seeker.StartPath(transform.position, destination - characterSpriteOffset); // Find path and go to destination
            isMoved = true; // Set move state true
        }

        // After arrive to current destination
        public override void OnTargetReached()
        {
            base.OnTargetReached(); // path finding OnTargetReached
            isMoved = false; // Set move state false

            OnReachDestination?.Invoke(); // Push current OnReachDestination event
        }

        /// <summary>
        /// Each character move
        /// </summary>
        /// <param name="nextPosition"></param>
        /// <param name="nextRotation"></param>
        public override void FinalizeMovement(Vector3 nextPosition, Quaternion nextRotation)
        {
            if (isMoved) // Checking is moving
            {
                // Set character flip state based on moving direction
                if (transform.position.x < nextPosition.x)
                    FlipSpriteAction?.Invoke(true);
                else if (transform.position.x > nextPosition.x)
                    FlipSpriteAction?.Invoke(false);

                // if transform.position.x == nextPosition.x, let the character flip state same as current flip state
            }

            base.FinalizeMovement(nextPosition, nextRotation); // path finding FinalizeMovement
        }

        /// <summary>
        /// Set moving speed
        /// </summary>
        /// <param name="speed"></param>
        public void SetMovementSpeed(float speed)
        {
            maxSpeed = speed; // Assign maxSpeed
        }
    }
}