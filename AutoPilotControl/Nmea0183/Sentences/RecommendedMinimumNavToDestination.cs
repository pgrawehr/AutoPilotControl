// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Globalization;
using System.Text;
using Iot.Device.Common;
using Iot.Device.Nmea0183;
using UnitsNet;

namespace Iot.Device.Nmea0183.Sentences
{
    /// <summary>
    /// RMB sentence: Recommended minimum navigation information (current leg).
    /// This sentence is the bare minimum a navigation system should send to the autopilot.
    /// Normally, you would also send at least BWC, XTE and MWV
    /// </summary>
    public class RecommendedMinimumNavToDestination : NmeaSentence
    {
        /// <summary>
        /// The sentence id "RMB"
        /// </summary>
        public static SentenceId Id => new SentenceId('R', 'M', 'B');
        private static bool Matches(SentenceId sentence) => Id == sentence;
        private static bool Matches(TalkerSentence sentence) => Matches(sentence.Id);

        /// <summary>
        /// See <see cref="NmeaSentence"/> for constructor usage
        /// </summary>
        public RecommendedMinimumNavToDestination(TalkerSentence sentence, DateTime time)
            : this(sentence.TalkerId, Matches(sentence) ? sentence.Fields : throw new ArgumentException($"SentenceId does not match expected id '{Id}'"), time)
        {
        }

        /// <summary>
        /// See <see cref="NmeaSentence"/> for constructor usage
        /// </summary>
        public RecommendedMinimumNavToDestination(TalkerId talkerId, IEnumerable fields, DateTime time)
            : base(talkerId, Id, time)
        {
            IEnumerator field = fields.GetEnumerator();
            PreviousWayPointName = string.Empty;
            NextWayPointName = String.Empty;
            NextWayPoint = new GeographicPosition();

            string overallStatus = ReadString(field);
            double crossTrackError = ReadValue(field);
            string directionToSteer = ReadString(field);
            string previousWayPoint = ReadString(field);
            string nextWayPoint = ReadString(field);
            double nextWayPointLatitude = ReadValue(field);
            CardinalDirection nextWayPointHemisphere = (CardinalDirection)ReadChar(field);
            double nextWayPointLongitude = ReadValue(field);
            CardinalDirection nextWayPointDirection = (CardinalDirection)ReadChar(field);
            double rangeToWayPoint = ReadValue(field);
            double bearing = ReadValue(field);
            double approachSpeed = ReadValue(field);
            string arrivalStatus = ReadString(field);

            if (overallStatus == "A")
            {
                Valid = true;
                if (directionToSteer == "R")
                {
                    CrossTrackError = Length.FromNauticalMiles(-crossTrackError);
                }
                else
                {
                    CrossTrackError = Length.FromNauticalMiles(crossTrackError);
                }

                PreviousWayPointName = previousWayPoint ?? string.Empty;
                NextWayPointName = nextWayPoint ?? string.Empty;
                double latitude = RecommendedMinimumNavigationInformation.Nmea0183ToDegrees(nextWayPointLatitude, nextWayPointHemisphere);
                double longitude = RecommendedMinimumNavigationInformation.Nmea0183ToDegrees(nextWayPointLongitude, nextWayPointDirection);

                if (latitude != 0 && longitude != 0)
                {
                    NextWayPoint = new GeographicPosition(latitude, longitude, 0);
                }

                DistanceToWayPoint = Length.FromNauticalMiles(rangeToWayPoint);
                BearingToWayPoint = Angle.FromDegrees(bearing);

                if (rangeToWayPoint > 0 && bearing != 0)
                {
	                WaypointValid = true;
                }
                else
                {
	                WaypointValid = false;
                }

                ApproachSpeed = RecommendedMinimumNavigationInformation.KnotsToMetersPerSecond(approachSpeed);

                if (arrivalStatus == "A")
                {
                    Arrived = true;
                }
                else
                {
                    Arrived = false;
                }
            }
        }
        
