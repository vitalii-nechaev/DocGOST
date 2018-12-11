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
using System.Windows;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using DocGOST.Data;

namespace DocGOST
{
    class PdfOperations
    {

        const int perech_first_page_rows_count = 23;
        const int perech_subseq_page_rows_count = 29;

        const int spec_first_page_rows_count = 23;
        const int spec_subseq_page_rows_count = 29;

        const int vedomost_first_page_rows_count = 24;
        const int vedomost_subseq_page_rows_count = 29;

        Font normal, big, veryBig;
        ProjectDB project;

        private enum DocType
        {
            Specification = 1, // Спецификация
            Perechen, // Перечень элементов
            Vedomost // Ведомость покупных изделий
        }

        public PdfOperations(string projectPath)
        {
            BaseFont fontGostA = BaseFont.CreateFont("GOST_A.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            normal = new Font(fontGostA, 11f, Font.ITALIC, BaseColor.BLACK);
            big = new Font(fontGostA, 18f, Font.ITALIC, BaseColor.BLACK);
            veryBig = new Font(fontGostA, 22f, Font.ITALIC, BaseColor.BLACK);

            project = new ProjectDB(projectPath);
        }

        public void CreateSpecification(string pdfPath, int startPage, bool addListRegistr, int tempNumber)
        {
            List<SpecificationItem> specPrintList = new List<SpecificationItem>();
            int numberOfStrings = project.GetSpecLength(tempNumber);
            int numberOfPrintStrings = 0;
            int prevSpSection = 0;

            //Формируем данные спецификации для печати, добавляя названия разделов.
            for (int i = 0; i < numberOfStrings; i++)
            {
                SpecificationItem sItem = new SpecificationItem();
                sItem = project.GetSpecItem(i + 1, tempNumber);

                if (sItem.spSection != prevSpSection)
                {
                    //Добавляем пустую строку
                    SpecificationItem empty = new SpecificationItem();
                    empty.name = String.Empty;
                    specPrintList.Add(empty);

                    //Добавляем название раздела
                    SpecificationItem header = new SpecificationItem();
                    if (sItem.spSection == ((int)Global.SpSections.Documentation)) header.name = "Документация";
                    if (sItem.spSection == ((int)Global.SpSections.Compleksi)) header.name = "Комплексы";
                    if (sItem.spSection == ((int)Global.SpSections.SborEd)) header.name = "Сборочные единицы";
                    if (sItem.spSection == ((int)Global.SpSections.Details)) header.name = "Детали";
                    if (sItem.spSection == ((int)Global.SpSections.Standard)) header.name = "Стандартные изделия";
                    if (sItem.spSection == ((int)Global.SpSections.Other)) header.name = "Прочие изделия";
                    if (sItem.spSection == ((int)Global.SpSections.Materials)) header.name = "Материалы";
                    if (sItem.spSection == ((int)Global.SpSections.Compleсts)) header.name = "Комплекты";
                    header.group = "Header";

                    specPrintList.Add(header);

                    //Добавляем пустую строку
                    empty = new SpecificationItem();
                    empty.name = String.Empty;
                    specPrintList.Add(empty);
                    numberOfPrintStrings += 3;
                }

                specPrintList.Add(sItem);
                numberOfPrintStrings++;
                prevSpSection = sItem.spSection;
            }


            bool isFileLocked = false;
            Document document = null;
            PdfWriter writer = null;
            try
            {
                document = new Document(PageSize.A4);
                writer = PdfWriter.GetInstance(document, new FileStream(pdfPath, FileMode.Create));
            }
            catch
            {
                MessageBox.Show("Не удаётся получить доступ к файлу " + pdfPath + ". Скорее всего, файл открыт в другой программе.", "Ошибка", MessageBoxButton.OK);
                isFileLocked = true;
            }

            if (isFileLocked == false)
            {
                document.Open();

                DrawCommonStampA4(document, writer, DocType.Specification);

                //Вычислим общее количество страниц без учёта листа регистрации
                int totalPageCount;
                if (numberOfPrintStrings <= spec_first_page_rows_count) totalPageCount = 1;
                else totalPageCount = 2 + (numberOfPrintStrings - spec_first_page_rows_count) / spec_subseq_page_rows_count;
                
                DrawFirstPageStampA4(document, writer, startPage, totalPageCount + (addListRegistr ? 1 : 0) + (startPage - 1), DocType.Specification);

                DrawSpecificationTable(document, writer, 0, specPrintList);
                                

                if (numberOfPrintStrings > spec_first_page_rows_count)
                    for (int i = 1; i < totalPageCount; i++)
                    {
                        document.NewPage();

                        DrawCommonStampA4(document, writer, DocType.Specification);
                        DrawSubsequentStampA4(document, writer, i + startPage, DocType.Specification);
                        DrawSpecificationTable(document, writer, i, specPrintList);

                    }
                if (addListRegistr)
                {
                    document.NewPage();

                    DrawCommonStampA4(document, writer, DocType.Specification);
                    DrawSubsequentStampA4(document, writer, totalPageCount + startPage, DocType.Specification);
                    DrawListRegistrTable(document, writer);
                }
                document.Close();
                writer.Close();
            }

        }

        public void CreatePerechen(string pdfPath, int startPage, bool addListRegistr, int tempNumber)
        {
            bool isFileLocked = false;

            Document document = null;
            PdfWriter writer = null;

            try
            {
                document = new Document(PageSize.A4);
                writer = PdfWriter.GetInstance(document, new FileStream(pdfPath, FileMode.Create));
            }
            catch
            {
                MessageBox.Show("Не удаётся получить доступ к файлу " + pdfPath + ". Скорее всего, файл открыт в другой программе.", "Ошибка", MessageBoxButton.OK);
                isFileLocked = true;
            }


            if (isFileLocked == false)
            {
                document.Open();

                DrawCommonStampA4(document, writer, DocType.Perechen);

                int numberOfValidStrings = project.GetPerechenLength(tempNumber);

                //Вычисляем общее количество листов без учёта листа регистрации
                int totalPageCount;
                if (numberOfValidStrings <= perech_first_page_rows_count) totalPageCount = 1;
                else totalPageCount = 2 + (numberOfValidStrings - perech_first_page_rows_count) / perech_subseq_page_rows_count;

                DrawFirstPageStampA4(document, writer, startPage, totalPageCount + (addListRegistr ? 1 : 0) + (startPage - 1), DocType.Perechen);

                DrawPerechenTable(document, writer, 0, tempNumber);
                
                if (numberOfValidStrings > perech_first_page_rows_count)
                    for (int i = 1; i < totalPageCount; i++)
                    {
                        document.NewPage();

                        DrawCommonStampA4(document, writer, DocType.Perechen);
                        DrawSubsequentStampA4(document, writer, i + startPage, DocType.Perechen);
                        DrawPerechenTable(document, writer, i, tempNumber);

                    }
                if (addListRegistr)
                {
                    document.NewPage();

                    DrawCommonStampA4(document, writer, DocType.Perechen);
                    DrawSubsequentStampA4(document, writer, totalPageCount + startPage, DocType.Perechen);
                    DrawListRegistrTable(document, writer);
                }
                document.Close();
                writer.Close();
            }

        }

        public void CreateVedomost(string pdfPath, int startPage, bool addListRegistr, int tempNumber)
        {
            bool isFileLocked = false;

            Document document = null;
            PdfWriter writer = null;

            try
            {
                document = new Document(PageSize.A3.Rotate());
                
                writer = PdfWriter.GetInstance(document, new FileStream(pdfPath, FileMode.Create));
            }
            catch
            {
                MessageBox.Show("Не удаётся получить доступ к файлу " + pdfPath + ". Скорее всего, файл открыт в другой программе.", "Ошибка", MessageBoxButton.OK);
                isFileLocked = true;
            }

            if (isFileLocked == false)
            {
                document.Open();

                DrawCommonStampA3(document, writer, DocType.Perechen);

                int numberOfValidStrings = project.GetVedomostLength(tempNumber);

                //Вычисляем общее количество листов без учёта листа регистрации
                int totalPageCount;
                if (numberOfValidStrings <= vedomost_first_page_rows_count) totalPageCount = 1;
                else totalPageCount = 2 + (numberOfValidStrings - vedomost_first_page_rows_count) / vedomost_subseq_page_rows_count;

                DrawFirstPageStampA3(document, writer, startPage, totalPageCount + (addListRegistr ? 1 : 0) + (startPage - 1), DocType.Vedomost);

                DrawVedomostTable(document, writer, 0, tempNumber);

                if (numberOfValidStrings > vedomost_first_page_rows_count)
                    for (int i = 1; i < totalPageCount; i++)
                    {
                        document.NewPage();

                        DrawCommonStampA3(document, writer, DocType.Vedomost);
                        DrawSubsequentStampA3(document, writer, i + startPage, DocType.Vedomost);
                        DrawVedomostTable(document, writer, i, tempNumber);

                    }
                if (addListRegistr)
                {
                    document.SetPageSize(PageSize.A4);
                    document.NewPage();                   

                    DrawCommonStampA4(document, writer, DocType.Vedomost);
                    DrawSubsequentStampA4(document, writer, totalPageCount + startPage, DocType.Vedomost);
                    DrawListRegistrTable(document, writer);
                }
                document.Close();
                writer.Close();
            }

        }

        private void DrawCommonStampA4(Document doc, PdfWriter wr, DocType docType)
        {

            PdfContentByte cb = wr.DirectContent;

            // Черчение рамки:
            float mm_A4 = doc.PageSize.Width / 210;

            cb.MoveTo(20 * mm_A4, 5 * mm_A4);
            cb.LineTo(20 * mm_A4, 292 * mm_A4);//Левая граница
            cb.LineTo(205 * mm_A4, 292 * mm_A4);//Верхняя граница
            cb.LineTo(205 * mm_A4, 5 * mm_A4);//Правая граница
            cb.LineTo(20 * mm_A4, 5 * mm_A4);//Нижняя граница
            cb.Stroke();

            #region Рисование табицы с графами 19-23            
            PdfPTable table19_23 = new PdfPTable(2);
            table19_23.TotalWidth = 12 * mm_A4;
            table19_23.LockedWidth = true;
            float[] tbldWidths = new float[2];
            tbldWidths[0] = 5;
            tbldWidths[1] = 7;
            table19_23.SetWidths(tbldWidths);

            // Заполнение графы 19:
            PdfPCell currentCell = new PdfPCell(new Phrase("Инв. № подл.", normal));
            currentCell.BorderWidth = 1;
            currentCell.Rotation = 90;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 25 * mm_A4;
            table19_23.AddCell(currentCell);
            string gr19Text = String.Empty;
            if (docType == DocType.Specification) gr19Text = project.GetOsnNadpisItem("19").specificationValue;
            if (docType == DocType.Perechen) gr19Text = project.GetOsnNadpisItem("19").perechenValue;
            currentCell.Phrase = new Phrase(gr19Text, normal);
            currentCell.PaddingLeft = 2;
            table19_23.AddCell(currentCell);
            table19_23.WriteSelectedRows(0, 1, 8 * mm_A4, 30 * mm_A4, cb);
            // Заполнение графы 20:
            currentCell.Phrase = new Phrase("Подп. и дата", normal);
            currentCell.PaddingLeft = 0;
            currentCell.FixedHeight = 35 * mm_A4;
            table19_23.AddCell(currentCell);
            currentCell.Phrase = new Phrase("", normal);
            currentCell.PaddingLeft = 2;
            table19_23.AddCell(currentCell);
            table19_23.WriteSelectedRows(1, 2, 8 * mm_A4, 65 * mm_A4, cb);
            // Заполнение графы 21:
            currentCell.Phrase = new Phrase("Взам. инв. №", normal);
            currentCell.PaddingLeft = 0;
            currentCell.FixedHeight = 25 * mm_A4;
            table19_23.AddCell(currentCell);
            string gr21Text = String.Empty;
            if (docType == DocType.Specification) gr21Text = project.GetOsnNadpisItem("21").specificationValue;
            if (docType == DocType.Perechen) gr21Text = project.GetOsnNadpisItem("21").perechenValue;
            currentCell.Phrase = new Phrase(gr21Text, normal);
            currentCell.PaddingLeft = 2;
            table19_23.AddCell(currentCell);
            table19_23.WriteSelectedRows(2, 3, 8 * mm_A4, 90 * mm_A4, cb);
            // Заполнение графы 22:
            currentCell.Phrase = new Phrase("Инв. № дубл.", normal);
            currentCell.PaddingLeft = 0;
            currentCell.FixedHeight = 25 * mm_A4;
            table19_23.AddCell(currentCell);
            string gr22Text = String.Empty;
            if (docType == DocType.Specification) gr22Text = project.GetOsnNadpisItem("22").specificationValue;
            if (docType == DocType.Perechen) gr22Text = project.GetOsnNadpisItem("22").perechenValue;
            currentCell.Phrase = new Phrase(gr22Text, normal);
            currentCell.PaddingLeft = 2;
            table19_23.AddCell(currentCell);
            table19_23.WriteSelectedRows(3, 4, 8 * mm_A4, 115 * mm_A4, cb);
            // Заполнение графы 23:
            currentCell.Phrase = new Phrase("Подп. и дата", normal);
            currentCell.PaddingLeft = 0;
            currentCell.FixedHeight = 35 * mm_A4;
            table19_23.AddCell(currentCell);
            currentCell.Phrase = new Phrase("", normal);
            currentCell.PaddingLeft = 2;
            table19_23.AddCell(currentCell);
            table19_23.WriteSelectedRows(4, 5, 8 * mm_A4, 150 * mm_A4, cb);

            #endregion


            #region Рисование табицы с графами 31, 32            
            PdfPTable table31_32 = new PdfPTable(2);
            table31_32.TotalWidth = 130 * mm_A4;
            table31_32.LockedWidth = true;
            tbldWidths[0] = 80;
            tbldWidths[1] = 50;
            table31_32.SetWidths(tbldWidths);

            // Заполнение графы 31:
            currentCell = new PdfPCell(new Phrase("Копировал", normal));
            currentCell.BorderWidth = 0;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 5 * mm_A4;
            table31_32.AddCell(currentCell);
            // Заполнение графы 31:
            currentCell.Phrase = new Phrase("Формат А4", normal);
            currentCell.PaddingLeft = 2;
            table31_32.AddCell(currentCell);
            table31_32.WriteSelectedRows(0, 1, 75 * mm_A4, 5 * mm_A4, cb);


            #endregion
        }

        private void DrawCommonStampA3(Document doc, PdfWriter wr, DocType docType)
        {

            PdfContentByte cb = wr.DirectContent;

            // Черчение рамки:
            float mm_A3 = doc.PageSize.Width / 420;

            cb.MoveTo(20 * mm_A3, 5 * mm_A3);
            cb.LineTo(20 * mm_A3, 292 * mm_A3);//Левая граница
            cb.LineTo(415 * mm_A3, 292 * mm_A3);//Верхняя граница
            cb.LineTo(415 * mm_A3, 5 * mm_A3);//Правая граница
            cb.LineTo(20 * mm_A3, 5 * mm_A3);//Нижняя граница
            cb.Stroke();

            #region Рисование табицы с графами 19-23            
            PdfPTable table19_23 = new PdfPTable(2);
            table19_23.TotalWidth = 12 * mm_A3;
            table19_23.LockedWidth = true;
            float[] tbldWidths = new float[2];
            tbldWidths[0] = 5;
            tbldWidths[1] = 7;
            table19_23.SetWidths(tbldWidths);

            // Заполнение графы 19:
            PdfPCell currentCell = new PdfPCell(new Phrase("Инв. № подл.", normal));
            currentCell.BorderWidth = 1;
            currentCell.Rotation = 90;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 25 * mm_A3;
            table19_23.AddCell(currentCell);
            string gr19Text = String.Empty;
            if (docType == DocType.Specification) gr19Text = project.GetOsnNadpisItem("19").specificationValue;
            if (docType == DocType.Perechen) gr19Text = project.GetOsnNadpisItem("19").perechenValue;
            currentCell.Phrase = new Phrase(gr19Text, normal);
            currentCell.PaddingLeft = 2;
            table19_23.AddCell(currentCell);
            table19_23.WriteSelectedRows(0, 1, 8 * mm_A3, 30 * mm_A3, cb);
            // Заполнение графы 20:
            currentCell.Phrase = new Phrase("Подп. и дата", normal);
            currentCell.PaddingLeft = 0;
            currentCell.FixedHeight = 35 * mm_A3;
            table19_23.AddCell(currentCell);
            currentCell.Phrase = new Phrase("", normal);
            currentCell.PaddingLeft = 2;
            table19_23.AddCell(currentCell);
            table19_23.WriteSelectedRows(1, 2, 8 * mm_A3, 65 * mm_A3, cb);
            // Заполнение графы 21:
            currentCell.Phrase = new Phrase("Взам. инв. №", normal);
            currentCell.PaddingLeft = 0;
            currentCell.FixedHeight = 25 * mm_A3;
            table19_23.AddCell(currentCell);
            string gr21Text = String.Empty;
            if (docType == DocType.Specification) gr21Text = project.GetOsnNadpisItem("21").specificationValue;
            if (docType == DocType.Perechen) gr21Text = project.GetOsnNadpisItem("21").perechenValue;
            currentCell.Phrase = new Phrase(gr21Text, normal);
            currentCell.PaddingLeft = 2;
            table19_23.AddCell(currentCell);
            table19_23.WriteSelectedRows(2, 3, 8 * mm_A3, 90 * mm_A3, cb);
            // Заполнение графы 22:
            currentCell.Phrase = new Phrase("Инв. № дубл.", normal);
            currentCell.PaddingLeft = 0;
            currentCell.FixedHeight = 25 * mm_A3;
            table19_23.AddCell(currentCell);
            string gr22Text = String.Empty;
            if (docType == DocType.Specification) gr22Text = project.GetOsnNadpisItem("22").specificationValue;
            if (docType == DocType.Perechen) gr22Text = project.GetOsnNadpisItem("22").perechenValue;
            currentCell.Phrase = new Phrase(gr22Text, normal);
            currentCell.PaddingLeft = 2;
            table19_23.AddCell(currentCell);
            table19_23.WriteSelectedRows(3, 4, 8 * mm_A3, 115 * mm_A3, cb);
            // Заполнение графы 23:
            currentCell.Phrase = new Phrase("Подп. и дата", normal);
            currentCell.PaddingLeft = 0;
            currentCell.FixedHeight = 35 * mm_A3;
            table19_23.AddCell(currentCell);
            currentCell.Phrase = new Phrase("", normal);
            currentCell.PaddingLeft = 2;
            table19_23.AddCell(currentCell);
            table19_23.WriteSelectedRows(4, 5, 8 * mm_A3, 150 * mm_A3, cb);

            #endregion


            #region Рисование табицы с графами 31, 32            
            PdfPTable table31_32 = new PdfPTable(2);
            table31_32.TotalWidth = 130 * mm_A3;
            table31_32.LockedWidth = true;
            tbldWidths[0] = 80;
            tbldWidths[1] = 50;
            table31_32.SetWidths(tbldWidths);

            // Заполнение графы 31:
            currentCell = new PdfPCell(new Phrase("Копировал", normal));
            currentCell.BorderWidth = 0;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 5 * mm_A3;
            table31_32.AddCell(currentCell);
            // Заполнение графы 31:
            currentCell.Phrase = new Phrase("Формат А3", normal);
            currentCell.PaddingLeft = 2;
            table31_32.AddCell(currentCell);
            table31_32.WriteSelectedRows(0, 1, 285 * mm_A3, 5 * mm_A3, cb);


            #endregion
        }

        private void DrawFirstPageStampA4(Document doc, PdfWriter wr, int pageNumber, int pagesTotal, DocType docType)
        {


            PdfContentByte cb = wr.DirectContent;

            float[] tbldWidths;
            float mm_A4 = doc.PageSize.Width / 210;


            #region Рисование табицы с графами 24,25            
            PdfPTable table24_25 = new PdfPTable(2);
            table24_25.TotalWidth = 12 * mm_A4;
            table24_25.LockedWidth = true;
            tbldWidths = new float[2];
            tbldWidths[0] = 5;
            tbldWidths[1] = 7;
            table24_25.SetWidths(tbldWidths);

            // Заполнение графы 24:
            PdfPCell currentCell = new PdfPCell(new Phrase("Справ. №", normal));
            currentCell.BorderWidth = 1;
            currentCell.Rotation = 90;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 60 * mm_A4;
            table24_25.AddCell(currentCell);
            string gr24Text = String.Empty;
            if (docType == DocType.Specification) gr24Text = project.GetOsnNadpisItem("24").specificationValue;
            if (docType == DocType.Perechen) gr24Text = project.GetOsnNadpisItem("24").perechenValue;
            currentCell.Phrase = new Phrase(gr24Text, normal);
            currentCell.PaddingLeft = 2;
            table24_25.AddCell(currentCell);
            table24_25.WriteSelectedRows(0, 1, 8 * mm_A4, 232 * mm_A4, cb);
            // Заполнение графы 25:
            currentCell.Phrase = new Phrase("Перв. примен.", normal);
            currentCell.PaddingLeft = 0;
            currentCell.FixedHeight = 60 * mm_A4;
            table24_25.AddCell(currentCell);
            string gr25Text = String.Empty;
            if (docType == DocType.Specification) gr25Text = project.GetOsnNadpisItem("25").specificationValue;
            if (docType == DocType.Perechen) gr25Text = project.GetOsnNadpisItem("25").perechenValue;
            currentCell.Phrase = new Phrase(gr25Text, normal);
            currentCell.PaddingLeft = 2;
            table24_25.AddCell(currentCell);
            table24_25.WriteSelectedRows(1, 2, 8 * mm_A4, 292 * mm_A4, cb);
            #endregion

            #region Рисование графы 1           
            PdfPTable table1 = new PdfPTable(1);
            table1.TotalWidth = 70 * mm_A4;
            table1.LockedWidth = true;

            //Определяем, сколько строчек нужно для наименования изделия:
            int kolvoStrGg1 = 2;
            int naimenovaieMaxLength = 25;
            string naimenovanieStr1 = "", naimenovanieStr2 = "";
            string gr1aText = String.Empty;
            if (docType == DocType.Specification) gr1aText = project.GetOsnNadpisItem("1a").specificationValue;
            if (docType == DocType.Perechen) gr1aText = project.GetOsnNadpisItem("1a").perechenValue;

            if (gr1aText.Length > naimenovaieMaxLength)
            {
                kolvoStrGg1 = 3;


                string[] naimenovanieStrings = gr1aText.Split(' ');
                foreach (string currentString in naimenovanieStrings)
                {
                    if (naimenovanieStr1.Length + currentString.Length + 1 < naimenovaieMaxLength) naimenovanieStr1 += currentString + " ";
                    else naimenovanieStr2 += currentString + " ";
                }
            }

            // Заполнение графы 1:
            if (kolvoStrGg1 == 2)
            {
                currentCell = new PdfPCell(new Phrase(gr1aText, big));
                currentCell.BorderWidth = 1;
                currentCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                currentCell.HasFixedHeight();
                currentCell.Padding = 0;
                currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
                currentCell.FixedHeight = 18 * mm_A4;
                table1.AddCell(currentCell);
                table1.WriteSelectedRows(0, 1, 85 * mm_A4, 30 * mm_A4, cb);
                string gr1bText = project.GetOsnNadpisItem("1b").perechenValue;
                if (docType == DocType.Specification) gr1bText = project.GetOsnNadpisItem("1b").specificationValue;
                currentCell.Phrase = new Phrase(gr1bText, normal);
                currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                currentCell.BorderWidth = 1;
                currentCell.DisableBorderSide(Rectangle.TOP_BORDER);
                currentCell.FixedHeight = 7 * mm_A4;
                table1.AddCell(currentCell);
                table1.WriteSelectedRows(1, 2, 85 * mm_A4, 12 * mm_A4, cb);
            }
            else
            {
                currentCell = new PdfPCell(new Phrase(naimenovanieStr1, big));
                currentCell.BorderWidth = 1;
                currentCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                currentCell.HasFixedHeight();
                currentCell.Padding = 0;
                currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
                currentCell.FixedHeight = 10 * mm_A4;
                table1.AddCell(currentCell);
                table1.WriteSelectedRows(0, 1, 85 * mm_A4, 30 * mm_A4, cb);
                currentCell.Phrase = new Phrase(naimenovanieStr2, big);
                currentCell.DisableBorderSide(Rectangle.TOP_BORDER);
                currentCell.FixedHeight = 10 * mm_A4;
                table1.AddCell(currentCell);
                table1.WriteSelectedRows(1, 2, 85 * mm_A4, 20 * mm_A4, cb);
                string gr1bText = project.GetOsnNadpisItem("1b").perechenValue;
                if (docType == DocType.Specification) gr1bText = project.GetOsnNadpisItem("1b").specificationValue;
                currentCell.Phrase = new Phrase(gr1bText, normal);
                currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                currentCell.EnableBorderSide(Rectangle.BOTTOM_BORDER);
                currentCell.FixedHeight = 5 * mm_A4;
                table1.AddCell(currentCell);
                table1.WriteSelectedRows(2, 3, 85 * mm_A4, 10 * mm_A4, cb);
            }

            #endregion

            #region Рисование графы 2           
            PdfPTable table2 = new PdfPTable(1);
            table2.TotalWidth = 120 * mm_A4;
            table2.LockedWidth = true;


            // Заполнение графы 2:
            string gr2Text = String.Empty;
            if (docType == DocType.Specification) gr2Text = project.GetOsnNadpisItem("2").specificationValue;
            if (docType == DocType.Perechen) gr2Text = project.GetOsnNadpisItem("2").perechenValue;

            currentCell = new PdfPCell(new Phrase(gr2Text, veryBig));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.PaddingBottom = 6;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 15 * mm_A4;
            table2.AddCell(currentCell);
            table2.WriteSelectedRows(0, 1, 85 * mm_A4, 45 * mm_A4, cb);
            #endregion

            #region Рисование табицы с графами 4,7,8            
            PdfPTable table4_8 = new PdfPTable(3);
            table4_8.TotalWidth = 50 * mm_A4;
            table4_8.LockedWidth = true;
            tbldWidths = new float[3];
            tbldWidths[0] = 15;
            tbldWidths[1] = 15;
            tbldWidths[2] = 20;
            table4_8.SetWidths(tbldWidths);

            // Заполнение заголовков граф 4,7,8:
            currentCell = new PdfPCell(new Phrase("Лит.", normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 5 * mm_A4;
            table4_8.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Лист", normal);
            table4_8.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Листов", normal);
            table4_8.AddCell(currentCell);
            // Заполнение граф 4,7,8:
            currentCell.Phrase = new Phrase(String.Empty, normal);
            table4_8.AddCell(currentCell);
            currentCell.Phrase = new Phrase(pageNumber.ToString(), normal);
            table4_8.AddCell(currentCell);
            string gr8Text = gr8Text = pagesTotal.ToString();            
            currentCell.Phrase = new Phrase(gr8Text, normal);
            table4_8.AddCell(currentCell);
            table4_8.WriteSelectedRows(0, 2, 155 * mm_A4, 30 * mm_A4, cb);

            // Заполнение графы 4:
            PdfPTable table4 = new PdfPTable(3);
            table4.TotalWidth = 15 * mm_A4;
            table4.LockedWidth = true;
            tbldWidths = new float[3];
            tbldWidths[0] = 5;
            tbldWidths[1] = 5;
            tbldWidths[2] = 5;
            table4.SetWidths(tbldWidths);
            string gr4aText = String.Empty;
            if (docType == DocType.Specification) gr4aText = project.GetOsnNadpisItem("4a").specificationValue;
            if (docType == DocType.Perechen) gr4aText = project.GetOsnNadpisItem("4a").perechenValue;
            currentCell = new PdfPCell(new Phrase(gr4aText, normal));
            currentCell.BorderWidth = 0.5f;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 5 * mm_A4;
            table4.AddCell(currentCell);
            string gr4bText = String.Empty;
            if (docType == DocType.Specification) gr4bText = project.GetOsnNadpisItem("4b").specificationValue;
            if (docType == DocType.Perechen) gr4bText = project.GetOsnNadpisItem("4b").perechenValue;
            currentCell.Phrase = new Phrase(gr4bText, normal);
            table4.AddCell(currentCell);
            string gr4cText = String.Empty;
            if (docType == DocType.Specification) gr4cText = project.GetOsnNadpisItem("4c").specificationValue;
            if (docType == DocType.Perechen) gr4cText = project.GetOsnNadpisItem("4c").perechenValue;
            currentCell.Phrase = new Phrase(gr4cText, normal);
            table4.AddCell(currentCell);
            table4.WriteSelectedRows(0, 1, 155 * mm_A4, 25 * mm_A4, cb);
            #endregion

            #region Рисование таблицы с графами 10-18
            //Рисование толстых линий и заполнение заголовков граф 14-18
            PdfPTable table10_18 = new PdfPTable(5);
            table10_18.TotalWidth = 65 * mm_A4;
            table10_18.LockedWidth = true;
            tbldWidths = new float[5];
            tbldWidths[0] = 7;
            tbldWidths[1] = 10;
            tbldWidths[2] = 23;
            tbldWidths[3] = 15;
            tbldWidths[4] = 10;
            table10_18.SetWidths(tbldWidths);
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 10 * mm_A4;
            for (int i = 0; i < 5; i++) table10_18.AddCell(currentCell);
            currentCell.FixedHeight = 5 * mm_A4;
            currentCell.Phrase = new Phrase("Изм.", normal);
            table10_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Лист", normal);
            table10_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("№ докум.", normal);
            table10_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Подп.", normal);
            table10_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Дата", normal);
            table10_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            currentCell.FixedHeight = 25 * mm_A4;
            currentCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
            table10_18.AddCell(currentCell);
            currentCell.EnableBorderSide(Rectangle.RIGHT_BORDER);
            currentCell.DisableBorderSide(Rectangle.LEFT_BORDER);
            table10_18.AddCell(currentCell);
            currentCell.EnableBorderSide(Rectangle.LEFT_BORDER);
            for (int i = 0; i < 3; i++) table10_18.AddCell(currentCell);
            table10_18.WriteSelectedRows(0, 3, 20 * mm_A4, 45 * mm_A4, cb);
            //Рисование тонкими линиями и заполнение граф 14-18
            PdfPTable table14_18 = new PdfPTable(5);
            table14_18.TotalWidth = 65 * mm_A4;
            table14_18.LockedWidth = true;
            tbldWidths = new float[5];
            tbldWidths[0] = 7;
            tbldWidths[1] = 10;
            tbldWidths[2] = 23;
            tbldWidths[3] = 15;
            tbldWidths[4] = 10;
            table14_18.SetWidths(tbldWidths);
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 0.5f;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 5 * mm_A4;
            table14_18.AddCell(currentCell);
            table14_18.AddCell(currentCell);
            table14_18.AddCell(currentCell);
            table14_18.AddCell(currentCell);
            table14_18.AddCell(currentCell);
            table14_18.WriteSelectedRows(0, 1, 20 * mm_A4, 45 * mm_A4, cb);
            string gr14aText = String.Empty;
            if (docType == DocType.Specification) gr14aText = project.GetOsnNadpisItem("14a").specificationValue;
            if (docType == DocType.Perechen) gr14aText = project.GetOsnNadpisItem("14a").perechenValue;
            currentCell.Phrase = new Phrase(gr14aText, normal);
            table14_18.AddCell(currentCell);
            string gr15aText = String.Empty;
            if (docType == DocType.Specification) gr15aText = project.GetOsnNadpisItem("15a").specificationValue;
            if (docType == DocType.Perechen) gr15aText = project.GetOsnNadpisItem("15a").perechenValue;
            currentCell.Phrase = new Phrase(gr15aText, normal);
            table14_18.AddCell(currentCell);
            string gr16aText = String.Empty;
            if (docType == DocType.Specification) gr16aText = project.GetOsnNadpisItem("16a").specificationValue;
            if (docType == DocType.Perechen) gr16aText = project.GetOsnNadpisItem("16a").perechenValue;
            currentCell.Phrase = new Phrase(gr16aText, normal);
            table14_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            table14_18.AddCell(currentCell);
            table14_18.AddCell(currentCell);
            table14_18.WriteSelectedRows(1, 2, 20 * mm_A4, 40 * mm_A4, cb);

            //Рисование тонкими линиями и заполнение граф 10-13
            PdfPTable table10_13 = new PdfPTable(4);
            table10_13.TotalWidth = 65 * mm_A4;
            table10_13.LockedWidth = true;
            tbldWidths = new float[4];
            tbldWidths[0] = 17;
            tbldWidths[1] = 23;
            tbldWidths[2] = 15;
            tbldWidths[3] = 10;
            table10_13.SetWidths(tbldWidths);
            currentCell = new PdfPCell(new Phrase("Разраб.", normal));
            currentCell.BorderWidth = 0.5f;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.PaddingLeft = 3;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_LEFT;
            currentCell.FixedHeight = 5 * mm_A4;
            table10_13.AddCell(currentCell);
            string gr11aText = String.Empty;
            if (docType == DocType.Specification) gr11aText = project.GetOsnNadpisItem("11a").specificationValue;
            if (docType == DocType.Perechen) gr11aText = project.GetOsnNadpisItem("11a").perechenValue;
            currentCell.Phrase = new Phrase(gr11aText, normal);
            table10_13.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            for (int i = 0; i < 2; i++) table10_13.AddCell(currentCell);
            table10_13.WriteSelectedRows(0, 1, 20 * mm_A4, 30 * mm_A4, cb);
            currentCell.Phrase = new Phrase("Пров.", normal);
            table10_13.AddCell(currentCell);
            string gr11bText = String.Empty;
            if (docType == DocType.Specification) gr11bText = project.GetOsnNadpisItem("11b").specificationValue;
            if (docType == DocType.Perechen) gr11bText = project.GetOsnNadpisItem("11b").perechenValue;
            currentCell.Phrase = new Phrase(gr11bText, normal);
            table10_13.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            for (int i = 0; i < 2; i++) table10_13.AddCell(currentCell);
            table10_13.WriteSelectedRows(1, 2, 20 * mm_A4, 25 * mm_A4, cb);
            string gr10Text = String.Empty;
            if (docType == DocType.Specification) gr10Text = project.GetOsnNadpisItem("10").specificationValue;
            if (docType == DocType.Perechen) gr10Text = project.GetOsnNadpisItem("10").perechenValue;
            currentCell.Phrase = new Phrase(gr10Text, normal);
            table10_13.AddCell(currentCell);
            string gr11cText = String.Empty;
            if (docType == DocType.Specification) gr11cText = project.GetOsnNadpisItem("11c").specificationValue;
            if (docType == DocType.Perechen) gr11cText = project.GetOsnNadpisItem("11c").perechenValue;
            currentCell.Phrase = new Phrase(gr11cText, normal);
            table10_13.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            for (int i = 0; i < 2; i++) table10_13.AddCell(currentCell);
            table10_13.WriteSelectedRows(2, 3, 20 * mm_A4, 20 * mm_A4, cb);
            currentCell.Phrase = new Phrase("Н. контр.", normal);
            table10_13.AddCell(currentCell);
            string gr11dText = String.Empty;
            if (docType == DocType.Specification) gr11dText = project.GetOsnNadpisItem("11d").specificationValue;
            if (docType == DocType.Perechen) gr11dText = project.GetOsnNadpisItem("11d").perechenValue;
            currentCell.Phrase = new Phrase(gr11dText, normal);
            table10_13.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            for (int i = 0; i < 2; i++) table10_13.AddCell(currentCell);
            table10_13.WriteSelectedRows(3, 4, 20 * mm_A4, 15 * mm_A4, cb);
            currentCell.Phrase = new Phrase("Утв.", normal);
            table10_13.AddCell(currentCell);
            string gr11eText = String.Empty;
            if (docType == DocType.Specification) gr11eText = project.GetOsnNadpisItem("11e").specificationValue;
            if (docType == DocType.Perechen) gr11eText = project.GetOsnNadpisItem("11e").perechenValue;
            currentCell.Phrase = new Phrase(gr11eText, normal);
            table10_13.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            for (int i = 0; i < 2; i++) table10_13.AddCell(currentCell);
            table10_13.WriteSelectedRows(4, 5, 20 * mm_A4, 10 * mm_A4, cb);


            #endregion

            #region Рисование табицы с графами 27-29            
            PdfPTable table27_29 = new PdfPTable(3);
            table27_29.TotalWidth = 120 * mm_A4;
            table27_29.LockedWidth = true;
            tbldWidths = new float[3];
            tbldWidths[0] = 14;
            tbldWidths[1] = 53;
            tbldWidths[2] = 53;
            table27_29.SetWidths(tbldWidths);

            // Заполнение граф 27-29:
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 14 * mm_A4;
            table27_29.AddCell(currentCell);
            table27_29.AddCell(currentCell);
            table27_29.AddCell(currentCell);
            table27_29.WriteSelectedRows(0, 1, 85 * mm_A4, 67 * mm_A4, cb);
            #endregion

            #region Рисование табицы с графой 30            
            PdfPTable table30 = new PdfPTable(1);
            table30.TotalWidth = 120 * mm_A4;
            table30.LockedWidth = true;

            // Заполнение графы 30:
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 8 * mm_A4;
            table30.AddCell(currentCell);
            table30.WriteSelectedRows(0, 1, 85 * mm_A4, 53 * mm_A4, cb);
            #endregion
        }

        private void DrawFirstPageStampA3(Document doc, PdfWriter wr, int pageNumber, int pagesTotal, DocType docType)
        {
            PdfContentByte cb = wr.DirectContent;

            float[] tbldWidths;
            float mm_A3 = doc.PageSize.Width / 420;
            
            #region Рисование табицы с графами 24,25            
            PdfPTable table24_25 = new PdfPTable(2);
            table24_25.TotalWidth = 12 * mm_A3;
            table24_25.LockedWidth = true;
            tbldWidths = new float[2];
            tbldWidths[0] = 5;
            tbldWidths[1] = 7;
            table24_25.SetWidths(tbldWidths);

            // Заполнение графы 24:
            PdfPCell currentCell = new PdfPCell(new Phrase("Справ. №", normal));
            currentCell.BorderWidth = 1;
            currentCell.Rotation = 90;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 60 * mm_A3;
            table24_25.AddCell(currentCell);
            string gr24Text = String.Empty;
            if (docType == DocType.Specification) gr24Text = project.GetOsnNadpisItem("24").specificationValue;
            if (docType == DocType.Perechen) gr24Text = project.GetOsnNadpisItem("24").perechenValue;
            if (docType == DocType.Vedomost) gr24Text = project.GetOsnNadpisItem("24").vedomostValue;
            currentCell.Phrase = new Phrase(gr24Text, normal);
            currentCell.PaddingLeft = 2;
            table24_25.AddCell(currentCell);
            table24_25.WriteSelectedRows(0, 1, 8 * mm_A3, 232 * mm_A3, cb);
            // Заполнение графы 25:
            currentCell.Phrase = new Phrase("Перв. примен.", normal);
            currentCell.PaddingLeft = 0;
            currentCell.FixedHeight = 60 * mm_A3;
            table24_25.AddCell(currentCell);
            string gr25Text = String.Empty;
            if (docType == DocType.Specification) gr25Text = project.GetOsnNadpisItem("25").specificationValue;
            if (docType == DocType.Perechen) gr25Text = project.GetOsnNadpisItem("25").perechenValue;
            if (docType == DocType.Vedomost) gr25Text = project.GetOsnNadpisItem("25").vedomostValue;
            currentCell.Phrase = new Phrase(gr25Text, normal);
            currentCell.PaddingLeft = 2;
            table24_25.AddCell(currentCell);
            table24_25.WriteSelectedRows(1, 2, 8 * mm_A3, 292 * mm_A3, cb);
            #endregion

            #region Рисование графы 1           
            PdfPTable table1 = new PdfPTable(1);
            table1.TotalWidth = 70 * mm_A3;
            table1.LockedWidth = true;

            //Определяем, сколько строчек нужно для наименования изделия:
            int kolvoStrGg1 = 2;
            int naimenovaieMaxLength = 25;
            string naimenovanieStr1 = "", naimenovanieStr2 = "";
            string gr1aText = String.Empty;
            if (docType == DocType.Specification) gr1aText = project.GetOsnNadpisItem("1a").specificationValue;
            if (docType == DocType.Perechen) gr1aText = project.GetOsnNadpisItem("1a").perechenValue;

            if (gr1aText.Length > naimenovaieMaxLength)
            {
                kolvoStrGg1 = 3;


                string[] naimenovanieStrings = gr1aText.Split(' ');
                foreach (string currentString in naimenovanieStrings)
                {
                    if (naimenovanieStr1.Length + currentString.Length + 1 < naimenovaieMaxLength) naimenovanieStr1 += currentString + " ";
                    else naimenovanieStr2 += currentString + " ";
                }
            }

            // Заполнение графы 1:
            if (kolvoStrGg1 == 2)
            {
                currentCell = new PdfPCell(new Phrase(gr1aText, big));
                currentCell.BorderWidth = 1;
                currentCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                currentCell.HasFixedHeight();
                currentCell.Padding = 0;
                currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
                currentCell.FixedHeight = 18 * mm_A3;
                table1.AddCell(currentCell);
                table1.WriteSelectedRows(0, 1, 295 * mm_A3, 30 * mm_A3, cb);
                string gr1bText = project.GetOsnNadpisItem("1b").perechenValue;
                if (docType == DocType.Specification) gr1bText = project.GetOsnNadpisItem("1b").specificationValue;
                if (docType == DocType.Vedomost) gr1bText = project.GetOsnNadpisItem("1b").vedomostValue;
                currentCell.Phrase = new Phrase(gr1bText, normal);
                currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                currentCell.BorderWidth = 1;
                currentCell.DisableBorderSide(Rectangle.TOP_BORDER);
                currentCell.FixedHeight = 7 * mm_A3;
                table1.AddCell(currentCell);
                table1.WriteSelectedRows(1, 2, 295 * mm_A3, 12 * mm_A3, cb);
            }
            else
            {
                currentCell = new PdfPCell(new Phrase(naimenovanieStr1, big));
                currentCell.BorderWidth = 1;
                currentCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                currentCell.HasFixedHeight();
                currentCell.Padding = 0;
                currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
                currentCell.FixedHeight = 10 * mm_A3;
                table1.AddCell(currentCell);
                table1.WriteSelectedRows(0, 1, 295 * mm_A3, 30 * mm_A3, cb);
                currentCell.Phrase = new Phrase(naimenovanieStr2, big);
                currentCell.DisableBorderSide(Rectangle.TOP_BORDER);
                currentCell.FixedHeight = 10 * mm_A3;
                table1.AddCell(currentCell);
                table1.WriteSelectedRows(1, 2, 295 * mm_A3, 20 * mm_A3, cb);
                string gr1bText = project.GetOsnNadpisItem("1b").perechenValue;
                if (docType == DocType.Specification) gr1bText = project.GetOsnNadpisItem("1b").specificationValue;
                if (docType == DocType.Vedomost) gr1bText = project.GetOsnNadpisItem("1b").vedomostValue;
                currentCell.Phrase = new Phrase(gr1bText, normal);
                currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                currentCell.EnableBorderSide(Rectangle.BOTTOM_BORDER);
                currentCell.FixedHeight = 5 * mm_A3;
                table1.AddCell(currentCell);
                table1.WriteSelectedRows(2, 3, 295 * mm_A3, 10 * mm_A3, cb);
            }

            #endregion

            #region Рисование графы 2           
            PdfPTable table2 = new PdfPTable(1);
            table2.TotalWidth = 120 * mm_A3;
            table2.LockedWidth = true;


            // Заполнение графы 2:
            string gr2Text = String.Empty;
            if (docType == DocType.Specification) gr2Text = project.GetOsnNadpisItem("2").specificationValue;
            if (docType == DocType.Perechen) gr2Text = project.GetOsnNadpisItem("2").perechenValue;
            if (docType == DocType.Vedomost) gr2Text = project.GetOsnNadpisItem("2").vedomostValue;

            currentCell = new PdfPCell(new Phrase(gr2Text, veryBig));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.PaddingBottom = 6;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 15 * mm_A3;
            table2.AddCell(currentCell);
            table2.WriteSelectedRows(0, 1, 295 * mm_A3, 45 * mm_A3, cb);
            #endregion

            #region Рисование табицы с графами 4,7,8            
            PdfPTable table4_8 = new PdfPTable(3);
            table4_8.TotalWidth = 50 * mm_A3;
            table4_8.LockedWidth = true;
            tbldWidths = new float[3];
            tbldWidths[0] = 15;
            tbldWidths[1] = 15;
            tbldWidths[2] = 20;
            table4_8.SetWidths(tbldWidths);

            // Заполнение заголовков граф 4,7,8:
            currentCell = new PdfPCell(new Phrase("Лит.", normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 5 * mm_A3;
            table4_8.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Лист", normal);
            table4_8.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Листов", normal);
            table4_8.AddCell(currentCell);
            // Заполнение граф 4,7,8:
            currentCell.Phrase = new Phrase(String.Empty, normal);
            table4_8.AddCell(currentCell);
            currentCell.Phrase = new Phrase(pageNumber.ToString(), normal);
            table4_8.AddCell(currentCell);
            string gr8Text = gr8Text = pagesTotal.ToString();
            currentCell.Phrase = new Phrase(gr8Text, normal);
            table4_8.AddCell(currentCell);
            table4_8.WriteSelectedRows(0, 2, 365 * mm_A3, 30 * mm_A3, cb);

            // Заполнение графы 4:
            PdfPTable table4 = new PdfPTable(3);
            table4.TotalWidth = 15 * mm_A3;
            table4.LockedWidth = true;
            tbldWidths = new float[3];
            tbldWidths[0] = 5;
            tbldWidths[1] = 5;
            tbldWidths[2] = 5;
            table4.SetWidths(tbldWidths);
            string gr4aText = String.Empty;
            if (docType == DocType.Specification) gr4aText = project.GetOsnNadpisItem("4a").specificationValue;
            if (docType == DocType.Perechen) gr4aText = project.GetOsnNadpisItem("4a").perechenValue;
            if (docType == DocType.Vedomost) gr4aText = project.GetOsnNadpisItem("4a").vedomostValue;
            currentCell = new PdfPCell(new Phrase(gr4aText, normal));
            currentCell.BorderWidth = 0.5f;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 5 * mm_A3;
            table4.AddCell(currentCell);
            string gr4bText = String.Empty;
            if (docType == DocType.Specification) gr4bText = project.GetOsnNadpisItem("4b").specificationValue;
            if (docType == DocType.Perechen) gr4bText = project.GetOsnNadpisItem("4b").perechenValue;
            if (docType == DocType.Vedomost) gr4bText = project.GetOsnNadpisItem("4b").vedomostValue;
            currentCell.Phrase = new Phrase(gr4bText, normal);
            table4.AddCell(currentCell);
            string gr4cText = String.Empty;
            if (docType == DocType.Specification) gr4cText = project.GetOsnNadpisItem("4c").specificationValue;
            if (docType == DocType.Perechen) gr4cText = project.GetOsnNadpisItem("4c").perechenValue;
            if (docType == DocType.Vedomost) gr4cText = project.GetOsnNadpisItem("4c").vedomostValue;
            currentCell.Phrase = new Phrase(gr4cText, normal);
            table4.AddCell(currentCell);
            table4.WriteSelectedRows(0, 1, 365 * mm_A3, 25 * mm_A3, cb);
            #endregion

            #region Рисование таблицы с графами 10-18
            //Рисование толстых линий и заполнение заголовков граф 14-18
            PdfPTable table10_18 = new PdfPTable(5);
            table10_18.TotalWidth = 65 * mm_A3;
            table10_18.LockedWidth = true;
            tbldWidths = new float[5];
            tbldWidths[0] = 7;
            tbldWidths[1] = 10;
            tbldWidths[2] = 23;
            tbldWidths[3] = 15;
            tbldWidths[4] = 10;
            table10_18.SetWidths(tbldWidths);
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 10 * mm_A3;
            for (int i = 0; i < 5; i++) table10_18.AddCell(currentCell);
            currentCell.FixedHeight = 5 * mm_A3;
            currentCell.Phrase = new Phrase("Изм.", normal);
            table10_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Лист", normal);
            table10_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("№ докум.", normal);
            table10_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Подп.", normal);
            table10_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Дата", normal);
            table10_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            currentCell.FixedHeight = 25 * mm_A3;
            currentCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
            table10_18.AddCell(currentCell);
            currentCell.EnableBorderSide(Rectangle.RIGHT_BORDER);
            currentCell.DisableBorderSide(Rectangle.LEFT_BORDER);
            table10_18.AddCell(currentCell);
            currentCell.EnableBorderSide(Rectangle.LEFT_BORDER);
            for (int i = 0; i < 3; i++) table10_18.AddCell(currentCell);
            table10_18.WriteSelectedRows(0, 3, 230 * mm_A3, 45 * mm_A3, cb);
            //Рисование тонкими линиями и заполнение граф 14-18
            PdfPTable table14_18 = new PdfPTable(5);
            table14_18.TotalWidth = 65 * mm_A3;
            table14_18.LockedWidth = true;
            tbldWidths = new float[5];
            tbldWidths[0] = 7;
            tbldWidths[1] = 10;
            tbldWidths[2] = 23;
            tbldWidths[3] = 15;
            tbldWidths[4] = 10;
            table14_18.SetWidths(tbldWidths);
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 0.5f;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 5 * mm_A3;
            table14_18.AddCell(currentCell);
            table14_18.AddCell(currentCell);
            table14_18.AddCell(currentCell);
            table14_18.AddCell(currentCell);
            table14_18.AddCell(currentCell);
            table14_18.WriteSelectedRows(0, 1, 230 * mm_A3, 45 * mm_A3, cb);
            string gr14aText = String.Empty;
            if (docType == DocType.Specification) gr14aText = project.GetOsnNadpisItem("14a").specificationValue;
            if (docType == DocType.Perechen) gr14aText = project.GetOsnNadpisItem("14a").perechenValue;
            if (docType == DocType.Vedomost) gr14aText = project.GetOsnNadpisItem("14a").vedomostValue;
            currentCell.Phrase = new Phrase(gr14aText, normal);
            table14_18.AddCell(currentCell);
            string gr15aText = String.Empty;
            if (docType == DocType.Specification) gr15aText = project.GetOsnNadpisItem("15a").specificationValue;
            if (docType == DocType.Perechen) gr15aText = project.GetOsnNadpisItem("15a").perechenValue;
            if (docType == DocType.Vedomost) gr15aText = project.GetOsnNadpisItem("15a").vedomostValue;
            currentCell.Phrase = new Phrase(gr15aText, normal);
            table14_18.AddCell(currentCell);
            string gr16aText = String.Empty;
            if (docType == DocType.Specification) gr16aText = project.GetOsnNadpisItem("16a").specificationValue;
            if (docType == DocType.Perechen) gr16aText = project.GetOsnNadpisItem("16a").perechenValue;
            if (docType == DocType.Vedomost) gr16aText = project.GetOsnNadpisItem("16a").vedomostValue;
            currentCell.Phrase = new Phrase(gr16aText, normal);
            table14_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            table14_18.AddCell(currentCell);
            table14_18.AddCell(currentCell);
            table14_18.WriteSelectedRows(1, 2, 230 * mm_A3, 40 * mm_A3, cb);

            //Рисование тонкими линиями и заполнение граф 10-13
            PdfPTable table10_13 = new PdfPTable(4);
            table10_13.TotalWidth = 65 * mm_A3;
            table10_13.LockedWidth = true;
            tbldWidths = new float[4];
            tbldWidths[0] = 17;
            tbldWidths[1] = 23;
            tbldWidths[2] = 15;
            tbldWidths[3] = 10;
            table10_13.SetWidths(tbldWidths);
            currentCell = new PdfPCell(new Phrase("Разраб.", normal));
            currentCell.BorderWidth = 0.5f;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.PaddingLeft = 3;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_LEFT;
            currentCell.FixedHeight = 5 * mm_A3;
            table10_13.AddCell(currentCell);
            string gr11aText = String.Empty;
            if (docType == DocType.Specification) gr11aText = project.GetOsnNadpisItem("11a").specificationValue;
            if (docType == DocType.Perechen) gr11aText = project.GetOsnNadpisItem("11a").perechenValue;
            if (docType == DocType.Vedomost) gr1aText = project.GetOsnNadpisItem("11a").vedomostValue;
            currentCell.Phrase = new Phrase(gr11aText, normal);
            table10_13.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            for (int i = 0; i < 2; i++) table10_13.AddCell(currentCell);
            table10_13.WriteSelectedRows(0, 1, 230 * mm_A3, 30 * mm_A3, cb);
            currentCell.Phrase = new Phrase("Пров.", normal);
            table10_13.AddCell(currentCell);
            string gr11bText = String.Empty;
            if (docType == DocType.Specification) gr11bText = project.GetOsnNadpisItem("11b").specificationValue;
            if (docType == DocType.Perechen) gr11bText = project.GetOsnNadpisItem("11b").perechenValue;
            if (docType == DocType.Vedomost) gr11bText = project.GetOsnNadpisItem("11b").vedomostValue;
            currentCell.Phrase = new Phrase(gr11bText, normal);
            table10_13.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            for (int i = 0; i < 2; i++) table10_13.AddCell(currentCell);
            table10_13.WriteSelectedRows(1, 2, 230 * mm_A3, 25 * mm_A3, cb);
            string gr10Text = String.Empty;
            if (docType == DocType.Specification) gr10Text = project.GetOsnNadpisItem("10").specificationValue;
            if (docType == DocType.Perechen) gr10Text = project.GetOsnNadpisItem("10").perechenValue;
            if (docType == DocType.Vedomost) gr10Text = project.GetOsnNadpisItem("10").vedomostValue;
            currentCell.Phrase = new Phrase(gr10Text, normal);
            table10_13.AddCell(currentCell);
            string gr11cText = String.Empty;
            if (docType == DocType.Specification) gr11cText = project.GetOsnNadpisItem("11c").specificationValue;
            if (docType == DocType.Perechen) gr11cText = project.GetOsnNadpisItem("11c").perechenValue;
            if (docType == DocType.Vedomost) gr11cText = project.GetOsnNadpisItem("11c").vedomostValue;
            currentCell.Phrase = new Phrase(gr11cText, normal);
            table10_13.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            for (int i = 0; i < 2; i++) table10_13.AddCell(currentCell);
            table10_13.WriteSelectedRows(2, 3, 230 * mm_A3, 20 * mm_A3, cb);
            currentCell.Phrase = new Phrase("Н. контр.", normal);
            table10_13.AddCell(currentCell);
            string gr11dText = String.Empty;
            if (docType == DocType.Specification) gr11dText = project.GetOsnNadpisItem("11d").specificationValue;
            if (docType == DocType.Perechen) gr11dText = project.GetOsnNadpisItem("11d").perechenValue;
            if (docType == DocType.Vedomost) gr11dText = project.GetOsnNadpisItem("11d").vedomostValue;
            currentCell.Phrase = new Phrase(gr11dText, normal);
            table10_13.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            for (int i = 0; i < 2; i++) table10_13.AddCell(currentCell);
            table10_13.WriteSelectedRows(3, 4, 230 * mm_A3, 15 * mm_A3, cb);
            currentCell.Phrase = new Phrase("Утв.", normal);
            table10_13.AddCell(currentCell);
            string gr11eText = String.Empty;
            if (docType == DocType.Specification) gr11eText = project.GetOsnNadpisItem("11e").specificationValue;
            if (docType == DocType.Perechen) gr11eText = project.GetOsnNadpisItem("11e").perechenValue;
            if (docType == DocType.Vedomost) gr11eText = project.GetOsnNadpisItem("11e").vedomostValue;
            currentCell.Phrase = new Phrase(gr11eText, normal);
            table10_13.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            for (int i = 0; i < 2; i++) table10_13.AddCell(currentCell);
            table10_13.WriteSelectedRows(4, 5, 230 * mm_A3, 10 * mm_A3, cb);


            #endregion

            #region Рисование табицы с графами 27-29            
            PdfPTable table27_29 = new PdfPTable(3);
            table27_29.TotalWidth = 120 * mm_A3;
            table27_29.LockedWidth = true;
            tbldWidths = new float[3];
            tbldWidths[0] = 14;
            tbldWidths[1] = 53;
            tbldWidths[2] = 53;
            table27_29.SetWidths(tbldWidths);

            // Заполнение граф 27-29:
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 14 * mm_A3;
            table27_29.AddCell(currentCell);
            table27_29.AddCell(currentCell);
            table27_29.AddCell(currentCell);
            table27_29.WriteSelectedRows(0, 1, 295 * mm_A3, 67 * mm_A3, cb);
            #endregion

            #region Рисование табицы с графой 30            
            PdfPTable table30 = new PdfPTable(1);
            table30.TotalWidth = 120 * mm_A3;
            table30.LockedWidth = true;

            // Заполнение графы 30:
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 8 * mm_A3;
            table30.AddCell(currentCell);
            table30.WriteSelectedRows(0, 1, 295 * mm_A3, 53 * mm_A3, cb);
            #endregion
        }

        private void DrawSubsequentStampA4(Document doc, PdfWriter wr, int pageNumber, DocType docType)
        {
            PdfContentByte cb = wr.DirectContent;

            float[] tbldWidths;

            float mm_A4 = doc.PageSize.Width / 210;

            #region Рисование графы 2           
            PdfPTable table2 = new PdfPTable(1);
            table2.TotalWidth = 110 * mm_A4;
            table2.LockedWidth = true;


            // Заполнение графы 2:
            string gr2Text = String.Empty;
            if (docType == DocType.Specification) gr2Text = project.GetOsnNadpisItem("2").specificationValue;
            else if (docType == DocType.Perechen) gr2Text = project.GetOsnNadpisItem("2").perechenValue;
            else if (docType == DocType.Vedomost) gr2Text = project.GetOsnNadpisItem("2").vedomostValue;
            PdfPCell currentCell = new PdfPCell(new Phrase(gr2Text, veryBig));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.PaddingBottom = 6;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 15 * mm_A4;
            table2.AddCell(currentCell);
            table2.WriteSelectedRows(0, 1, 85 * mm_A4, 20 * mm_A4, cb);
            #endregion

            #region Рисование графы 7           
            PdfPTable table7 = new PdfPTable(1);
            table7.TotalWidth = 10 * mm_A4;
            table7.LockedWidth = true;


            // Заполнение графы 7:
            currentCell = new PdfPCell(new Phrase("Лист", normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.PaddingBottom = 6;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 7 * mm_A4;
            table7.AddCell(currentCell);
            currentCell.Phrase = new Phrase(pageNumber.ToString(), normal);
            currentCell.FixedHeight = 8 * mm_A4;
            table7.AddCell(currentCell);
            table7.WriteSelectedRows(0, 2, 195 * mm_A4, 20 * mm_A4, cb);
            #endregion

            #region Рисование таблицы с графами 14-18
            //Рисование толстых линий и заполнение заголовков граф 14-18
            PdfPTable table14_18 = new PdfPTable(5);
            table14_18.TotalWidth = 65 * mm_A4;
            table14_18.LockedWidth = true;
            tbldWidths = new float[5];
            tbldWidths[0] = 7;
            tbldWidths[1] = 10;
            tbldWidths[2] = 23;
            tbldWidths[3] = 15;
            tbldWidths[4] = 10;
            table14_18.SetWidths(tbldWidths);
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 10 * mm_A4;
            for (int i = 0; i < 5; i++) table14_18.AddCell(currentCell);
            currentCell.FixedHeight = 5 * mm_A4;
            currentCell.Phrase = new Phrase("Изм.", normal);
            table14_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Лист", normal);
            table14_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("№ докум.", normal);
            table14_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Подп.", normal);
            table14_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Дата", normal);
            table14_18.AddCell(currentCell);
            table14_18.WriteSelectedRows(0, 2, 20 * mm_A4, 20 * mm_A4, cb);
            //Рисование тонких линий и заполнение граф 14-18
            table14_18 = new PdfPTable(5);
            table14_18.TotalWidth = 65 * mm_A4;
            table14_18.LockedWidth = true;
            table14_18.SetWidths(tbldWidths);
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 0.5f;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 5 * mm_A4;
            for (int i = 0; i < 5; i++) table14_18.AddCell(currentCell);
            string gr14aText = project.GetOsnNadpisItem("14a").perechenValue;
            currentCell.Phrase = new Phrase(gr14aText, normal);
            table14_18.AddCell(currentCell);
            string gr15aText = project.GetOsnNadpisItem("15a").perechenValue;
            currentCell.Phrase = new Phrase(gr15aText, normal);
            table14_18.AddCell(currentCell);
            string gr16aText = project.GetOsnNadpisItem("16a").perechenValue;
            currentCell.Phrase = new Phrase(gr16aText, normal);
            table14_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            table14_18.AddCell(currentCell);
            table14_18.AddCell(currentCell);
            table14_18.WriteSelectedRows(0, 2, 20 * mm_A4, 20 * mm_A4, cb);

            #endregion
        }

        private void DrawSubsequentStampA3(Document doc, PdfWriter wr, int pageNumber, DocType docType)
        {
            PdfContentByte cb = wr.DirectContent;

            float[] tbldWidths;

            float mm_A3 = doc.PageSize.Width / 420;

            #region Рисование графы 2           
            PdfPTable table2 = new PdfPTable(1);
            table2.TotalWidth = 110 * mm_A3;
            table2.LockedWidth = true;


            // Заполнение графы 2:
            string gr2Text = String.Empty;
            if (docType == DocType.Specification) gr2Text = project.GetOsnNadpisItem("2").specificationValue;
            else if (docType == DocType.Perechen) gr2Text = project.GetOsnNadpisItem("2").perechenValue;
            else if (docType == DocType.Vedomost) gr2Text = project.GetOsnNadpisItem("2").vedomostValue;
            PdfPCell currentCell = new PdfPCell(new Phrase(gr2Text, veryBig));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.PaddingBottom = 6;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 15 * mm_A3;
            table2.AddCell(currentCell);
            table2.WriteSelectedRows(0, 1, 295 * mm_A3, 20 * mm_A3, cb);
            #endregion

            #region Рисование графы 7           
            PdfPTable table7 = new PdfPTable(1);
            table7.TotalWidth = 10 * mm_A3;
            table7.LockedWidth = true;


            // Заполнение графы 7:
            currentCell = new PdfPCell(new Phrase("Лист", normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.PaddingBottom = 6;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 7 * mm_A3;
            table7.AddCell(currentCell);
            currentCell.Phrase = new Phrase(pageNumber.ToString(), normal);
            currentCell.FixedHeight = 8 * mm_A3;
            table7.AddCell(currentCell);
            table7.WriteSelectedRows(0, 2, 405 * mm_A3, 20 * mm_A3, cb);
            #endregion

            #region Рисование таблицы с графами 14-18
            //Рисование толстых линий и заполнение заголовков граф 14-18
            PdfPTable table14_18 = new PdfPTable(5);
            table14_18.TotalWidth = 65 * mm_A3;
            table14_18.LockedWidth = true;
            tbldWidths = new float[5];
            tbldWidths[0] = 7;
            tbldWidths[1] = 10;
            tbldWidths[2] = 23;
            tbldWidths[3] = 15;
            tbldWidths[4] = 10;
            table14_18.SetWidths(tbldWidths);
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 10 * mm_A3;
            for (int i = 0; i < 5; i++) table14_18.AddCell(currentCell);
            currentCell.FixedHeight = 5 * mm_A3;
            currentCell.Phrase = new Phrase("Изм.", normal);
            table14_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Лист", normal);
            table14_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("№ докум.", normal);
            table14_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Подп.", normal);
            table14_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Дата", normal);
            table14_18.AddCell(currentCell);
            table14_18.WriteSelectedRows(0, 2, 230 * mm_A3, 20 * mm_A3, cb);
            //Рисование тонких линий и заполнение граф 14-18
            table14_18 = new PdfPTable(5);
            table14_18.TotalWidth = 65 * mm_A3;
            table14_18.LockedWidth = true;
            table14_18.SetWidths(tbldWidths);
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 0.5f;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 5 * mm_A3;
            for (int i = 0; i < 5; i++) table14_18.AddCell(currentCell);
            string gr14aText = project.GetOsnNadpisItem("14a").perechenValue;
            currentCell.Phrase = new Phrase(gr14aText, normal);
            table14_18.AddCell(currentCell);
            string gr15aText = project.GetOsnNadpisItem("15a").perechenValue;
            currentCell.Phrase = new Phrase(gr15aText, normal);
            table14_18.AddCell(currentCell);
            string gr16aText = project.GetOsnNadpisItem("16a").perechenValue;
            currentCell.Phrase = new Phrase(gr16aText, normal);
            table14_18.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, normal);
            table14_18.AddCell(currentCell);
            table14_18.AddCell(currentCell);
            table14_18.WriteSelectedRows(0, 2, 230 * mm_A3, 20 * mm_A3, cb);

            #endregion
        }

        private void DrawPerechenTable(Document doc, PdfWriter wr, int pagesCount, int tempNumber)
        {
            float rowsHeight = 8.86f;

            PdfContentByte cb = wr.DirectContent;
            float mm_A4 = doc.PageSize.Width / 210;

            BaseFont fontGostA = BaseFont.CreateFont("GOST_A.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font normal = new Font(fontGostA, 12f, Font.ITALIC, BaseColor.BLACK);
            Font underline = new Font(fontGostA, 12f, Font.UNDERLINE | Font.ITALIC, BaseColor.BLACK);

            //Размеры в соответствии с рис. 5 ГОСТ 2.701-2008

            PdfPTable perechTable = new PdfPTable(4);
            perechTable.TotalWidth = 185 * mm_A4;
            perechTable.LockedWidth = true;
            float[] tbldWidths = new float[4];
            tbldWidths[0] = 20;
            tbldWidths[1] = 110;
            tbldWidths[2] = 10;
            tbldWidths[3] = 45;
            perechTable.SetWidths(tbldWidths);

            // Заполнение заголовков:
            PdfPCell currentCell = new PdfPCell(new Phrase("Поз. обозначение", normal));
            currentCell.BorderWidth = 0.5f;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 15 * mm_A4;
            perechTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Наименование", normal);
            perechTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Кол.", normal);
            perechTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Примечание", normal);
            perechTable.AddCell(currentCell);
            // Заполнение граф:                        
            int startIndex = perech_first_page_rows_count * (pagesCount == 0 ? 0 : 1) + perech_subseq_page_rows_count * (pagesCount > 1 ? pagesCount - 1 : 0); //номер первой строки на странице из общего кол-ва строк
            int rowsCount = (pagesCount == 0) ? perech_first_page_rows_count : perech_subseq_page_rows_count;           
            int numberOfValidStrings = project.GetPerechenLength(tempNumber);
            List<PerechenItem> pData = new List<PerechenItem>();

            for (int i = 1; i <= numberOfValidStrings; i++)
            {
                pData.Add(project.GetPerechenItem(i, tempNumber));
            }

            for (int j = startIndex; j < startIndex + rowsCount; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (j >= numberOfValidStrings) currentCell.Phrase = new Phrase(String.Empty);
                    else
                       if (i == 0) currentCell.Phrase = new Phrase(pData[j].designator, normal);
                    else if (i == 1)
                    {
                        if (pData[j].type == "Составное устройство")
                        {
                            currentCell.Phrase = new Phrase("   ", normal);
                            currentCell.Phrase.Add(new Chunk(pData[j].name.Substring(1), underline));
                        }
                        else
                        {
                            currentCell.Phrase = new Phrase(" ", normal);
                            currentCell.Phrase.Add(new Phrase(" " + pData[j].name, (pData[j].isNameUnderlined == true) ? underline : normal));
                        }


                    }
                    else if (i == 2) currentCell.Phrase = new Phrase(pData[j].quantity, normal);
                    else if (i == 3) currentCell.Phrase = new Phrase(' ' + pData[j].note, normal);


                    else currentCell.Phrase = new Phrase(String.Empty, normal);
                    currentCell.FixedHeight = rowsHeight * mm_A4;

                    //Для графы "Наименование" устанавливаем выравниванеие по левому краю:
                    if (i == 1) currentCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    else currentCell.HorizontalAlignment = Element.ALIGN_CENTER;

                    perechTable.AddCell(currentCell);
                }
            }

            perechTable.WriteSelectedRows(0, rowsCount + 1, 20 * mm_A4, 292 * mm_A4, cb);

            //Рисование толстых линий:
            perechTable = new PdfPTable(4);
            perechTable.TotalWidth = 185 * mm_A4;
            perechTable.LockedWidth = true;
            perechTable.SetWidths(tbldWidths);
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 15 * mm_A4;
            for (int j = 0; j < 4; j++) perechTable.AddCell(currentCell);
            currentCell.FixedHeight = rowsHeight * rowsCount * mm_A4;
            for (int j = 0; j < 4; j++) perechTable.AddCell(currentCell);

            perechTable.WriteSelectedRows(0, 2, 20 * mm_A4, 292 * mm_A4, cb);
        }

        int position = 1;

        private void DrawSpecificationTable(Document doc, PdfWriter wr, int pagesCount, List<SpecificationItem> sData)
        {
            float rowsHeight = 8.86f;

            PdfContentByte cb = wr.DirectContent;
            float mm_A4 = doc.PageSize.Width / 210;

            BaseFont fontGostA = BaseFont.CreateFont("GOST_A.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font normal = new Font(fontGostA, 12f, Font.ITALIC, BaseColor.BLACK);
            Font underline = new Font(fontGostA, 12f, Font.UNDERLINE | Font.ITALIC, BaseColor.BLACK);

            //Размеры в соответствии с Приложением А ГОСТ 2.106-96

            PdfPTable specTable = new PdfPTable(7);
            specTable.TotalWidth = 185 * mm_A4;
            specTable.LockedWidth = true;
            float[] tbldWidths = new float[7];
            tbldWidths[0] = 6;
            tbldWidths[1] = 6;
            tbldWidths[2] = 8;
            tbldWidths[3] = 70;
            tbldWidths[4] = 63;
            tbldWidths[5] = 10;
            tbldWidths[6] = 22;
            specTable.SetWidths(tbldWidths);

            // Заполнение заголовков:
            PdfPCell currentCell = new PdfPCell(new Phrase("Формат", normal));
            currentCell.BorderWidth = 0.5f;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_CENTER;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 6 * mm_A4;
            currentCell.Rotation = 90;
            specTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Зона", normal);
            specTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Поз.", normal);
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.FixedHeight = 8 * mm_A4;
            specTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Обозначение", normal);
            currentCell.Rotation = 0;
            currentCell.FixedHeight = 15 * mm_A4;
            specTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Наименование", normal);
            specTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Кол.", normal);
            currentCell.Rotation = 90;
            currentCell.FixedHeight = 10 * mm_A4;
            specTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase(" Приме-  чание", normal);
            currentCell.Rotation = 0;
            currentCell.FixedHeight = 15 * mm_A4;
            specTable.AddCell(currentCell);

            // Заполнение граф:                        
            int startIndex = spec_first_page_rows_count * (pagesCount == 0 ? 0 : 1) + spec_subseq_page_rows_count * (pagesCount > 1 ? pagesCount - 1 : 0); //номер первой строки на странице из общего кол-ва строк
            int rowsCount = (pagesCount == 0) ? spec_first_page_rows_count : spec_subseq_page_rows_count;
            //rowsCount = Math.Min(rowsCount, numberOfValidStrings - startIndex);


            for (int j = startIndex; j < startIndex + rowsCount; j++)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (j >= sData.Count) currentCell.Phrase = new Phrase(String.Empty);
                    else
                       if (i == 0) currentCell.Phrase = new Phrase(sData[j].format, normal);
                    else if (i == 1) currentCell.Phrase = new Phrase(sData[j].zona, normal);
                    else if (i == 2)
                    {
                        if (sData[j].position != null)
                        {
                            sData[j].position = sData[j].position.Replace(" ", "");
                            if (sData[j].position != String.Empty)
                            {
                                currentCell.Phrase = new Phrase(position.ToString(), normal);
                                position++;
                            }
                        }
                                                    
                    }                        
                    else if (i == 3) currentCell.Phrase = new Phrase(' ' + sData[j].oboznachenie, normal);
                    else if (i == 4)
                    {
                        if (sData[j].group == "Header")
                        {
                            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            currentCell.Phrase = new Phrase(sData[j].name, underline);
                        }
                        else
                        {
                            currentCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            currentCell.Phrase = new Phrase(' ' + sData[j].name, normal);
                        }
                    }
                    else if (i == 5) currentCell.Phrase = new Phrase(sData[j].quantity, normal);
                    else if (i == 6) currentCell.Phrase = new Phrase(' ' + sData[j].note, normal);

                    else currentCell.Phrase = new Phrase(String.Empty, normal);
                    currentCell.FixedHeight = rowsHeight * mm_A4;

                    //Для граф "Обозначение" и "Наименование" устанавливаем выравниванеие по левому краю:
                    if (i == 3) currentCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    else if (i != 4) currentCell.HorizontalAlignment = Element.ALIGN_CENTER;

                    specTable.AddCell(currentCell);
                }
            }

            specTable.WriteSelectedRows(0, rowsCount + 1, 20 * mm_A4, 292 * mm_A4, cb);

            //Рисование толстых линий:
            specTable = new PdfPTable(7);
            specTable.TotalWidth = 185 * mm_A4;
            specTable.LockedWidth = true;
            specTable.SetWidths(tbldWidths);
            currentCell = new PdfPCell(new Phrase(String.Empty, normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 15 * mm_A4;
            for (int j = 0; j < 7; j++) specTable.AddCell(currentCell);
            currentCell.FixedHeight = rowsHeight * rowsCount * mm_A4;
            for (int j = 0; j < 7; j++) specTable.AddCell(currentCell);

            specTable.WriteSelectedRows(0, 2, 20 * mm_A4, 292 * mm_A4, cb);
        }

        private void DrawVedomostTable(Document doc, PdfWriter wr, int pagesCount, int tempNumber)
        {
            float rowsHeight;

            if (pagesCount == 0) rowsHeight = 8.25f;
            else rowsHeight = 8.448f;

            PdfContentByte cb = wr.DirectContent;
            float mm_A3 = doc.PageSize.Width / 420;

            BaseFont fontGostA = BaseFont.CreateFont("GOST_A.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font normal = new Font(fontGostA, 12f, Font.ITALIC, BaseColor.BLACK);
            Font header = new Font(fontGostA, 14f, Font.ITALIC, BaseColor.BLACK);
            Font underline = new Font(fontGostA, 14f, Font.UNDERLINE | Font.ITALIC, BaseColor.BLACK);

            //Размеры в соответствии с формой 5 Приложения А ГОСТ 2.106-96

            #region Заполнение заголовков таблицы
            PdfPTable headerTable = new PdfPTable(8);
            headerTable.TotalWidth = 395 * mm_A3;
            headerTable.LockedWidth = true;
            float[] tbldWidths = new float[8];
            tbldWidths[0] = 7;
            tbldWidths[1] = 60;
            tbldWidths[2] = 45;
            tbldWidths[3] = 70;
            tbldWidths[4] = 55;
            tbldWidths[5] = 70;
            tbldWidths[6] = 64;
            tbldWidths[7] = 24;
            headerTable.SetWidths(tbldWidths);

            // Заполнение заголовков:
            PdfPCell currentCell = new PdfPCell(new Phrase("№ строки", header));
            currentCell.BorderWidth = 0.5f;
            currentCell.HasFixedHeight();
            currentCell.Rotation = 90;
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 27 * mm_A3;
            headerTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Наименование", header);
            currentCell.Rotation = 0;
            headerTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Код" + Environment.NewLine + "продукции", header);
            headerTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Обозначение" + Environment.NewLine + "документа на" + Environment.NewLine + "поставку", header);
            headerTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Поставщик", header);
            headerTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Куда входит" + Environment.NewLine + "(обозначение)", header);
            headerTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase(String.Empty, header);
            headerTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("Приме-" + Environment.NewLine + "чание", header);
            headerTable.AddCell(currentCell);

            headerTable.WriteSelectedRows(0, 1, 20 * mm_A3, 292 * mm_A3, cb);

            //Добавляем заголовок "Количество"
            headerTable = new PdfPTable(1);
            headerTable.TotalWidth = 64 * mm_A3;
            headerTable.LockedWidth = true;
            tbldWidths = new float[1];
            tbldWidths[0] = 64;
            headerTable.SetWidths(tbldWidths);
            currentCell.Phrase = new Phrase("Количество", header);
            currentCell.FixedHeight = 9 * mm_A3;
            headerTable.AddCell(currentCell);
            headerTable.WriteSelectedRows(0, 1, 327 * mm_A3, 292 * mm_A3, cb);

            //Добавляем заголовки разных типов "Количества"
            headerTable = new PdfPTable(4);
            headerTable.TotalWidth = 64 * mm_A3;
            headerTable.LockedWidth = true;
            tbldWidths = new float[4];
            tbldWidths[0] = 16;
            tbldWidths[1] = 16;
            tbldWidths[2] = 16;
            tbldWidths[3] = 16;
            headerTable.SetWidths(tbldWidths);
            currentCell.FixedHeight = 18 * mm_A3;
            currentCell.Phrase = new Phrase("на из- делие", header);
            headerTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("в ком- плекты", header);
            headerTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("на ре- гулир.", header);
            headerTable.AddCell(currentCell);
            currentCell.Phrase = new Phrase("всего", header);
            headerTable.AddCell(currentCell);
            headerTable.WriteSelectedRows(0, 1, 327 * mm_A3, 283 * mm_A3, cb);
            #endregion

            #region Заполнение граф таблицы
            PdfPTable vedomostTable = new PdfPTable(11);
            vedomostTable.TotalWidth = 395 * mm_A3;
            vedomostTable.LockedWidth = true;
            tbldWidths = new float[11];
            tbldWidths[0] = 7;
            tbldWidths[1] = 60;
            tbldWidths[2] = 45;
            tbldWidths[3] = 70;
            tbldWidths[4] = 55;
            tbldWidths[5] = 70;
            tbldWidths[6] = 16;
            tbldWidths[7] = 16;
            tbldWidths[8] = 16;
            tbldWidths[9] = 16;
            tbldWidths[10] = 24;
            vedomostTable.SetWidths(tbldWidths);

            int startIndex = vedomost_first_page_rows_count * (pagesCount == 0 ? 0 : 1) + vedomost_subseq_page_rows_count * (pagesCount > 1 ? pagesCount - 1 : 0); //номер первой строки на странице из общего кол-ва строк
            int rowsCount = (pagesCount == 0) ? vedomost_first_page_rows_count : vedomost_subseq_page_rows_count;            
            int numberOfValidStrings = project.GetVedomostLength(tempNumber);
            List<VedomostItem> vData = new List<VedomostItem>();
            
            for (int i = 1; i <= numberOfValidStrings; i++)
            {
                vData.Add(project.GetVedomostItem(i, tempNumber));
            }

            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;

            for (int j = startIndex; j < startIndex + rowsCount; j++)
            {
                for (int i = 0; i < 11; i++)
                {
                    if (j >= numberOfValidStrings) currentCell.Phrase = new Phrase(String.Empty);
                    else if (i == 0) currentCell.Phrase = new Phrase((j - startIndex + 1).ToString(), normal);
                    else if (i == 1)
                    {
                        currentCell.Phrase = new Phrase(" ", normal);
                        currentCell.Phrase.Add(new Phrase(vData[j].name, (vData[j].isNameUnderlined == true) ? underline : normal));

                    }
                    else if (i == 2) currentCell.Phrase = new Phrase(' ' + vData[j].kod, normal);
                    else if (i == 3) currentCell.Phrase = new Phrase(vData[j].docum, normal);
                    else if (i == 4) currentCell.Phrase = new Phrase(vData[j].supplier, normal);
                    else if (i == 5) currentCell.Phrase = new Phrase(vData[j].belongs, normal);
                    else if (i == 6) currentCell.Phrase = new Phrase(vData[j].quantityIzdelie, normal);
                    else if (i == 7) currentCell.Phrase = new Phrase(vData[j].quantityComplects, normal);
                    else if (i == 8) currentCell.Phrase = new Phrase(vData[j].quantityRegul, normal);
                    else if (i == 9) currentCell.Phrase = new Phrase(vData[j].quantityTotal, normal);
                    else if (i == 10) currentCell.Phrase = new Phrase(vData[j].note, normal);else currentCell.Phrase = new Phrase(String.Empty, normal);
                    currentCell.FixedHeight = rowsHeight * mm_A3;

                    //Для графы "Наименование" устанавливаем выравниванеие по левому краю:
                    if (i == 1) currentCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    else currentCell.HorizontalAlignment = Element.ALIGN_CENTER;

                    vedomostTable.AddCell(currentCell);
                }
            }
            vedomostTable.WriteSelectedRows(0, rowsCount + 1, 20 * mm_A3, 265 * mm_A3, cb);
            #endregion
                       
        }

        private void DrawListRegistrTable(Document doc, PdfWriter wr)
        {
            PdfContentByte cb = wr.DirectContent;
            float mm_A4 = doc.PageSize.Width / 210;

            BaseFont fontGostA = BaseFont.CreateFont("GOST_A.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font normal = new Font(fontGostA, 12f, Font.ITALIC, BaseColor.BLACK);

            #region Заполнение ячейки "Лист регистрации изменений"
            PdfPTable registrTable1 = new PdfPTable(1);
            registrTable1.TotalWidth = 185 * mm_A4;
            registrTable1.LockedWidth = true;

            // Заполнение заголовка:
            PdfPCell currentCell = new PdfPCell(new Phrase("Лист регистрации изменений", normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 10 * mm_A4;
            registrTable1.AddCell(currentCell);

            registrTable1.WriteSelectedRows(0, 1, 20 * mm_A4, 292 * mm_A4, cb);
            #endregion

            #region Заполнение ячейки "Изм."
            PdfPTable registrTable2 = new PdfPTable(1);
            registrTable2.TotalWidth = 10 * mm_A4;
            registrTable2.LockedWidth = true;

            // Заполнение заголовка:
            currentCell = new PdfPCell(new Phrase("Изм.", normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 27 * mm_A4;
            registrTable2.AddCell(currentCell);

            registrTable2.WriteSelectedRows(0, 1, 20 * mm_A4, 282 * mm_A4, cb);
            #endregion

            #region Заполнение ячейки "Номера листов (страниц)"
            PdfPTable registrTable3 = new PdfPTable(1);
            registrTable3.TotalWidth = 76 * mm_A4;
            registrTable3.LockedWidth = true;

            // Заполнение заголовка:
            currentCell = new PdfPCell(new Phrase("Номера листов (страниц)", normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 9 * mm_A4;
            registrTable3.AddCell(currentCell);

            registrTable3.WriteSelectedRows(0, 1, 30 * mm_A4, 282 * mm_A4, cb);
            #endregion

            #region Заполнение ячейки "Измененных"
            PdfPTable registrTable4 = new PdfPTable(1);
            registrTable4.TotalWidth = 19 * mm_A4;
            registrTable4.LockedWidth = true;
            currentCell = new PdfPCell(new Phrase("изменен-", normal));
            currentCell.BorderWidth = 1;
            currentCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 5 * mm_A4;
            registrTable4.AddCell(currentCell);
            currentCell.Phrase = new Phrase("ных", normal);
            currentCell.VerticalAlignment = Element.ALIGN_TOP;
            currentCell.FixedHeight = 13 * mm_A4;
            currentCell.DisableBorderSide(Rectangle.TOP_BORDER);
            currentCell.EnableBorderSide(Rectangle.BOTTOM_BORDER);
            registrTable4.AddCell(currentCell);
            registrTable4.WriteSelectedRows(0, 2, 30 * mm_A4, 273 * mm_A4, cb);
            #endregion

            #region Заполнение ячейки "Замененных"
            PdfPTable registrTable5 = new PdfPTable(1);
            registrTable5.TotalWidth = 19 * mm_A4;
            registrTable5.LockedWidth = true;
            currentCell = new PdfPCell(new Phrase("заменен-", normal));
            currentCell.BorderWidth = 1;
            currentCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 5 * mm_A4;
            registrTable5.AddCell(currentCell);
            currentCell.Phrase = new Phrase("ных", normal);
            currentCell.VerticalAlignment = Element.ALIGN_TOP;
            currentCell.FixedHeight = 13 * mm_A4;
            currentCell.DisableBorderSide(Rectangle.TOP_BORDER);
            currentCell.EnableBorderSide(Rectangle.BOTTOM_BORDER);
            registrTable5.AddCell(currentCell);
            registrTable5.WriteSelectedRows(0, 2, 49 * mm_A4, 273 * mm_A4, cb);
            #endregion

            #region Заполнение ячейки "Новых"
            PdfPTable registrTable6 = new PdfPTable(1);
            registrTable6.TotalWidth = 19 * mm_A4;
            registrTable6.LockedWidth = true;
            currentCell = new PdfPCell(new Phrase("новых", normal));
            currentCell.BorderWidth = 1;
            currentCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 9 * mm_A4;
            registrTable6.AddCell(currentCell);
            currentCell.Phrase = new Phrase("", normal);
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.FixedHeight = 9 * mm_A4;
            currentCell.DisableBorderSide(Rectangle.TOP_BORDER);
            currentCell.EnableBorderSide(Rectangle.BOTTOM_BORDER);
            registrTable6.AddCell(currentCell);
            registrTable6.WriteSelectedRows(0, 2, 68 * mm_A4, 273 * mm_A4, cb);
            #endregion

            #region Заполнение ячейки "Аннулированных"
            PdfPTable registrTable7 = new PdfPTable(1);
            registrTable7.TotalWidth = 19 * mm_A4;
            registrTable7.LockedWidth = true;
            currentCell = new PdfPCell(new Phrase("аннулиро-", normal));
            currentCell.BorderWidth = 1;
            currentCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 5 * mm_A4;
            registrTable7.AddCell(currentCell);
            currentCell.Phrase = new Phrase("ванных", normal);
            currentCell.VerticalAlignment = Element.ALIGN_TOP;
            currentCell.FixedHeight = 13 * mm_A4;
            currentCell.DisableBorderSide(Rectangle.TOP_BORDER);
            currentCell.EnableBorderSide(Rectangle.BOTTOM_BORDER);
            registrTable7.AddCell(currentCell);
            registrTable7.WriteSelectedRows(0, 2, 87 * mm_A4, 273 * mm_A4, cb);
            #endregion

            #region Заполнение ячейки "Всего листов (страниц) в документе"
            PdfPTable registrTable8 = new PdfPTable(1);
            registrTable8.TotalWidth = 19 * mm_A4;
            registrTable8.LockedWidth = true;
            currentCell = new PdfPCell(new Phrase("Всего", normal));
            currentCell.BorderWidth = 1;
            currentCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 4.3f * mm_A4;
            registrTable8.AddCell(currentCell);
            currentCell.Phrase = new Phrase("листов", normal);
            currentCell.DisableBorderSide(Rectangle.TOP_BORDER);
            registrTable8.AddCell(currentCell);
            currentCell.Phrase = new Phrase("(страниц)", normal);
            registrTable8.AddCell(currentCell);
            currentCell.Phrase = new Phrase("в доку-", normal);
            registrTable8.AddCell(currentCell);
            currentCell.Phrase = new Phrase("менте", normal);
            registrTable8.AddCell(currentCell);
            currentCell.Phrase = new Phrase(" ", normal);
            currentCell.FixedHeight = 5.5f * mm_A4;
            currentCell.EnableBorderSide(Rectangle.BOTTOM_BORDER);
            registrTable8.AddCell(currentCell);
            registrTable8.WriteSelectedRows(0, 6, 106 * mm_A4, 282 * mm_A4, cb);
            #endregion

            #region Заполнение ячейки "Номер документа"
            PdfPTable registrTable9 = new PdfPTable(1);
            registrTable9.TotalWidth = 19 * mm_A4;
            registrTable9.LockedWidth = true;
            currentCell = new PdfPCell(new Phrase("Номер", normal));
            currentCell.BorderWidth = 1;
            currentCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 4.3f * mm_A4;
            registrTable9.AddCell(currentCell);
            currentCell.Phrase = new Phrase("доку-", normal);
            currentCell.DisableBorderSide(Rectangle.TOP_BORDER);
            registrTable9.AddCell(currentCell);
            currentCell.Phrase = new Phrase("мента", normal);
            registrTable9.AddCell(currentCell);
            currentCell.Phrase = new Phrase(" ", normal);
            currentCell.FixedHeight = 14.1f * mm_A4;
            currentCell.EnableBorderSide(Rectangle.BOTTOM_BORDER);
            registrTable9.AddCell(currentCell);
            registrTable9.WriteSelectedRows(0, 4, 125 * mm_A4, 282 * mm_A4, cb);
            #endregion

            #region Заполнение ячейки "Входящий номер сопроводительного документа"
            PdfPTable registrTable10 = new PdfPTable(1);
            registrTable10.TotalWidth = 24 * mm_A4;
            registrTable10.LockedWidth = true;
            currentCell = new PdfPCell(new Phrase("Входящий", normal));
            currentCell.BorderWidth = 1;
            currentCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_TOP;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 4.3f * mm_A4;
            registrTable10.AddCell(currentCell);
            currentCell.Phrase = new Phrase("номер", normal);
            currentCell.DisableBorderSide(Rectangle.TOP_BORDER);
            registrTable10.AddCell(currentCell);
            currentCell.Phrase = new Phrase("сопроводи-", normal);
            registrTable10.AddCell(currentCell);
            currentCell.Phrase = new Phrase("тельного", normal);
            registrTable10.AddCell(currentCell);
            currentCell.Phrase = new Phrase("документа и", normal);
            registrTable10.AddCell(currentCell);
            currentCell.Phrase = new Phrase("дата", normal);
            currentCell.FixedHeight = 5.5f * mm_A4;
            currentCell.EnableBorderSide(Rectangle.BOTTOM_BORDER);
            registrTable10.AddCell(currentCell);
            registrTable10.WriteSelectedRows(0, 6, 144 * mm_A4, 282 * mm_A4, cb);
            #endregion

            #region Заполнение ячейки "Подпись"
            PdfPTable registrTable11 = new PdfPTable(1);
            registrTable11.TotalWidth = 18.5f * mm_A4;
            registrTable11.LockedWidth = true;
            currentCell = new PdfPCell(new Phrase("Подпись", normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 27 * mm_A4;
            registrTable11.AddCell(currentCell);
            registrTable11.WriteSelectedRows(0, 1, 168 * mm_A4, 282 * mm_A4, cb);
            #endregion

            #region Заполнение ячейки "Дата"
            PdfPTable registrTable12 = new PdfPTable(1);
            registrTable12.TotalWidth = 18.5f * mm_A4;
            registrTable12.LockedWidth = true;
            currentCell = new PdfPCell(new Phrase("Дата", normal));
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 27 * mm_A4;
            registrTable12.AddCell(currentCell);
            registrTable12.WriteSelectedRows(0, 1, 186.5f * mm_A4, 282 * mm_A4, cb);
            #endregion

            #region Черчение всех остальных ячеек
            //Черчение тонких линий
            PdfPTable registrTable13 = new PdfPTable(1);
            registrTable13.TotalWidth = 10 * mm_A4;
            registrTable13.LockedWidth = true;
            currentCell = new PdfPCell();
            currentCell.BorderWidth = 0.5f;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = (235f / 15f) * mm_A4;
            for (int j = 0; j < 29; j++)
                registrTable13.AddCell(currentCell);
            registrTable13.WriteSelectedRows(0, 15, 20 * mm_A4, 255 * mm_A4, cb);
            registrTable13.TotalWidth = 19 * mm_A4;
            registrTable13.WriteSelectedRows(0, 15, 30 * mm_A4, 255 * mm_A4, cb);
            registrTable13.WriteSelectedRows(0, 15, 49 * mm_A4, 255 * mm_A4, cb);
            registrTable13.WriteSelectedRows(0, 15, 68 * mm_A4, 255 * mm_A4, cb);
            registrTable13.WriteSelectedRows(0, 15, 87 * mm_A4, 255 * mm_A4, cb);
            registrTable13.WriteSelectedRows(0, 15, 106 * mm_A4, 255 * mm_A4, cb);
            registrTable13.WriteSelectedRows(0, 15, 125 * mm_A4, 255 * mm_A4, cb);
            registrTable13.TotalWidth = 24 * mm_A4;
            registrTable13.WriteSelectedRows(0, 15, 144 * mm_A4, 255 * mm_A4, cb);
            registrTable13.TotalWidth = 18.5f * mm_A4;
            registrTable13.WriteSelectedRows(0, 15, 168 * mm_A4, 255 * mm_A4, cb);
            registrTable13.WriteSelectedRows(0, 15, 186.5f * mm_A4, 255 * mm_A4, cb);
            //Черчение толстых линий
            PdfPTable registrTable14 = new PdfPTable(1);
            registrTable14.TotalWidth = 10 * mm_A4;
            registrTable14.LockedWidth = true;
            currentCell = new PdfPCell();
            currentCell.BorderWidth = 1;
            currentCell.HasFixedHeight();
            currentCell.Padding = 0;
            currentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            currentCell.HorizontalAlignment = Element.ALIGN_CENTER;
            currentCell.FixedHeight = 235 * mm_A4;
            registrTable14.AddCell(currentCell);
            registrTable14.WriteSelectedRows(0, 1, 20 * mm_A4, 255 * mm_A4, cb);
            registrTable14.TotalWidth = 19 * mm_A4;
            registrTable14.WriteSelectedRows(0, 1, 30 * mm_A4, 255 * mm_A4, cb);
            registrTable14.WriteSelectedRows(0, 1, 49 * mm_A4, 255 * mm_A4, cb);
            registrTable14.WriteSelectedRows(0, 1, 68 * mm_A4, 255 * mm_A4, cb);
            registrTable14.WriteSelectedRows(0, 1, 87 * mm_A4, 255 * mm_A4, cb);
            registrTable14.WriteSelectedRows(0, 1, 106 * mm_A4, 255 * mm_A4, cb);
            registrTable14.WriteSelectedRows(0, 1, 125 * mm_A4, 255 * mm_A4, cb);
            registrTable14.TotalWidth = 24 * mm_A4;
            registrTable14.WriteSelectedRows(0, 1, 144 * mm_A4, 255 * mm_A4, cb);
            registrTable14.TotalWidth = 18.5f * mm_A4;
            registrTable14.WriteSelectedRows(0, 1, 168 * mm_A4, 255 * mm_A4, cb);
            registrTable14.WriteSelectedRows(0, 1, 186.5f * mm_A4, 255 * mm_A4, cb);
            #endregion



        }

    }
}
