using System;
using System.Linq;
using CorrectionCodes.Core;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using static System.Console;

namespace Tests
{
	[TestFixture]
	public class CrcTests
	{
		[TestCase("abcs")]
		[TestCase("11aaaee09")]
		[TestCase("x")]
		[TestCase("ASsawb4tuwehi&jbweu-*&5$#@itjee")]
		public void Crc16_CrcWithPayloadComputesToZero(string input)
		{
			var bytes = input.ToBytes();
			var crc = Crc16.ComputeChecksum(bytes);

			var payload = bytes.ConcatArray(BitConverter.GetBytes(crc));
			var secondCrc = Crc16.ComputeChecksum(payload);

			secondCrc.Should().Be(0);
		}

		[TestCase("abcs")]
		[TestCase("11aaaee09")]
		[TestCase("x")]
		[TestCase("ASsawb4tuwehi&jbweu-*&5$#@itjee")]
		public void Crc16_CrcWithModifiedPayloadComputesToNotZero(string input)
		{
			var bytes = input.ToBytes();
			var crc   = Crc16.ComputeChecksum(bytes);

			bytes[0] ^= 0x81;
			var payload   = bytes.ConcatArray(BitConverter.GetBytes(crc));
			var secondCrc = Crc16.ComputeChecksum(payload);

			secondCrc.Should().NotBe(0);
		}

		[TestCase("abcs")]
		[TestCase("11aaaee09")]
		[TestCase("x")]
		[TestCase("ASsawb4tuwehi&jbweu-*&5$#@itjee")]
		public void Crc32_CrcWithPayloadComputesToZero(string input)
		{
			var bytes = input.ToBytes();
			var crc   = Crc32.ComputeChecksum(bytes);

			var payload   = bytes.ConcatArray(BitConverter.GetBytes(crc));
			var secondCrc = Crc32.ComputeChecksum(payload);

			secondCrc.Should().Be(0);
		}

		[TestCase("abcs")]
		[TestCase("11aaaee09")]
		[TestCase("x")]
		[TestCase("ASsawb4tuwehi&jbweu-*&5$#@itjee")]
		public void Crc32_CrcWithModifiedPayloadComputesToNotZero(string input)
		{
			var bytes = input.ToBytes();
			var crc   = Crc32.ComputeChecksum(bytes);
			
			bytes[0] ^= 0x14;
			var payload   = bytes.ConcatArray(BitConverter.GetBytes(crc));
			var secondCrc = Crc32.ComputeChecksum(payload);

			secondCrc.Should().NotBe(0);
		}

		[TestCase("abcs")]
		[TestCase("11aaaee09")]
		[TestCase("x")]
		[TestCase("ASsawb4tuwehi&jbweu-*&5$#@itjee")]
		public void Crc16Ccitt_CrcWithPayloadComputesToZero(string input)
		{
			var bytes = input.ToBytes();
			var crc   = Crc16Sdlc.ComputeChecksum(bytes);
			//var crcBtes = new byte[] { (byte)(crc >> 8), (byte)(crc & 0xff) };
			//Console.WriteLine(crcBtes.Select(b => b.ToString("X")).Aggregate((a, b) => a + b));
			//Console.WriteLine(BitConverter.GetBytes(crc).Select(b => b.ToString("X")).Aggregate((a, b) => a + b));
			var payload   = bytes.ConcatArray(BitConverter.GetBytes(crc));
			var secondCrc = Crc16Sdlc.ComputeChecksum(payload);

			WriteLine($"{crc:X} {secondCrc:X}");
			secondCrc.Should().Be(0);
		}

		[TestCase("abcs")]
		[TestCase("11aaaee09")]
		[TestCase("x")]
		[TestCase("ASsawb4tuwehi&jbweu-*&5$#@itjee")]
		public void SdlcReverse_CrcWithPayloadComputesToZero(string input)
		{
			var bytes = input.ToBytes();
			var crc   = SdlcReverse.ComputeChecksum(bytes);

			var payload   = bytes.ConcatArray(BitConverter.GetBytes(crc));
			var secondCrc = SdlcReverse.ComputeChecksum(payload);

			secondCrc.Should().Be(0);
		}

		[TestCase("abcs")]
		[TestCase("11aaaee09")]
		[TestCase("x")]
		[TestCase("ASsawb4tuwehi&jbweu-*&5$#@itjee")]
		public void SdlcReverse_CrcWithModifiedPayloadComputesToNotZero(string input)
		{
			var bytes = input.ToBytes();
			var crc   = SdlcReverse.ComputeChecksum(bytes);

			bytes[0] ^= 0x81;
			var payload   = bytes.ConcatArray(BitConverter.GetBytes(crc));
			var secondCrc = SdlcReverse.ComputeChecksum(payload);

			secondCrc.Should().NotBe(0);
		}

		[Test]
		public void HammingCodes_IfMyGroupIsBunchOfRetardThenYesTheyHaveADownSyndrome()
		{
			/*
			int n = 1;
			int take = n - 1;
			WriteLine(string.Join(',', Enumerable.Range(0, 20).Where(i =>
			{
				if (i >= take && i < take + n)
					return true;
				if (i == take + n)
					take += n * 2;
				return false;
			}).Select(p =>  (p).ToString())));
			n = 2;
			take = n - 1;
			WriteLine(string.Join(',', Enumerable.Range(0, 20).Where(i =>
			{
				if (i >= take && i < take + n)
					return true;
				if (i == take + n)
					take += n * 2;
				return false;
			}).Select(p =>  (p).ToString())));
			n    = 4;
			take = n - 1;
			WriteLine(string.Join(',', Enumerable.Range(0, 20).Where(i =>
			{
				if (i >= take && i < take + n)
					return true;
				if (i == take + n)
					take += n * 2;
				return false;
			}).Select(p =>  (p).ToString())));
			n    = 8;
			take = n - 1;
			WriteLine(string.Join(',', Enumerable.Range(0, 20).Where(i =>
			{
				if (i >= take && i < take + n)
					return true;
				if (i == take + n)
					take += n * 2;
				return false;
			}).Select(p =>  (p).ToString())));
			n    = 16;
			take = n - 1;
			WriteLine(string.Join(',', Enumerable.Range(0, 20).Where(i  =>
			{
				if (i >= take && i < take + n)
					return true;
				if (i == take + n)
					take += n * 2;
				return false;
			}).Select(p => (p).ToString())));
			*/

			var bits = new byte[] { 1,0,0,1,1,0,1,0 };
			var transmitted = HammingCodes.Encode(bits);

			WriteLine($"{bits.Length} {transmitted.Length}");

			//transmitted[5] ^= 1;
			WriteLine(string.Join(",", transmitted));
			WriteLine(HammingCodes.ErrorSyndrome(transmitted));

			var decoded = HammingCodes.Decode(transmitted, bits.Length);
			WriteLine(bits.SequenceEqual(decoded));

			Program.Main(null);

		}
	}
}
