// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Text;
using Iot.Device.Nmea0183;
using UnitsNet;

namespace Iot.Device.Nmea0183
{
    /// <summary>
    /// Extensions for positions
    /// </summary>
    public static partial class GeographicPositionExtensions
    {
        /// <summary>
        /// Normalizes the longitude to +/- 180°
        /// This is the common form for displaying longitudes. <see cref="NormalizeLongitudeTo360Degrees(GeographicPosition)"/> is used when the area of interest
        /// is close to the date border (in the pacific ocean)
        /// </summary>
        public static GeographicPosition NormalizeLongitudeTo180Degrees(this GeographicPosition position)
        {
            return new GeographicPosition(position.Latitude, NormalizeAngleTo180Degrees(position.Longitude), position.EllipsoidalHeight);
        }

        /// <summary>
        /// Normalizes the angle to +/- 180°
        /// </summary>
        internal static double NormalizeAngleTo180Degrees(double angleDegree)
        {
            angleDegree %= 360;
            if (angleDegree <= -180)
            {
                angleDegree += 360;
            }
            else if (angleDegree > 180)
            {
                angleDegree -= 360;
            }

            return angleDegree;
        }

        /// <summary>
        /// Normalizes the longitude to [0..360°)
        /// This coordinate form is advised if working in an area near the date border in the pacific ocean.
        /// </summary>
        public static GeographicPosition NormalizeLongitudeTo360Degrees(this GeographicPosition position)
        {
            return new GeographicPosition(position.Latitude, NormalizeAngleTo360Degrees(position.Longitude), position.EllipsoidalHeight);
        }

        /// <summary>
        /// Normalizes an angle to [0..360°)
        /// </summary>
        internal static double NormalizeAngleTo360Degrees(double angleDegree)
        {
            angleDegree %= 360;
            if (angleDegree < 0)
            {
                angleDegree += 360;
            }

            return angleDegree;
        }
    }
}
