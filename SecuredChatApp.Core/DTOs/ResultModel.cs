namespace SecuredChatApp.Core.DTOs
{
    public class ResultModel<T> where T : class
    {
        public ResultModel(T data = null, int statusCode = 200, ResultType type = ResultType.SUCCESS, string message = null)
        {
            this.statusCode = statusCode;
            this.message = message;

            if (type == ResultType.SUCCESS)
            {
                this.data = data;
                this.success = true;
            }
            else
            {
                this.error = data;
                this.success = false;
            }
        }

        public int statusCode { get; set; }
        public bool success { get; private set; }
        public T data { get; set; }
        public T error { get; set; }
        public string message { get; set; }

        public enum ResultType
        {
            SUCCESS,
            FAIL
        }
    }
}