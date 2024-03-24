// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;

namespace Iot.Device.Nmea0183.Sentences
{
    /// <summary>
    /// This sentence type is used either if no better matching type is known or as placeholder for whole messages.
    /// This allows forwarding of messages even if we don't need/understand them.
    /// </summary>
    public class RawSentence : NmeaSentence
    {
        private string[] _fields;

        /// <summary>
        /// Creates an unknown sentence from a split of parameters
        /// </summary>
        public RawSentence(TalkerId talkerId, SentenceId id, IEnumerable fields, DateTime time)
            : base(talkerId, id, time)
        {
	        int cnt = 0;
	        var e = fields.GetEnumerator();
	        while (e.MoveNext())
	        {
		        cnt++;
	        }
            e.Reset();
            _fields = new string[cnt];
            cnt = 0;
            while (e.MoveNext())
            {
	            _fields[cnt++] = (string)e.Current;
            }

            Valid = true;
        }

        internal string[] Fields => _fields;

        /// <summary>
        /// Returns the formatted payload
        /// </summary>
        public override string ToNmeaParameterList()
        {
            return NmeaSentence.Join(",", _fields);
        }

        /// <inheritdoc />
        public override string ToReadableContent()
        {
            return $"${TalkerId}{SentenceId},{NmeaSentence.Join(",", _fields)}"; // Cannot do much else here
        }
    }
}
