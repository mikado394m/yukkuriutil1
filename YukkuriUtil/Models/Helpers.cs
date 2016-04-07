using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YukkuriUtil.Models {
	public struct Range {
		public int FirstValue {
			get;
			set;
		}

		public int LastValue {
			get;
			set;
		}

		public Range(int firstValue = int.MinValue, int lastValue = int.MaxValue) {
			FirstValue = firstValue;
			LastValue = lastValue;
		}

		public static bool operator ==(int value, Range range) {
			return (value >= range.FirstValue && value <= range.LastValue);
		}

		public static bool operator !=(int value, Range range) {
			return !(value == range);
		}
	}
}
