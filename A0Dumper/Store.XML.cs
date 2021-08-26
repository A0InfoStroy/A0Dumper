// $Date: 2019-10-15 14:18:33 +0300 (Вт, 15 окт 2019) $
// $Revision: 7 $
// $Author: vbutov $

using System;
using System.Collections;
using System.Collections.Generic;

namespace A0Dumper
{
    using A0Service;

    namespace Store
    {
        namespace XML
        {
            using System.Xml;
            using System.Diagnostics;

            public class StoreXMLException : Exception
            {
                public StoreXMLException() : base() { }
                public StoreXMLException(string message) : base(message) { }
                public StoreXMLException(string message, Exception innerException) : base(message, innerException) { }
            }

            /// <summary>
            /// Вывод в XML файл
            /// </summary>
            public class FileService
            {
                public delegate void Message(string s);

                public FileService(string FileName, Guid ID, IA0EstimateRepo EstimateRepo, Message Message)
                {
                    m_FileName = FileName;
                    m_ID = ID;
                    m_EstimateRepo = EstimateRepo;
                    m_Message = Message;
                }

                public void Execute()
                {
                    // Документ XML
                    var xmlDocument = new XmlDocument();

                    XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                    XmlElement RootNode = xmlDocument.DocumentElement;
                    xmlDocument.InsertBefore(xmlDeclaration, RootNode);

                    XmlNode A0Node = xmlDocument.CreateElement("A0");
                    xmlDocument.AppendChild(A0Node);

                    XmlNode ProjsNode = xmlDocument.CreateElement("Projects");
                    A0Node.AppendChild(ProjsNode);

                    Stopwatch W = new Stopwatch();
					
                    try
                    {     
                        // Грузим проект
                        m_Message(string.Format("Загрузка проекта {0}...", m_ID.ToString())); W.Restart();
                        var Proj = m_EstimateRepo.Proj.Load(m_ID, false);
                        W.Stop(); m_Message(string.Format(" Ok[{0}]\n", W.Elapsed));

						// Сохраняем проект в XML
						XmlNode ProjNode = new Project(Proj).Save(ProjsNode, xmlDocument);

                        XmlNode OSsNode = xmlDocument.CreateElement("Items");
                        ProjNode.AppendChild(OSsNode);

                        // ОС проекта
                        var itOSs = m_EstimateRepo.OSID.Read(Proj.ID.GUID, null, null, null);
						// По всем ОС в проекте
						while (itOSs.Next())
                        {
                            var OSID = itOSs.Item.ID as IA0OSID;

                            m_Message(string.Format("\tЗагрузка ОС {0}...", OSID.GUID.ToString())); W.Restart();
                            var OS = m_EstimateRepo.OS.Load(OSID.GUID, false);
                            W.Stop(); m_Message(string.Format(" Ok[{0}]\n", W.Elapsed));

                            // Сохраняем ОС в XML
                            XmlNode OSNode = new OS(OS).Save(OSsNode, xmlDocument);

                            var LSsNode = xmlDocument.CreateElement("Items");
                            OSNode.AppendChild(LSsNode);

							// ЛС ОС
							var Where = m_EstimateRepo.LSID.GetWhereRequest();
							// Выбираем ЛС для ОС.
							// Для этого надо установить фильтр по GUID ОС.
							Where.Node.And3("[LSTitle].[OSGUID]", "=", string.Format("'{0}'", OS.ID.GUID.ToString()));
							var itLSs = m_EstimateRepo.LSID.Read(OS.ID.ProjGUID, null, Where, null);
							// По всем ЛС в ОС
							while (itLSs.Next())
                            {
                                var LSID = itLSs.Item.ID as IA0LSID;

                                m_Message(string.Format("\t\tЗагрузка ЛС {0}...", LSID.GUID.ToString())); W.Restart();

								var LS = m_EstimateRepo.LS.Load2(LSID.GUID);
                                W.Stop(); m_Message(string.Format(" Ok[{0}]\n", W.Elapsed));

                                m_Message(string.Format("\t\tСохранение ЛС ({0}){1}...", LS.Title.Mark, LS.Title.Name)); W.Restart();
                                // Сохраняем ЛС в XML
                                XmlNode LSNode = new LS(LS).Save(LSsNode, xmlDocument);
                                W.Stop(); m_Message(string.Format(" Ok[{0}]\n", W.Elapsed));

                                m_Message(string.Format("\t\t\tструктура...")); W.Restart();
                                // Сохраняем структуру ЛС в XML
                                new LSTree(LS.Tree, LS.Strings).Save(LSNode, xmlDocument);
                                W.Stop(); m_Message(string.Format(" Ok[{0}]\n", W.Elapsed));

                                /*// Сохраняем строки ЛС в XML
                                m_Message(string.Format("\t\t\tстроки...")); W.Restart();
                                new LSStrings(LS.Strings).Save(LSNode, xmlDocument);
                                W.Stop(); m_Message(string.Format(" Ok[{0}]\n", W.Elapsed));

                                // Сораняем ресурсы строки ЛС в XML
                                m_Message(string.Format("\t\t\tресурсы...")); W.Restart();
                                for (var i = 0; i < LS.Strings.Count; ++i)
                                    new LSResources(LS.Strings.Items[i].Resources).Save(LSNode, xmlDocument);
                                W.Stop(); m_Message(string.Format(" Ok[{0}]\n", W.Elapsed));  */
                            }
                        }
                    }
                    finally
                    {
                        // Сохраняем XML документ в файл
                        xmlDocument.Save(m_FileName);
                    }
                }

