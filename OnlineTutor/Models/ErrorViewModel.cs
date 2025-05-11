namespace OnlineTutor.Models
{
    /// <summary>
    /// Модель представления для страницы ошибки
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Идентификатор запроса, при котором произошла ошибка
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Флаг, определяющий, нужно ли отображать идентификатор запроса
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Детали ошибки (только для разработки)
        /// </summary>
        public string ErrorDetails { get; set; }

        /// <summary>
        /// Код ошибки HTTP (например, 404, 500)
        /// </summary>
        public int? StatusCode { get; set; }
    }
}
