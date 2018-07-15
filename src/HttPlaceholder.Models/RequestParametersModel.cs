namespace HttPlaceholder.Models
{
   public class RequestParametersModel
   {
      public string Method { get; set; }

      public string Url{ get; set; }

      public string Body { get; set; }

      public string HeaderString { get; set; }

      public string ClientIp { get; set; }
   }
}
