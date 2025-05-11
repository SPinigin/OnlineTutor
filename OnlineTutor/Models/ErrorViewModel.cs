namespace OnlineTutor.Models
{
    /// <summary>
    /// ������ ������������� ��� �������� ������
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// ������������� �������, ��� ������� ��������� ������
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// ����, ������������, ����� �� ���������� ������������� �������
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>
        /// ��������� �� ������
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// ������ ������ (������ ��� ����������)
        /// </summary>
        public string ErrorDetails { get; set; }

        /// <summary>
        /// ��� ������ HTTP (��������, 404, 500)
        /// </summary>
        public int? StatusCode { get; set; }
    }
}
