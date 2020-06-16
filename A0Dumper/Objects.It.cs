// $Date: 2019-10-15 14:18:33 +0300 (Вт, 15 окт 2019) $
// $Revision: 7 $
// $Author: vbutov $


namespace A0Dumper
{
    namespace Objects
    {
        using A0Service;

        /// <summary>
        /// Итератор по объектам в А0
        /// </summary>
        public abstract class It
        {
            public It(IA0EstimateRepo Repo)
            {
                m_Repo = Repo;
            }

            public void Run()
            {
				// Комплексы 
				var it = m_Repo.ComplexID.Read2(m_Repo.ComplexID.HeadComplexGUID, null);
				InternalRun(it, 0);
				// Проекты
				it = m_Repo.ProjID.Read2(m_Repo.ProjID.HeadComplexGUID, null);
				InternalRun(it, 0);
			}

            protected virtual void InternalRun(IA0ObjectIterator it, int Level)
            {
                while (it.Next())
                {
                    // Элемент итератора
                    var Item = it.Item;

                    // Обработка элемента
                    Do(Item, Level);

					// Опускаемся по структуре до актов
					if (IsDown(Item))
					{
						switch (Item.Kind)
						{
							case EA0ObjectKind.okUnknown:
								break;
							case EA0ObjectKind.okComplex:
								{
									// Комплексы 
									var It = m_Repo.ComplexID.Read2(Item.ID.GUID, null);
									InternalRun(It, Level + 1);
									// Проекты
									It = m_Repo.ProjID.Read2(Item.ID.GUID, null);
									InternalRun(It, Level + 1);
									break;
								}
							case EA0ObjectKind.okProject:
								{
									// ОС 
									var It = m_Repo.OSID.Read(Item.ID.GUID, null, null, null);
									InternalRun(It, Level + 1);
									break;
								}
							case EA0ObjectKind.okOS:
								{
									// ЛС 
									var It = m_Repo.LSID.Read(Item.ID.GUID, null, null, null);
									InternalRun(It, Level + 1);
									break;
								}
							case EA0ObjectKind.okLS:
								{
									break;
								}
							case EA0ObjectKind.okAct:
								{
									break;
								}
						}
					}
                }
            }

            /// <summary>
            /// Выполнение действий над объектом А0
            /// </summary>
            protected abstract void Do(IA0Object Obj, int Level);
            /// <summary>
            /// Опускаться ли по структуре ниже
            /// </summary>
            protected abstract bool IsDown(IA0Object Obj);

            /// <summary>
            /// Каталог обощенных объектов А0
            /// </summary>
            private IA0EstimateRepo m_Repo;                                      
        }
    }
}
