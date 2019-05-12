/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using System;

namespace SAGESharp
{
    class ValidationsTests
    {
        [Test]
        public void Test_Validate_ArgumentNotNull_With_A_Not_Null_Argument()
            => new object().Invoking(o => Validate.ArgumentNotNull("arg", o))
                .Should()
                .NotThrow<ArgumentNullException>();

        [Test]
        public void Test_Validate_ArgumentNotNull_With_A_Null_Argument()
        {
            string argumentName = "arg";
            Action action = () => Validate.ArgumentNotNull<object>(argumentName, null);

            action
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage($"*{argumentName}*");
        }
    }
}
