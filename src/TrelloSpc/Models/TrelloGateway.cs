using System;

namespace TrelloSpc.Models
{
    public interface ITrelloGateway
    {
        string GetJsonData(string url);
    }

    /// <summary>
    /// Gateway for sending URL requests to Trello. Wraps functionality
    /// for opening an HTTP request, and configuring an HTTP proxy.
    /// </summary>
    public class TrelloGateway : ITrelloGateway
    {
        public string GetJsonData(string url)
        {
            throw new NotImplementedException();
        }
    }
}