                private string m_FileName;
                private Guid m_ID;
                private IA0EstimateRepo m_EstimateRepo;
                private Message m_Message;
            }

            /// <summary>
            /// Сохранение Проекта А0 в XML
            /// </summary>
            class Project
            {
                public Project(IA0Proj Proj)
                {
                    m_Proj = Proj;
                }

                public XmlNode Save(XmlNode xmlRoot, XmlDocument xmlDocument)
                {
                    try
                    {
                        var xmlNode = xmlDocument.CreateElement("Project");
                        xmlRoot.AppendChild(xmlNode);

                        var Attr = xmlDocument.CreateAttribute("GUID");
                        Attr.Value = m_Proj.ID.GUID.ToString();
                        xmlNode.Attributes.Append(Attr);

                        Attr = xmlDocument.CreateAttribute("Mark");
                        Attr.Value = m_Proj.Title.Mark;
                        xmlNode.Attributes.Append(Attr);

                        Attr = xmlDocument.CreateAttribute("Name");
                        Attr.Value = m_Proj.Title.Name;
                        xmlNode.Attributes.Append(Attr);

                        return xmlNode;
                    }
                    catch (Exception e)
                    {
                        throw new StoreXMLException("Сохранение проекта", e);
                    }
                }

                /// <summary>
                /// для выгрузки
                /// </summary>
                private IA0Proj m_Proj;
            }

            /// <summary>
            /// Сохранение ОС А0 в XML
            /// </summary>
            class OS
            {
                public OS(IA0OS OS)
                {
                    m_OS = OS;
                }

                public XmlNode Save(XmlNode xmlRoot, XmlDocument xmlDocument)
                {
                    try
                    {
                        var xmlNode = xmlDocument.CreateElement("OS");
                        xmlRoot.AppendChild(xmlNode);

                        var Attr = xmlDocument.CreateAttribute("GUID");
                        Attr.Value = m_OS.ID.GUID.ToString();
                        xmlNode.Attributes.Append(Attr);

                        Attr = xmlDocument.CreateAttribute("Mark");
                        Attr.Value = m_OS.Title.Mark;
                        xmlNode.Attributes.Append(Attr);

                        Attr = xmlDocument.CreateAttribute("Name");
                        Attr.Value = m_OS.Title.Name;
                        xmlNode.Attributes.Append(Attr);

                        return xmlNode;
                    }
                    catch (Exception e)
                    {
                        throw new StoreXMLException("Сохранение ОС", e);
                    }
                }

                /// <summary>
                /// для выгрузки
                /// </summary>
                private IA0OS m_OS;
            }

            /// <summary>
            /// Сохранение ЛС А0 в XML
            /// </summary>
            class LS
            {
                public LS(IA0LS LS)
                {
                    m_LS = LS;
                }

