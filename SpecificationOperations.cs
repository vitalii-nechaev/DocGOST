/*
 *
 * This file is part of the DocGOST project.    
 * Copyright (C) 2018 Vitalii Nechaev.
 * 
 * This program is free software; you can redistribute it and/or modify it 
 * under the terms of the GNU Affero General Public License version 3 as 
 * published by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. *
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using DocGOST.Data;


namespace DocGOST
{
    class SpecificationOperations
    {
        const int maxNoteLength = 12;

        public List<SpecificationItem> groupSpecificationElements(List<SpecificationItem> sList, ref int numberOfValidStrings)
        {
            #region Группировка элементов спецификации из раздела "Прочие" с одинаковым наименованием, которые идут подряд            
            List<SpecificationItem> tempList = new List<SpecificationItem>();
            SpecificationItem tempItem = new SpecificationItem();

            string prevElemName = sList[0].name;
            int position = 0;


            tempItem.makeEmpty();
            tempItem.spSection = sList[0].spSection;
            tempItem.position = (position + 1).ToString();
            tempItem.name = sList[0].name;
            tempItem.quantity = "1";
            tempItem.note = sList[0].designator;
            tempItem.docum = sList[0].docum;
            tempItem.group = sList[0].group;


            for (int i = 1; i < numberOfValidStrings; i++)
            {
                if (sList[i].name == prevElemName)
                {
                    tempItem.note += "," + sList[i].note;
                    tempItem.quantity = (int.Parse(tempItem.quantity) + 1).ToString();
                }
                else
                {
                    tempList.Add(tempItem);
                    position++;
                    tempItem = new SpecificationItem();
                    tempItem.makeEmpty();
                    tempItem.spSection = sList[i].spSection;
                    tempItem.position = (position + 1).ToString();
                    tempItem.name = sList[i].name;
                    tempItem.quantity = "1";
                    tempItem.note = sList[i].designator;
                    tempItem.docum = sList[i].docum;
                    tempItem.group = sList[i].group;
                }
                if (i == (numberOfValidStrings - 1)) { tempList.Add(tempItem); position++; }
                prevElemName = sList[i].name;
            }

            numberOfValidStrings = position;

            for (int i = 0; i < numberOfValidStrings; i++)
            {
                string[] designators = new string[tempList[i].note.Split(new Char[] { ',' }).Length];
                designators = tempList[i].note.Split(new Char[] { ',' });
                if (designators.Length > 2) tempList[i].note = designators[0] + '-' + designators[designators.Length - 1];
            }



            #endregion

            #region Группировка всех элементов спецификации из раздела "Прочие" с одинаковым наименованием

            for (int i = 0; i < numberOfValidStrings; i++)
            {
                for (int j = i + 1; j < numberOfValidStrings; j++)
                    if ((tempList[j].name == tempList[i].name) & (tempList[j].name != String.Empty))
                    {
                        tempList[i].note += "," + tempList[j].note;
                        tempList[i].quantity = (int.Parse(tempList[i].quantity) + int.Parse(tempList[j].quantity)).ToString();
                        tempList[j].name = String.Empty;
                    }
            }

            #endregion

            #region Удаление лишних строк и сортировка по алфавиту
            List<SpecificationItem> tempList1 = new List<SpecificationItem>();

            for (int i = 0; i < numberOfValidStrings; i++)
            {
                if (tempList[i].name != String.Empty)
                {
                    tempItem = new SpecificationItem();
                    tempItem.makeEmpty();
                    tempItem.spSection = tempList[i].spSection;
                    tempItem.name = tempList[i].name;
                    tempItem.quantity = tempList[i].quantity;
                    tempItem.note = tempList[i].note;
                    tempItem.docum = tempList[i].docum;
                    tempItem.group = tempList[i].group;
                    tempList1.Add(tempItem);
                }
            }

            tempList = new List<SpecificationItem>();
            tempList = tempList1.OrderBy(x => x.name).ToList();

            foreach (SpecificationItem item in tempList) item.position = "Авто";

            numberOfValidStrings = tempList.Count;
            #endregion

            
            #region Разбиение каждой записи на нужное количество строк
            List<SpecificationItem> tempList2 = new List<SpecificationItem>();
            const int maxNameLength = 36;
            const int maxNoteLength = 13;
            //Удаление лишних строк
            for (int i = 0; i < numberOfValidStrings; i++)
            {
                if (tempList[i].name != String.Empty)
                {
                    //Разбиение строк, чтобы все надписи вмещались в ячейки.

                    string name = tempList[i].name;
                    string note = tempList[i].note;
                    string quantity = tempList[i].quantity;
                    string pos = tempList[i].position;

                    while ((name != String.Empty) | (note != String.Empty))
                    {
                        tempItem = new SpecificationItem();
                        tempItem.makeEmpty();
                        tempItem.spSection = tempList[i].spSection;
                        if (pos != String.Empty)
                        {
                            tempItem.position = tempList[i].position;
                            pos = String.Empty;
                        }                        
                        if (quantity != String.Empty)
                        {
                            tempItem.quantity = quantity;
                            quantity = String.Empty;
                        }

                        string group = tempList[i].group;
                        string docum = tempList[i].docum;

                        //Разбираемся с наименованием
                        if (name.Length > maxNameLength)
                        {
                            if ((name.Length - docum.Length - 1) < maxNameLength)
                            {
                                tempItem.name = name.Substring(0, name.Length - docum.Length - 1);
                                name =docum;
                                docum = String.Empty;
                            }
                            else
                            if (group != String.Empty)
                            {
                                tempItem.name = group;
                                name = name.Replace(group + ' ', String.Empty);
                                group = String.Empty;
                            }
                            else
                            {
                                string[] words = name.Split(new Char[] { ' ' });
                                tempItem.name = words[0] + ' ';

                                for (int j = 1; j < words.Length; j++)
                                {
                                    if ((tempItem.name.Length + words[j].Length) > maxNameLength)
                                    {
                                        tempItem.name.Substring(0, tempItem.name.Length - 1); // удаляем последний пробел
                                        name = string.Empty;
                                        for (int k = j; k < words.Length; k++)
                                            if (k != (words.Length - 1)) name += words[k] + ' ';
                                            else name += words[k];
                                        break;
                                    }
                                    else tempItem.name += words[j] + ' ';
                                }
                            }
                        }
                        else if (name != String.Empty)
                        {
                            tempItem.name = name;
                            name = String.Empty;
                        }

                        //Разбираемся с примечанием
                        if (note.Length > maxNoteLength)
                        {
                            string[] designators = note.Split(new Char[] { ',' });
                            tempItem.note = designators[0] + ", ";

                            for (int j = 1; j < designators.Length; j++)
                            {
                                if ((tempItem.note.Length + designators[j].Length) >= maxNoteLength)
                                {
                                    note = string.Empty;
                                    for (int k = j; k < designators.Length; k++)
                                        if (k != (designators.Length - 1)) note += designators[k] + ',';
                                        else note += designators[k];
                                    break;
                                }
                                else tempItem.note += designators[j] + ", ";
                            }
                        }
                        else if (note != String.Empty)
                        {
                            tempItem.note = note;
                            note = String.Empty;
                        }


                        tempList2.Add(tempItem);
                    }


                }

            }

            numberOfValidStrings = tempList2.Count();

            #endregion

            return tempList2;
        }


    }
}
