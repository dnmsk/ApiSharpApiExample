namespace ApiSharpApiExample
{
    public class LocalSecurityInfo
    {

        /// <summary>
        /// Код инструмента
        /// </summary>
        public string SecCode { get; }

        /// <summary>
        /// Наименование инструмента
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Краткое наименование
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Код класса
        /// </summary>
        public string ClassCode { get; }

        /// <summary>
        /// Наименование класса
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Номинал
        /// </summary>
        public decimal FaceValue { get; set; }

        /// <summary>
        /// Код валюты номинала
        /// </summary>
        public string FaceUnit { get; set; }

        /// <summary>
        /// Количество значащих цифр после запятой
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// Дата погашения (в QLUA это число, но на самом деле дата записанная как YYYYMMDD),
        /// поэтому здесь сохраняем просто как строку
        /// </summary>
        public int MatDate { get; set; }

        /// <summary>
        /// Размер лота
        /// </summary>
        public decimal LotSize { get; }

        /// <summary>
        /// ISIN-код
        /// </summary>
        public string IsinCode { get; set; }

        /// <summary>
        /// Минимальный шаг цены
        /// </summary>
        public decimal MinPriceStep { get; }

        /// <summary>
        /// Валюта цены
        /// </summary>
        public string Currency { get; }

        public LocalSecurityInfo(string classCode, string secCode, string name, string currency, decimal lotSize, decimal minPriceStep) {
            ClassCode = ClassName = classCode;
            SecCode = ShortName = secCode;
            Name = name;
            Currency = FaceUnit = currency;
            LotSize = lotSize;
            MinPriceStep = minPriceStep;
            FaceValue = 1;
        }
    }
}