                public XmlNode Save(XmlNode xmlRoot, XmlDocument xmlDocument)
                {
                    try
                    {

                        var xmlNode = xmlDocument.CreateElement("LS");
                        xmlRoot.AppendChild(xmlNode);

                        var Attr = xmlDocument.CreateAttribute("GUID");
                        Attr.Value = m_LS.ID.GUID.ToString();
                        xmlNode.Attributes.Append(Attr);

                        Attr = xmlDocument.CreateAttribute("Mark");
                        Attr.Value = m_LS.Title.Mark;
                        xmlNode.Attributes.Append(Attr);

                        Attr = xmlDocument.CreateAttribute("Name");
                        Attr.Value = m_LS.Title.Name;
                        xmlNode.Attributes.Append(Attr);

                        var NodeTotal = xmlDocument.CreateElement("Total");
                        xmlNode.AppendChild(NodeTotal);

                        var T = m_LS.Totals.ByName[@"9 Сметная стоимость"];

                        // Всего
                        Attr = xmlDocument.CreateAttribute("Total");
                        Attr.Value = T.Total.ToString();
                        NodeTotal.Attributes.Append(Attr);

                        // Строительные
                        Attr = xmlDocument.CreateAttribute("Construction");
                        Attr.Value = T.Construction.ToString();
                        NodeTotal.Attributes.Append(Attr);

                        // Монтажные
                        Attr = xmlDocument.CreateAttribute("Mounting");
                        Attr.Value = T.Mounting.ToString();
                        NodeTotal.Attributes.Append(Attr);

                        // Оборудование
                        Attr = xmlDocument.CreateAttribute("Equipment");
                        Attr.Value = T.Equipment.ToString();
                        NodeTotal.Attributes.Append(Attr);

                        // Прочие
                        Attr = xmlDocument.CreateAttribute("Other");
                        Attr.Value = T.Other.ToString();
                        NodeTotal.Attributes.Append(Attr);

                        NodeTotal = xmlDocument.CreateElement("Total_basic");
                        xmlNode.AppendChild(NodeTotal);

                        T = m_LS.Totals.ByName[@"баз.  9 Сметная стоимость"];
                        // Всего
                        Attr = xmlDocument.CreateAttribute("Total");
                        Attr.Value = T.Total.ToString();
                        NodeTotal.Attributes.Append(Attr);

                        // Строительные
                        Attr = xmlDocument.CreateAttribute("Construction");
                        Attr.Value = T.Construction.ToString();
                        NodeTotal.Attributes.Append(Attr);

                        // Монтажные
                        Attr = xmlDocument.CreateAttribute("Mounting");
                        Attr.Value = T.Mounting.ToString();
                        NodeTotal.Attributes.Append(Attr);

                        // Оборудование
                        Attr = xmlDocument.CreateAttribute("Equipment");
                        Attr.Value = T.Equipment.ToString();
                        NodeTotal.Attributes.Append(Attr);

                        // Прочие
                        Attr = xmlDocument.CreateAttribute("Other");
                        Attr.Value = T.Other.ToString();
                        NodeTotal.Attributes.Append(Attr);

						T = null;

                        return xmlNode;
                    }
                    catch (Exception e)
                    {
                        throw new StoreXMLException("Сохранение ЛС", e);
                    }

                }

                /// <summary>
                /// для выгрузки
                /// </summary>
                private IA0LS m_LS;
            }

            /// <summary>
            /// Сохранение структуры ЛС А0 в XML
            /// </summary>
            class LSTree
            {
                public LSTree(IA0Tree Tree, IA0LSStrings Strings)
                {
                    m_Tree = Tree;
                    m_Strings = Strings;
					m_Nodes = new Dictionary<int, List<IA0LSString>>();
				}

