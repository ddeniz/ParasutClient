using ParasutClient.Api;
using ParasutClient.Client;
using System;


namespace Deneme
{
    class Program
    {
        static void Main(string[] args)
        {
            ParasutClient.Parasut parasut = new ParasutClient.Parasut("companyId", "clientId", "clientSecret", "username", "password");

            parasut.Home.ShowMe();
           
        }
    }
}
