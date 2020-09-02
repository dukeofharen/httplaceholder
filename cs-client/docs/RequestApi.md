# HttPlaceholder.Client.Api.RequestApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**RequestCreateStubForRequest**](RequestApi.md#requestcreatestubforrequest) | **POST** /ph-api/requests/{CorrelationId}/stubs | An endpoint which accepts the correlation ID of a request made earlier. HttPlaceholder will create a stub based on this request for you to tweak lateron.
[**RequestDeleteAll**](RequestApi.md#requestdeleteall) | **DELETE** /ph-api/requests | Delete all requests. This call flushes all the requests.
[**RequestGetAll**](RequestApi.md#requestgetall) | **GET** /ph-api/requests | Get all Requests.
[**RequestGetOverview**](RequestApi.md#requestgetoverview) | **GET** /ph-api/requests/overview | Get overview of all Requests.
[**RequestGetRequest**](RequestApi.md#requestgetrequest) | **GET** /ph-api/requests/{correlationId} | Gets a specific request by correlation ID.


<a name="requestcreatestubforrequest"></a>
# **RequestCreateStubForRequest**
> FullStubDto RequestCreateStubForRequest (string correlationId)

An endpoint which accepts the correlation ID of a request made earlier. HttPlaceholder will create a stub based on this request for you to tweak lateron.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class RequestCreateStubForRequestExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RequestApi(config);
            var correlationId = correlationId_example;  // string | 

            try
            {
                // An endpoint which accepts the correlation ID of a request made earlier. HttPlaceholder will create a stub based on this request for you to tweak lateron.
                FullStubDto result = apiInstance.RequestCreateStubForRequest(correlationId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RequestApi.RequestCreateStubForRequest: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **correlationId** | **string**|  | 

### Return type

[**FullStubDto**](FullStubDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **200** | OK, with the generated stub |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="requestdeleteall"></a>
# **RequestDeleteAll**
> void RequestDeleteAll ()

Delete all requests. This call flushes all the requests.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class RequestDeleteAllExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RequestApi(config);

            try
            {
                // Delete all requests. This call flushes all the requests.
                apiInstance.RequestDeleteAll();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RequestApi.RequestDeleteAll: " + e.Message );
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

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **204** | OK, but no content returned |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="requestgetall"></a>
# **RequestGetAll**
> List&lt;RequestResultDto&gt; RequestGetAll ()

Get all Requests.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class RequestGetAllExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RequestApi(config);

            try
            {
                // Get all Requests.
                List<RequestResultDto> result = apiInstance.RequestGetAll();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RequestApi.RequestGetAll: " + e.Message );
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

[**List&lt;RequestResultDto&gt;**](RequestResultDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **200** | All request results |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="requestgetoverview"></a>
# **RequestGetOverview**
> List&lt;RequestOverviewDto&gt; RequestGetOverview ()

Get overview of all Requests.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class RequestGetOverviewExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RequestApi(config);

            try
            {
                // Get overview of all Requests.
                List<RequestOverviewDto> result = apiInstance.RequestGetOverview();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RequestApi.RequestGetOverview: " + e.Message );
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

[**List&lt;RequestOverviewDto&gt;**](RequestOverviewDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **200** | All request results |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="requestgetrequest"></a>
# **RequestGetRequest**
> RequestResultDto RequestGetRequest (string correlationId)

Gets a specific request by correlation ID.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class RequestGetRequestExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RequestApi(config);
            var correlationId = correlationId_example;  // string | The original correlation ID.

            try
            {
                // Gets a specific request by correlation ID.
                RequestResultDto result = apiInstance.RequestGetRequest(correlationId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RequestApi.RequestGetRequest: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **correlationId** | **string**| The original correlation ID. | 

### Return type

[**RequestResultDto**](RequestResultDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **200** | The request. |  -  |
| **404** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

