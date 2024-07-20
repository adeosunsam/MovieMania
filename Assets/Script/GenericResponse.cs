public class GenericResponse<T>
{
    public string ResponseMessage { get; set; }
    public string ResponseCode { get; set; }
    public T? Data { get; set; }

    public GenericResponse()
    {
    }

    public static GenericResponse<T> Fail(string responseMessage, string responseCode)
    {
        return new GenericResponse<T> { ResponseMessage = responseMessage, ResponseCode = responseCode };
    }
    public static GenericResponse<T> Success(string responseMessage, string responseCode = "200", T? data = default)
    {
        return new GenericResponse<T> { ResponseMessage = responseMessage, Data = data, ResponseCode = responseCode };
    }
}
