/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NSubstitute;
using System;

namespace SAGESharp.Tests.IO.Binary
{
    static class Positionable
    {
        /// <summary>
        /// Verifies <paramref name="action"/> is executed after setting the position of
        /// <paramref name="positionable"/> to <paramref name="temporalPosition"/> and then
        /// gets restored to <paramref name="originalPosition"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// This should be called within a <see cref="Received.InOrder(Action)"/> block.
        /// </remarks>
        /// 
        /// <param name="positionable">The posibionable to verify.</param>
        /// <param name="originalPosition">The expected original position.</param>
        /// <param name="temporalPosition">The expected temporal position.</param>
        /// <param name="action">The action to be executed.</param>
        public static void VerifyDoAtPosition(this IPositionable positionable, long originalPosition, long temporalPosition, Action action)
        {
            positionable.GetPosition();
            positionable.SetPosition(temporalPosition);
            action.Invoke();
            positionable.SetPosition(originalPosition);
        }
    }
}
