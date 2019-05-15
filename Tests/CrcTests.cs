using System;
using System.Linq;
using CorrectionCodes.Core;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Tests
{
	[TestFixture]
	public class CrcTests
	{
		[TestCase("abcs")]
		[TestCase("11aaaee09")]
		[TestCase("x")]
		public void Crc16_CrcWithPayloadComputesToZero(string input)
		{
			var bytes = input.ToBytes();
			var crc = Crc16.ComputeChecksum(bytes);

			var payload = bytes.Concat(BitConverter.GetBytes(crc)).ToArray();
			var secondCrc = Crc16.ComputeChecksum(payload);

			secondCrc.Should().Be(0);
		}

		[TestCase("abcs")]
		[TestCase("11aaaee09")]
		[TestCase("x")]
		public void Crc16_CrcWithModifiedPayloadComputesToNotZero(string input)
		{
			var bytes = input.ToBytes();
			var crc   = Crc16.ComputeChecksum(bytes);

			bytes[0] ^= 0x81;
			var payload   = bytes.Concat(BitConverter.GetBytes(crc)).ToArray();
			var secondCrc = Crc16.ComputeChecksum(payload);

			secondCrc.Should().NotBe(0);
		}

		[TestCase("abcs")]
		[TestCase("11aaaee09")]
		[TestCase("x")]
		public void Crc32_CrcWithPayloadComputesToZero(string input)
		{
			var bytes = input.ToBytes();
			var crc   = Crc32.ComputeChecksum(bytes);

			var payload   = bytes.Concat(BitConverter.GetBytes(crc)).ToArray();
			var secondCrc = Crc32.ComputeChecksum(payload);

			secondCrc.Should().Be(0);
		}

		[TestCase("abcs")]
		[TestCase("11aaaee09")]
		[TestCase("x")]
		public void Crc32_CrcWithModifiedPayloadComputesToNotZero(string input)
		{
			var bytes = input.ToBytes();
			var crc   = Crc32.ComputeChecksum(bytes);
			
			bytes[0] ^= 0x14;
			var payload   = bytes.Concat(BitConverter.GetBytes(crc)).ToArray();
			var secondCrc = Crc32.ComputeChecksum(payload);

			secondCrc.Should().NotBe(0);
		}
	}
}
