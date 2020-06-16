// $Date: 2019-10-15 14:18:33 +0300 (Вт, 15 окт 2019) $
// $Revision: 7 $
// $Author: vbutov $

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A0Dumper
{
	namespace NSI
	{
		using A0Service;

		/// <summary>
		/// Базовый итератор по объектам домена НСИ
		/// </summary>
		public abstract class It
		{
			public abstract bool Next();
		}

		/// <summary>
		/// Итератор по БД НСИ в системе
		/// </summary>
		public class BaseIt: It
		{
			private A0Service.INSIBases m_Bases;
			private int m_It;

			public BaseIt(A0Service.INSIBases bases)
			{
				m_Bases = bases;
				m_It = -1;
			}

			public override bool Next()
			{
				return ++m_It < m_Bases.Count;
			}

			public INSIBase Current
			{
				get
				{
					return m_Bases.Item[m_It];
				}
			}
		}
	}
}
