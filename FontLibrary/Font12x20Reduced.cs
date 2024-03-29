﻿using System;
using nanoFramework.UI;

namespace AutoPilotControl
{
	public sealed class Font12x20Reduced : IFont
	{
		private readonly byte[][] _font_12x20 =
		{
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x20
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x00, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x21
			new byte[] { 0x00, 0x00, 0x8C, 0x01, 0x8C, 0x01, 0x8C, 0x01, 0x8C, 0x01, 0x8C, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x22
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x20, 0x02, 0x20, 0x02, 0x10, 0x01, 0x10, 0x01, 0x10, 0x01, 0xFE, 0x0F, 0x88, 0x00, 0x88, 0x00, 0x88, 0x00, 0xFE, 0x07, 0x44, 0x00, 0x44, 0x00, 0x22, 0x00, 0x22, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x23
			new byte[] { 0x00, 0x00, 0x40, 0x00, 0xF0, 0x01, 0xF8, 0x03, 0x4C, 0x02, 0x4C, 0x00, 0x4C, 0x00, 0x78, 0x00, 0x70, 0x00, 0xC0, 0x01, 0xC0, 0x01, 0x40, 0x03, 0x40, 0x03, 0x44, 0x03, 0xFC, 0x01, 0xF8, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x24
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x1E, 0x08, 0x33, 0x04, 0x33, 0x02, 0x33, 0x01, 0xB3, 0x00, 0xB3, 0x00, 0x5E, 0x00, 0xA0, 0x07, 0xD0, 0x0C, 0xD0, 0x0C, 0xC8, 0x0C, 0xC4, 0x0C, 0xC2, 0x0C, 0x81, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x25
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0xF0, 0x00, 0xF8, 0x01, 0x98, 0x01, 0x98, 0x01, 0xD8, 0x00, 0x70, 0x00, 0x3C, 0x00, 0x66, 0x0C, 0xE3, 0x0C, 0xC3, 0x0C, 0x83, 0x07, 0x87, 0x07, 0xFE, 0x07, 0xFC, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x26
			new byte[] { 0x00, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x27
			new byte[] { 0x00, 0x00, 0x00, 0x03, 0xC0, 0x03, 0xE0, 0x00, 0x60, 0x00, 0x30, 0x00, 0x30, 0x00, 0x18, 0x00, 0x18, 0x00, 0x18, 0x00, 0x18, 0x00, 0x18, 0x00, 0x18, 0x00, 0x30, 0x00, 0x30, 0x00, 0x60, 0x00, 0xE0, 0x00, 0xC0, 0x03, 0x00, 0x03, 0x00, 0x00 }, // 0x28
			new byte[] { 0x00, 0x00, 0x0C, 0x00, 0x3C, 0x00, 0x70, 0x00, 0x60, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0xC0, 0x00, 0xC0, 0x00, 0x60, 0x00, 0x70, 0x00, 0x3C, 0x00, 0x0C, 0x00, 0x00, 0x00 }, // 0x29
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x30, 0x00, 0x36, 0x03, 0xCE, 0x03, 0x00, 0x00, 0xD8, 0x00, 0x9C, 0x01, 0x88, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x2A
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0xFE, 0x07, 0xFE, 0x07, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x2B
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70, 0x00, 0x70, 0x00, 0x70, 0x00, 0x60, 0x00, 0x20, 0x00, 0x30, 0x00, 0x00, 0x00 }, // 0x2C
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFC, 0x03, 0xFC, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x2D
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70, 0x00, 0x70, 0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x2E
			new byte[] { 0x00, 0x00, 0x00, 0x06, 0x00, 0x03, 0x00, 0x03, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0xC0, 0x00, 0xC0, 0x00, 0x60, 0x00, 0x60, 0x00, 0x30, 0x00, 0x30, 0x00, 0x18, 0x00, 0x18, 0x00, 0x18, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x06, 0x00, 0x00, 0x00 }, // 0x2F
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0xF0, 0x00, 0xF8, 0x01, 0x0C, 0x03, 0x0C, 0x03, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x0C, 0x03, 0x0C, 0x03, 0xF8, 0x01, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x30
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x60, 0x00, 0x7C, 0x00, 0x66, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0xFE, 0x07, 0xFE, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x31
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0xF8, 0x00, 0xFC, 0x01, 0x84, 0x03, 0x00, 0x03, 0x00, 0x03, 0x00, 0x03, 0x80, 0x01, 0xC0, 0x00, 0x60, 0x00, 0x30, 0x00, 0x18, 0x00, 0x0C, 0x00, 0xFC, 0x03, 0xFC, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x32
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0xF8, 0x00, 0xFC, 0x03, 0x04, 0x03, 0x00, 0x03, 0x80, 0x01, 0xF8, 0x00, 0xF8, 0x00, 0x80, 0x01, 0x00, 0x03, 0x00, 0x03, 0x00, 0x03, 0x84, 0x03, 0xFC, 0x01, 0xFC, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x33
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x80, 0x01, 0xC0, 0x01, 0xE0, 0x01, 0xA0, 0x01, 0x90, 0x01, 0x98, 0x01, 0x8C, 0x01, 0x84, 0x01, 0xFE, 0x07, 0xFE, 0x07, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x34
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0xF8, 0x03, 0xF8, 0x03, 0x18, 0x00, 0x18, 0x00, 0x18, 0x00, 0x18, 0x00, 0xF8, 0x00, 0xF8, 0x01, 0x80, 0x03, 0x00, 0x03, 0x00, 0x03, 0x80, 0x03, 0xF8, 0x01, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x35
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0xF0, 0x00, 0xF8, 0x01, 0x1C, 0x01, 0x0C, 0x00, 0x06, 0x00, 0xE6, 0x00, 0xF6, 0x01, 0x8E, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x8C, 0x03, 0xFC, 0x01, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x36
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFC, 0x07, 0xFC, 0x07, 0x00, 0x06, 0x00, 0x03, 0x00, 0x01, 0x80, 0x01, 0xC0, 0x00, 0x40, 0x00, 0x60, 0x00, 0x20, 0x00, 0x30, 0x00, 0x30, 0x00, 0x18, 0x00, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x37
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0xF0, 0x00, 0xFC, 0x01, 0x8C, 0x01, 0x8C, 0x01, 0x9C, 0x01, 0xF8, 0x00, 0x70, 0x00, 0xEC, 0x01, 0x86, 0x03, 0x06, 0x03, 0x06, 0x03, 0x8E, 0x03, 0xFC, 0x01, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x38
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x78, 0x00, 0xFC, 0x01, 0x8E, 0x01, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x8E, 0x03, 0x7C, 0x03, 0x38, 0x03, 0x00, 0x03, 0x80, 0x01, 0xC4, 0x01, 0xFC, 0x00, 0x78, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x39
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70, 0x00, 0x70, 0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70, 0x00, 0x70, 0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x3A
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70, 0x00, 0x70, 0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70, 0x00, 0x70, 0x00, 0x70, 0x00, 0x60, 0x00, 0x20, 0x00, 0x30, 0x00, 0x00, 0x00 }, // 0x3B
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x07, 0xC0, 0x03, 0xE0, 0x00, 0x38, 0x00, 0x0E, 0x00, 0x38, 0x00, 0xE0, 0x00, 0xC0, 0x03, 0x00, 0x07, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x3C
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFE, 0x07, 0xFE, 0x07, 0x00, 0x00, 0x00, 0x00, 0xFE, 0x07, 0xFE, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x3D
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x0E, 0x00, 0x3C, 0x00, 0x70, 0x00, 0xC0, 0x01, 0x00, 0x07, 0xC0, 0x01, 0x70, 0x00, 0x3C, 0x00, 0x0E, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x3E
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFE, 0x00, 0xFE, 0x03, 0x82, 0x03, 0x00, 0x03, 0x00, 0x03, 0x80, 0x01, 0xC0, 0x00, 0x60, 0x00, 0x30, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x3F
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0xF0, 0x01, 0x18, 0x03, 0x0C, 0x06, 0xC6, 0x07, 0x63, 0x06, 0x33, 0x06, 0x33, 0x06, 0x33, 0x07, 0x33, 0x07, 0xF3, 0x06, 0x66, 0x0E, 0x06, 0x00, 0x0C, 0x00, 0xF0, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x40
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x00, 0xF0, 0x00, 0xF0, 0x00, 0xD0, 0x00, 0x98, 0x01, 0x98, 0x01, 0x8C, 0x03, 0x0C, 0x03, 0xFC, 0x03, 0xFE, 0x07, 0x06, 0x06, 0x06, 0x06, 0x03, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x41
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFE, 0x00, 0xFE, 0x01, 0x86, 0x01, 0x86, 0x01, 0xC6, 0x00, 0x7E, 0x00, 0xFE, 0x00, 0x86, 0x01, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0xFE, 0x01, 0xFE, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x42
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE0, 0x03, 0xF8, 0x07, 0x1C, 0x04, 0x0C, 0x00, 0x06, 0x00, 0x06, 0x00, 0x06, 0x00, 0x06, 0x00, 0x06, 0x00, 0x0C, 0x00, 0x3C, 0x04, 0xF8, 0x07, 0xE0, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x43
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFE, 0x00, 0xFE, 0x01, 0x86, 0x03, 0x06, 0x07, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x03, 0x86, 0x03, 0xFE, 0x01, 0xFE, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x44
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFC, 0x07, 0xFC, 0x07, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0xFC, 0x03, 0xFC, 0x03, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0xFC, 0x07, 0xFC, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x45
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFC, 0x07, 0xFC, 0x07, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0xFC, 0x03, 0xFC, 0x03, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x46
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE0, 0x03, 0xF8, 0x07, 0x1C, 0x04, 0x0C, 0x00, 0x06, 0x00, 0x06, 0x00, 0x86, 0x07, 0x86, 0x07, 0x06, 0x06, 0x0C, 0x06, 0x1C, 0x06, 0xF8, 0x07, 0xE0, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x47
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0xFE, 0x03, 0xFE, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x48
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFE, 0x01, 0xFE, 0x01, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0xFE, 0x01, 0xFE, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x49
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x01, 0xF8, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0xC0, 0x01, 0xFC, 0x00, 0x7C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x4A
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x07, 0x86, 0x03, 0xC6, 0x01, 0xE6, 0x00, 0x66, 0x00, 0x36, 0x00, 0x3E, 0x00, 0x76, 0x00, 0xE6, 0x00, 0xC6, 0x01, 0x86, 0x03, 0x06, 0x07, 0x06, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x4B
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0xFC, 0x07, 0xFC, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x4C
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x07, 0x07, 0x07, 0x8F, 0x07, 0x8B, 0x06, 0x8B, 0x06, 0xDB, 0x06, 0x53, 0x06, 0x53, 0x06, 0x73, 0x06, 0x23, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x4D
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x06, 0x0E, 0x06, 0x1E, 0x06, 0x1E, 0x06, 0x36, 0x06, 0x76, 0x06, 0x66, 0x06, 0xE6, 0x06, 0xC6, 0x06, 0x86, 0x07, 0x86, 0x07, 0x06, 0x07, 0x06, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x4E
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x00, 0xFC, 0x01, 0x8E, 0x03, 0x07, 0x07, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x07, 0x07, 0x8E, 0x03, 0xFC, 0x01, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x4F
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFC, 0x01, 0xFC, 0x03, 0x0C, 0x07, 0x0C, 0x06, 0x0C, 0x06, 0x0C, 0x07, 0xFC, 0x03, 0xFC, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x50
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x00, 0xFC, 0x01, 0x8E, 0x03, 0x07, 0x07, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x06, 0x03, 0x8E, 0x03, 0xFC, 0x01, 0xF8, 0x00, 0x80, 0x03, 0x00, 0x0F, 0x00, 0x04, 0x00, 0x00 }, // 0x51
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7E, 0x00, 0xFE, 0x01, 0x86, 0x01, 0x86, 0x01, 0x86, 0x01, 0xC6, 0x01, 0xFE, 0x00, 0x7E, 0x00, 0xE6, 0x00, 0xC6, 0x01, 0x86, 0x03, 0x06, 0x07, 0x06, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x52
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x00, 0xFC, 0x01, 0x06, 0x01, 0x06, 0x00, 0x0E, 0x00, 0x3C, 0x00, 0xF8, 0x00, 0xC0, 0x03, 0x00, 0x03, 0x00, 0x03, 0x86, 0x03, 0xFE, 0x01, 0xFC, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x53
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x0F, 0xFF, 0x0F, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x54
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x8E, 0x03, 0xFC, 0x01, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x55
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x0C, 0x06, 0x06, 0x06, 0x06, 0x0E, 0x06, 0x0C, 0x03, 0x0C, 0x03, 0x1C, 0x03, 0x98, 0x01, 0xB8, 0x01, 0xB0, 0x00, 0xF0, 0x00, 0xF0, 0x00, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x56
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x0C, 0x03, 0x0C, 0x03, 0x0C, 0x62, 0x04, 0x62, 0x04, 0xE2, 0x06, 0xF6, 0x06, 0x96, 0x06, 0x96, 0x06, 0x96, 0x03, 0x9C, 0x03, 0x9C, 0x03, 0x0C, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x57
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x0E, 0x0E, 0x06, 0x0C, 0x03, 0x98, 0x01, 0xF8, 0x00, 0xF0, 0x00, 0x60, 0x00, 0xF0, 0x00, 0xD8, 0x01, 0x98, 0x01, 0x0C, 0x03, 0x06, 0x07, 0x03, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x58
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x0C, 0x06, 0x06, 0x0C, 0x03, 0x1C, 0x03, 0x98, 0x01, 0xF0, 0x00, 0xF0, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x59
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFE, 0x07, 0xFE, 0x07, 0x00, 0x06, 0x00, 0x03, 0x80, 0x01, 0xC0, 0x00, 0x60, 0x00, 0x30, 0x00, 0x18, 0x00, 0x0C, 0x00, 0x06, 0x00, 0xFE, 0x07, 0xFE, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x5A
			new byte[] { 0x00, 0x00, 0xF0, 0x03, 0xF0, 0x03, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0xF0, 0x03, 0xF0, 0x03, 0x00, 0x00 }, // 0x5B
			new byte[] { 0x00, 0x00, 0x06, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x18, 0x00, 0x18, 0x00, 0x18, 0x00, 0x30, 0x00, 0x30, 0x00, 0x60, 0x00, 0x60, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x00, 0x03, 0x00, 0x03, 0x00, 0x06, 0x00, 0x00 }, // 0x5C
			new byte[] { 0x00, 0x00, 0xFC, 0x00, 0xFC, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xFC, 0x00, 0xFC, 0x00, 0x00, 0x00 }, // 0x5D
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x40, 0x00, 0xE0, 0x00, 0xA0, 0x00, 0xB0, 0x00, 0xB0, 0x01, 0x18, 0x01, 0x18, 0x03, 0x0C, 0x03, 0x0C, 0x02, 0x06, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x5E
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x0F, 0xFF, 0x0F, 0x00, 0x00, 0x00, 0x00 }, // 0x5F
			new byte[] { 0x60, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x60
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x01, 0xFC, 0x03, 0x04, 0x03, 0x00, 0x03, 0x00, 0x03, 0xF8, 0x03, 0x0C, 0x03, 0x06, 0x03, 0x86, 0x03, 0xFE, 0x0F, 0x7C, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x61
			new byte[] { 0x00, 0x00, 0x06, 0x00, 0x06, 0x00, 0x06, 0x00, 0x06, 0x00, 0xE6, 0x00, 0xF6, 0x01, 0x9E, 0x03, 0x0E, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x8E, 0x01, 0xFE, 0x01, 0xF6, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x62
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x01, 0xFC, 0x03, 0x1C, 0x02, 0x0E, 0x00, 0x06, 0x00, 0x06, 0x00, 0x06, 0x00, 0x0E, 0x00, 0x1C, 0x00, 0xFC, 0x03, 0xF0, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x63
			new byte[] { 0x00, 0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x03, 0x00, 0x03, 0x78, 0x03, 0xFC, 0x03, 0x8C, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x8E, 0x03, 0xFC, 0x03, 0x38, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x64
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x00, 0xFC, 0x01, 0x8C, 0x03, 0x06, 0x03, 0xFE, 0x03, 0xFE, 0x03, 0x06, 0x00, 0x06, 0x00, 0x0C, 0x02, 0xFC, 0x03, 0xF0, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x65
			new byte[] { 0x00, 0x00, 0xE0, 0x07, 0xF0, 0x07, 0x30, 0x00, 0x30, 0x00, 0xFE, 0x07, 0xFE, 0x07, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x66
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x78, 0x03, 0xFC, 0x03, 0x8C, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x8E, 0x03, 0xFC, 0x03, 0x78, 0x03, 0x00, 0x03, 0x84, 0x03, 0xFC, 0x01, 0xF8, 0x00 }, // 0x67
			new byte[] { 0x00, 0x00, 0x06, 0x00, 0x06, 0x00, 0x06, 0x00, 0x06, 0x00, 0xE6, 0x01, 0xF6, 0x03, 0x1E, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x68
			new byte[] { 0x00, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFC, 0x00, 0xFC, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x69
			new byte[] { 0x00, 0x00, 0x80, 0x01, 0x80, 0x01, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x01, 0xF8, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0x80, 0x01, 0xC0, 0x01, 0xFC, 0x00, 0x7C, 0x00 }, // 0x6A
			new byte[] { 0x00, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x07, 0x8C, 0x03, 0xCC, 0x01, 0xEC, 0x00, 0x6C, 0x00, 0x7C, 0x00, 0xEC, 0x00, 0xCC, 0x01, 0x8C, 0x03, 0x0C, 0x07, 0x0C, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x6B
			new byte[] { 0x00, 0x00, 0xFC, 0x00, 0xFC, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x6C
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9B, 0x03, 0xFF, 0x07, 0x77, 0x06, 0x33, 0x06, 0x33, 0x06, 0x33, 0x06, 0x33, 0x06, 0x33, 0x06, 0x33, 0x06, 0x33, 0x06, 0x33, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x6D
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE6, 0x01, 0xF6, 0x03, 0x1E, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x6E
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x00, 0xFC, 0x03, 0x0C, 0x03, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x0C, 0x03, 0xFC, 0x03, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x6F
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF6, 0x00, 0xFE, 0x01, 0x8E, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x8E, 0x01, 0xFE, 0x01, 0xF6, 0x00, 0x06, 0x00, 0x06, 0x00, 0x06, 0x00, 0x06, 0x00 }, // 0x70
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x78, 0x03, 0xFC, 0x03, 0x8C, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x8E, 0x03, 0x7C, 0x03, 0x38, 0x03, 0x00, 0x03, 0x00, 0x03, 0x00, 0x03, 0x00, 0x03 }, // 0x71
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xCC, 0x03, 0xEC, 0x03, 0x3C, 0x02, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x72
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x01, 0xFC, 0x01, 0x0C, 0x00, 0x0C, 0x00, 0x3C, 0x00, 0xF0, 0x01, 0x80, 0x03, 0x00, 0x03, 0x04, 0x03, 0xFC, 0x01, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x73
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x30, 0x00, 0xFE, 0x07, 0xFE, 0x07, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0xF0, 0x07, 0xE0, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x74
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0x06, 0x03, 0xC6, 0x03, 0x7E, 0x03, 0x3C, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x75
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x06, 0x0C, 0x02, 0x0C, 0x03, 0x0C, 0x03, 0x18, 0x01, 0x98, 0x01, 0x98, 0x01, 0xB0, 0x00, 0xF0, 0x00, 0xF0, 0x00, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x76
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x0C, 0x63, 0x0C, 0x63, 0x0C, 0xE2, 0x04, 0xF6, 0x04, 0x96, 0x04, 0x96, 0x06, 0x96, 0x07, 0x9C, 0x03, 0x0C, 0x03, 0x0C, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x77
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x07, 0x0C, 0x03, 0x98, 0x01, 0xB8, 0x00, 0xF0, 0x00, 0x60, 0x00, 0xF0, 0x00, 0xD8, 0x01, 0x98, 0x01, 0x0C, 0x03, 0x06, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x78
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x06, 0x0C, 0x02, 0x0C, 0x03, 0x1C, 0x03, 0x98, 0x01, 0x98, 0x01, 0xB0, 0x00, 0xF0, 0x00, 0xF0, 0x00, 0x60, 0x00, 0x60, 0x00, 0x20, 0x00, 0x30, 0x00, 0x3C, 0x00, 0x1C, 0x00 }, // 0x79
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFE, 0x03, 0xFE, 0x03, 0x00, 0x03, 0x80, 0x01, 0xC0, 0x00, 0x60, 0x00, 0x30, 0x00, 0x18, 0x00, 0x0C, 0x00, 0xFE, 0x03, 0xFE, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x7A
			new byte[] { 0x00, 0x00, 0xC0, 0x03, 0xE0, 0x03, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x3C, 0x00, 0x3C, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0xE0, 0x03, 0xC0, 0x03, 0x00, 0x00 }, // 0x7B
			new byte[] { 0x00, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x00, 0x00 }, // 0x7C
			new byte[] { 0x00, 0x00, 0x3C, 0x00, 0x7C, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0xC0, 0x03, 0xC0, 0x03, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x60, 0x00, 0x7C, 0x00, 0x3C, 0x00, 0x00, 0x00 }, // 0x7D
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3C, 0x04, 0xFE, 0x07, 0xC2, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x7E
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFC, 0x03, 0x04, 0x02, 0x04, 0x02, 0x04, 0x02, 0x04, 0x02, 0x04, 0x02, 0x04, 0x02, 0x04, 0x02, 0x04, 0x02, 0x04, 0x02, 0x04, 0x02, 0x04, 0x02, 0xFC, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0x7F
			new byte[] { 0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x48, 0x00, 0x48, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // 0xF8, °
		};

		/// <inheritdoc/>
		public override byte Width
		{
			get
			{
				return 12;
			}
		}

		/// <inheritdoc/>
		public override byte Height
		{
			get
			{
				return 20;
			}
		}

		public override byte[] this[char character]
		{
			get
			{
				char c = character switch
				{
					> ' ' and < '\u007f' => character,
					'°' => '\u0080', // the ° character
					_ => ' ',
				};

				return _font_12x20[c - 0x20];
			}
		}
	}
}