                private void Save(XmlNode xmlRoot, XmlDocument xmlDocument, IA0TreeNode A0Node)
                {
                    var xmlNode = xmlDocument.CreateElement("Node");
                    xmlRoot.AppendChild(xmlNode);

                    var Attr = xmlDocument.CreateAttribute("Name");
                    Attr.Value = A0Node.Name;
                    xmlNode.Attributes.Append(Attr);

                    Attr = xmlDocument.CreateAttribute("Level");
                    Attr.Value = A0Node.Level.ToString();
                    xmlNode.Attributes.Append(Attr);

					// Вложенные узлы
					if (A0Node.Count > 0)
					{
						var xmlNodes = xmlDocument.CreateElement("Nodes");
						xmlNode.AppendChild(xmlNodes);

						for (var i = 0; i < A0Node.Count; ++i)
							Save(xmlNodes, xmlDocument, A0Node.Item[i]);
					}
					else
					{
						// Строки
						List<IA0LSString> Strings = new List<IA0LSString>();

						for (var i = 0; i < m_Strings.Count; ++i)
							if (A0Node.ID == m_Strings.Items[i].ParentID)
								Strings.Add(m_Strings.Items[i]);

						if (Strings.Count > 0)
						    new LSStrings(Strings).Save(xmlNode, xmlDocument);

						Strings.Clear();
					}
                }

                public void Save(XmlNode xmlRoot, XmlDocument xmlDocument)
                {
                    try
                    {
                        var xmlNode = xmlDocument.CreateElement("Tree");
                        xmlRoot.AppendChild(xmlNode);

                        var xmlNodes = xmlDocument.CreateElement("Nodes");
                        xmlNode.AppendChild(xmlNodes);

                        Save(xmlNodes, xmlDocument, m_Tree.Head);
                    }
                    catch (Exception e)
                    {
                        throw new StoreXMLException("Сохранение структуры ЛС", e);
                    }
                }

                private IA0Tree m_Tree;
                private IA0LSStrings m_Strings;
				private Dictionary<int, List<IA0LSString>> m_Nodes;
            }

            /// <summary>
            /// Сохранение строк А0 в XML
            /// </summary>
            class LSStrings
            {
                public LSStrings(List<IA0LSString> Strings)
                {
                    m_Strings = Strings;
                }

