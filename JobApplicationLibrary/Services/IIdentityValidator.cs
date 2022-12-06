namespace JobApplicationLibrary.Services
{
    public interface IIdentityValidator
    {
        bool isValid(string IdentityNumber);
        public ValidationMode ValidationMode { get; set; }
    }
    public enum ValidationMode
    {
        Detailed,
        Quick
    }
}