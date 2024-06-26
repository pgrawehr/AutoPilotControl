﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Globalization;
using System.Text;
using UnitsNet;

namespace Iot.Device.Nmea0183.Sentences
{
    /// <summary>
    /// This is the status reply from the autopilot. It is similar to the HTC message <see cref="HeadingAndTrackControl"/> with 4 extra fields.
    /// </summary>
    public class HeadingAndTrackControlStatus : NmeaSentence
    {
        /// <summary>
        /// This sentence's id
        /// </summary>
        public static SentenceId Id => new SentenceId("HTD");

        private static bool Matches(SentenceId sentence) => Id == sentence;

        private static bool Matches(TalkerSentence sentence) => Matches(sentence.Id);

        /// <summary>
        /// Initialize a new instance of this type.
        /// See the individual properties for further explanation to the parameters.
        /// </summary>
        /// <param name="status">Status of Autopilot</param>
        /// <param name="commandedRudderAngle">Commanded rudder angle</param>
        /// <param name="commandedRudderDirection">Rudder direction</param>
        /// <param name="turnMode">Turn mode</param>
        /// <param name="rudderLimit">Rudder limit</param>
        /// <param name="offHeadingLimit">Off-Heading limit</param>
        /// <param name="turnRadius">Desired turn radius</param>
        /// <param name="rateOfTurn">Desired rate of turn</param>
        /// <param name="desiredHeading">Desired heading</param>
        /// <param name="offTrackLimit">Desired off-track limit</param>
        /// <param name="commandedTrack">Commanded track direction</param>
        /// <param name="headingIsTrue">Heading uses true angles</param>
        /// <param name="rudderLimitExceeded">True if rudder limit is exceeded</param>
        /// <param name="headingLimitExceeded">True if the heading is out of limits</param>
        /// <param name="trackLimitExceeded">True if the track limits are exceeded</param>
        /// <param name="actualHeading">Heading from the Autopilot's onw heading sensor</param>
        public HeadingAndTrackControlStatus(string status, Angle commandedRudderAngle, string commandedRudderDirection, string turnMode,
            Angle rudderLimit, Angle offHeadingLimit, Length turnRadius, double rateOfTurn, Angle desiredHeading,
            Length offTrackLimit, Angle commandedTrack, bool headingIsTrue,
            bool rudderLimitExceeded, bool headingLimitExceeded, bool trackLimitExceeded, Angle actualHeading)
            : base(OwnTalkerId, Id, DateTime.UtcNow)
        {
            Valid = true;
            Status = status;
            DesiredHeading = desiredHeading;
            DesiredHeadingValid = desiredHeading.Degrees >= 0;
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
            RudderLimitExceeded = rudderLimitExceeded;
            HeadingLimitExceeded = headingLimitExceeded;
            TrackLimitExceeded = trackLimitExceeded;
            ActualHeading = actualHeading;
        }

        /// <summary>
        /// Internal constructor
        /// </summary>
        public HeadingAndTrackControlStatus(TalkerSentence sentence, DateTime time)
            : this(sentence.TalkerId, Matches(sentence) ? sentence.Fields : throw new ArgumentException($"SentenceId does not match expected id '{Id}'"), time)
        {
        }

        /// <summary>
        /// Decoding constructor
        /// </summary>
        public HeadingAndTrackControlStatus(TalkerId talkerId, IEnumerable fields, DateTime time)
            : base(talkerId, Id, time)
        {
            IEnumerator field = fields.GetEnumerator();
            string manualOverride = ReadString(field);
            CommandedRudderAngle = HeadingAndTrackControl.AsAngle(ReadValue(field));
            CommandedRudderDirection = ReadString(field);

            string autoPilotMode = ReadString(field);
            TurnMode = ReadString(field);
            RudderLimit = HeadingAndTrackControl.AsAngle(ReadValue(field));
            OffHeadingLimit = HeadingAndTrackControl.AsAngle(ReadValue(field));
            TurnRadius = HeadingAndTrackControl.AsLength(ReadValue(field));
            double turnRate = ReadValue(field);
            RateOfTurn = turnRate;
            DesiredHeading = HeadingAndTrackControl.AsAngle(ReadValue(field, out bool valid));
            DesiredHeadingValid = valid;
            OffTrackLimit = HeadingAndTrackControl.AsLength(ReadValue(field));
            CommandedTrack = HeadingAndTrackControl.AsAngle(ReadValue(field));
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

            RudderLimitExceeded = ReadString(field) == "V";
            HeadingLimitExceeded = ReadString(field) == "V";
            TrackLimitExceeded = ReadString(field) == "V";
            ActualHeading = HeadingAndTrackControl.AsAngle(ReadValue(field));
        }

        /// <summary>
        /// Autopilot status. Known values:
        /// M = Manual
        /// S = Stand-alone heading control
        /// H = Heading control with external source
        /// T = Track control
        /// R = Direct rudder control
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

        /// <summary>
        /// True if the rudder limit is exceeded
        /// </summary>
        public bool RudderLimitExceeded { get; }

        /// <summary>
        /// True if the heading limit is exceeded
        /// </summary>
        public bool HeadingLimitExceeded { get; }

        /// <summary>
        /// True if the track limit is exceeded
        /// </summary>
        public bool TrackLimitExceeded { get; }

        /// <summary>
        /// The actual heading, from the Autopilot's own heading sensor
        /// </summary>
        public Angle ActualHeading { get; }

        /// <inheritdoc />
        public override string ToNmeaParameterList()
        {
            if (!Valid)
            {
                return string.Empty;
            }

            StringBuilder b = new StringBuilder();
            b.Append(Status == "M" ? "A," : "V,");
            b.Append(HeadingAndTrackControl.FromAngle(CommandedRudderAngle));
            b.Append(CommandedRudderDirection + ",");
            b.Append(Status + ",");
            b.Append(TurnMode + ",");
            b.Append(HeadingAndTrackControl.FromAngle(RudderLimit));
            b.Append(HeadingAndTrackControl.FromAngle(OffHeadingLimit));
            b.Append(HeadingAndTrackControl.FromLength(TurnRadius));
            b.Append(RateOfTurn.ToString("F1") + ",");

            if (DesiredHeadingValid)
            {
	            b.Append(HeadingAndTrackControl.FromAngle(DesiredHeading));
            }
            else
            {
	            b.Append(",");
            }

            b.Append(HeadingAndTrackControl.FromLength(OffTrackLimit));
            b.Append(HeadingAndTrackControl.FromAngle(CommandedTrack));
            b.Append(HeadingIsTrue ? "T" : "M");

            string angleString = ActualHeading.Degrees.ToString("F1");
            b.Append(($",{(RudderLimitExceeded ? "V" : "A")},{(HeadingLimitExceeded ? "V" : "A")},{(TrackLimitExceeded ? "V" : "A")},{angleString}"));

            return b.ToString();
        }

        /// <inheritdoc />
        public override string ToReadableContent()
        {
            return $"Autopilot status: {Status}, Autopilot Heading: {ActualHeading}, CommandedTrack: {CommandedTrack}, TurnMode: {TurnMode}";
        }
    }
}
