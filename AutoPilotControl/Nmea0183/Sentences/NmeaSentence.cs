﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Device.Spi;
using System.Globalization;
using System.Text;

namespace Iot.Device.Nmea0183.Sentences
{
    /// <summary>
    /// Base class for Nmea Sentences.
    /// All sentences can be constructed using three different approaches:
    /// - A constructor taking a talker sentence and a time is used for automatic message construction by the parser or for manual decoding
    /// - A constructor taking the talker id and a field list is used as helper function for the parser.
    /// - A constructor taking individual values for the data is used to construct new messages to be sent out.
    /// If sending out messages, you might want to use the third constructor, it is usually the one with most arguments and not taking a talker sentence, as this
    /// is added automatically from the static field <see cref="OwnTalkerId"/>.
    /// </summary>
    public abstract class NmeaSentence
    {
        /// <summary>
        /// The default talker id of ourselves (applied when we send out messages)
        /// </summary>
        public static readonly TalkerId DefaultTalkerId = TalkerId.ElectronicChartDisplayAndInformationSystem;

        private static TalkerId _ownTalkerId = DefaultTalkerId;

        /// <summary>
        /// Our own talker ID (default when we send messages ourselves)
        /// </summary>
        public static TalkerId OwnTalkerId
        {
            get
            {
                return _ownTalkerId;
            }
            set
            {
                _ownTalkerId = value;
            }
        }

        /// <summary>
        /// Constructs an instance of this abstract class
        /// </summary>
        /// <param name="talker">The talker (sender) of this message</param>
        /// <param name="id">Sentence Id</param>
        /// <param name="time">Date/Time this message was valid (derived from last time message)</param>
        protected NmeaSentence(TalkerId talker, SentenceId id, DateTime time)
        {
            SentenceId = id;
            TalkerId = talker;
            DateTime = time;
        }

        /// <summary>
        /// The talker (sender) of this message
        /// </summary>
        public TalkerId TalkerId
        {
            get;
            init;
        }

        /// <summary>
        /// The sentence Id of this packet
        /// </summary>
        public SentenceId SentenceId
        {
            get;
        }

        /// <summary>
        /// The time tag on this message
        /// </summary>
        public DateTime DateTime
        {
            get;
            set;
        }

        /// <summary>
        /// True if the contents of this message are valid / understood
        /// This is false if the message type could be decoded, but the contents seem invalid or there's no useful data
        /// </summary>
        public bool Valid
        {
            get;
            protected set;
        }

        /// <summary>
        /// Age of this message
        /// </summary>
        public TimeSpan Age
        {
            get
            {
                if (!Valid)
                {
                    return TimeSpan.Zero;
                }

                return DateTime.UtcNow - DateTime;
            }
        }

        /// <summary>
        /// The relative age of this sentence against a time stamp.
        /// Useful when analyzing recorded data, where "now" should also be a time in the past.
        /// </summary>
        /// <param name="now">Time to compare against</param>
        /// <returns>The time difference</returns>
        public TimeSpan AgeTo(DateTime now)
        {
            if (!Valid)
            {
                return TimeSpan.Zero;
            }

            return now - DateTime;
        }

        /// <summary>
        /// Parses a date and a time field or any possible combinations of those
        /// </summary>
        protected static DateTime ParseDateTime(string date, string time)
        {
            DateTime d1;
            TimeSpan t1;

            if (time.Length != 0)
            {
                // DateTime.Parse often fails for no apparent reason
                int hour = int.Parse(time.Substring(0, 2));
                int minute = int.Parse(time.Substring(2, 2));
                int seconds = int.Parse(time.Substring(4, 2));
                double millis = double.Parse("0" + time.Substring(6)) * 1000;
                t1 = new TimeSpan(0, hour, minute, seconds, (int)millis);
            }
            else
            {
                t1 = new TimeSpan();
            }

            if (date.Length != 0)
            {
	            int day = int.Parse(date.Substring(0, 2));
	            int month = int.Parse(date.Substring(2, 2));
	            int year = int.Parse(date.Substring(4, 2)) + 2000;
	            d1 = new DateTime(year, month, day);
            }
            else
            {
                d1 = DateTime.UtcNow.Date;
            }

            return new DateTime(d1.Year, d1.Month, d1.Day, t1.Hours, t1.Minutes, t1.Seconds, t1.Milliseconds);
        }

