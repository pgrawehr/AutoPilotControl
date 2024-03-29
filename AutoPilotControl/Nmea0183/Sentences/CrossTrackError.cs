// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Text;
using Iot.Device.Nmea0183;
using UnitsNet;

namespace Iot.Device.Nmea0183.Sentences
{
    /// <summary>
    /// XTE sentence: Cross track error (one of the most important messages used to control autopilot)
    /// </summary>
    public class CrossTrackError : NmeaSentence
    {
        /// <summary>
        /// This sentence's id
        /// </summary>
        public static SentenceId Id => new SentenceId("XTE");
        private static bool Matches(SentenceId sentence) => Id == sentence;
        private static bool Matches(TalkerSentence sentence) => Matches(sentence.Id);

        /// <summary>
        /// Constructs a new MWV sentence
        /// </summary>
        public CrossTrackError(Length distance)
            : base(OwnTalkerId, Id, DateTime.UtcNow)
        {
            Distance = distance;
            Valid = true;
        }

        /// <summary>
        /// Internal constructor
        /// </summary>
        public CrossTrackError(TalkerSentence sentence, DateTime time)
            : this(sentence.TalkerId, Matches(sentence) ? sentence.Fields : throw new ArgumentException($"SentenceId does not match expected id '{Id}'"), time)
        {
        }

        /// <summary>
        /// Date and time message (ZDA). This should not normally need the last time as argument, because it defines it.
        /// </summary>
        public CrossTrackError(TalkerId talkerId, IEnumerable fields, DateTime time)
            : base(talkerId, Id, time)
        {
            IEnumerator field = fields.GetEnumerator();

            string status1 = ReadString(field);
            string status2 = ReadString(field);
            double distance = ReadValue(field);
            string direction = ReadString(field);
            string unit = ReadString(field);

            if (status1 == "A" && status2 == "A" && distance > 0 && (direction == "L" || direction == "R") && unit == "N")
            {
                Distance = Length.FromNauticalMiles(distance);
                if (direction == "R")
                {
                    Distance = Length.FromNauticalMiles(distance * -1);
                }

                Valid = true;
            }
            else
            {
                Distance = Length.Zero;
                Valid = false;
            }
        }

        /// <summary>
        /// Cross track distance. Positive if to the right of the track (meaning one shall steer left or to port)
        /// </summary>
        public Length Distance
        {
            get;
        }

        /// <summary>
        /// Presents this message as output
        /// </summary>
        public override string ToNmeaParameterList()
        {
            if (Valid)
            {
                if (Distance.Value >= 0)
                {
                    return $"A,A,{Distance.NauticalMiles.ToString("F3")},L,N,D";
                }
                else
                {
                    return $"A,A,{(-Distance.NauticalMiles).ToString("F3")},R,N,D";
                }
            }

            return string.Empty;
        }

        /// <inheritdoc />
        public override string ToReadableContent()
        {
            if (Valid)
            {
                if (Distance.Value >= 0)
                {
                    return $"The route is {Distance.NauticalMiles:F3} nm to the left";
                }
                else
                {
                    return $"The route is {Math.Abs(Distance.NauticalMiles):F3} nm to the right";
                }
            }

            return "No valid direction to route";
        }
    }
}
