namespace PG_Core
{
    /// <summary>
    /// This class is used to help implement authentication. Along with some code in the Startup class
    /// and a configuration on the web (on Okta Developer), it enforces OAuth 2.0 which is used as a way 
    /// to grant websites or applications access to the API but without giving away a password. It uses
    /// authorization tokens instead.
    /// </summary>
    public class Okta
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
    }
}