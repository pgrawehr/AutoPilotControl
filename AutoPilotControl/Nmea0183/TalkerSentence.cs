// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Globalization;
using System.Text;
using Iot.Device.Nmea0183.Sentences;

namespace Iot.Device.Nmea0183
{
    /// <summary>
    /// Represents NMEA0183 talker sentence
    /// </summary>
    public class TalkerSentence
    {
        private IEnumerable _fields;

        /// <summary>
        /// NMEA0183 talker identifier (identifier of the sender)
        /// </summary>
        public TalkerId TalkerId { get; }

        /// <summary>
        /// NMEA0183 sentence identifier
        /// </summary>
        public SentenceId Id { get; private set; }

        /// <summary>
        /// Fields of the NMEA0183 sentence
        /// </summary>
        public IEnumerable Fields => _fields;

        /// <inheritdoc/>
        public override string ToString()
        {
            string mainPart = string.Format("{0}{1},{2}", TalkerId, Id, NmeaSentence.Join(",", Fields));
            byte checksum = CalculateChecksum(mainPart);
            if (TalkerId == TalkerId.Ais)
            {
                return string.Format("!{0}*{1:X2}", mainPart, checksum);
            }

            return string.Format("${0}*{1:X2}", mainPart, checksum);
        }

        /// <summary>
        /// Constructs NMEA0183 talker identifier
        /// </summary>
        /// <param name="talkerId">NMEA0183 talker identifier of the device sending the sentence</param>
        /// <param name="sentenceId">NMEA0183 sentence identifier</param>
        /// <param name="fields">fields related to the sentence</param>
        public TalkerSentence(TalkerId talkerId, SentenceId sentenceId, IEnumerable fields)
        {
            TalkerId = talkerId;
            Id = sentenceId;
            _fields = fields;
        }

        /// <summary>
        /// Constructs a message from a typed sentence
        /// </summary>
        /// <param name="sentence">Sentence to send. It must be valid</param>
        public TalkerSentence(NmeaSentence sentence)
        {
            TalkerId = sentence.TalkerId;
            Id = sentence.SentenceId;
            var content = sentence.ToNmeaParameterList();
            if (string.IsNullOrEmpty(content) || sentence.Valid == false)
            {
                throw new InvalidOperationException("Input sentence not valid or cannot be encoded");
            }

            _fields = content.Split(new char[] { ',' });
        }

        /// <summary>
        /// Reads NMEA0183 talker sentence from provided string
        /// </summary>
        /// <param name="sentence">NMEA0183 talker sentence</param>
        /// <param name="errorCode">Returns an error code, if the parsing failed</param>
        /// <returns>TalkerSentence instance, or null in case of an error</returns>
        /// <remarks><paramref name="sentence"/> does not include new line characters</remarks>
        public static TalkerSentence FromSentenceString(string sentence, out NmeaError errorCode)
        {
            return FromSentenceString(sentence, TalkerId.Any, out errorCode);
        }

        /// <summary>
        /// Reads NMEA0183 talker sentence from provided string
        /// </summary>
        /// <param name="sentence">NMEA0183 talker sentence</param>
        /// <param name="expectedTalkerId">If this is not TalkerId.Any, only messages with this talker id are parsed,
        /// all others are ignored. This reduces workload if a source acts as repeater, but the repeated messages are not needed.</param>
        /// <param name="errorCode">Returns an error code, if the parsing failed</param>
        /// <returns>TalkerSentence instance, or null in case of an error</returns>
        /// <remarks><paramref name="sentence"/> does not include new line characters</remarks>
        public static TalkerSentence FromSentenceString(string sentence, TalkerId expectedTalkerId, out NmeaError errorCode)
        {
            // $XXY, ...
            const int sentenceHeaderMinLength = 4;

            // http://www.tronico.fi/OH6NT/docs/NMEA0183.pdf page 2
            // defines this as 80 + 1 (for $), but we don't really care if it is something within a reasonable limit.
            const int MaxSentenceLength = 256;

            if (sentence == null)
            {
                throw new ArgumentNullException(nameof(sentence));
            }

            int idx = sentence.IndexOfAny(new char[] { '!', '$' });
            if (idx < 0 || idx > 6)
            {
                // Valid sentences start with $ or ! (for the AIS sentences)
                errorCode = NmeaError.NoSyncByte;
                return null;
            }

            if (idx != 0)
            {
                // When the index of the $ is larger than 0 but less than a small amount, try to decode the remainder of the line
                sentence = sentence.Substring(idx);
            }

            if (sentence.Length < sentenceHeaderMinLength)
            {
                errorCode = NmeaError.MessageToShort;
                return null;
            }

            if (sentence.Length > MaxSentenceLength)
            {
                errorCode = NmeaError.MessageToLong;
                return null;
            }

            TalkerId talkerId = new TalkerId(sentence[1], sentence[2]);
            if (expectedTalkerId != TalkerId.Any && expectedTalkerId != talkerId)
            {
                errorCode = NmeaError.None;
                return null;
            }

            int firstComma = sentence.IndexOf(',', 1);
            if (firstComma == -1)
            {
                errorCode = NmeaError.MessageToShort;
                return null;
            }

            string sentenceIdString = sentence.Substring(3, firstComma - 3);

            SentenceId sentenceId = new SentenceId(sentenceIdString);

            string[] fields = sentence.Substring(firstComma + 1).Split(',');
            int lastFieldIdx = fields.Length - 1;
            // This returns -1 as the checksum if there was none, or a very big number if the checksum couldn't be parsed
            int checksum = GetChecksumAndLastField(fields[lastFieldIdx], out string lastField);
            fields[lastFieldIdx] = lastField;

            TalkerSentence result = new TalkerSentence(talkerId, sentenceId, fields);

            if (checksum != -1)
            {
                byte realChecksum = CalculateChecksumFromSentenceString(sentence);

                if (realChecksum != checksum)
                {
                    errorCode = NmeaError.InvalidChecksum;
                    return null;
                }
            }

            errorCode = NmeaError.None;
            return result;
        }

