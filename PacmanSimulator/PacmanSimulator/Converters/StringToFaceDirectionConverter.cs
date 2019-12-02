using PacmanSimulator.Views;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace PacmanSimulator.Converters
{
    /// <summary>
    /// Converter to convert string value to a face direction enum value.
    /// </summary>
    public class StringToFaceDirectionConverter : IValueConverter
    {
        public const string __EAST = "EAST";
        public const string __SOUTH = "SOUTH";
        public const string __WEST = "WEST";
        public const string __NORTH = "NORTH";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty((string)value))
                throw new InvalidOperationException("Input cannot be null");

            if (value is String)
            {
                string direction = ((string)value).ToUpper();
                FaceDirection faceDirection = FaceDirection.East;
                switch (direction)
                {
                    case __EAST:
                        faceDirection = FaceDirection.East;
                        break;
                    case __SOUTH:
                        faceDirection = FaceDirection.South;
                        break;
                    case __WEST:
                        faceDirection = FaceDirection.West;
                        break;
                    case __NORTH:
                        faceDirection = FaceDirection.North;
                        break;
                }

                return faceDirection;
            }
            else
                throw new InvalidOperationException("Input should be a string value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value is FaceDirection)
            {
                string stringDirection = null;
                switch ((FaceDirection)value)
                {
                    case FaceDirection.East:
                        stringDirection = __EAST;
                        break;
                    case FaceDirection.South:
                        stringDirection = __SOUTH;
                        break;
                    case FaceDirection.West:
                        stringDirection = __WEST;
                        break;
                    case FaceDirection.North:
                        stringDirection = __NORTH;
                        break;
                }

                return stringDirection;
            }
            else
                throw new InvalidCastException("Input should be a FaceDirection");
        }
    }
}