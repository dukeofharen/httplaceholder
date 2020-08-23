# HttPlaceholder.Client.Api.MetadataApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**MetadataGet**](MetadataApi.md#metadataget) | **GET** /ph-api/metadata | Gets metadata about the API (like the assembly version).


<a name="metadataget"></a>
# **MetadataGet**
> MetadataDto MetadataGet ()

Gets metadata about the API (like the assembly version).

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class MetadataGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MetadataApi(config);

            try
            {
                // Gets metadata about the API (like the assembly version).
                MetadataDto result = apiInstance.MetadataGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MetadataApi.MetadataGet: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

### Parameters
This endpoint does not need any parameter.

### Return type

[**MetadataDto**](MetadataDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **200** | HttPlaceholder metadata. |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

