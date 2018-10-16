using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YukkuriUtil.Models {
	public static class Helpers {
		public static bool InRange(int value, int first = int.MinValue, int last = int.MaxValue) {
			return (first <= value && value <= last);
		}
	}
}
