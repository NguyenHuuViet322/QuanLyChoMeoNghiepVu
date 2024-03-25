namespace DASNotify.Infrastructure.HttpClientAccessors.Interfaces
{
    public interface IBaseHttpClientFactory
    {
        IBaseHttpClient Create();
    }
}