                public void Save(XmlNode xmlRoot, XmlDocument xmlDocument)
                {
                    try
                    {
                        var xmlNode = xmlDocument.CreateElement("Strings");
                        xmlRoot.AppendChild(xmlNode);

                        for (var i = 0; i < m_Strings.Count; ++i)
                        {
                            var xmlNodeString = xmlDocument.CreateElement("String");
                            xmlNode.AppendChild(xmlNodeString);

                            var S = m_Strings[i];

                            var Attr = xmlDocument.CreateAttribute("GUID");
                            Attr.Value = S.GUID.ToString();
                            xmlNodeString.Attributes.Append(Attr);
                            // Номер
                            Attr = xmlDocument.CreateAttribute("LSNumber");
                            Attr.Value = S.LSNumber.ToString();
                            xmlNodeString.Attributes.Append(Attr);
                            // Наименование
                            Attr = xmlDocument.CreateAttribute("Name");
                            Attr.Value = S.Name;
                            xmlNodeString.Attributes.Append(Attr);
                            // Группа
                            Attr = xmlDocument.CreateAttribute("Group");
                            Attr.Value = S.Group;
                            xmlNodeString.Attributes.Append(Attr);
                            // Расценка
                            Attr = xmlDocument.CreateAttribute("Basing");
                            Attr.Value = S.Basing;
                            xmlNodeString.Attributes.Append(Attr);
                            // Тип учёта 
                            Attr = xmlDocument.CreateAttribute("IncludeKind");
                            Attr.Value = S.IncludeKind.ToString();
                            xmlNodeString.Attributes.Append(Attr);
                            // Тип строки 
                            Attr = xmlDocument.CreateAttribute("StringKind");
                            Attr.Value = S.StringKind.ToString();
                            xmlNodeString.Attributes.Append(Attr);
                            // Единица измерения
                            Attr = xmlDocument.CreateAttribute("Unit");
                            Attr.Value = S.MUnit;
                            xmlNodeString.Attributes.Append(Attr);
                            // Объём 
                            Attr = xmlDocument.CreateAttribute("TotalVolume");
                            Attr.Value = S.TotalVolume.ToString();
                            xmlNodeString.Attributes.Append(Attr);
                            // Вид затрат 
                            Attr = xmlDocument.CreateAttribute("ExpenseKind");
                            Attr.Value = S.ExpenseKind.ToString();
                            xmlNodeString.Attributes.Append(Attr);
                            // Вид работ (для НР/СП)
                            Attr = xmlDocument.CreateAttribute("WorkKindNRSP");
                            Attr.Value = S.WorkKindNRSP;
                            xmlNodeString.Attributes.Append(Attr);

                            Attr = xmlDocument.CreateAttribute("NRBase");
                            Attr.Value = S.NRBase.ToString();
                            xmlNodeString.Attributes.Append(Attr);
							
                            // % НР
                            Attr = xmlDocument.CreateAttribute("NRProc");
                            Attr.Value = S.NRProc.ToString();
                            xmlNodeString.Attributes.Append(Attr);

							/*
							// Значение НР
							Attr = xmlDocument.CreateAttribute("NR");
							Attr.Value = S.TotalForVolume.NR.ToString();
							xmlNodeString.Attributes.Append(Attr);
							
							Attr = xmlDocument.CreateAttribute("SPBase");
                            Attr.Value = S.SPBase.ToString();
                            xmlNodeString.Attributes.Append(Attr);	

                            // % СП
                            Attr = xmlDocument.CreateAttribute("SPProc");
                            Attr.Value = S.SPProc.ToString();
                            xmlNodeString.Attributes.Append(Attr);

							// Значение СП
							Attr = xmlDocument.CreateAttribute("SP");
							Attr.Value = S.TotalForVolume.SP.ToString();
							xmlNodeString.Attributes.Append(Attr);

							Attr = xmlDocument.CreateAttribute("BaseNRBase");
                            Attr.Value = S.BaseNRBase.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // % НР
                            Attr = xmlDocument.CreateAttribute("BaseNRProc");
                            Attr.Value = S.BaseNRProc.ToString();
                            xmlNodeString.Attributes.Append(Attr);

							// Значение НР
							Attr = xmlDocument.CreateAttribute("BaseNR");
							Attr.Value = S.TotalForVolume.BaseNR.ToString();
							xmlNodeString.Attributes.Append(Attr);

							Attr = xmlDocument.CreateAttribute("BaseSPBase");
                            Attr.Value = S.BaseSPBase.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // % СП
                            Attr = xmlDocument.CreateAttribute("BaseSPProc");
                            Attr.Value = S.BaseSPProc.ToString();
                            xmlNodeString.Attributes.Append(Attr);

							// Значение СП
							Attr = xmlDocument.CreateAttribute("BaseSP");
							Attr.Value = S.TotalForVolume.BaseSP.ToString();
							xmlNodeString.Attributes.Append(Attr);
							
							// Стоимости 
							// Из базы
							// ПЗ
							Attr = xmlDocument.CreateAttribute("PZ");
                            Attr.Value = S.StrBasing.PZ.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // ОЗП
                            Attr = xmlDocument.CreateAttribute("OZP");
                            Attr.Value = S.StrBasing.OZP.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // ЭМ
                            Attr = xmlDocument.CreateAttribute("EM");
                            Attr.Value = S.StrBasing.EM.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // ЗПМ
                            Attr = xmlDocument.CreateAttribute("ZPM");
                            Attr.Value = S.StrBasing.ZPM.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // МЗ
                            Attr = xmlDocument.CreateAttribute("MZ");
                            Attr.Value = S.StrBasing.MZ.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // На единицу
                            if (S.CalcMode == ECalcMode.cmBase)
                            {
                                // ПЗ
                                Attr = xmlDocument.CreateAttribute("PZ_Corr");
                                Attr.Value = S.StrBasing.PZ_Corr.ToString();
                                xmlNodeString.Attributes.Append(Attr);

                                // ОЗП
                                Attr = xmlDocument.CreateAttribute("OZP_Corr");
                                Attr.Value = S.StrBasing.OZP_Corr.ToString();
                                xmlNodeString.Attributes.Append(Attr);

                                // ЭМ
                                Attr = xmlDocument.CreateAttribute("EM_Corr");
                                Attr.Value = S.StrBasing.EM_Corr.ToString();
                                xmlNodeString.Attributes.Append(Attr);

                                // ЗПМ
                                Attr = xmlDocument.CreateAttribute("ZPM_Corr");
                                Attr.Value = S.StrBasing.ZPM_Corr.ToString();
                                xmlNodeString.Attributes.Append(Attr);

                                // МЗ
                                Attr = xmlDocument.CreateAttribute("MZ_Corr");
                                Attr.Value = S.StrBasing.MZ_Corr.ToString();
                                xmlNodeString.Attributes.Append(Attr);
                            }
                            else
                            {
                                // ПЗ
                                Attr = xmlDocument.CreateAttribute("PZ_Corr");
                                Attr.Value = S.StrBasing.PZ_Direct.ToString();
                                xmlNodeString.Attributes.Append(Attr);

                                // ОЗП
                                Attr = xmlDocument.CreateAttribute("OZP_Corr");
                                Attr.Value = S.StrBasing.OZP_Direct.ToString();
                                xmlNodeString.Attributes.Append(Attr);

                                // ЭМ
                                Attr = xmlDocument.CreateAttribute("EM_Corr");
                                Attr.Value = S.StrBasing.EM_Direct.ToString();
                                xmlNodeString.Attributes.Append(Attr);

                                // ЗПМ
                                Attr = xmlDocument.CreateAttribute("ZPM_Corr");
                                Attr.Value = S.StrBasing.ZPM_Direct.ToString();
                                xmlNodeString.Attributes.Append(Attr);

                                // МЗ
                                Attr = xmlDocument.CreateAttribute("MZ_Corr");
                                Attr.Value = S.StrBasing.MZ_Direct.ToString();
                                xmlNodeString.Attributes.Append(Attr);
                            }

                            // С начислением
                            // ПЗ
                            Attr = xmlDocument.CreateAttribute("PZ_Single");
                            Attr.Value = S.Total.PZ.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // ОЗП
                            Attr = xmlDocument.CreateAttribute("OZP_Single");
                            Attr.Value = S.Total.OZP.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // ЭМ
                            Attr = xmlDocument.CreateAttribute("EM_Single");
                            Attr.Value = S.Total.EM.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // ЗПМ
                            Attr = xmlDocument.CreateAttribute("ZPM_Single");
                            Attr.Value = S.Total.ZPM.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // МЗ
                            Attr = xmlDocument.CreateAttribute("MZ_Single");
                            Attr.Value = S.Total.MZ.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // По строке
                            // ПЗ
                            Attr = xmlDocument.CreateAttribute("PZ_Total");
                            Attr.Value = S.TotalForVolume.PZ.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // ОЗП
                            Attr = xmlDocument.CreateAttribute("OZP_Total");
                            Attr.Value = S.TotalForVolume.OZP.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // ЭМ
                            Attr = xmlDocument.CreateAttribute("EM_Total");
                            Attr.Value = S.TotalForVolume.EM.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // ЗПМ
                            Attr = xmlDocument.CreateAttribute("ZPM_Total");
                            Attr.Value = S.TotalForVolume.ZPM.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            // МЗ
                            Attr = xmlDocument.CreateAttribute("MZ_Total");
                            Attr.Value = S.TotalForVolume.MZ.ToString();
                            xmlNodeString.Attributes.Append(Attr);

							// ВМ
							Attr = xmlDocument.CreateAttribute("VM_Total");
							Attr.Value = S.TotalForVolume.VM.ToString();
							xmlNodeString.Attributes.Append(Attr);

							// Сметная стоимость
							Attr = xmlDocument.CreateAttribute("Estimate");
                            Attr.Value = S.Estimate.ToString();
                            xmlNodeString.Attributes.Append(Attr);

                            Attr = xmlDocument.CreateAttribute("BaseEstimate");
                            Attr.Value = S.BaseEstimate.ToString();
                            xmlNodeString.Attributes.Append(Attr);  

                            new LSResources(S.Resources).Save(xmlNodeString, xmlDocument);

                            // Комментарии строки
                            Attr = xmlDocument.CreateAttribute("Comment");
                            Attr.Value = S.Comment;
                            xmlNodeString.Attributes.Append(Attr);
                            
                            for (var j = 0; j < S.Comments.Count; ++j)
                            {
                                Attr = xmlDocument.CreateAttribute(string.Format("Comment{0}", j));
                                Attr.Value = S.Comments.Item[j];
                                xmlNodeString.Attributes.Append(Attr);
                            }
							*/
							S = null;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new StoreXMLException("Сохранение строк ЛС", e);
                    }
                }

                private List<IA0LSString> m_Strings;
            }

