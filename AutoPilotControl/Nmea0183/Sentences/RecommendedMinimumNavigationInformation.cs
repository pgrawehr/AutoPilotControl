// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Globalization;
using System.Text;
using Iot.Device.Common;
using UnitsNet;

namespace Iot.Device.Nmea0183.Sentences
{
    // http://www.tronico.fi/OH6NT/docs/NMEA0183.pdf
    // page 14

    /// <summary>
    /// Represents RMC NMEA0183 sentence (Recommended Minimum Navigation Information)
    /// </summary>
    public class RecommendedMinimumNavigationInformation : NmeaSentence
    {
        /// <summary>
        /// The sentence id "RMC"
        /// </summary>
        public static SentenceId Id => new SentenceId('R', 'M', 'C');
        private static bool Matches(SentenceId sentence) => Id == sentence;
        private static bool Matches(TalkerSentence sentence) => Matches(sentence.Id);

        /// <summary>
        /// The navigation status of the message
        /// </summary>
        public NavigationStatus Status
        {
            get;
        }

        /// <summary>
        /// The position
        /// </summary>
        public GeographicPosition Position
        {
            get;
        }

        /// <summary>
        /// Speed over ground (m/s)
        /// </summary>
        public double SpeedOverGround
        {
            get;
        }

        /// <summary>
        /// The track over ground
        /// </summary>
        public Angle TrackMadeGoodInDegreesTrue
        {
            get;
        }

        /// <summary>
        /// The (estimated) magnetic variation at the current location
        /// </summary>
        public Angle MagneticVariationInDegrees
        {
            get;
        }

        public bool MagneticVariationInDegreesValid
        {
	        get;
        }

        /// <summary>
        /// Extended status. Not usually required.
        /// </summary>
        /// <remarks>
        /// http://www.tronico.fi/OH6NT/docs/NMEA0183.pdf
        /// doesn't mention this field but all other sentences have this
        /// and at least NEO-M8 sends it
        /// possibly each status is related with some part of the message
        /// but this unofficial spec does not clarify it
        /// </remarks>
        public NavigationStatus Status2 { get; private set; }

        public static double MetersPerSecondToKnots(double ms)
        {
	        return ms * 1.944012;
        }

        public static double KnotsToMetersPerSecond(double knots)
        {
	        return knots / 1.944012;
        }

		/// <inheritdoc />
		public override string ToNmeaParameterList()
        {
            // seems nullable don't interpolate well
            StringBuilder b = new StringBuilder();
            string time = Valid ? DateTime.ToString("HHmmss.fff") : string.Empty;
            b.Append($"{time},");
            string status = $"{(char)Status}";
            b.Append($"{status},");
            double degrees;
            CardinalDirection direction;
            DegreesToNmea0183(Position.Latitude, true, out degrees, out direction);
            b.Append($"{TalkerSentence.DoubleToString(degrees, 4, 5)},{(char)direction},");
            
            DegreesToNmea0183(Position.Longitude, false, out degrees, out direction);
            b.Append($"{TalkerSentence.DoubleToString(degrees, 4, 5)},{(char)direction},");

            string speed = MetersPerSecondToKnots(SpeedOverGround).ToString("0.000");
            b.Append($"{speed},");
            string track = TrackMadeGoodInDegreesTrue.Degrees.ToString("0.000");
            b.Append($"{track},");
            string date = Valid ? DateTime.ToString("ddMMyy") : string.Empty;
            b.Append($"{date},");
            if (MagneticVariationInDegreesValid)
            {
                string mag = Math.Abs(MagneticVariationInDegrees.Value).ToString("0.000");
                if (MagneticVariationInDegrees.Value >= 0)
                {
                    b.Append($"{mag},E");
                }
                else
                {
                    b.Append($"{mag},W");
                }
            }
            else
            {
                b.Append(",");
            }

            return b.ToString();
        }

        /// <summary>
        /// See <see cref="NmeaSentence"/> for constructor usage
        /// </summary>
        public RecommendedMinimumNavigationInformation(TalkerSentence sentence, DateTime time)
            : this(sentence.TalkerId, Matches(sentence) ? sentence.Fields : throw new ArgumentException($"SentenceId does not match expected id '{Id}'"), time)
        {
        }

