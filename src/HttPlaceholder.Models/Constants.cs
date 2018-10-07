namespace HttPlaceholder.Models
{
   public static class Constants
   {
      public static class ConfigKeys
      {
         public const string ApiUsernameKey = "apiUsername";

         public const string ApiPasswordKey = "apiPassword";

         public const string HttpsPortKey = "httpsPort";

         public const string InputFileKey = "inputFile";

         public const string OldRequestsQueueLengthKey = "oldRequestsQueueLength";

         public const string PfxPasswordKey = "pfxPassword";

         public const string PfxPathKey = "pfxPath";

         public const string PortKey = "port";

         public const string UseHttpsKey = "useHttps";

         public const string EnableRequestLogging = "enableRequestLogging";

         public const string FileStorageLocation = "fileStorageLocation";
      }

      public static class DefaultValues
      {
         public const int MaxRequestsQueueLength = 40;
      }
   }
}