        /// <summary>
        /// Parses a date and a time field or any possible combinations of those
        /// </summary>
        protected static DateTime ParseDateTime(DateTime lastSeenDate, string time)
        {
            DateTime dateTime;

            if (time.Length != 0)
            {
                int hour = int.Parse(time.Substring(0, 2));
                int minute = int.Parse(time.Substring(2, 2));
                int seconds = int.Parse(time.Substring(4, 2));
                double millis = double.Parse("0" + time.Substring(6)) * 1000;
                dateTime = new DateTime(lastSeenDate.Year, lastSeenDate.Month, lastSeenDate.Day,
                               hour, minute, seconds, (int)millis);
            }
            else
            {
                dateTime = lastSeenDate;
            }

            return dateTime;
        }

        /// <summary>
        /// Decodes the next field into a string
        /// </summary>
        protected string ReadString(IEnumerator field)
        {
            if (!field.MoveNext())
            {
                return string.Empty;
            }

            if (field.Current is string s)
            {
	            return s;
            }

            return string.Empty;
        }

        /// <summary>
        /// Decodes the next field into a char
        /// </summary>
        protected char ReadChar(IEnumerator field)
        {
            string val = ReadString(field);
            if (string.IsNullOrEmpty(val))
            {
                return '\0';
            }

            if (val.Length == 1)
            {
                return val[0];
            }
            else
            {
                return '\0'; // Probably also illegal
            }
        }

        /// <summary>
        /// Decodes the next field into a double
        /// </summary>
        protected double ReadValue(IEnumerator field)
        {
            string val = ReadString(field);
            if (string.IsNullOrEmpty(val))
            {
                return 0;
            }
            else
            {
                return double.Parse(val);
            }
        }

        protected double ReadValue(IEnumerator field, out bool valid)
        {
	        string val = ReadString(field);
	        if (string.IsNullOrEmpty(val))
	        {
		        valid = false;
		        return 0;
	        }
	        else
	        {
		        valid = true;
		        return double.Parse(val);
	        }
        }

		/// <summary>
		/// Decodes the next field into an int
		/// </summary>
		protected int ReadInt(IEnumerator field)
        {
            string val = ReadString(field);
            if (string.IsNullOrEmpty(val))
            {
                return 0;
            }
            else
            {
                return int.Parse(val);
            }
        }

        /// <summary>
        /// Translates the properties of this instance into an NMEA message body,
        /// without <see cref="TalkerId"/>, <see cref="SentenceId"/> and checksum.
        /// </summary>
        /// <returns>The NMEA sentence string for this message</returns>
        public abstract string ToNmeaParameterList();

        /// <summary>
        /// Gets an user-readable string about this message
        /// </summary>
        public abstract string ToReadableContent();

        /// <summary>
        /// Translates the properties of this instance into an NMEA message
        /// </summary>
        /// <returns>A complete NMEA message</returns>
        public virtual string ToNmeaMessage()
        {
            string start = TalkerId == TalkerId.Ais ? "!" : "$";
            string msg = $"{TalkerId}{SentenceId},{ToNmeaParameterList()}";
            byte checksum = TalkerSentence.CalculateChecksum(msg);
            return start + msg + "*" + checksum.ToString("X2");
        }

        /// <summary>
        /// Generates a readable instance of this string.
        /// Not overridable, use <see cref="ToReadableContent"/> to override.
        /// (this is to prevent confusion with <see cref="ToNmeaMessage"/>.)
        /// Do not use this method to create an NMEA sentence.
        /// </summary>
        /// <returns>An user-readable string representation of this message</returns>
        public sealed override string ToString()
        {
            return ToReadableContent();
        }

        public static string Join(string delimiter, IEnumerable fields)
        {
	        StringBuilder sb = new StringBuilder();
	        foreach (string s in fields)
	        {
		        sb.Append(s);
		        sb.Append(delimiter);
	        }

	        // Only if not empty
	        if (sb.Length > 0)
	        {
		        sb.Remove(sb.Length - delimiter.Length, delimiter.Length);
	        }

	        return sb.ToString();
        }
	}
}
