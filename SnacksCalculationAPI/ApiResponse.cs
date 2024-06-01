namespace SnacksCalculationAPI
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public int StatusCode { get; internal set; }
        public string Status { get; internal set; }
        public string Msg { get; internal set; }

        public ApiResponse()
        {
            Success = true;
        }
    }
}
