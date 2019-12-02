using PacmanSimulator.Converters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PacmanSimulator.Views
{
    /// <summary>
    /// Enum representing the different Face directions allowed for the Pacman.
    /// </summary>
    public enum FaceDirection
    {
        East = 0,
        South,
        West,
        North
    }

    /// <summary>
    /// View Model for the Pacman Page.
    /// </summary>
    public class PacmanViewModel : ObservableObject
    {
        /* Constants */
        private static Dictionary<FaceDirection, FaceDirection> _rightMoves = new Dictionary<FaceDirection, FaceDirection>()
        {
            { FaceDirection.East, FaceDirection.South },
            { FaceDirection.South, FaceDirection.West },
            { FaceDirection.West, FaceDirection.North },
            { FaceDirection.North, FaceDirection.East }
        };

        private static Dictionary<FaceDirection, FaceDirection> _leftMoves = new Dictionary<FaceDirection, FaceDirection>()
        {
            { FaceDirection.South, FaceDirection.East },
            { FaceDirection.West, FaceDirection.South },
            { FaceDirection.North, FaceDirection.West },
            { FaceDirection.East, FaceDirection.North }
        };

        public const string __PLACE = "PLACE";
        public const string __MOVE = "MOVE";
        public const string __RIGHT = "RIGHT";
        public const string __LEFT = "LEFT";
        public const string __REPORT = "REPORT";

        /* Dependencies & Private fields */
        private StringToIntConverter _stringToIntConverter;
        private StringToFaceDirectionConverter _faceDirectionConverter;

        private int _internalInputX;
        private int _internalInputY;

        /* Properties */
        public List<string> Directions { get; private set; } = new List<string> { "EAST", "SOUTH", "WEST", "NORTH" };

        /// <summary>
        /// X position of the Pacman.
        /// </summary>
        public int X
        {
            get => _x;
            private set => SetProperty<int>(ref _x, value);
        }
        private int _x = -1;

        /// <summary>
        /// Y position of the Pacman.
        /// </summary>
        public int Y
        {
            get => _y;
            private set => SetProperty<int>(ref _y, value);
        }
        private int _y = -1;

        /// <summary>
        /// Face direction of the Pacman.
        /// </summary>
        public FaceDirection Face
        {
            get => _faceDirection;
            private set => SetProperty<FaceDirection>(ref _faceDirection, value, onChanged: () => onFaceDirectionChanged());
        }
        private FaceDirection _faceDirection = FaceDirection.East;

        /// <summary>
        /// Rotation value in degrees for rotating the Pacman.
        /// </summary>
        public int FaceRotation { get; private set; } = 0;

        /// <summary>
        /// Indicates whether Pacman can be placed or is visible at any moment.
        /// </summary>
        public bool CanShowPacman { get; private set; } = false;

        // User Input Fields
        /// <summary>
        /// X position value provided by user as Text input.
        /// </summary>
        public string InputX { get; set; }

        /// <summary>
        /// Y position value provided by user as Text input.
        /// </summary>
        public string InputY { get; set; }

        /// <summary>
        /// Face direction chosen by the user as input.
        /// </summary>
        public string InputDirection { get; set; }

        /// <summary>
        /// Input statement entered by the user in the command line input.
        /// </summary>
        public string InputCommand { get; set; }

        /// <summary>
        /// Indicates whether the input command from the user is valid or not. (displays the error message accordingly)
        /// </summary>
        public bool IsInvalidInput { get => _isInvalid; private set => SetProperty<bool>(ref _isInvalid, value); }
        private bool _isInvalid = false;

        /// <summary>
        /// Command for invoking the PLACE action.
        /// </summary>
        public ICommand PlaceCommand { get; private set; }

        /// <summary>
        /// Command for invoking the RIGHT rotation action.
        /// </summary>
        public ICommand RightCommand { get; private set; }

        /// <summary>
        /// Command for invoking the LEFT rotation action.
        /// </summary>
        public ICommand LeftCommand { get; private set; }

        /// <summary>
        /// Command for invoking the MOVE pacman action.
        /// </summary>
        public ICommand MoveCommand { get; private set; }

        /// <summary>
        /// Command for displaying the current Pacman position.
        /// </summary>
        public ICommand ReportCommand { get; private set; }

        /// <summary>
        /// Command for executing the string/command input for the Pacman actions.
        /// </summary>
        public ICommand ExecuteCommand { get; private set; }

        /// <summary>
        /// Cosntructor - creates an instance of the view model.
        /// </summary>
        public PacmanViewModel()
        {
            _stringToIntConverter = new StringToIntConverter();
            _faceDirectionConverter = new StringToFaceDirectionConverter();

            // Sets up the commands.
            PlaceCommand = new Command(() => placePacman(InputX, InputY, InputDirection));
            RightCommand = new Command(() => right());
            LeftCommand = new Command(() => left());
            MoveCommand = new Command(() => move());
            ReportCommand = new Command(async () => await report());
            ExecuteCommand = new Command(() => processCommandlineInput(InputCommand));
        }

        /// <summary>
        /// Action for rotating the Pacman in clockwise direction.
        /// </summary>
        private void right()
        {
            FaceDirection direction;

            if (_rightMoves.TryGetValue(this.Face, out direction))
                this.Face = direction;
        }

        /// <summary>
        /// Action for rotating the Pacman in anti-clockwise direction.
        /// </summary>
        private void left()
        {
            FaceDirection direction;

            if (_leftMoves.TryGetValue(this.Face, out direction))
                this.Face = direction;
        }

        /// <summary>
        /// Action for moving the Pacman based on its current face direction.
        /// </summary>
        private void move()
        {
            switch (this.Face)
            {
                case FaceDirection.East:
                    _internalInputX = (++_internalInputX <= 4) ? _internalInputX : 4;
                    break;
                case FaceDirection.North:
                    _internalInputY = (++_internalInputY <= 4) ? _internalInputY : 4;
                    break;
                case FaceDirection.West:
                    _internalInputX = (--_internalInputX >= 0) ? _internalInputX : 0;
                    break;
                case FaceDirection.South:
                    _internalInputY = (--_internalInputY >= 0) ? _internalInputY : 0;
                    break;
            }

            if (CanShowPacman)
                updatePacmanPosition();
        }

        /// <summary>
        /// Shows the current position of the Pacman as an alert.
        /// </summary>
        /// <returns>The task</returns>
        private async Task report()
        {
            string title = "Warning";
            string message = "Pacman is not placed yet!";
            if (X != -1 && Y != -1)
            {
                string faceDirection = (string)_faceDirectionConverter.ConvertBack(Face, typeof(string), null, null);
                title = "Pacman Position";
                message = $"Pacman is placed at X: {X}, Y: {(4 - Y)} facing {faceDirection}";
                Console.WriteLine($"Pacman Position: {X}, {(4 - Y)}, {faceDirection}");
            }

            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }

        /// <summary>
        /// Checks whether the user inputs are valid and can the Pacman be placed.
        /// </summary>
        /// <param name="inputX">X position input</param>
        /// <param name="inputY">Y position input</param>
        /// <param name="faceDirection">Face direction input</param>
        /// <returns></returns>
        private bool canPlacePacman(string inputX, string inputY, string faceDirection)
        {
            if (_stringToIntConverter == null)
                _stringToIntConverter = new StringToIntConverter();
            if (_faceDirectionConverter == null)
                _faceDirectionConverter = new StringToFaceDirectionConverter();

            _internalInputX = (int)_stringToIntConverter.Convert(inputX, typeof(int), null, null);
            _internalInputY = (int)_stringToIntConverter.Convert(inputY, typeof(int), null, null);

            if (_internalInputX < 0 || _internalInputX > 4 ||
                _internalInputY < 0 || _internalInputY > 4 ||
                !Directions.Contains(faceDirection.ToUpper()))
                return false;

            return true;
        }

        /// <summary>
        /// Places the Pacman based on the user inputs.
        /// </summary>
        /// <param name="inputX">X position input</param>
        /// <param name="inputY">Y position input</param>
        /// <param name="inputDirection">Face direction input</param>
        private void placePacman(string inputX, string inputY, string inputDirection)
        {
            var canPlace = canPlacePacman(inputX, inputY, inputDirection);
            if (canPlace)
            {
                updatePacmanPosition();
                Face = (FaceDirection)_faceDirectionConverter.Convert(inputDirection, typeof(FaceDirection), null, null);
            }
            CanShowPacman = canPlace;
            OnPropertyChanged(nameof(CanShowPacman));
        }

        /// <summary>
        /// Action on face direction of the Pacman is updated.
        /// Updates the rotation component so that the Image rotates through binding.
        /// </summary>
        private void onFaceDirectionChanged()
        {
            switch (Face)
            {
                case FaceDirection.North:
                    FaceRotation = 270;
                    break;
                case FaceDirection.South:
                    FaceRotation = 90;
                    break;
                case FaceDirection.West:
                    FaceRotation = 180;
                    break;
                case FaceDirection.East:
                default:
                    FaceRotation = 0;
                    break;
            }

            OnPropertyChanged(nameof(FaceRotation));
        }

        /// <summary>
        /// Updates the position of the Pacman based on the 
        /// </summary>
        private void updatePacmanPosition()
        {
            X = _internalInputX;
            Y = Math.Abs(_internalInputY - 4);    // This is to handle the positioning in the Xamarin Forms/mobile UI rendering
        }

        /// <summary>
        /// Processes the user input string and does the Pacman action accordingly.
        /// </summary>
        /// <param name="input">string input</param>
        private void processCommandlineInput(string input)
        {
            bool validInput = false;

            if (!string.IsNullOrEmpty(input))
            {
                char[] whitespace = { ' ', '\t' };
                string[] commands = input.Trim().Split(whitespace);

                var commandCount = commands.Length;
                if (commandCount >= 2)  // Only PLACE command comes with more than two arguments
                {
                    for (int i = 0; i < commandCount; i++)
                    {
                        var command = commands[i].Trim();

                        if (command.ToUpper() == __PLACE && i < (commandCount - 1))   // Found PLACE command
                        {
                            var positionInfo = commands[(i + 1)].Trim();
                            string[] positions = positionInfo.Split(',');

                            if (positions.Length == 3)
                            {
                                var inputX = positions[0].Trim();
                                var inputY = positions[1].Trim();
                                var direction = positions[2].Trim().ToUpper();

                                placePacman(inputX, inputY, direction); // Places the Pacman based on input.

                                validInput = CanShowPacman;

                                if (CanShowPacman)  // If Pacman gets placed, we can exit from the loop.
                                    break;
                            }
                        }
                    }
                }
                else if (commandCount == 1)    // All other commands should be single line. (LEFT, RIGHT, MOVE, and REPORT)
                {
                    if (!CanShowPacman)
                        return;

                    // Only if the Pacman is shown we can proceed with other commands.
                    var command = commands[0].Trim().ToUpper();

                    switch (command)
                    {
                        case __LEFT:
                            validInput = true;
                            left();
                            break;
                        case __RIGHT:
                            validInput = true;
                            right();
                            break;
                        case __MOVE:
                            validInput = true;
                            move();
                            break;
                        case __REPORT:
                            validInput = true;
                            ReportCommand.Execute(null);
                            break;
                        default:
                            break;
                    }
                }
            }

            IsInvalidInput = !validInput;
        }
    }
}
