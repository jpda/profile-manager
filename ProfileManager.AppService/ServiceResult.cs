using System;

namespace ProfileManager.AppService
{
    public class ServiceResult<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public string ErrorCode { get; set; }
        public Exception Exception { get; set; }
        public T Value { get; set; }
        public ServiceResult() { }

        public ServiceResult(T value)
        {
            Value = value;
            Success = true;
        }
    }
}