        /// <summary>
        /// See <see cref="NmeaSentence"/> for constructor usage
        /// </summary>
        public RecommendedMinimumNavToDestination(
            DateTime dateTime,
            Length crossTrackError,
            string previousWayPointName,
            string nextWayPointName,
            GeographicPosition nextWayPoint,
            Length distanceToWayPoint,
            Angle bearingToWayPoint,
            double approachSpeedToWayPoint,
            bool arrived)
        : base(OwnTalkerId, Id, dateTime)
        {
            CrossTrackError = crossTrackError;
            PreviousWayPointName = previousWayPointName;
            NextWayPointName = nextWayPointName;
            NextWayPoint = nextWayPoint;
            DistanceToWayPoint = distanceToWayPoint;
            BearingToWayPoint = bearingToWayPoint;
            ApproachSpeed = approachSpeedToWayPoint;
            Arrived = arrived;
            Valid = true;
        }

        /// <summary>
        /// Cross track error. Positive: we are to the right of the desired route
        /// </summary>
        public Length CrossTrackError
        {
            get;
        }
        public bool WaypointValid
        {
	        get; set;
        }

		/// <summary>
		/// Name of previous waypoint
		/// </summary>
		public string PreviousWayPointName
        {
            get;
        }

        /// <summary>
        /// Name of next waypoint
        /// </summary>
        public string NextWayPointName
        {
            get;
        }

        /// <summary>
        /// Position of next waypoint (the waypoint we're heading to)
        /// </summary>
        public GeographicPosition NextWayPoint
        {
            get;
        }

        /// <summary>
        /// Distance to next waypoint
        /// </summary>
        public Length DistanceToWayPoint
        {
            get;
        }

        /// <summary>
        /// True bearing to waypoint
        /// </summary>
        public Angle BearingToWayPoint
        {
            get;
        }

        /// <summary>
        /// Speed of approach to the waypoint
        /// </summary>
        public double ApproachSpeed
        {
            get;
        }

        /// <summary>
        /// True: Within arrival circle of waypoint
        /// </summary>
        public bool Arrived
        {
            get;
        }

        /// <inheritdoc />
        public override string ToNmeaParameterList()
        {
            if (Valid)
            {
                StringBuilder b = new StringBuilder(256);
                b.Append("A,"); // Status = Valid
                if (CrossTrackError.Value >= 0)
                {
                    b.Append($"{CrossTrackError.NauticalMiles.ToString("F3")},L,");
                }
                else
                {
                    b.Append($"{(-CrossTrackError.NauticalMiles).ToString("F3")},R,");
                }

                b.Append($"{PreviousWayPointName},{NextWayPointName}");

                double degrees = 0;
                CardinalDirection direction = CardinalDirection.None;
                if (NextWayPoint != null)
                {
                    RecommendedMinimumNavigationInformation.DegreesToNmea0183(NextWayPoint.Latitude, true, out degrees, out direction);
                }

                if (degrees != 0 && direction != CardinalDirection.None)
                {
                    b.Append($"{TalkerSentence.DoubleToString(degrees, 4, 5)},{(char)direction},");
                }
                else
                {
                    b.Append(",,");
                }

                degrees = 0;
                direction = 0;
                if (NextWayPoint != null)
                {
                    RecommendedMinimumNavigationInformation.DegreesToNmea0183(NextWayPoint.Longitude, false, out degrees, out direction);
                }

                if (degrees != 0 && direction != CardinalDirection.None)
                {
                    b.Append($"{TalkerSentence.DoubleToString(degrees, 4, 5)},{(char)direction},");
                }
                else
                {
                    b.Append(",,");
                }

                if (WaypointValid)
                {
                    b.Append($"{DistanceToWayPoint.Value.ToString("F3")},");
                    b.Append($"{BearingToWayPoint.Value.ToString("F1")},");
				}
                else
                {
                    b.Append(",,");
                }

                b.Append($"{RecommendedMinimumNavigationInformation.MetersPerSecondToKnots(ApproachSpeed).ToString("F1")},");
                

                if (Arrived)
                {
                    // Not sure what the final D means here. My receiver sends it, but it is not documented.
                    b.Append("A,D");
                }
                else
                {
                    b.Append("V,D");
                }

                return b.ToString();
            }

            return string.Empty;
        }

        /// <inheritdoc />
        public override string ToReadableContent()
        {
            if (Valid)
            {
                if (NextWayPoint != null)
                {
                    return $"Next waypoint: {NextWayPoint}, Track deviation: {CrossTrackError}, Distance: {DistanceToWayPoint}";
                }
            }

            return "Not a valid RMB message";
        }
    }
}
