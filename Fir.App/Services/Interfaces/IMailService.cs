namespace Fir.App.Services.Interfaces
{
    public interface IMailService
    {
        public Task Send(string from,string to,string link,string subject);
    }
}
