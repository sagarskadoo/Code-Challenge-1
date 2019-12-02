using Microsoft.VisualStudio.TestTools.UnitTesting;
using PacmanSimulator.Views;

namespace PacmanSimulator.Tests
{
    /// <summary>
    /// Tests for <see cref="PacmanViewModel"/> class.
    /// </summary>
    [TestClass]
    public class PacmanViewModelTests
    {
        /// <summary>
        /// Tests for <see cref="PacmanViewModel"/> constructor.
        /// </summary>
        [TestMethod]
        public void PacmanViewModel_ctor()
        {
            var instance = new PacmanViewModel();

            // Verify
            Assert.IsNotNull(instance.PlaceCommand);
            Assert.IsNotNull(instance.MoveCommand);
            Assert.IsNotNull(instance.LeftCommand);
            Assert.IsNotNull(instance.RightCommand);
            Assert.IsNotNull(instance.ReportCommand);
            Assert.IsNotNull(instance.ExecuteCommand);
        }

        /// <summary>
        /// Tests for <see cref="PacmanViewModel"/> ExecuteCommand.
        /// Tests all the command flows.
        /// </summary>
        [TestMethod]
        public void PacmanViewModel_ExecuteInputCommand()
        {
            var instance = new PacmanViewModel();

            // Default values - Verify.
            Assert.IsFalse(instance.CanShowPacman);
            Assert.IsFalse(instance.IsInvalidInput);
            Assert.IsTrue(instance.X == -1);
            Assert.IsTrue(instance.Y == -1);
            Assert.IsTrue(instance.Face == FaceDirection.East);
            Assert.IsTrue(instance.FaceRotation == 0);

            /// Incorrect scenario.
            // Setup
            instance.InputCommand = "PLACE 5,8,LONG";

            // Invoke
            instance.ExecuteCommand.Execute(null);

            // Verify
            Assert.IsFalse(instance.CanShowPacman);
            Assert.IsTrue(instance.IsInvalidInput);
            Assert.IsTrue(instance.X == -1);
            Assert.IsTrue(instance.Y == -1);
            Assert.IsTrue(instance.Face == FaceDirection.East);
            Assert.IsTrue(instance.FaceRotation == 0);

            /// PLACE - Correct scenario.
            // Setup
            instance.InputCommand = "PLACE 0,0,SOUTH";

            // Invoke
            instance.ExecuteCommand.Execute(null);

            // Verify
            Assert.IsTrue(instance.X == 0);
            Assert.IsTrue(instance.Y == 4);
            Assert.IsTrue(instance.Face == FaceDirection.South);
            Assert.IsTrue(instance.FaceRotation == 90);
            Assert.IsTrue(instance.CanShowPacman);
            Assert.IsFalse(instance.IsInvalidInput);

            // MOVE/RIGHT/LEFT - scenarios

            /// LEFT
            // Setup;
            instance.InputCommand = "LEFT";

            // Invoke
            instance.ExecuteCommand.Execute(null);

            // Verify
            Assert.IsTrue(instance.X == 0);
            Assert.IsTrue(instance.Y == 4);
            Assert.IsTrue(instance.Face == FaceDirection.East);
            Assert.IsTrue(instance.FaceRotation == 0);
            Assert.IsFalse(instance.IsInvalidInput);

            /// MOVE
            // Setup
            instance.InputCommand = "MOVE";

            // Invoke
            instance.ExecuteCommand.Execute(null);

            Assert.IsTrue(instance.X == 1);
            Assert.IsTrue(instance.Y == 4);
            Assert.IsTrue(instance.Face == FaceDirection.East);
            Assert.IsTrue(instance.FaceRotation == 0);
            Assert.IsFalse(instance.IsInvalidInput);

            /// RIGHT
            // Setup
            instance.InputCommand = "RIGHT";

            // Invoke
            instance.ExecuteCommand.Execute(null);

            Assert.IsTrue(instance.X == 1);
            Assert.IsTrue(instance.Y == 4);
            Assert.IsTrue(instance.Face == FaceDirection.South);
            Assert.IsTrue(instance.FaceRotation == 90);
            Assert.IsFalse(instance.IsInvalidInput);

            /// INVALID Command scenario
            // Setup
            instance.InputCommand = "INVALID";

            // Invoke
            instance.ExecuteCommand.Execute(null);

            // Verify
            Assert.IsTrue(instance.IsInvalidInput);
        }
    }
}