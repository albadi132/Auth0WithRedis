namespace Auth0WithRedis.Models.Api
{
    public class ServiceResponse<T>
    {
        
        public T? Data { get; set; }
        public bool Success { get; set; } = false;
        public int Code { get; set; } = 404;
        public string Meesage { get; set; } = "Not Find";
    }
}
