namespace Kawerk.Infastructure.ResponseClasses
{
    public class GetterResponses<T>
    {
        public int status { get; set; }
        public string msg { get; set; } = string.Empty;
        public PagedList<T>? Data { get; set; }
        public T? Value { get; set; }
    }
}
