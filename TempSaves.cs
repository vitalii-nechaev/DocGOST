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

namespace DocGOST
{
    class TempSaves //Класс для действий с номерами временных сохранений
    { 
        private int current; // Номер текущего временного сохранения
        private int numberOfSavesUsed = 0; // Количество временных сохранений
        private int lastSavedState = 0; // Номер временного сохранения, при котором в последний раз проект сохранялся
        
        /// <summary>
        /// Создаёт экземпляр класса с номером текущего сохранения 1
        /// </summary>
        public TempSaves()
        {
            current = 1;
            numberOfSavesUsed = 1;
        }

        /// <summary>
        /// Возвращает номер текущего временного сохранения
        /// </summary>
        public int GetCurrent()
        {
            return current;
        }

        /// <summary>
        /// Устанавливает следующее временное сохранение и возвращает его номер
        /// </summary>
        /// <remarks> Вызывается перед сохранением нового временного состояния </remarks>
        public int SetNext()
        {
            current++;
            numberOfSavesUsed = current;
            return current;
        }

        /// <summary>
        /// Устанавливает следующее временное сохранение, если оно есть
        /// </summary>
        /// <returns> Вызывается для чтения следующего временного состояния, т.е. при нажатии кнопки Redo </returns>
        public bool SetNextIfExist()
        {
            if (current<numberOfSavesUsed) current++;
            return (current < numberOfSavesUsed);
        }

        /// <summary>
        /// Устанавливает предыдущее временное сохранение, если оно есть
        /// </summary>
        /// <returns> Вызывается для чтения предыдущего временного состояния, т.е. при нажатии кнопки Undo </returns>
        public bool SetPrevIfExist()
        {
            if (current>1) current--;
            return (current > 1);
        }

        /// <summary>
        /// Обновляет lastSavedStat - должна вызываться после каждого сохранения проекта
        /// </summary>
        public void ProjectSaved() 
        {
            lastSavedState = current;
        }

        /// <summary>
        /// Возвращает номер последнего сохранения проекта
        /// </summary>
        /// <remarks> Нужно для определения того, нужно ли спрашивать о сохранении проекта, если номер текущего временного сохранения не совпадает с последним сохранением проекта </remarks>
        public int GetLastSavedState()
        {
            return lastSavedState;
        }
    }
}
