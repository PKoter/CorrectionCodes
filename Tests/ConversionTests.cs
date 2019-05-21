using System;
using System.Linq;
using System.Text;
using CorrectionCodes.Core;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class ConversionTests
	{
		[TestCase("a")]
		[TestCase("bc")]
		[TestCase("yo nibbas we'll ride on 'em")]
		public void Extensions_ConvertToBitsAndBackCorrect(string input)
		{
			var bytes = input.ToBytes();
			Console.WriteLine(string.Concat(bytes.Select(b => Convert.ToString(b, 2))));

			var bits = bytes.ConvertToBits();
			Console.WriteLine(string.Concat(bits));

			var byteArrays = bits.GroupToBytes();
			Console.WriteLine(string.Concat(byteArrays.Select(b => string.Concat(b))));

			var byteStream = byteArrays.Select(ba => ba.ToByte());
			Console.WriteLine(string.Concat(byteStream.Select(b => Convert.ToString(b, 2))));

			var output = Encoding.ASCII.GetString(byteStream.ToArray());
			Assert.AreEqual(input, output);
		}
	}
}
