// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Globalization;
using System.Text;
using UnitsNet;

namespace Iot.Device.Nmea0183.Sentences
{
    /// <summary>
    /// HTC sequence. This is used to control an attached auto-pilot.
    /// Documentation of this sentence is very poor, and one has to dig deep
    /// to find information about it.
    /// Since the actual encoding might be autopilot-dependent, this message outputs all values in raw format.
    /// </summary>
    /// <remarks>
    /// This class is preliminary and pending verification.
    /// </remarks>
    public class HeadingAndTrackControl : NmeaSentence
    {
        /// <summary>
        /// This sentence's id
        /// </summary>
        public static SentenceId Id => new SentenceId("HTC");
        private static bool Matches(SentenceId sentence) => Id == sentence;
        private static bool Matches(TalkerSentence sentence) => Matches(sentence.Id);

        /// <summary>
        /// Initialize a new instance of this type.
        /// See the individual properties for further explanation to the parameters.
        /// </summary>
        /// <param name="status">Status of Autopilot (see <see cref="Status"/>)</param>
        /// <param name="commandedRudderAngle">Commanded rudder angle</param>
        /// <param name="commandedRudderDirection">Rudder direction</param>
        /// <param name="turnMode">Turn mode</param>
        /// <param name="rudderLimit">Rudder limit</param>
        /// <param name="offHeadingLimit">Off-Heading limit</param>
        /// <param name="turnRadius">Desired turn radius</param>
        /// <param name="rateOfTurn">Desired rate of turn</param>
        /// <param name="desiredHeading">Desired heading</param>
        /// <param name="desiredHeadingValid">True if desired heading is to be set</param>
        /// <param name="offTrackLimit">Desired off-track limit</param>
        /// <param name="commandedTrack">Commanded track direction</param>
        /// <param name="headingIsTrue">Heading uses true angles</param>
        public HeadingAndTrackControl(string status, Angle commandedRudderAngle, string commandedRudderDirection, string turnMode,
            Angle rudderLimit, Angle offHeadingLimit, Length turnRadius, double rateOfTurn, Angle desiredHeading, bool desiredHeadingValid, 
            Length offTrackLimit, Angle commandedTrack, bool headingIsTrue)
        : base(OwnTalkerId, Id, DateTime.UtcNow)
        {
            Valid = true;
            Status = status;
            DesiredHeading = desiredHeading;
            DesiredHeadingValid = desiredHeadingValid;
            CommandedRudderAngle = commandedRudderAngle;
            HeadingIsTrue = headingIsTrue;
            CommandedRudderDirection = commandedRudderDirection;
            TurnMode = turnMode;
            RudderLimit = rudderLimit;
            OffHeadingLimit = offHeadingLimit;
            TurnRadius = turnRadius;
            RateOfTurn = rateOfTurn;
            OffTrackLimit = offTrackLimit;
            CommandedTrack = commandedTrack;
        }

        /// <summary>
        /// Internal constructor
        /// </summary>
        public HeadingAndTrackControl(TalkerSentence sentence, DateTime time)
            : this(sentence.TalkerId, Matches(sentence) ? sentence.Fields : throw new ArgumentException($"SentenceId does not match expected id '{Id}'"), time)
        {
        }

        /// <summary>
        /// Parsing constructor.
        /// </summary>
        public HeadingAndTrackControl(TalkerId talkerId, IEnumerable fields, DateTime time)
            : base(talkerId, Id, time)
        {
            IEnumerator field = fields.GetEnumerator();
            bool valid = false;
            string manualOverride = ReadString(field);
            CommandedRudderAngle = AsAngle(ReadValue(field));
            CommandedRudderDirection = ReadString(field);

            string autoPilotMode = ReadString(field);
            TurnMode = ReadString(field);
            RudderLimit = AsAngle(ReadValue(field));
            OffHeadingLimit = AsAngle(ReadValue(field));
            TurnRadius = AsLength(ReadValue(field));
            RateOfTurn = ReadValue(field);
            DesiredHeading = AsAngle(ReadValue(field, out valid));
            DesiredHeadingValid = valid;
            OffTrackLimit = AsLength(ReadValue(field));
            CommandedTrack = AsAngle(ReadValue(field));
            string headingReference = ReadString(field);

            // If override is active ("A"), then we treat this as standby
            if (manualOverride == "A")
            {
                Status = "M";
                Valid = true;
            }
            else
            {
                // It appears that on the NMEA2000 side, various proprietary messages are also used to control the autopilot,
                // hence this is missing some states, such as Wind mode.
                Status = autoPilotMode;
                Valid = true;
            }

            HeadingIsTrue = headingReference == "T";
        }

