using System;

namespace DataAccessLibrary
{
    public class DbEventArgs : EventArgs
    {
        public string Message { get; set; }
        public DbEventArgs(string message)
        {
            Message = message;
        }
    }
}