        /// <summary>
        /// See <see cref="NmeaSentence"/> for constructor usage
        /// </summary>
        public RecommendedMinimumNavigationInformation(TalkerId talkerId, IEnumerable fields, DateTime time)
            : base(talkerId, Id, time)
        {
            IEnumerator field = fields.GetEnumerator();
            Position = new GeographicPosition();

            string newTime = ReadString(field);
            NavigationStatus status = (NavigationStatus)ReadChar(field);
            double lat = ReadValue(field);
            CardinalDirection latTurn = (CardinalDirection)ReadChar(field);
            double lon = ReadValue(field);
            CardinalDirection lonTurn = (CardinalDirection)ReadChar(field);
            double speed = ReadValue(field);
            double track = ReadValue(field);
            string date = ReadString(field);

            DateTime dateTime;
            if (date.Length != 0)
            {
                dateTime = ParseDateTime(date, newTime);
            }
            else
            {
                dateTime = ParseDateTime(time, newTime);
            }

            double mag = ReadValue(field);
            CardinalDirection magTurn = (CardinalDirection)ReadChar(field);

            DateTime = dateTime;
            Status = status;
            double latitude = Nmea0183ToDegrees(lat, latTurn);
            double longitude = Nmea0183ToDegrees(lon, lonTurn);

            Position = new GeographicPosition(latitude, longitude, 0);
            // If the message contains no position, it is unusable.
            // On the other hand, if the position is known (meaning the GPS receiver works), speed and track are known, too.
            Valid = true;
            

            SpeedOverGround = KnotsToMetersPerSecond(speed);

            TrackMadeGoodInDegreesTrue = Angle.FromDegrees(track);

            if (magTurn != CardinalDirection.None)
            {
                MagneticVariationInDegrees = Angle.FromDegrees(mag * DirectionToSign(magTurn));
                MagneticVariationInDegreesValid = true;
            }
            else
            {
	            MagneticVariationInDegreesValid = false;
            }
        }

        /// <summary>
        /// See <see cref="NmeaSentence"/> for constructor usage
        /// </summary>
        public RecommendedMinimumNavigationInformation(
            DateTime dateTime,
            NavigationStatus status,
            GeographicPosition position,
            double speedOverGround,
            Angle trackMadeGoodInDegreesTrue,
            Angle magneticVariationInDegrees)
        : base(OwnTalkerId, Id, dateTime)
        {
            Status = status;
            Position = position;
            SpeedOverGround = speedOverGround;
            TrackMadeGoodInDegreesTrue = trackMadeGoodInDegreesTrue;
            MagneticVariationInDegrees = magneticVariationInDegrees;
            Valid = true;
        }

        private static int DirectionToSign(CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.North:
                case CardinalDirection.East:
                    return 1;
                case CardinalDirection.South:
                case CardinalDirection.West:
                    return -1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
        }

        internal static double Nmea0183ToDegrees(double degreesMinutes, CardinalDirection direction)
        {
            // ddddmm.mm
            double degrees = Math.Floor(degreesMinutes / 100);
            double minutes = degreesMinutes - (degrees * 100);
            return ((double)degrees + (double)minutes / 60.0) * DirectionToSign(direction);
        }

        /// <inheritdoc />
        public override string ToReadableContent()
        {
            if (Valid)
            {
                return $"Position: {Position} / Speed {SpeedOverGround}m/s / Track {TrackMadeGoodInDegreesTrue.Degrees}°";
            }

            return "Position unknown";
        }

        internal static void DegreesToNmea0183(double degrees, bool isLatitude, out double degreesMinutes, out CardinalDirection direction)
        {
            double positiveDegrees;

            if (degrees >= 0)
            {
                direction = isLatitude ? CardinalDirection.North : CardinalDirection.East;
                positiveDegrees = degrees;
            }
            else
            {
                direction = isLatitude ? CardinalDirection.South : CardinalDirection.West;
                positiveDegrees = -degrees;
            }

            int integerDegrees = (int)positiveDegrees;
            double fractionDegrees = positiveDegrees - integerDegrees;
            double minutes = fractionDegrees * 60;

            // ddddmm.mm
            degreesMinutes = integerDegrees * 100 + minutes;
        }
    }
}