            /// <summary>
            /// Сохранение ресурсов строки А0 в XML
            /// </summary>
            class LSResources
            {
                public LSResources(IA0Resources Resources)
                {
                    m_Resources = Resources;
                }

                public void Save(XmlNode xmlRoot, XmlDocument xmlDocument)
                {
                    try
                    {
                        var xmlNode = xmlDocument.CreateElement("Resources");
                        xmlRoot.AppendChild(xmlNode);

                        for (var i = 0; i < m_Resources.Count; ++i)
                        {
                            var xmlNodeRes = xmlDocument.CreateElement("Resource");
                            xmlNode.AppendChild(xmlNodeRes);

                            var R = m_Resources.Items[i];

                            var Attr = xmlDocument.CreateAttribute("ID");
                            Attr.Value = R.ID.ToString();
                            xmlNodeRes.Attributes.Append(Attr);

                            // Расценка
                            Attr = xmlDocument.CreateAttribute("Basing");
                            Attr.Value = R.Basing;
                            xmlNodeRes.Attributes.Append(Attr);

                            // Группа
                            Attr = xmlDocument.CreateAttribute("Group");
                            Attr.Value = R.Group;
                            xmlNodeRes.Attributes.Append(Attr);

                            // Наименование
                            Attr = xmlDocument.CreateAttribute("Name");
                            Attr.Value = R.Name;
                            xmlNodeRes.Attributes.Append(Attr);

                            // Тип учёта 
                            Attr = xmlDocument.CreateAttribute("Accounting");
                            Attr.Value = R.Accounting.ToString();
                            xmlNodeRes.Attributes.Append(Attr);

                            // Тип ресурса 
                            Attr = xmlDocument.CreateAttribute("Kind");
                            Attr.Value = R.Kind.ToString();
                            xmlNodeRes.Attributes.Append(Attr);

                            // Единица измерения
                            Attr = xmlDocument.CreateAttribute("MUnit");
                            Attr.Value = R.MUnit;
                            xmlNodeRes.Attributes.Append(Attr);

                            // Количество Расход. На объём
                            Attr = xmlDocument.CreateAttribute("Volume");
                            Attr.Value = R.Volume.ToString();
                            xmlNodeRes.Attributes.Append(Attr);

                            // Норма на единицу Из базы
                            Attr = xmlDocument.CreateAttribute("Norm");
                            Attr.Value = R.Norm.ToString();
                            xmlNodeRes.Attributes.Append(Attr);

                            // Норма на единицу С поправкой
                            Attr = xmlDocument.CreateAttribute("Norm_Corr");
                            Attr.Value = R.Norm_Corr.ToString();
                            xmlNodeRes.Attributes.Append(Attr);

                            // Норма на единицу С пересч., Н-парам.
                            Attr = xmlDocument.CreateAttribute("Norm_Calc");
                            Attr.Value = R.Norm_Calc.ToString();
                            xmlNodeRes.Attributes.Append(Attr);

                            // Цена
                            Attr = xmlDocument.CreateAttribute("Price");
                            Attr.Value = R.Price.ToString();
                            xmlNodeRes.Attributes.Append(Attr);

                            // Стоимость
                            Attr = xmlDocument.CreateAttribute("Cost");
                            Attr.Value = R.Cost.ToString();
                            xmlNodeRes.Attributes.Append(Attr);
                        }

                    }
                    catch (Exception e)
                    {
                        throw new StoreXMLException("Сохранение ресурсов строки ЛС", e);
                    }
                }

                private IA0Resources m_Resources;
            }
        }
    }
}