        private static int GetChecksumAndLastField(string lastEntry, out string lastField)
        {
            int lastStarIdx = lastEntry.LastIndexOf('*');

            if (lastStarIdx == -1 || lastStarIdx != lastEntry.Length - 3)
            {
                // there is no checksum, last entry is the last field
                lastField = lastEntry;
                return -1;
            }

            int lastIdx = lastEntry.Length - 1;
            int sum = HexDigitToDecimal(lastEntry[lastIdx - 1]) * 16 + HexDigitToDecimal(lastEntry[lastIdx]);
            lastField = lastEntry.Substring(0, lastStarIdx);

            return sum;
        }

        private static int HexDigitToDecimal(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return (int)(c - '0');
            }

            if (c >= 'a' && c <= 'f')
            {
                return 10 + (int)(c - 'a');
            }

            if (c >= 'A' && c <= 'F')
            {
                return 10 + (int)(c - 'A');
            }

            return 0xFFFF; // Will later fail with "invalid checksum"
        }

        private static byte CalculateChecksumFromSentenceString(string sentenceString)
        {
            // remove leading $ (1 char) and checksum (3 chars)
            var checksumChars = sentenceString.Substring(1, sentenceString.Length - 4);
            return CalculateChecksum(checksumChars);
        }

        public static byte CalculateChecksum(string checksumChars)
        {
            byte ret = 0;

            for (int i = 0; i < checksumChars.Length; i++)
            {
                char c = checksumChars[i];

                // Non ascii-characters (>=128) are rare, but seem to be allowed.
                if (c >= 256)
                {
                    // this should generally not be possible but checking for sanity
                    c = '\0'; // will likely result in a checksum mismatch
                }

                ret ^= (byte)c;
            }

            return ret;
        }

        private NmeaSentence ConstructSentence(DateTime dateTime)
        {
	        if (Id == RecommendedMinimumNavigationInformation.Id)
	        {
		        return new RecommendedMinimumNavigationInformation(this, dateTime);
	        }

	        return null;
        }

		/// <summary>
		/// Compares sentence identifier with all known identifiers.
		/// If found returns typed object corresponding to the identifier.
		/// If not found returns a raw sentence instead. Also returns a raw sentence on a parser error (e.g. invalid date/time field)
		/// </summary>
		/// <param name="lastMessageTime">The date/time the last packet was seen. Used to time-tag packets that do not provide
		/// their own time or only a time but not a date</param>
		/// <returns>Object corresponding to the identifier</returns>
		public NmeaSentence TryGetTypedValue(ref DateTime lastMessageTime)
		{
			NmeaSentence retVal = ConstructSentence(lastMessageTime);

            if (retVal == null)
            {
                retVal = new RawSentence(TalkerId, Id, Fields, lastMessageTime);
            }

            if (retVal.Valid && retVal.DateTime != DateTime.MinValue)
            {
                lastMessageTime = retVal.DateTime;
            }

            return retVal;
        }

        /// <summary>
        /// Returns this sentence without parsing its contents
        /// </summary>
        /// <returns>A raw sentence</returns>
        public RawSentence GetAsRawSentence(ref DateTime lastMessageTime)
        {
            return new RawSentence(TalkerId, Id, Fields, lastMessageTime);
        }
    }
}