        /// <summary>
        /// Autopilot status. Known values:
        /// M = Manual
        /// S = Stand-alone heading control
        /// H = Heading control with external source
        /// T = Track control
        /// R = Direct rudder control
        /// W = Wind mode (inofficial)
        /// Anything else = ???
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// Heading to steer.
        /// </summary>
        public Angle DesiredHeading { get; private set; }

        public bool DesiredHeadingValid { get; private set; }

        /// <summary>
        /// Angle for directly controlling the rudder. Unsigned. (See <see cref="CommandedRudderDirection"/>)
        /// </summary>
        public Angle CommandedRudderAngle { get; private set; }

        /// <summary>
        /// True if all angles are true, otherwise false.
        /// </summary>
        public bool HeadingIsTrue { get; private set; }

        /// <summary>
        /// Commanded rudder direction "L" or "R" for port/starboard.
        /// </summary>
        public string CommandedRudderDirection { get; private set; }

        /// <summary>
        /// Turn mode (probably only valid for very expensive autopilots)
        /// Known values:
        /// R = Radius controlled
        /// T = Turn rate controlled
        /// N = Neither
        /// </summary>
        public string TurnMode { get; private set; }

        /// <summary>
        /// Maximum rudder angle
        /// </summary>
        public Angle RudderLimit { get; private set; }

        /// <summary>
        /// Maximum off-heading limit (in heading control mode)
        /// </summary>
        public Angle OffHeadingLimit { get; private set; }

        /// <summary>
        /// Desired turn Radius (when <see cref="TurnMode"/> is "R")
        /// </summary>
        public Length TurnRadius { get; private set; }

        /// <summary>
        /// Desired turn rate (when <see cref="TurnMode"/> is "T")
        /// Base unit is degrees/second
        /// </summary>
        public double RateOfTurn { get; private set; }

        /// <summary>
        /// Off-track warning limit, unsigned.
        /// </summary>
        public Length OffTrackLimit { get; private set; }

        /// <summary>
        /// Commanded track
        /// </summary>
        public Angle CommandedTrack { get; private set; }

        /// <inheritdoc />
        public override string ToNmeaParameterList()
        {
            if (!Valid)
            {
                return string.Empty;
            }

            StringBuilder b = new StringBuilder();
            b.Append(Status == "M" ? "A," : "V,");
            b.Append(FromAngle(CommandedRudderAngle));
            b.Append(CommandedRudderDirection + ",");
            b.Append(Status + ",");
            b.Append(TurnMode + ",");
            b.Append(FromAngle(RudderLimit));
            b.Append(FromAngle(OffHeadingLimit));
            b.Append(FromLength(TurnRadius));
            if (RateOfTurn != 0)
            {
                b.Append(RateOfTurn.ToString("F1") + ",");
            }
            else
            {
                b.Append(',');
            }

            if (DesiredHeadingValid)
            {
	            b.Append(FromAngle(DesiredHeading));
            }
            else
            {
	            b.Append(",");
            }

            b.Append(FromLength(OffTrackLimit));
            b.Append(FromAngle(CommandedTrack));
            b.Append(HeadingIsTrue ? "T" : "M");

            return b.ToString();
        }

        /// <inheritdoc />
        public override string ToReadableContent()
        {
            return $"Mode: {Status}, CommandedTrack: {CommandedTrack}, TurnMode: {TurnMode}";
        }

        /// <summary>
        /// Get a nullable double field as angle
        /// </summary>
        /// <param name="value">Input angle</param>
        /// <returns>An angle or null, if the input is null</returns>
        internal static Angle AsAngle(double value)
        {
            return Angle.FromDegrees(value);
        }

        /// <summary>
        /// Translate as nullable angle to a value in degrees with one digit
        /// </summary>
        /// <param name="angle">Angle to translate</param>
        /// <returns>The translated angle or just a comma</returns>
        internal static string FromAngle(Angle angle)
        {
            return angle.Degrees.ToString("F1") + ",";
        }

        internal static Length AsLength(double value)
        {
            return Length.FromNauticalMiles(value);
        }

        internal static string FromLength(Length length)
        {
            return length.NauticalMiles.ToString("F1") + ",";
        }
    }
}
